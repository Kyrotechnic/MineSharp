using System.Net;
using System.Net.Sockets;
using MineSharp.Client;

namespace MineSharp.Protocol;

public class NetworkManager
{
    //public UdpClient UdpClient;
    public TcpListener TcpListener;
    public Thread? ListeningThread;
    public bool Listening = false;
    public MineServer MineServer;
    public NetworkManager(int port, MineServer server)
    {
        //UdpClient = new("224.0.2.60", 4445);
        TcpListener = new(IPAddress.Any, port);

        MineServer = server;
    }

    public void Start()
    {
        //Add UDP eventually

        Listening = true;

        ListeningThread = new Thread(() => {
            Thread.CurrentThread.Name = "Network Manager";

            TcpListener.Start();

            while (Listening)
            {
                TcpClient client = TcpListener.AcceptTcpClient();

                Console.WriteLine("New connection!");

                new Task(() => ReadNewClient(new(client))).Start();
            }
        });
        
        ListeningThread.Start();
    }

    public void ReadNewClient(ClientWrapper wrapper)
    {
        MineServer.ClientManager.Add(wrapper);

        NetworkStream clientStream = wrapper.TcpClient.GetStream();

        while (wrapper.IsConnected())
        {
            try
            {
                while (!clientStream.DataAvailable)
                {
                    if (!wrapper.IsConnected())
                        break;
                }

                if (!wrapper.IsConnected())
                    break;
                
                //Find out protocol Version

                if (!ReadPacket(wrapper, clientStream))
                {
                    //Disconnect
                    
                    Console.WriteLine("Failed to read packet or packet is 0 in length");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        MineServer.ClientManager.Remove(wrapper);
    }

    private int ReadVarInt(NetworkStream stream)
    {
        var value = 0;
        var size = 0;
        int b;

        while (((b = stream.ReadByte()) & 0x80) == 0x80)
        {
            value |= (b & 0x7F) << (size++ * 7);
            if (size > 5)
            {
                throw new IOException("VarInt too long");
            }
        }
        return value | ((b & 0x7F) << (size * 7));
    }

    private bool ReadPacket(ClientWrapper wrapper, NetworkStream clientStream)
    {
        int dlength = ReadVarInt(clientStream);

        byte[] buffer = new byte[dlength];

        int receivedData;
        receivedData = clientStream.Read(buffer, 0, buffer.Length);

        if (receivedData > 0)
        {
            PacketBuffer buf = new(buffer);

            buf.Size = dlength;

            int packetId = buf.ReadVarInt();

            if (wrapper.Protocol == null)
            {
                //This means this is the first packet (EVER) sent!
                
                if (HandleNoProtocol(dlength, packetId, buf, wrapper))
                {
                    return false;
                }

                return true;
            }
            else
            {
                ServerboundPacket packet = new(wrapper, buf, this)
                {
                    Id = packetId
                };

                return wrapper.Protocol.HandlePacket(packet);
            }
        }

        return false;
    }

    public bool HandleNoProtocol(int dlength, int id, PacketBuffer buf, ClientWrapper wrapper)
    {
        //true == fail
        int protocolVersion = buf.ReadVarInt();

        IProtocol protocol = MineServer.DetermineProtocol(protocolVersion)!;

        if (protocol == null)
        {
            return true;
        }

        if (!protocol.HandleNewConnection(buf, wrapper))
        {
            return true;
        }

        wrapper.Protocol = protocol;

        return false;
    }

    public void Stop()
    {
        Listening = false;
    }
}