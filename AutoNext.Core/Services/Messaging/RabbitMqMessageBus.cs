using AutoNext.Core.Abstractions;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace AutoNext.Core.Services.Messaging
{
    public class RabbitMqMessageBus : IMessageBus, IAsyncDisposable, IDisposable
    {
        private readonly IChannel _channel;
        private bool _disposed;

        public RabbitMqMessageBus(IChannel channel)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }

        public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            var eventName = message.GetType().Name;
            var routingKey = $"event.{eventName}";
            return PublishAsync(message, routingKey, cancellationToken);
        }

        public async Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default) where T : class
        {
            var body = SerializeMessage(message);

            // BasicPublishAsync replaces the synchronous BasicPublish in RabbitMQ.Client 7.x
            await _channel.BasicPublishAsync(
                exchange: "autonext.events",
                routingKey: routingKey,
                body: body,
                cancellationToken: cancellationToken);
        }

        public async Task PublishDelayedAsync<T>(T message, TimeSpan delay, CancellationToken cancellationToken = default) where T : class
        {
            await Task.Delay(delay, cancellationToken);
            await PublishAsync(message, cancellationToken);
        }

        private static byte[] SerializeMessage<T>(T message)
        {
            var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return Encoding.UTF8.GetBytes(json);
        }

        // IAsyncDisposable — preferred path in v7.x since Close is now async
        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                try
                {
                    // CloseAsync replaces the synchronous Close(replyCode, replyText) in v7.x
                    await _channel.CloseAsync();
                    await _channel.DisposeAsync();
                }
                catch { /* Ignore dispose errors */ }
                _disposed = true;
            }
        }

        // Sync IDisposable fallback — calls DisposeAsync().AsTask().GetAwaiter().GetResult()
        // so the channel is still properly closed if Dispose() is called directly
        public void Dispose()
        {
            if (!_disposed)
            {
                DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
        }
    }
}