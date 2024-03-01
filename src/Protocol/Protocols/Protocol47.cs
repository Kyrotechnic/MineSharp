using MineSharp.Client;
using MineSharp.Api;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using MineSharp.Api.Messages;
using MineSharp.Api.Cryptography;

namespace MineSharp.Protocol.Protocols;

public class Protocol47 : IProtocol
{
    public override void Initialize()
    {
        ProtocolInfo = new();

        ProtocolInfo.SetMaximumProtocolVersion(47)
            .SetMinimumProtocolVersion(47)
            .SetReccomendedProtocolVersion(47);
        //Login Packets
        RegisterLogin(0, HandleLoginStart);
        RegisterLogin(1, HandleEncryptionResponse);
        
        //Status Packets
        RegisterStatus(0, HandleRequest);
        RegisterStatus(1, HandlePing);

        //Rest
        Register(0, HandleKeepAlive);
        Register(1, HandleChatMessage);
    }

    public override bool HandleNewConnection(PacketBuffer buffer, ClientWrapper wrapper)
    {
        Console.WriteLine("User connected with protocol 47!");

        //manage state, host (NOT USED YET)
        buffer.ReadString();
        buffer.ReadShort();

        //Get state (state 1 == status, state 2 == login)
        wrapper.PacketMode = buffer.ReadVarInt() == 1 ? PacketMode.Status : PacketMode.Login;

        return true;
    }

    public bool HandleKeepAlive(ServerboundPacket packet)
    {
        Console.WriteLine("Handles duplicates!");

        return true;
    }

    public bool HandleChatMessage(ServerboundPacket packet)
    {
        return true;
    }

    //Login

    public bool HandleLoginStart(ServerboundPacket packet)
    {
        PacketBuffer buffer = packet.PacketBuffer;
        ClientWrapper client = packet.Client;

        string usernameRaw = buffer.ReadString();
        string username = new string(usernameRaw.Where(c => char.IsLetter(c) || char.IsPunctuation(c) || char.IsDigit(c)).ToArray());

        packet.Client.Username = username;

        //packet.Client.GetUuid().Wait();

        Console.WriteLine("Starting login request");

        //Send encryption

        if (true)
        {
            client.PacketMode = PacketMode.Login;
            client.Username = username;
            
            ClientboundPacket encPacket = new(0x01);

            PacketBuffer encBuffer = encPacket.PacketBuffer;

            encBuffer.WriteString("");
            byte[] key = PacketCryptography.PublicKeyToAsn1(packet.Manager.MineServer.ServerKey);
            encBuffer.WriteVarInt(key.Length);
            encBuffer.Write(key);

            byte[] verifyToken = PacketCryptography.GetRandomToken();
            encBuffer.WriteVarInt(verifyToken.Length);
            encBuffer.Write(verifyToken);

            encPacket.PacketBuffer = encBuffer;

            client.SendPacket(encPacket);
        }

        return true;
    }

    public bool HandleEncryptionResponse(ServerboundPacket packet)
    {
        Console.WriteLine("Client sent back Encryption Packet!");

        return true;
    }

    //Status
    
    //This packet is to send the server details back.
    //It is sent after initial connection
    public bool HandleRequest(ServerboundPacket packet)
    {
        //Respond with a new packet
        //Broken MOTD response (idk why)

        Console.WriteLine("Sending Request!");

        ClientboundPacket responsePacket = new(0x00);

        PacketBuffer buffer = responsePacket.PacketBuffer;

        buffer.WriteString(packet.Manager.MineServer.GetCurrentMotd());

        responsePacket.PacketBuffer = buffer;
        
        packet.Client.SendPacket(responsePacket);

        return true;
    }

    public bool HandlePing(ServerboundPacket packet)
    {
        return true;
    }
}