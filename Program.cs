using App.ScopedService;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using TaskMessage.Config;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<ScopedBackgroundService>();
        services.AddScoped<IScopedProcessingService, DefaultScopedProcessingService>();
        var kafkaConfig = new ConsumerConfig();
        var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
        kafkaConfig.BootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers");
        services.Configure<MessageConfig>(configuration.GetSection("MessageConfig"));
        kafkaConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
        kafkaConfig.GroupId = configuration.GetValue<string>("Kafka:GroupId");

        kafkaConfig.BootstrapServers = "localhost:9092";
        services.AddSingleton<IConsumer<string, string>>(new ConsumerBuilder<string, string>(kafkaConfig).Build());
    });

IHost host = builder.Build();
host.Run();