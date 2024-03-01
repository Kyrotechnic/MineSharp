using MineSharp.Client;
using MineSharp.Api;

namespace MineSharp.Protocol;

public abstract class IProtocol
{
    //Constants
    public const int StatusOffset = 250;
    public const int LoginOffset = 500;
    //End
    private bool First = false;
    public ProtocolInfo ProtocolInfo = new();

    public Dictionary<int, Func<ServerboundPacket, bool>> ProtocolMapping = new();
    public abstract void Initialize();
    public abstract bool HandleNewConnection(PacketBuffer buffer, ClientWrapper wrapper);


    //Methods
    public void Register(int id, Func<ServerboundPacket, bool> func)
    {
        if (First)
        {
            First = false;
        }

        ProtocolMapping.Add(id, func);
    }

    public void RegisterStatus(int id, Func<ServerboundPacket, bool> func) => Register(id + StatusOffset, func);
    public void RegisterLogin(int id, Func<ServerboundPacket, bool> func) => Register(id + LoginOffset, func);

    public int GetOffset(ClientWrapper wrapper)
    {
        switch (wrapper.PacketMode)
        {
            case PacketMode.Play:
                return 0;
            case PacketMode.Status:
                return StatusOffset;
            case PacketMode.Login:
                return LoginOffset;
        }

        return 0;

        //Tried optimizing by checking play first, no work ig? idk
    }

    public bool HandlePacket(ServerboundPacket packet)
    {
        int offset = GetOffset(packet.Client);
        Func<ServerboundPacket, bool> func = ProtocolMapping[packet.Id + offset];

        return func(packet);
    }
}