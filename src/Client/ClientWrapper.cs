using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Xml.XPath;
using Ionic.Zlib;
using MineSharp.Api;
using MineSharp.Protocol;

namespace MineSharp.Client;

public class ClientWrapper
{
    public bool IsClientOnline = true;
    public bool Encrypted = false;
    public int Compression = -1;
    public TcpClient TcpClient;
    public IProtocol? Protocol;
    public PacketMode PacketMode;
    public string? Uuid;
    public string? Username;
    internal byte[]? SharedKey { get; set; }
	internal ICryptoTransform? Encrypter { get; set; } = null;
	internal ICryptoTransform? Decrypter { get; set; } = null;
    public ClientWrapper(TcpClient tcpClient)
    {
        TcpClient = tcpClient;

        PacketMode = PacketMode.Ping;
    }

    public async Task<string> GetUuid()
    {
        if (Uuid == null)
        {
            HttpClient client = new();

            string response = await client.GetStringAsync("https://api.mojang.com/users/profiles/minecraft/" + Username);

            string[] split = response.Split();
            
            if (split.Length > 1)
            {
                string uuid = split[3];
                
                Uuid = new(uuid);
            }
            else
            {
                Uuid = Guid.NewGuid().ToString();
            }
        }

        return Uuid;
    }

    public void SendPacket(ClientboundPacket packet)
    {
        try
        {
            byte[] data = packet.PacketBuffer.BufferedData.ToArray();

            if (data.Length >= Compression)
            {
                //Compress packet
                
                byte[] bLength = data.Length.GetVarIntBytes();

                byte[] compressed = ZlibStream.CompressBuffer(data);
                int packetLength = compressed.Length + bLength.Length;

                PacketBuffer compressedBuffer = new();

                compressedBuffer.WriteVarInt(packetLength);
                compressedBuffer.WriteVarInt(data.Length);
                compressedBuffer.Write(compressed);

                data = compressedBuffer.BufferedData.ToArray();
            }
            else
            {
                PacketBuffer newBuf = new();

                newBuf.WriteVarInt(data.Length);
                newBuf.Write(data);

                data = newBuf.BufferedData.ToArray();
            }

            if (Encrypter != null)
            {
                //Encrypt packet
                byte[] toEncrypt = data;
                data = new byte[toEncrypt.Length];
                Encrypter.TransformBlock(toEncrypt, 0, toEncrypt.Length, data, 0);
            }

            var stream = TcpClient.GetStream();
            stream.Write(data, 0, data.Length);
            stream.Flush();
            Console.WriteLine("Sent packet!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public bool IsConnected() => TcpClient.Connected;
}