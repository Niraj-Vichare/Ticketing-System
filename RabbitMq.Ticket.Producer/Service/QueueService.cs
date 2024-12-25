using RabbitMq.Ticket.Producer.Model;
using RabbitMQ.Client;

namespace RabbitMq.Ticket.Producer.Service
{
    public class QueueService
    {
        private IConnection _connection;
        private IChannel _channel; 
        public async Task<string> InitializeQueueSetup()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                };
                _connection =await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                return "";

            }catch (Exception ex)
            {
                return "";

            }

        }

        public async Task<bool> PublishMessage(TicketModel ticketModel,byte[] body)
        {
            try
            {
                string queueName = ticketModel.ChannelType.ToString() + "_queue";
                await _channel.QueueDeclareAsync(queueName, true, false, true, null);
                await _channel.BasicPublishAsync(exchange:"",routingKey:queueName,body:body);
                return true;    

            }catch(Exception ex)
            {
                return false;
            }


        }

    }
}
