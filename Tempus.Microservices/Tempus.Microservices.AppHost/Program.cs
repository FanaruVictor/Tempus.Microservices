using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var rabbitmq = builder.AddRabbitMQ("messaging", username, password)
	.WithExternalHttpEndpoints()
	.WithManagementPlugin();

var sql = builder.AddSqlServer("sql")
	.WithDataVolume();

var userServiceDb = sql.AddDatabase("UserService");
var categoryServiceDb = sql.AddDatabase("CategoryService");
var groupServiceDb = sql.AddDatabase("GroupService");
var registrationServiceDb = sql.AddDatabase("RegistrationService");

var userMigrationService = builder.AddProject<Projects.UserService_MigrationService>("user-migrationservice")
	.WithReference(userServiceDb)
	.WaitFor(userServiceDb);

var categoryMigrationService = builder.AddProject<Projects.CategoryService_MigrationService>("category-migrationservice")
	.WithReference(categoryServiceDb)
	.WaitFor(categoryServiceDb);

var registrationMigrationService = builder.AddProject<Projects.RegistrationService_MigrationService>("registration-migrationservice")
	.WithReference(registrationServiceDb)
	.WaitFor(registrationServiceDb);

var groupMigrationService = builder.AddProject<Projects.GroupService_MigrationService>("group-migrationservice")
	.WithReference(groupServiceDb)
	.WaitFor(groupServiceDb);

var userAPI = builder.AddProject<Projects.UserService_API>("userservice-api")
	.WithReference(rabbitmq)
	.WithReference(userServiceDb)
	.WaitForCompletion(userMigrationService)
	.WaitFor(rabbitmq);

var categoryAPI = builder.AddProject<Projects.CategoryService_API>("categoryservice-api")
	.WithReference(rabbitmq)
	.WithReference(categoryServiceDb)
	.WaitForCompletion(categoryMigrationService)
	.WaitFor(rabbitmq);

var registrationAPI = builder.AddProject<Projects.RegistrationService_API>("registrationservice-api")
	.WithReference(rabbitmq)
	.WithReference(registrationServiceDb)
	.WithReference(categoryAPI)
	.WaitForCompletion(registrationMigrationService)
	.WaitFor(rabbitmq)
	.WaitFor(categoryAPI);

var groupAPI = builder.AddProject<Projects.GroupService_API>("groupservice-api")
	.WithReference(rabbitmq)
	.WithReference(groupServiceDb)
	.WithReference(userAPI)
	.WaitForCompletion(groupMigrationService)
	.WaitFor(rabbitmq)
	.WaitFor(userAPI);

builder.AddProject<Projects.APIGateway>("apigateway")
	.WithExternalHttpEndpoints()
	.WithReference(userAPI)
	.WithReference(categoryAPI)
	.WithReference(registrationAPI)
	.WithReference(groupAPI);

builder.Build().Run();
