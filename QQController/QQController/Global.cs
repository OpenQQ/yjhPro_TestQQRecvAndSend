using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using QQController.Common;

namespace QQController
{
    public static class Global
    {
        [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")] private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")] private static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")] private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        [DllImport("user32.dll")] private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);
        //ShowWindow参数
        private const int SW_SHOWNORMAL = 1;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWNOACTIVATE = 4;
        //SendMessage参数
        private const int WM_KEYDOWN = 0X100;
        private const int WM_KEYUP = 0X101;
        private const int WM_SYSCHAR = 0X106;
        private const int WM_SYSKEYUP = 0X105;
        private const int WM_SYSKEYDOWN = 0X104;
        private const int WM_CHAR = 0X102;
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWindowsProc ewp, int lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int PostMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        public static EnumWindowsProc Ewp = delegate(IntPtr hWnd, int param)
        {
            int pid;
            GetWindowThreadProcessId(hWnd, out pid);
            if (QqProcessDictionary.ContainsValue(pid))
            {
                MessageBox.Show(hWnd.ToString(), "11", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return true;
        };

        // 存qq->ProcessId，Handle, Pipe
        public static Dictionary<string, int> QqProcessDictionary = new Dictionary<string, int>();

        public static string AirPath = ConfigurationManager.AppSettings.Get("AirPath");

        public static void CreateQQProcess(string qq)
        {
            while (true)
            {
                IntPtr hIntPtr = FindWindow("CQUI", "酷Q 5.11.10A (180130)");
                if (hIntPtr != IntPtr.Zero)
                {
                    int pid = 0;
                    GetWindowThreadProcessId(hIntPtr, out pid);
                    if (pid != 0)
                    {
                        var process = Process.GetProcessById(pid);
                        if (process != null)
                        {
                            process.Kill();
                            process.Close();
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            
            Process p = new Process();
            p.StartInfo.FileName = Path.Combine(AirPath, "CQA.exe");
            p.StartInfo.UseShellExecute = false;
            p.Start();
            if (QqProcessDictionary.ContainsKey(qq))
            {
                var process = Process.GetProcessById(QqProcessDictionary[qq]);
                if (process != null)
                {
                    process.Kill();
                    process.Close();
                }
            }
            QqProcessDictionary[qq] = p.Id;
        }

        public static void LoginQQ(string qq, string pwd, Form2 form2)
        {
            var pipe = new PipeServer("pipe_" + qq);
            pipe.MessageReceviceEvent += Pipe_MessageReceviceEvent;
            pipe.ReceviceMessage();
            int pid = QqProcessDictionary[qq];
            IntPtr hIntPtr = IntPtr.Zero;
            var t = DateTime.Now;
            while (hIntPtr == IntPtr.Zero)
            {
                hIntPtr = FindWindow("CQUI", "酷Q 5.11.10A (180130)");
                if (DateTime.Now - t > new TimeSpan(0, 0, 10))
                {
                    break;
                }
            }
            
            MoveWindow(hIntPtr, -1000, -1000, 500, 400, true);
            ShowWindow(hIntPtr, SW_RESTORE);
            SetForegroundWindow(hIntPtr);
            Thread.Sleep(50);
            SendMessage(hIntPtr, WM_SYSKEYDOWN, 0X09, 0);
            Thread.Sleep(50);
            SendMessage(hIntPtr, WM_SYSKEYDOWN, 0X09, 0);
            Thread.Sleep(50);
            InputStr(hIntPtr, qq);
            Thread.Sleep(50);
            SendMessage(hIntPtr, WM_SYSKEYDOWN, 0X09, 0);
            Thread.Sleep(50);
            InputStr(hIntPtr, pwd);
            Thread.Sleep(50);
            SendMessage(hIntPtr, WM_SYSKEYDOWN, 0X09, 0);
            Thread.Sleep(50);
            SendMessage(hIntPtr, WM_SYSKEYDOWN, 0X0D, 0);
            Thread.Sleep(50);
            SendMessage(hIntPtr, WM_SYSKEYUP, 0X0D, 0);
            Thread.Sleep(3000);
            hIntPtr = FindWindow("CQUI", "酷Q 5.11.10A (180130)");
            if (hIntPtr != IntPtr.Zero)
            {
                var p = Process.GetProcessById(pid);
                if (p != null)
                {
                    p.Kill();
                    p.Close();
                }
                pipe.Dispose();
                QqProcessDictionary.Remove(qq);
            }
            else
            {
                // 登陆成功
                form2.UpdateStatusDesc(qq, "登陆成功");
                
            }
        }

        private static void Pipe_MessageReceviceEvent(string msg)
        {
            Console.WriteLine(msg);
        }

        /// <summary>
        /// 发送一个字符串
        /// </summary>
        /// <param name="myIntPtr">窗口句柄</param>
        /// <param name="Input">字符串</param>
        public static void InputStr(IntPtr myIntPtr, string Input)
        {
            byte[] ch = (ASCIIEncoding.ASCII.GetBytes(Input));
            for (int i = 0; i < ch.Length; i++)
            {
                SendMessage(myIntPtr, WM_CHAR, ch[i], 0);
            }
        }
    }
}
