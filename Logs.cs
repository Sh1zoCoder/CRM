namespace CRM;
public static class Logs
{
    public static bool Enabled = false; 

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
