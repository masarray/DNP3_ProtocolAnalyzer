namespace DNP3_ProtocolAnalyzer.Infrastructure.Transport;

public sealed class ChannelProfile
{
    public string Name { get; set; } = string.Empty;
    public string TransportType { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
