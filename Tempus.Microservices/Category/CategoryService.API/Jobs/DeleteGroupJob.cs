using CategoryService.Infrastructure.Commands.Registrations.DeleteAllForOwner;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CategoryService.API.Jobs
{
    public class DeleteGroupJob(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<DeleteGroupJob> logger) : BackgroundService
    {

        private readonly IConfiguration configuration = configuration;
        private readonly IServiceProvider serviceProvider = serviceProvider;
        private readonly ILogger<DeleteGroupJob> logger = logger;
        private IConnection? connection;
        private IModel? messageChannel;
        private EventingBasicConsumer? consumer;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string exchangeName = "deleteGroupExchange";

            connection = serviceProvider.GetService<IConnection>();

            messageChannel = connection.CreateModel();

            messageChannel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

            var queueName = messageChannel.QueueDeclare(
                queue: "",
                durable: false,
                exclusive: true,
                autoDelete: true,
                arguments: null).QueueName;

            messageChannel.QueueBind(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "");

            consumer = new EventingBasicConsumer(messageChannel);
            consumer.Received += ProcessMessageAsync;

            messageChannel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer);

            logger.LogInformation("DeleteGroupJob is now listening on queue: {queueName}", queueName);

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            if (consumer != null)
            {
                consumer.Received -= ProcessMessageAsync;
            }

            messageChannel?.Dispose();
            connection?.Dispose();
        }

        private void ProcessMessageAsync(object? sender, BasicDeliverEventArgs args)
        {
            string groupId = Encoding.UTF8.GetString(args.Body.ToArray());
            logger.LogInformation("DeleteGroupJob received message at {now}. Message Text: {text}", DateTime.Now, groupId);

            using var scope = serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetService<IMediator>();

            var result = mediator.Send(new DeleteAllCategoriesForOwnerCommand
            {
                OwnerId = groupId
            }).GetAwaiter().GetResult();

            if (result.Resource != true)
            {
                logger.LogError("DeleteGroupJob failed to delete all categories for group {groupId}. Error: {error}", groupId, string.Join("\n", result.Errors));
            }
            else
            {
                logger.LogInformation("DeleteGroupJob successfully deleted all categories for group {groupId}", groupId);
            }
        }
    }
}

