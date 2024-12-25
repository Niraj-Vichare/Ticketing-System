using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMq.Channel.Consumer
{
    public class QueueService
    {
        private IConnection _connection;
        private IChannel _channel;
        private const string exchangeName = "categorized_exchanges";

        public async Task<bool> InializationQueueSetup()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                };
                _connection =await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                return true;
            }catch (Exception ex)
            {
                return false;
            }
        }
        public Task ConsumeMessage(string queueName)
        {
            _channel.QueueDeclareAsync(queueName,true,false,true,null);
           
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async(model, ea) =>
            {
                var body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                MessageProcessor messageProcessor = new MessageProcessor();
                string queueName = messageProcessor.GetTicketCategory(message);
                if (!String.IsNullOrEmpty(queueName))
                {
                    bool isPublished = PublishMessage(queueName, message);
                }
            };
            _channel.BasicConsumeAsync(queueName,autoAck:true,consumer:consumer);
            return Task.CompletedTask;

        }

        public bool PublishMessage(string queueName,string body)
        {
            try
            {
                // Declare the exchange if it's not already declare
                _channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct,true,false,null);

                // Declare the queue    
                _channel.QueueDeclareAsync(queueName,false,false,false,null);

                // Bind the exchange and queue.
                _channel.QueueBindAsync(queueName, exchangeName, queueName);

                var messageBody = Encoding.UTF8.GetBytes(body);
                _channel.BasicPublishAsync(exchangeName, queueName, messageBody);


                return true;
            }catch(Exception ex)
            {
                return false;
            }
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
