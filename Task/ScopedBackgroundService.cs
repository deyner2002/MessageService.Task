namespace App.ScopedService;
using Confluent.Kafka;
using TaskMessage.Logic;
using Channel = TaskMessage.Enum.Channel;
using Newtonsoft.Json;
using RazorEngineNetCore = RazorEngine;
using RazorEngine.Templating;
using TaskMessage.Config;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Twilio.Types;
using Twilio;
using TaskMessage.Model;
using Twilio.Rest.Api.V2010.Account;

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
                try
                {
                    var message = response.Message.Value;

                    LogicaNotificacion logicaNotificacion = new LogicaNotificacion(message);
                    object obj = JsonConvert.DeserializeObject<object>(logicaNotificacion.notificacion.ObjectTemplate);
                    foreach (Template template in logicaNotificacion.notificacion.Templates)
                    {
                        if (template.Channel == Channel.Email)
                        {
                            logicaNotificacion.notificacion.Templates.FirstOrDefault(x => x.Channel == Channel.Email).Body = RenderString(Guid.NewGuid().ToString(), logicaNotificacion.notificacion.Templates.FirstOrDefault(x => x.Channel == Channel.Email).Body, obj);
                            SentEmail(logicaNotificacion.notificacion.Contacts.Where(x => x.Mail != string.Empty).ToList(), logicaNotificacion.notificacion.Templates.FirstOrDefault(x => x.Channel == Channel.Email));
                        }
                        if (template.Channel == Channel.SMS)
                        {
                            logicaNotificacion.notificacion.Templates.FirstOrDefault(x => x.Channel == Channel.SMS).Body = RenderString(Guid.NewGuid().ToString(), logicaNotificacion.notificacion.Templates.FirstOrDefault(x => x.Channel == Channel.SMS).Body, obj);
                            SentSMS(logicaNotificacion.notificacion.Contacts.Where(x => x.Phone != string.Empty).ToList(), logicaNotificacion.notificacion.Templates.FirstOrDefault(x => x.Channel == Channel.SMS));
                        }
                        if (template.Channel == Channel.Whatsapp)
                        {
                            logicaNotificacion.notificacion.Templates.FirstOrDefault(x => x.Channel == Channel.Whatsapp).Body = RenderString(Guid.NewGuid().ToString(), logicaNotificacion.notificacion.Templates.FirstOrDefault(x => x.Channel == Channel.Whatsapp).Body, obj);
                            SentWhatsapp(logicaNotificacion.notificacion.Contacts.Where(x => x.Phone != string.Empty).ToList(), logicaNotificacion.notificacion.Templates.FirstOrDefault(x => x.Channel == Channel.Whatsapp));
                        }
                    }
                }
                catch (Exception ex)
                {

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
            return "";
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
        try
        {
            TwilioClient.Init(_messageConfig.AccountSidWapp, _messageConfig.AuthTokenWapp);

            foreach (var contact in contacts)
            {
                var messageOptions = new CreateMessageOptions(
                new PhoneNumber("whatsapp:" + contact.Phone));
                messageOptions.From = new PhoneNumber("whatsapp:" + template.Sender);
                messageOptions.Body = template.Body;
                var message = MessageResource.Create(messageOptions);

                if (template.Attachments != null && template.Attachments != string.Empty)
                {
                    foreach (var item in template.Attachments.Split("***").ToList())
                    {
                        messageOptions.MediaUrl = new List<Uri>();
                        if (item.Contains(".jpg"))
                        {
                            messageOptions.Body = "";
                            messageOptions.MediaUrl.Add(new Uri(item));
                        }
                        messageOptions.MediaUrl.Add(new Uri(item));
                        message = MessageResource.Create(messageOptions);
                    }
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    public void SentSMS(List<Contact> contacts, Template template)
    {
        try
        {
            TwilioClient.Init(_messageConfig.AccountSidSMS, _messageConfig.AuthTokenSMS);

            foreach (var contact in contacts)
            {
                var messageOptions = new CreateMessageOptions(
                new PhoneNumber(contact.Phone));
                messageOptions.From = new PhoneNumber(template.Sender);
                messageOptions.Body = template.Body;

                var message = MessageResource.Create(messageOptions);

                if (template.Attachments != null && template.Attachments != string.Empty)
                {
                    foreach (var item in template.Attachments.Split("***").ToList())
                    {
                        messageOptions.MediaUrl = new List<Uri>();
                        if (item.Contains(".jpg"))
                        {
                            messageOptions.Body = "";
                            messageOptions.MediaUrl.Add(new Uri(item));
                        }
                        messageOptions.MediaUrl.Add(new Uri(item));
                        message = MessageResource.Create(messageOptions);
                    }
                }
            }
        }
        catch(Exception ex)
        {

        }
    }
}