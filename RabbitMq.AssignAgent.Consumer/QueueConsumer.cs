using Microsoft.Extensions.Hosting;
using RabbitMq.AssignAgent.Consumer.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq.AssignAgent.Consumer
{
    public class QueueConsumer: IHostedService
    {
        private QueueService _queueService;

        public QueueConsumer(QueueService queueService)
        {
            _queueService = queueService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            string[] queueNames = { "integration_queue", "other_queue", "product_queue", "billing_queue" };
            foreach (string queueName in queueNames)
            {
                _queueService.CategorizedConsumer(queueName);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            bool isStop = _queueService.StopQueue();

            return Task.CompletedTask;
        }
    }
}
