namespace CRM;
public static class Log
{
    public static bool Enabled = true; 

    public static void Info(string message)
    {
        if (!Enabled) return;
        Console.WriteLine($"[INFO] {message}");
    }

    public static void Error(string message)
    {
        if (!Enabled) return;
        Console.WriteLine($"[ERROR] {message}");
    }
}
