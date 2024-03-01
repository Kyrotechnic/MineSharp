namespace MineSharp.Api.Server;

public class ServerInfo
{
    public string ProtocolName;
    public int DefaultProtocol;
    public int MaxPlayers;
    public Func<int> GetOnlinePlayers;
    public int CompressionThreshold;
    public ServerInfo(string protocolName, int defaultProtocol, int maxPlayers, Func<int> onlinePlayers, int compression)
    {
        this.ProtocolName = protocolName;
        this.DefaultProtocol = defaultProtocol;
        this.MaxPlayers = maxPlayers;
        this.GetOnlinePlayers = onlinePlayers;
        this.CompressionThreshold = compression;
    }
}