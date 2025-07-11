using Api;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapEndpoints();

app.Run();