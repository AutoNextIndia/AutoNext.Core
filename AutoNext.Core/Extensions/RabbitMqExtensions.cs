using AutoNext.Core.Abstractions;
using AutoNext.Core.Services.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace AutoNext.Core.Extensions
{
    public static class RabbitMqExtensions
    {
        public static IServiceCollection AddRabbitMqMessageBus(
            this IServiceCollection services,
            IConfiguration configuration,
            string configSection = "RabbitMQ")
        {
            var settings = ValidateAndConfigureSettings(services, configuration, configSection);

            // 1. Connection Factory
            services.AddSingleton<IConnectionFactory>(provider =>
            {
                return new ConnectionFactory
                {
                    HostName = settings.Host,
                    Port = settings.Port,
                    UserName = settings.Username,
                    Password = settings.Password,
                    VirtualHost = settings.VirtualHost,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                    RequestedHeartbeat = TimeSpan.FromSeconds(10),
                    TopologyRecoveryEnabled = true,
                    ConsumerDispatchConcurrency = 1
                };
            });

            // 2. Singleton IConnection using the public interface (IConnection, not internal Connection class)
            services.AddSingleton<IConnection>(provider =>
            {
                var factory = provider.GetRequiredService<IConnectionFactory>();
                // CreateConnectionAsync is the correct async API in RabbitMQ.Client 7.x
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });

            // 3. Channel Factory — IChannel replaces IModel in RabbitMQ.Client 7.x
            services.AddTransient<IChannel>(provider =>
            {
                var connection = provider.GetRequiredService<IConnection>();
                // CreateChannelAsync replaces the obsolete CreateModel() in v7.x
                var channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
                channel.ExchangeDeclareAsync("autonext.events", ExchangeType.Topic, durable: true)
                       .GetAwaiter().GetResult();
                channel.ExchangeDeclareAsync("autonext.commands", ExchangeType.Direct, durable: true)
                       .GetAwaiter().GetResult();
                return channel;
            });

            // 4. Message Bus
            services.AddTransient<IMessageBus, RabbitMqMessageBus>();

            // 5. Health Check
            services.AddHealthChecks()
                .AddRabbitMQ(
                    sp => sp.GetRequiredService<IConnectionFactory>().CreateConnectionAsync().GetAwaiter().GetResult(),
                    name: "rabbitmq",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "rabbitmq", "messagebroker" });

            return services;
        }

        private static RabbitMqSettings ValidateAndConfigureSettings(
            IServiceCollection services,
            IConfiguration configuration,
            string configSection)
        {
            var settings = configuration.GetSection(configSection).Get<RabbitMqSettings>()
                ?? throw new InvalidOperationException($"RabbitMQ settings '{configSection}' not found");

            if (string.IsNullOrWhiteSpace(settings.Host) ||
                string.IsNullOrWhiteSpace(settings.Username) ||
                string.IsNullOrWhiteSpace(settings.Password))
            {
                throw new InvalidOperationException("RabbitMQ Host, Username, and Password are required");
            }

            services.AddSingleton(settings);
            return settings;
        }
    }
}