using MineSharp.Client;

namespace MineSharp.Protocol;

public class ServerboundPacket : Packet
{
    public PacketBuffer PacketBuffer;
    public ClientWrapper Client;
    public NetworkManager Manager;
    public ServerboundPacket(ClientWrapper wrapper, PacketBuffer buffer, NetworkManager manager)
    {
        this.PacketBuffer = buffer;
        this.Client = wrapper;
        this.Manager = manager;
    }
}