using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq.Channel.Consumer
{
    public class QueueConsumer:IHostedService
    {
        private QueueService _queueService;
         
        public QueueConsumer(QueueService queueService)
        {
            _queueService = queueService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //_queueService.InializationQueueSetup();
            string[] queueNames = { "Email_queue", "Chat_queue", "Phone_queue" };
            foreach(string queueName in queueNames)
            {
                _queueService.ConsumeMessage(queueName);
            }
            return Task.CompletedTask;
        }

        public  Task StopAsync(CancellationToken cancellationToken)
        {

            bool isStop = _queueService.StopQueue();
            
            return Task.CompletedTask;
            
        }

        
    }
}
