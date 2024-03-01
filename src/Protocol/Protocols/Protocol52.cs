using MineSharp.Client;

namespace MineSharp.Protocol.Protocols;

public class Protocol52 : IProtocol
{
    public override bool HandleNewConnection(PacketBuffer buffer, ClientWrapper wrapper)
    {
        throw new NotImplementedException();
    }

    public override void Initialize()
    {
        ProtocolInfo = new();

        ProtocolInfo.SetMaximumProtocolVersion(52)
            .SetMinimumProtocolVersion(52)
            .SetReccomendedProtocolVersion(52);

        Register(0, HandleChatMessage);
    }

    public bool HandleChatMessage(ServerboundPacket packet)
    {
        Console.WriteLine("Does not handle duplicates.");

        return true;
    }
}