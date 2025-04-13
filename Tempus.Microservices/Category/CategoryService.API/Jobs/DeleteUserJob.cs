using CategoryService.Infrastructure.Commands.Registrations.DeleteAllForOwner;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CategoryService.API.Jobs
{
    public class DeleteUserJob(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<DeleteUserJob> logger) : BackgroundService
    {
        private readonly IConfiguration configuration = configuration;
        private readonly IServiceProvider serviceProvider = serviceProvider;
        private readonly ILogger<DeleteUserJob> logger = logger;
        private IConnection? connection;
        private IModel? messageChannel;
        private EventingBasicConsumer? consumer;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string exchangeName = "deleteUserExchange";

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

            logger.LogInformation("DeleteUserJob is now listening on queue: {queueName}", queueName);

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
            string userId = Encoding.UTF8.GetString(args.Body.ToArray());
            logger.LogInformation("DeleteUserJob received message at {now}. Message Text: {text}", DateTime.Now, userId);

            using var scope = serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetService<IMediator>();

            var result = mediator.Send(new DeleteAllCategoriesForOwnerCommand
            {
                OwnerId = userId
            }).GetAwaiter().GetResult();

            if (result.Resource != true)
            {
                logger.LogError("DeleteUserJob failed to delete all categories for user {userId}. Error: {error}", userId, string.Join("\n", result.Errors));
            }
            else
            {
                logger.LogInformation("DeleteUserJob successfully deleted all categories for user {userId}", userId);
            }
        }
    }
}

