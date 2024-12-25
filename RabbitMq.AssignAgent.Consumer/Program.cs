
using RabbitMq.AssignAgent.Consumer;
using RabbitMq.AssignAgent.Consumer.Services;

public class Program
{
    private static QueueService _queueService;
    public static async Task Main(string[] args)
    {
        _queueService = new QueueService();
        bool isQueueInitialized = await _queueService.InializeQueueSetUp();
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
        await queueConsumer.StopAsync(CancellationToken.None);
    }
}