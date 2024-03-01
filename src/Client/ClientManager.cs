namespace MineSharp.Client;

public class ClientManager
{
    public List<ClientWrapper> Clients = new();

    public ClientManager()
    {
        
    }

    public void Add(ClientWrapper wrapper)
    {
        Clients.Add(wrapper);
    }

    public void Remove(ClientWrapper wrapper)
    {
        Clients.Remove(wrapper);
    }

    public int GetCurrentOnlineCount() => Clients.Count;
}