namespace NWNCommandWriter
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public static class ProcessUtils
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr handle);
        
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        
        public static void TrySetFocus(this Process process)
        {
            IntPtr handle = process.MainWindowHandle;
            ShowWindow(handle, 9); // SW_RESTORE
            SetForegroundWindow(handle);
        }
    }
}