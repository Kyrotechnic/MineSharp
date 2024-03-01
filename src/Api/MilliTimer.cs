namespace MineSharp.Api;

public class MilliTimer
{
    public long current {get => DateTimeOffset.Now.ToUnixTimeMilliseconds();}
    public long last;
    public void Start()
    {
        last = current;
    }

    public long Elapsed()
    {
        return current - last;
    }

    public long Reset()
    {
        long store = last;

        last = current;

        return current - store;
    }
}