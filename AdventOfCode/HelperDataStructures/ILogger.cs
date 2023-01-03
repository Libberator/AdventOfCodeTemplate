using System;

namespace AoC;

public interface ILogger
{
    public string? LastMessage { get; }
    public void Log(string? msg);
    public void Log(int msg) => Log(msg.ToString());
    public void Log(long msg) => Log(msg.ToString());
    public void Log(float msg) => Log(msg.ToString());
    public void Log(double msg) => Log(msg.ToString());
    public void Log(char msg) => Log(msg.ToString());
    public void Log(object msg) => Log(msg.ToString());
}

public class ConsoleLogger : ILogger
{
    public string? LastMessage { get; private set; }

    public void Log(string? msg)
    {
        LastMessage = msg;
        Console.WriteLine(msg);
    }
}

// TODO: maybe add a Write to Local Disk Logger?