namespace NWNCommandWriter
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;
    using NWNCommandWriter.Config;

    public class CommandRunner
    {
        public void Execute(AppConfig config, Process nwnProcess, CommandQueue queue)
        {
            nwnProcess.TrySetFocus();

            foreach (Command command in queue.Commands)
            {
                switch (command.CommandType)
                {
                    case CommandType.NewLine:
                        SendEnter();
                        break;
                    case CommandType.Text:
                        SendString(command.Text);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                Thread.Sleep(config.CommandDelay);
            }
        }

        private void SendEnter()
        {
            List<INPUT> inputs = new List<INPUT>();
            inputs.Add(GetInput(0x01C, KEYEVENTF_SCANCODE, false));
            inputs.Add(GetInput(0x01C, KEYEVENTF_SCANCODE, true));
            
            SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }
        
        private void SendString(string str)
        {
            List<INPUT> inputs = new List<INPUT>();
            
            foreach (char c in str)
            {
                // Prepare a key down, then a key up.
                inputs.Add(GetInput(c, KEYEVENTF_UNICODE, false));
                inputs.Add(GetInput(c, KEYEVENTF_UNICODE, true));
            }

            // Send all inputs together using a Windows API call.
            SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }

        private INPUT GetInput(ushort c, uint flags, bool keyUp)
        {
            // INPUT is a multi-purpose structure which can be used 
            // for synthesizing keystrokes, mouse motions, and button clicks.
            return new INPUT
            {
                // Need a keyboard event.
                type = INPUT_KEYBOARD,
                u = new InputUnion
                {
                    // KEYBDINPUT will contain all the information for a single keyboard event
                    // (more precisely, for a single key-down or key-up).
                    ki = new KEYBDINPUT
                    {
                        // Virtual-key code must be 0 since we are using scan codes.
                        wVk = 0,
                        wScan = c,
                        dwFlags = flags | (keyUp ? KEYEVENTF_KEYUP : 0),
                        dwExtraInfo = GetMessageExtraInfo(),
                    }
                }
            };
        }

        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_SCANCODE = 0x0008;
        const uint XBUTTON1 = 0x0001;
        const uint XBUTTON2 = 0x0002;
        const uint MOUSEEVENTF_MOVE = 0x0001;
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        const uint MOUSEEVENTF_XDOWN = 0x0080;
        const uint MOUSEEVENTF_XUP = 0x0100;
        const uint MOUSEEVENTF_WHEEL = 0x0800;
        const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
        const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        private struct INPUT
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            /*Virtual Key code.  Must be from 1-254.  If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0.*/
            public ushort wVk;

            /*A hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which is to be sent to the foreground application.*/
            public ushort wScan;

            /*Specifies various aspects of a keystroke.  See the KEYEVENTF_ constants for more information.*/
            public uint dwFlags;

            /*The time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp.*/
            public uint time;

            /*An additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information.*/
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
    }
}