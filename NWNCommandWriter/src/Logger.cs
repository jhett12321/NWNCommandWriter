namespace NWNCommandWriter
{
    using System;
    using System.Diagnostics;

    public static class Logger
    {
        [Conditional("DEBUG")]
        public static void LogDebug(string msg) => Log("Debug", msg, ConsoleColor.White);
        public static void LogInfo(string msg) => Log("Info", msg, ConsoleColor.Gray);
        public static void LogWarning(string msg) => Log("Warning", msg, ConsoleColor.DarkYellow);
        public static void LogError(string msg) => Log("Error", msg, ConsoleColor.DarkRed);

        public static void LogExceptionWarning(string msg, Exception e) => Log("Warning", $"{msg}\n{e}", ConsoleColor.DarkYellow);
        public static void LogExceptionError(string msg, Exception e) => Log("Error", $"{msg}\n{e}", ConsoleColor.DarkRed);


        private static void Log(string logLevel, string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now:ddd MMM d HH:mm:ss}][{logLevel}] {message}");
            Console.ResetColor();
        }
    }
}