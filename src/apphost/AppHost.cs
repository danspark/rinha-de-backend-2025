using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("yarp.json", optional: false, reloadOnChange: true);

var yarp = builder.AddYarp("reverse-proxy")
    .WithConfigFile("yarp.json")
    .PublishAsDockerComposeService((_, s) =>
    {
        s.Ports = ["9999:5000"];
        s.AddVolume(new()
        {
            Name = "yarp-config",
            Source = "src/apphost/yarp.json",
            Target = "/etc/yarp.config",
            Type = "bind",
            ReadOnly = true
        });
        
        s.Deploy = new()
        {
            Resources = new()
            {
                Limits = new()
                {
                    Cpus = "0.5",
                    Memory = "50M"
                }
            }
        };
    });

const string paymentProcessorNetwork = "payment-processor";

var defaultPaymentProcessorConnectionString = builder.AddConnectionString("payment-processor-default");
var fallbackPaymentProcessorConnectionString = builder.AddConnectionString("payment-processor-fallback");

int index = 0;
foreach (var apiDestination in builder.Configuration.GetSection("ReverseProxy:Clusters:api-cluster:Destinations").GetChildren())
{
    var api = builder.AddProject<Projects.Api>(apiDestination.Key,
            configure: options => { options.ExcludeLaunchProfile = true; })
        .WithHttpEndpoint(port: 9000 + ++index)
        .PublishAsDockerComposeService((_, s) =>
        {
            s.Networks.Add(paymentProcessorNetwork);
            s.Deploy = new()
            {
                Resources = new()
                {
                    Limits = new()
                    {
                        Cpus = "0.5",
                        Memory = "150M"
                    }
                }
            };
        });

    var addressKey = $"ReverseProxy__Clusters__api-cluster__Destinations__{apiDestination.Key}__Address";
    yarp.WithEnvironment(addressKey, api.GetEndpoint("http"));

    if (builder.ExecutionContext.IsPublishMode)
    {
        api
            .WithReference(defaultPaymentProcessorConnectionString)
            .WithReference(fallbackPaymentProcessorConnectionString);
    }
}

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
