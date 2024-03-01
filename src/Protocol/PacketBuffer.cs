using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace MineSharp.Protocol;

public class PacketBuffer : IDisposable
{
    public byte[] BufferedData = new byte[4096];
	private int _lastByte;
	public int Size = 0;

    public PacketBuffer(byte[] data)
	{
		BufferedData = data;
	}

    public PacketBuffer()
    {
        
    }

    public void Dispose()
    {
        
    }

    //Methods

    public void SetDataSize(int size)
    {
        Array.Resize(ref BufferedData, size);
        Size = size;
    }

    public byte ReadByte()
    {
        byte value = BufferedData[_lastByte];
        _lastByte++;
        return value;
    }

    public byte[] Read(int length)
    {
        byte[] buff = new byte[length];

        Array.Copy(BufferedData, _lastByte, buff, 0, length);
        _lastByte += length;

        return buff;
    }

    public int ReadInt()
    {
        byte[] data = Read(4);
        int value = BitConverter.ToInt32(data, 0);

        return IPAddress.NetworkToHostOrder(value);
    }

    public float ReadFloat()
    {
        byte[] data = Read(4);
        float value = BitConverter.ToSingle(data, 0);

        return NetworkToHostOrder(value);
    }

    public bool ReadBool()
    {
        return ReadByte() == 1;
    }

    public double ReadDouble()
    {
        byte[] data = Read(8);

        return NetworkToHostOrder(data);
    }

    public int ReadVarInt()
    {
        var value = 0;
        var size = 0;
        int b;
        while (((b = ReadByte()) & 0x80) == 0x80)
        {
            value |= (b & 0x7F) << (size++*7);
            if (size > 5)
            {
                throw new IOException("VarInt too long.");
            }
        }
        return value | ((b & 0x7F) << (size*7));
    }

    public long ReadVarLong()
    {
        var value = 0;
        var size = 0;
        int b;
        while (((b = ReadByte()) & 0x80) == 0x80)
        {
            value |= (b & 0x7F) << (size++*7);
            if (size > 10)
            {
                throw new IOException("VarLong too long.");
            }
        }
        return value | ((b & 0x7F) << (size*7));
    }

    public short ReadShort()
    {
        byte[] data = Read(2);
        short value = BitConverter.ToInt16(data);

        return IPAddress.NetworkToHostOrder(value);
    }

    public ushort ReadUShort()
    {
        byte[] data = Read(2);

        return NetworkToHostOrder(BitConverter.ToUInt16(data));
    }

    public string ReadString()
    {
        int length = ReadVarInt();
        byte[] data = Read(length);

        return Encoding.UTF8.GetString(data);
    }

    public long ReadLong()
    {
        return IPAddress.NetworkToHostOrder(BitConverter.ToInt64(Read(8)));
    }

    private double NetworkToHostOrder(byte[] data)
    {
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(data);
        }
        return BitConverter.ToDouble(data, 0);
    }

    private float NetworkToHostOrder(float network)
    {
        var bytes = BitConverter.GetBytes(network);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return BitConverter.ToSingle(bytes, 0);
    }

    private ushort[] NetworkToHostOrder(ushort[] network)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(network);
        return network;
    }

    private ushort NetworkToHostOrder(ushort network)
    {
        var net = BitConverter.GetBytes(network);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(net);
        return BitConverter.ToUInt16(net, 0);
    }


    private readonly List<byte> _bffr = new List<byte>();

	public byte[] ExportWriter
	{
		get { return _bffr.ToArray(); }
	}

    //Writing methods
    public void Write(byte[] data)
    {
        _bffr.AddRange(data);
    }

    public void WriteVarInt(int integer)
    {
        while ((integer & -128) != 0)
        {
            _bffr.Add((byte) (integer & 127 | 128));
            integer = (int) (((uint) integer) >> 7);
        }
        _bffr.Add((byte) integer);
    }

    public void WriteVarLong(long i)
    {
        var f = i;
        while ((f & ~0x7F) != 0)
        {
            _bffr.Add((byte) ((f & 0x7F) | 0x80));
            f >>= 7;
        }
        _bffr.Add((byte) f);
    }

    public void WriteInt(int data)
    {
        var buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data));
        Write(buffer);
    }

    public void WriteString(string data)
    {
        var stringData = Encoding.UTF8.GetBytes(data);
        WriteVarInt(stringData.Length);
        Write(stringData);
    }

    public void WriteShort(short data)
    {
        var shortData = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data));
        Write(shortData);
    }

    public void WriteUShort(ushort data)
    {
        var uShortData = BitConverter.GetBytes(data);
        Write(uShortData);
    }

    public void WriteByte(byte data)
    {
        _bffr.Add(data);
    }

    public void WriteBool(bool data)
    {
        Write(BitConverter.GetBytes(data));
    }

    public void WriteDouble(double data)
    {
        Write(HostToNetworkOrder(data));
    }

    public void WriteFloat(float data)
    {
        Write(HostToNetworkOrder(data));
    }

    public void WriteLong(long data)
    {
        Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data)));
    }

    public void WriteUuid(Guid uuid)
    {
        var guid = uuid.ToByteArray();
        var long1 = new byte[8];
        var long2 = new byte[8];
        Array.Copy(guid, 0, long1, 0, 8);
        Array.Copy(guid, 8, long2, 0, 8);
        Write(long1);
        Write(long2);
    }

    private byte[] GetVarIntBytes(int integer)
    {
        List<Byte> bytes = new List<byte>();
        while ((integer & -128) != 0)
        {
            bytes.Add((byte)(integer & 127 | 128));
            integer = (int)(((uint)integer) >> 7);
        }
        bytes.Add((byte)integer);
        return bytes.ToArray();
    }

            private byte[] HostToNetworkOrder(double d)
    {
        var data = BitConverter.GetBytes(d);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(data);

        return data;
    }

    private byte[] HostToNetworkOrder(float host)
    {
        var bytes = BitConverter.GetBytes(host);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return bytes;
    }
}