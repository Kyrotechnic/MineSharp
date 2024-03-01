namespace MineSharp.Api;

public static class Extensions
{
    public static byte[] GetVarIntBytes(this int integer)
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
}