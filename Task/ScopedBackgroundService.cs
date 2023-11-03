namespace App.ScopedService;
using Confluent.Kafka;
using System.Threading.Channels;
using TaskMessage.Logic;
using static Confluent.Kafka.ConfigPropertyNames;
using TaskMessage.Enum;
using Channel = TaskMessage.Enum.Channel;

public sealed class DefaultScopedProcessingService : IScopedProcessingService
{
    private int _executionCount;
    private readonly ILogger<DefaultScopedProcessingService> _logger;
    private readonly IConsumer<string, string> _kafkaConsumer;

    public DefaultScopedProcessingService(
        ILogger<DefaultScopedProcessingService> logger, IConsumer<string, string> kafkaConsumer)
    {
        _logger = logger;
        _kafkaConsumer = kafkaConsumer;
        _kafkaConsumer.Subscribe("test-kafka");
    }

    public async Task DoWorkAsync(CancellationToken stoppingToken)
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
                        Email email = new Email();
                        email.EnviarCorreo(logicaNotificacion.notificacion.Contacts[0].Mail, logicaNotificacion.notificacion.Templates[0].Subject, logicaNotificacion.notificacion.Templates[0].Body);
                    }
                    if (i == Channel.SMS)
                    {

                    }
                    if (i == Channel.Whatsapp)
                    {
                    }
                }
                // meter logica 
            }
        }
    }
}