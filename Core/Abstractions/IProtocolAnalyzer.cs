using DNP3_ProtocolAnalyzer.Core.Models;

namespace DNP3_ProtocolAnalyzer.Core.Abstractions;

public interface IProtocolAnalyzer
{
    SessionSnapshot Analyze(CaptureSession session);
}
