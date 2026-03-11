using System;
using System.Collections.Generic;

namespace DNP3_ProtocolAnalyzer.Core.Models;

public sealed class CaptureSession
{
    public string SessionName { get; set; } = string.Empty;
    public DateTime StartedAtUtc { get; set; }
    public string TransportProfile { get; set; } = string.Empty;
    public List<PacketRecord> Packets { get; } = new();
}
