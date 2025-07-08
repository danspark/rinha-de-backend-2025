var builder = DistributedApplication.CreateBuilder(args);

const int apiPort = 9999;
const string paymentProcessorNetwork = "payment-processor";

var db = builder.AddPostgres("db-server")
    .WithImage("dddanielreis/orleans-postgres")
    .WithPgAdmin()
    .AddDatabase("db");

builder.AddProject<Projects.Api>("api", configure: options =>
    {
        options.ExcludeLaunchProfile = true;
    })
    .WithReference(db)
    .WaitFor(db)
    .WithHttpEndpoint(apiPort)
    .PublishAsDockerComposeService((_, s) =>
    {
        s.Networks.Add(paymentProcessorNetwork);
    });

builder.AddDockerComposeEnvironment("docker-compose")
    .ConfigureComposeFile(compose =>
    {
        compose.AddNetwork(new()
        {
            Name = paymentProcessorNetwork,
            External = true
        });
    });

builder.Build().Run();
