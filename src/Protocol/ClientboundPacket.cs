namespace MineSharp.Protocol;

public class ClientboundPacket : Packet
{
    public PacketBuffer PacketBuffer;
    public ClientboundPacket(int id, bool autoWrite = true)
    {
        Id = id;
        PacketBuffer = new();
        if (autoWrite)
            PacketBuffer.WriteVarInt(id);
    }
}