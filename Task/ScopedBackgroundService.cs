namespace App.ScopedService;
using Confluent.Kafka;
using System.Threading.Channels;
using TaskMessage.Logic;
using static Confluent.Kafka.ConfigPropertyNames;
using TaskMessage.Enum;
using Channel = TaskMessage.Enum.Channel;
using SMS=TaskMessage.Logic.SMS;
using Newtonsoft.Json;
using RazorEngineNetCore = RazorEngine;
using RazorEngine.Templating;
using Newtonsoft.Json.Linq;
using TaskMessage.Config;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Twilio.Http;
using Twilio.Types;
using Twilio;
using TaskMessage.Model;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Voice;

public sealed class DefaultScopedProcessingService : IScopedProcessingService
{
    private int _executionCount;
    private readonly ILogger<DefaultScopedProcessingService> _logger;
    private readonly IConsumer<string, string> _kafkaConsumer;
    public readonly MessageConfig _messageConfig;

    public DefaultScopedProcessingService(
        ILogger<DefaultScopedProcessingService> logger, IConsumer<string, string> kafkaConsumer, IOptions<MessageConfig> messageConfig )
    {
        _logger = logger;
        _kafkaConsumer = kafkaConsumer;
        _messageConfig = messageConfig.Value;
        _kafkaConsumer.Subscribe("test-kafka");
    }

    public void DoWorkAsync(CancellationToken stoppingToken)
    {
        CancellationTokenSource token = new();
        bool bandera = true;
        while (!stoppingToken.IsCancellationRequested)
        {
            ++_executionCount;

            _logger.LogInformation(
                "{ServiceName} working, execution count: {Count}",
                nameof(DefaultScopedProcessingService),
                _executionCount);

            var response = _kafkaConsumer.Consume(token.Token);
            if (response.Message != null)
            {
                var message = response.Message.Value;

                LogicaNotificacion logicaNotificacion = new LogicaNotificacion(message);

                foreach (Channel i in logicaNotificacion.notificacion.Channels)
                {
                    if (i == Channel.Email)
                    {
                        object obj = JsonConvert.DeserializeObject<object>(logicaNotificacion.notificacion.Object);
                        logicaNotificacion.notificacion.Templates.FirstOrDefault(x => x.Channel == Channel.Email).Body = RenderString(new Guid().ToString(), logicaNotificacion.notificacion.Templates.FirstOrDefault(x => x.Channel == Channel.Email).Body, obj);
                        SentEmail(logicaNotificacion.notificacion.Contacts.Where(x => x.Mail != string.Empty).ToList(), logicaNotificacion.notificacion.Templates.FirstOrDefault(x => x.Channel == Channel.Email));
                    }
                    if (i == Channel.SMS)
                    {
                        SMS sMS = new SMS();
                        sMS.EnviarMensaje(logicaNotificacion.notificacion.Contacts[0].Phone + "", logicaNotificacion.notificacion.Templates[0].Body.ToString());
                    }
                    if (i == Channel.Whatsapp)
                    {
                        SentWhatsapp(logicaNotificacion.notificacion.Contacts.Where(x => x.Phone != string.Empty).ToList(), logicaNotificacion.notificacion.Templates.FirstOrDefault(x => x.Channel == Channel.Whatsapp));
                    }
                }
            }
        }
    }


    public string RenderString(string key, string body, object Object )
    {
        try
        {
            _logger.LogInformation($"Render item with key {key} start");

            if (!RazorEngineNetCore.Engine.Razor.IsTemplateCached(key, null))
            {
                RazorEngineNetCore.Engine.Razor.AddTemplate(key, body);
                RazorEngineNetCore.Engine.Razor.Compile(key);
            }

            var renderedString = RazorEngineNetCore.Engine.Razor.Run(key, null, Object);

            _logger.LogDebug($"Rendered string: {renderedString}");

            _logger.LogInformation($"Render item with key {key} finish");

            return renderedString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Render item with key {key} error");
            throw;
        }
    }

    public void SentEmail(List<Contact> contacts,Template template)
    {
        SmtpClient cliente = new SmtpClient(_messageConfig.URLEmail, _messageConfig.Port)
        {
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_messageConfig.FromEmail, _messageConfig.PasswordEmail)
        };

        foreach (var contact in contacts)
        {
            MailMessage email = new MailMessage(_messageConfig.FromEmail, contact.Mail, template.Subject, template.Body);
            email.IsBodyHtml = template.IsHtml;
            cliente.Send(email);
        }

    }

    public void SentWhatsapp(List<Contact> contacts, Template template)
    {
        TwilioClient.Init(_messageConfig.AccountSidWapp, _messageConfig.AuthTokenWapp);

        foreach (var contact in contacts)
        {
            var messageOptions = new CreateMessageOptions(
            new PhoneNumber("whatsapp:" + contact.Phone));
            messageOptions.From = new PhoneNumber("whatsapp:"+ template.Sender);
            messageOptions.Body = template.Body;
            if(template.AttachmentUrl != null && template.AttachmentUrl != string.Empty)
            {
                messageOptions.MediaUrl = new List<Uri> { new Uri(template.AttachmentUrl) };
            }
            var message = MessageResource.Create(messageOptions);
        }
    }
}