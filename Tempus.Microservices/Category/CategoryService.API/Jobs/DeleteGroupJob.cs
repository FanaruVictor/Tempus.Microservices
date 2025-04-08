using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CategoryService.API.Jobs
{
    public class DeleteGroupJob(IConfiguration configuration, IServiceProvider serviceProvider) : BackgroundService
    {

        private readonly IConfiguration configuration = configuration;
        private readonly IServiceProvider serviceProvider = serviceProvider;
        private IConnection? connection;
        private IModel? messageChannel;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queueName = configuration["RabbitMQ:DeleteGroupQueue"] ?? "deleteGroup";

            connection = serviceProvider.GetService<IConnection>();

            messageChannel = connection.CreateModel();
            messageChannel.QueueDeclare(
                queue: queueName,
                exclusive: false);

            var consumer = new EventingBasicConsumer(messageChannel);
            consumer.Received += ProcessMessageAsync;

            messageChannel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            messageChannel?.Dispose();
        }

        private void ProcessMessageAsync(object? sender, BasicDeliverEventArgs args)
        {
            var message = args.Body;
            return;
        }
    }
}
