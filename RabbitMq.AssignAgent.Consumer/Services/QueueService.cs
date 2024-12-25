using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq.AssignAgent.Consumer.Services
{
    public class QueueService
    {
        private IConnection _connection;
        private IChannel _channel;
        public async Task<bool> InializeQueueSetUp()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = "localhost"
                };
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task CategorizedConsumer(string queueName)
        {
            await _channel.QueueDeclareAsync(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, @event) =>
            {
                try
                {
                    var body = @event.Body.ToArray();
                    string message = Encoding.UTF8.GetString(body);

                    AssignService assignService = new AssignService();
                    bool isAssigned =await assignService.AssignTicketToAgent(queueName, message);

                    if (isAssigned)
                    {
                        await _channel.BasicAckAsync(@event.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        await _channel.BasicNackAsync(@event.DeliveryTag, multiple: false, requeue: false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message from {queueName}: {ex.Message}");
                    await _channel.BasicNackAsync(@event.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await _channel.BasicConsumeAsync(queueName, autoAck: false, consumer: consumer);
            Console.WriteLine($"Started consuming from {queueName}");
        }


        public bool StopQueue()
        {
            try
            {
                _channel?.CloseAsync();
                _connection?.CloseAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
