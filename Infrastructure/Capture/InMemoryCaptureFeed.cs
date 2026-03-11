using DNP3_ProtocolAnalyzer.Core.Models;

namespace DNP3_ProtocolAnalyzer.Infrastructure.Capture;

public sealed class InMemoryCaptureFeed
{
    public CaptureSession CreateLiveSession()
    {
        var session = CreateSampleSession();
        session.SessionName = "Live DNP3 Relay Session";
        session.Packets.Add(new PacketRecord
        {
            TimestampUtc = DateTime.UtcNow.AddSeconds(-1),
            Direction = "TX",
            Summary = "Operate CROB point 201 latch on",
            Payload = [0x05, 0x64, 0x18, 0xC4, 0x01, 0x00, 0x04, 0x01, 0x0C, 0x01, 0x17, 0x01, 0xC9, 0x00, 0x00, 0x03]
        });
        session.Packets.Add(new PacketRecord
        {
            TimestampUtc = DateTime.UtcNow,
            Direction = "RX",
            Summary = "Operate confirmation CROB point 201 success",
            Payload = [0x05, 0x64, 0x14, 0x44, 0x81, 0x00, 0x04, 0x81, 0x0C, 0x01, 0x17, 0x01, 0xC9, 0x00, 0x00]
        });
        return session;
    }

    public CaptureSession CreateSampleSession()
    {
        var session = new CaptureSession
        {
            SessionName = "Sample DNP3 Analyzer Session",
            StartedAtUtc = DateTime.UtcNow.AddMinutes(-5),
            TransportProfile = "DNP3 over TCP"
        };

        session.Packets.Add(new PacketRecord
        {
            TimestampUtc = DateTime.UtcNow.AddSeconds(-15),
            Direction = "TX",
            Summary = "Enable unsolicited for classes 1/2/3",
            Payload = [0x05, 0x64, 0x14, 0xC4, 0x01, 0x00, 0x64, 0x00, 0xC0, 0x14, 0x3C, 0x02, 0x06, 0x3C, 0x03, 0x06]
        });
        session.Packets.Add(new PacketRecord
        {
            TimestampUtc = DateTime.UtcNow.AddSeconds(-13),
            Direction = "RX",
            Summary = "Enable unsolicited confirmation",
            Payload = [0x05, 0x64, 0x0A, 0x44, 0x81, 0x00, 0x01, 0x00, 0xC0, 0x00]
        });
        session.Packets.Add(new PacketRecord
        {
            TimestampUtc = DateTime.UtcNow.AddSeconds(-10),
            Direction = "TX",
            Summary = "Integrity poll request class 0",
            Payload = [0x05, 0x64, 0x0E, 0xC4, 0x01, 0x00, 0x64, 0x00, 0xC1, 0x01, 0x3C, 0x01, 0x06]
        });
        session.Packets.Add(new PacketRecord
        {
            TimestampUtc = DateTime.UtcNow.AddSeconds(-8),
            Direction = "RX",
            Summary = "Integrity poll response binary input and analog input",
            Payload = [0x05, 0x64, 0x1E, 0x44, 0x81, 0x00, 0x01, 0x00, 0xC1, 0x81, 0x02, 0x01, 0x17, 0x01, 0x01, 0x00, 0x01, 0x1E, 0x02, 0x17, 0x64, 0x00, 0x00, 0x00]
        });
        session.Packets.Add(new PacketRecord
        {
            TimestampUtc = DateTime.UtcNow.AddSeconds(-5),
            Direction = "RX",
            Summary = "Unsolicited binary event feeder breaker open",
            Payload = [0x05, 0x64, 0x16, 0x44, 0x81, 0x00, 0x64, 0x00, 0xE2, 0x00, 0x02, 0x02, 0x28, 0x01, 0x01, 0x00, 0x00]
        });
        session.Packets.Add(new PacketRecord
        {
            TimestampUtc = DateTime.UtcNow.AddSeconds(-3),
            Direction = "TX",
            Summary = "Confirm unsolicited response",
            Payload = [0x05, 0x64, 0x0A, 0xC4, 0x01, 0x00, 0x64, 0x00, 0x00, 0x00]
        });

        return session;
    }
}
