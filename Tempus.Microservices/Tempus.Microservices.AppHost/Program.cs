var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var rabbitmq = builder.AddRabbitMQ("messaging", username, password)
    .WithManagementPlugin();

var sql = builder.AddSqlServer("sql")
    .WithDataVolume();

var userServiceDb = sql.AddDatabase("UserService");
var categoryServiceDb = sql.AddDatabase("CategoryService");
var groupServiceDb = sql.AddDatabase("GroupService");
var registrationServiceDb = sql.AddDatabase("RegistrationService");

builder.AddProject<Projects.APIGateway>("apigateway");

var userAPI = builder.AddProject<Projects.UserService_API>("userservice-api")
    .WithReference(rabbitmq)
    .WithReference(userServiceDb)
    .WaitFor(rabbitmq);

var categoryAPI = builder.AddProject<Projects.CategoryService_API>("categoryservice-api")
    .WithReference(rabbitmq)
    .WithReference(categoryServiceDb)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.RegistrationService_API>("registrationservice-api")
    .WithReference(rabbitmq)
    .WithReference(registrationServiceDb)
    .WaitFor(rabbitmq)
    .WaitFor(categoryAPI);

builder.AddProject<Projects.GroupService_API>("groupservice-api")
    .WithReference(rabbitmq)
    .WithReference(groupServiceDb)
    .WaitFor(rabbitmq)
    .WaitFor(userAPI);

builder.AddProject<Projects.UserService_MigrationService>("userservice-migrationservice")
    .WithReference(userServiceDb)
    .WaitFor(userServiceDb);

builder.AddProject<Projects.CategoryService_MigrationService>("categoryservice-migrationservice")
    .WithReference(categoryServiceDb)
    .WaitFor(categoryServiceDb);

builder.AddProject<Projects.RegistrationService_MigrationService>("registrationservice-migrationservice")
    .WithReference(registrationServiceDb)
    .WaitFor(registrationServiceDb);

builder.AddProject<Projects.GroupService_MigrationService>("groupservice-migrationservice")
    .WithReference(groupServiceDb)
    .WaitFor(groupServiceDb);

builder.Build().Run();
