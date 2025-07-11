using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api;

public sealed record PaymentRequest(Guid CorrelationId, decimal Amount);

public sealed record PaymentsSummary
{
    public required PaymentProcessorSummary Default { get; init; }
    
    public required PaymentProcessorSummary Fallback { get; init; }
}

public sealed record PaymentProcessorSummary
{
    public required int TotalRequests { get; init; }
    
    public required decimal TotalAmount { get; init; }
}

[JsonSerializable(typeof(PaymentRequest))]
[JsonSerializable(typeof(PaymentsSummary))]
[JsonSerializable(typeof(PaymentProcessorSummary))]
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web,
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public sealed partial class SerializationContext : JsonSerializerContext;