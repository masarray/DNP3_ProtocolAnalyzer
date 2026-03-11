using System;

namespace DNP3_ProtocolAnalyzer.Core.Models;

public sealed class PacketRecord
{
    public DateTime TimestampUtc { get; set; }
    public string Direction { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public byte[] Payload { get; set; } = Array.Empty<byte>();
}
