using DNP3_ProtocolAnalyzer.Core.Models;

namespace DNP3_ProtocolAnalyzer.Core.Abstractions;

public interface IFrameDecoder
{
    DecodedFrame Decode(PacketRecord packet);
}
