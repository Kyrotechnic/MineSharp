namespace MineSharp.Api;

public class Logger
{
    public bool isDebug;
    public Logger(bool debug)
    {
        isDebug = debug;
    }

    public Logger()
    {
        isDebug = false;
    }

    public void Log(string line) => Console.WriteLine(line);
    public void Fine(string line) => Console.WriteLine("[FINE] " + line);
    public void Debug(string line)
    {
        if (isDebug)
            Console.WriteLine("[DEBUG] " + line);
    }

    public void Info(string line)
    {
        Console.WriteLine("[INFO] " + line);
    }
}