var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var rabbitmq = builder.AddRabbitMQ("messaging", username, password)
    .WithManagementPlugin();

builder.AddProject<Projects.APIGateway>("apigateway");

var userAPI = builder.AddProject<Projects.UserService_API>("userservice-api")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

var categoryAPI = builder.AddProject<Projects.CategoryService_API>("categoryservice-api")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.RegistrationService_API>("registrationservice-api")
    .WithReference(rabbitmq)
    .WaitFor(categoryAPI)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.GroupService_API>("groupservice-api")
    .WithReference(rabbitmq)
    .WaitFor(userAPI)
    .WaitFor(rabbitmq);

builder.Build().Run();
