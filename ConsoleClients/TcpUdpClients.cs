using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPUDPClient
{
  class Program
  {
    static void Main(string[] args)
    {
      UdpClient      udpClient = null;
      TcpClient      tcpClient = null;
      NetworkStream  tcpStream = null;
      int            port      = 8081;
      ConsoleKeyInfo key;
      bool           run = true;
      byte[]         buffer;

      Console.WriteLine(string.Format("Starting TCP and UDP clients on port {0}...", port));

			//program accept port, in args[0]
			if(args.Length>=1){
				int TryGetPort = 0;
				if(int.TryParse(args[0], out TryGetPort))
				{
					port = TryGetPort;
				}
				Console.WriteLine("port: "+port);
			}

      try
      {
        udpClient = new UdpClient();
        udpClient.Connect(IPAddress.Loopback, port);

        tcpClient = new TcpClient();
        tcpClient.Connect(IPAddress.Loopback, port);

        while(run)
        {
          Console.WriteLine("Press 'T' for TCP sending, 'U' for UDP sending or 'X' to exit.");
          key = Console.ReadKey(true);

          switch (key.Key)
          {
            case ConsoleKey.X:
              run = false;
              break;

            case ConsoleKey.U:
              buffer = Encoding.ASCII.GetBytes(DateTime.Now.ToString("HH:mm:ss.fff"));
              udpClient.Send(buffer, buffer.Length);

			IPEndPoint remoteEP = null; // адрес входящего подключения
          buffer   = udpClient.Receive(ref remoteEP);

          if (buffer != null && buffer.Length > 0)
          {
            Console.WriteLine("UDP: " + Encoding.ASCII.GetString(buffer));
          }			  
              break;

            case ConsoleKey.T:
              buffer = Encoding.ASCII.GetBytes(DateTime.Now.ToString("HH:mm:ss.fff"));

              if (tcpStream == null)
                tcpStream = tcpClient.GetStream();

              tcpStream.Write(buffer, 0, buffer.Length);
            break;
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Main exception: " + ex);
      }
      finally
      { 
        if(udpClient != null)
          udpClient.Close();

        if(tcpStream != null)
          tcpStream.Close();

        if(tcpClient != null)
          tcpClient.Close();
      }

      Console.WriteLine("Press <ENTER> to exit.");
      Console.ReadLine();
    }
  }
}