using DNP3_ProtocolAnalyzer.Core.Models;

namespace DNP3_ProtocolAnalyzer.Core.Abstractions;

public interface ISessionService
{
    CaptureSession StartSession();
    SessionSnapshot LoadSampleCapture();
}
