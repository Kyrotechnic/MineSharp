using System.Net;

namespace MineSharp.Api;

public enum PacketMode
{
    Status = 0x00,
    Play = 0x01,
    Login = 0x02,
    Ping = 0x03
}