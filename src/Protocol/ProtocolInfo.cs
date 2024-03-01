namespace MineSharp.Protocol;

public class ProtocolInfo
{
    private int minProtocol;
    private int maxProtocol;
    private int reccomendedProtocol;
    public ProtocolInfo()
    {

    }

    public ProtocolInfo SetReccomendedProtocolVersion(int version)
    {
        reccomendedProtocol = version;

        return this;
    }

    public ProtocolInfo SetMinimumProtocolVersion(int minimumVersion)
    {
        minProtocol = minimumVersion;

        return this;
    }

    public ProtocolInfo SetMaximumProtocolVersion(int maximumVersion)
    {
        maxProtocol = maximumVersion;

        return this;
    }

    public int GetMaximumProtocolVersion() => maxProtocol;
    public int GetMinimumProtocolVersion() => minProtocol;
    public int GetReccomendedProtocolVersion() => reccomendedProtocol;
}