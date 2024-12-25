using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq.Channel.Consumer
{
    public class Program
    {
        private static QueueService _queueService;

        public static async Task Main(string[] args)
        {
            // Instantiate the QueueService
            _queueService = new QueueService();

            // Initialize the QueueService setup
            bool isQueueInitialized = await _queueService.InializationQueueSetup();
            if (!isQueueInitialized)
            {
                Console.WriteLine("Failed to initialize the queue.");
                return;
            }

            // Create the QueueConsumer with the initialized QueueService
            QueueConsumer queueConsumer = new QueueConsumer(_queueService);

            // Start consuming messages
            await queueConsumer.StartAsync(CancellationToken.None);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            // Stop consuming messages when done
            await queueConsumer.StopAsync(CancellationToken.None);
        }
    }

}
