namespace Api;

public static class Endpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/payments", (PaymentRequest request, CancellationToken cancellationToken) =>
        {
            return Results.Ok();
        });

        app.MapGet("/payments-summary", () =>
        {
            return Results.Ok(new PaymentsSummary
            {
                Default = new()
                {
                    TotalAmount = 10,
                    TotalRequests = 10
                },
                Fallback = new()
                {
                    TotalAmount = 10,
                    TotalRequests = 10
                }
            });
        });
    }
}