var builder = DistributedApplication.CreateBuilder(args);

const int apiPort = 9999;
const string paymentProcessorNetwork = "payment-processor";

builder.AddProject<Projects.Api>("api", configure: options =>
    {
        options.ExcludeLaunchProfile = true;
    })
    .WithHttpEndpoint(apiPort)
    .PublishAsDockerComposeService((_, s) => s.Networks.Add(paymentProcessorNetwork));

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
