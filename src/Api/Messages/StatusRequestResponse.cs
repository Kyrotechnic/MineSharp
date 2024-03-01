using MineSharp.Api.Server;

namespace MineSharp.Api.Messages;

	public class StatusRequestResponse
	{
		public StatusRequestResponse(string version, int protocol, int maxPlayers, int onlinePlayers, string description)
		{
			this.version = new StatusVersionClass(version, protocol);
			this.players = new StatusPlayersClass(maxPlayers, onlinePlayers);
			this.description = new ChatMessage(description);
		}

        public StatusRequestResponse(ServerInfo info, string motd)
        {
            this.version = new(info.ProtocolName, info.DefaultProtocol);
            this.players = new StatusPlayersClass(info.MaxPlayers, info.GetOnlinePlayers());
            this.description = new(motd);
        }

		public StatusVersionClass version;
		public StatusPlayersClass players;
		public ChatMessage description;
	}

	public class StatusVersionClass
	{
		public StatusVersionClass(string name, int protocol)
		{
			this.name = name;
			this.protocol = protocol;
		}

		public string name = string.Empty;
		public int protocol = 0;
	}

	public class StatusPlayersClass
	{
		public StatusPlayersClass(int max, int online)
		{
			this.max = max;
			this.online = online;
		}

		public int max;
		public int online;
	}