using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QQController.Common
{
    public class PipeServer:IDisposable
    {
        [DllImport("kernel32.dll", SetLastError = true)]

        public static extern IntPtr CreateNamedPipe

(

String lpName,                                          // pipe name

uint dwOpenMode,                                   // pipe open mode

uint dwPipeMode,                                   // pipe-specific modes

uint nMaxInstances,                                // maximum number of instances

uint nOutBufferSize,                           // output buffer size

uint nInBufferSize,                                // input buffer size

uint nDefaultTimeOut,                          // time-out interval

IntPtr pipeSecurityDescriptor        // SD

);



        [DllImport("kernel32.dll", SetLastError = true)]

        public static extern bool ConnectNamedPipe

        (

        IntPtr hHandle,                                         // handle to named pipe

        Overlapped lpOverlapped                   // overlapped structure

        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void CloseHandle(IntPtr hpipe);

        [DllImport("kernel32.dll", SetLastError = true)]

        public static extern IntPtr CreateFile

        (

        String lpFileName,                          // file name

        uint dwDesiredAccess,                       // access mode

        uint dwShareMode,                                  // share mode

        SecurityAttribute attr,                  // SD

        uint dwCreationDisposition,          // how to create

        uint dwFlagsAndAttributes,           // file attributes

        uint hTemplateFile);                      // handle to template file



        [DllImport("kernel32.dll", SetLastError = true)]

        public static extern bool ReadFile

        (

        IntPtr hHandle,    // handle to file

        byte[] lpBuffer,// data buffer字节流

        uint nNumberOfBytesToRead,// number of bytes to read

        out int len,// number of bytes read

        uint lpOverlapped// overlapped buffer

        );



        [DllImport("kernel32.dll", SetLastError = true)]

        public static extern bool WriteFile

        (

        IntPtr hHandle,    // handle to file

        byte[] lpBuffer,// data buffer字节流

        uint nNumberOfBytesToWrite, // number of bytes to write

        byte[] lpNumberOfBytesWritten,   // number of bytes written

        uint lpOverlapped // overlapped buffer

        );

        public const uint PIPE_ACCESS_DUPLEX = 0x00000003;

        public const uint PIPE_ACCESS_OUTBOUND = 0x00000002;

        public const uint PIPE_TYPE_BYTE = 0x00000000;

        public const uint PIPE_TYPE_MESSAGE = 0x00000004;

        public string Name { set; get; }

        private IntPtr hPipe;

        public PipeServer(string name)
        {
            Name = "\\\\.\\Pipe\\pipe_"+name;
            hPipe = CreateNamedPipe(Name, PIPE_ACCESS_DUPLEX, PIPE_TYPE_BYTE, 100, 1024, 1024, 0, IntPtr.Zero);
        }

        ~PipeServer()
        {
            lock (this)

            {

                if (hPipe != (IntPtr)(-1))

                {

                    CloseHandle(hPipe);

                    hPipe = (IntPtr)(-1);

                }

            }
        }

        public delegate void MessageReceiveCallBack(string msg);

        public event MessageReceiveCallBack MessageReceviceEvent;

        public void ReceviceMessage()
        {
            new Thread(() =>
            {
                try
                {
                    byte[] buf = new byte[1024];
                    int len = 0;
                    int messageLen = 0; // 前四个字节
                    int hasReadIndex = 0;
                    byte[] frame = null;
                    while (true)
                    {
                        ReadFile(hPipe, buf, 1024, out len, 0);
                        if (len <= 0)
                        {
                            continue;
                        }

                        for (int i = 0; i < len; i++)
                        {
                            if (hasReadIndex >= 0 && hasReadIndex < 4)
                            {
                                messageLen += buf[i] << (8 * hasReadIndex);
                                hasReadIndex++;
                            }
                            else
                            {
                                if (frame == null)
                                {
                                    frame = new byte[messageLen];
                                }

                                frame[hasReadIndex - 4] = (byte) buf[i];
                                hasReadIndex++;
                                if (hasReadIndex - messageLen == 4)
                                {
                                    hasReadIndex = 0;
                                    messageLen = 0;
                                    MessageReceviceEvent(Encoding.UTF8.GetString(frame));
                                    frame = null;
                                }
                            }
                        }
                    }
                }
                catch (Exception){}
            })
            {
                IsBackground = true
            }.Start();
        }

        public void Dispose()
        {
            lock (this)

            {

                if (hPipe != (IntPtr)(-1))

                {

                    CloseHandle(hPipe);

                    hPipe = (IntPtr)(-1);

                }

            }
        }
    }
}
