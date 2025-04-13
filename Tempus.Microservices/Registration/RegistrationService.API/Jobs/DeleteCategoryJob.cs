using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RegistrationService.Infrastructure.Commands.Registrations.DeleteAllForCategory;
using System.Text;

namespace RegistrationService.API.Jobs
{
    public class DeleteCategoryJob(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<DeleteCategoryJob> logger) : BackgroundService
    {
        private readonly IConfiguration configuration = configuration;
        private readonly IServiceProvider serviceProvider = serviceProvider;
        private readonly ILogger<DeleteCategoryJob> logger = logger;
        private IConnection? connection;
        private IModel? messageChannel;
        private EventingBasicConsumer? consumer;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string exchangeName = "deleteCategoryExchange";

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

            logger.LogInformation("DeleteCategoryJob is now listening on queue: {queueName}", queueName);

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
            string categoryId = Encoding.UTF8.GetString(args.Body.ToArray());
            logger.LogInformation("DeleteCategoryJob received message at {now}. Message Text: {text}", DateTime.Now, categoryId);

            using var scope = serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetService<IMediator>();

            var result = mediator.Send(new DeleteAllRegistrationsForCategoryCommand
            {
                CategoryId = categoryId
            }).GetAwaiter().GetResult();

            if (result.Resource != true)
            {
                logger.LogError("DeleteCategoryJob failed to delete all registrations for category {cateogryId}. Error: {error}", categoryId, string.Join("\n", result.Errors));
            }
            else
            {
                logger.LogInformation("DeleteCategoryJob successfully deleted all registrations for category {categoryId}", categoryId);
            }
        }
    }
}

