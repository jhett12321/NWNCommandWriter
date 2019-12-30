namespace NWNCommandWriter
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using NWNCommandWriter.Config;

    internal class Program
    {
        private const string WINDOW_TITLE = "Command Writer";
        
        private static AppConfig appConfig = new AppConfig();
        private static CommandRunner commandRunner = new CommandRunner();
        private static string tmpFileName = null;
        
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        private static EventHandler onConsoleCtl;

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool OnConsoleCtrl(CtrlType sig)
        {
            Dispose();
            return true;
        }

        private static void Dispose()
        {
            File.Delete(tmpFileName);
        }

        public static int Main()
        {
            onConsoleCtl += OnConsoleCtrl;
            SetConsoleCtrlHandler(onConsoleCtl, true);
            Console.Title = WINDOW_TITLE;
            
            appConfig.Deserialize();
            
            tmpFileName = Path.GetTempPath() + Guid.NewGuid().ToString("N") + ".txt";
            File.Create(tmpFileName).Dispose();

            Logger.LogInfo("Write, or copy your full message into the opened text document");

            Process.Start(tmpFileName).WaitForInputIdle();
            Process.GetCurrentProcess().TrySetFocus();

            Logger.LogInfo("---Press [ENTER] to validate the specified text---");
            WaitForEnter();
            
            string rawText = File.ReadAllText(tmpFileName);

            Process nwnProcess = null;
            while (nwnProcess == null || rawText.Length > appConfig.TextLimit)
            {
                appConfig.Deserialize();
                rawText = File.ReadAllText(tmpFileName);
                nwnProcess = Process.GetProcessesByName(appConfig.ProcessName).FirstOrDefault();

                if (rawText.Length > appConfig.TextLimit)
                {
                    Logger.LogError("Specified text exceeds the item limit. Modify the text, or change the configured limit to continue.");
                    Logger.LogInfo("Press Enter to retry.");
                    Process.Start(tmpFileName);
                    WaitForEnter();
                    continue;
                }
                
                if (nwnProcess == null)
                {
                    Logger.LogError("Could not find a NWN process! Ensure the game is running, or change the process name in config.json.");
                    Logger.LogInfo("Press Enter to retry.");
                    WaitForEnter();
                    continue;
                }
            }
            
            Logger.LogInfo("---Validation succeeded. Ensure the writing target is selected using the writing widget, then return to this window and press [ENTER].---");
            WaitForEnter();

            CommandQueue commandQueue = new CommandQueue(appConfig, rawText);
            commandRunner.Execute(appConfig, nwnProcess, commandQueue);

            Dispose();
            return 0;
        }

        private static void WaitForEnter()
        {
            while (Console.ReadKey().Key != ConsoleKey.Enter) {}
        }
    }
}