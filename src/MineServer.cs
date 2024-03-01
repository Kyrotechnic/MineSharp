using System.Security.Cryptography;
using MineSharp.Api;
using MineSharp.Api.Cryptography;
using MineSharp.Api.Files;
using MineSharp.Api.Server;
using MineSharp.Client;
using MineSharp.Protocol;
using MineSharp.Protocol.Protocols;

namespace MineSharp;

public class MineServer
{
    public ConfigFile ConfigFile;
    public Logger Logger;
    public List<IProtocol> Protocols = new();
    public NetworkManager NetworkManager;
    public ClientManager ClientManager = new();
    public ServerInfo ServerInfo;
    public RSAParameters ServerKey = PacketCryptography.GenerateKeyPair();

    //Server Info
    public int defaultProtocol = 47;
    public string protocolName = "MineSharp";
    public int maxPlayers = 399999;
    public int port;
    public bool testing = true;

    public MineServer(string configPath)
    {

        ConfigFile = new(new(configPath));

        Logger = new(true);

        Logger.Log("Welcome to MineSharp!\nThis server is very bare bones, and meant for modification\nand only supports Protocol 47\nYou may however register protocol handlers in ProtocolManager.cs\n");

        Logger.Log("Starting server...");

        //Register Protocol Handlers
        RegisterProtocolHandlers();

        //if (testing)
        //{
        //    RunTests();
        //    return;
        //}

        ServerInfo = new("MineSharp 0.1", 47, 999, ClientManager.GetCurrentOnlineCount, 256);

        port = int.Parse(ConfigFile.Get<string>("port", "25565"));
        //yada yada
        NetworkManager = new(port, this);

        ConfigFile.Save();

        //Start Network Manager

        NetworkManager.Start();

        Logger.Debug("Listening on port " + port);

        while (true);
    }

    /*public void RunTests()
    {
        IProtocol proto = DetermineProtocol(47);

        ServerboundPacket packet = new(null, new());

        packet.Id = 0x00;

        proto.HandlePacket(packet);
    }*/

    public void RegisterProtocolHandlers()
    {
        Protocols.Add(new Protocol47());
        Protocols.Add(new Protocol52());

        for (int i = 0; i < Protocols.Count; i++)
        {
            IProtocol protocol = Protocols[i];

            protocol.Initialize();
        }

        //Add your protocols below.
    }

    public IProtocol? DetermineProtocol(int version)
    {
        foreach (IProtocol protocol in Protocols)
        {
            ProtocolInfo info = protocol.ProtocolInfo;

            if (info.GetReccomendedProtocolVersion() == version || (info.GetMaximumProtocolVersion() >= version && info.GetMinimumProtocolVersion() <= version))
            {
                return protocol;
            }
        }

        return null;
    }

    public Func<string> MotdFunction;
    //motd + motd func handler
    public string GetCurrentMotd()
    {
        string message = "";

        if (MotdFunction != null)
        {
            message = MotdFunction();
        }

        message = "Sick server i swear lol";

        string motdFull = "{{\"version\":{{\"name\":\"" + protocolName + "\",\"protocol\":" + defaultProtocol + "}},\"players\":{{\"max\":" + maxPlayers + ",\"online\":" + 0 + "}},\"description\":{{\"text\":\"" + message + "\"}}}}";

        return motdFull;
    }
}