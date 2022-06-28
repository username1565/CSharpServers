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

            Console.Write("Введите свое имя:");
            string userName = Console.ReadLine();

      try
      {
        udpClient = new UdpClient();
        udpClient.Connect(IPAddress.Loopback, port);

        tcpClient = new TcpClient();
        tcpClient.Connect(IPAddress.Loopback, port);
		
		bool udp = false;	//true - udp, false - tcp;

        while(run)
        {
		
          Console.WriteLine("Press 'T' for TCP sending, 'U' for UDP sending or 'CTRL+C' to exit.");
          key = Console.ReadKey(true);
		
          switch (key.Key)
          {
            case ConsoleKey.U:
				Console.WriteLine("UDP protocol selected");
				udp = true;
				break;
			case ConsoleKey.T:
				Console.WriteLine("TCP protocol selected");
				udp = false;
				break;
		  }

			string	message = Console.ReadLine();
			message = String.Format("{0}, ({1}): {2}", DateTime.Now.ToString("HH:mm:ss.fff"), userName, message);
			//buffer = Encoding.ASCII.GetBytes(DateTime.Now.ToString("HH:mm:ss.fff"));
			buffer = Encoding.Unicode.GetBytes(message);
			

		if(udp == true){
			udpClient.Send(buffer, buffer.Length);

			IPEndPoint remoteEP = null; // адрес входящего подключения
			buffer   = udpClient.Receive(ref remoteEP);

			if (buffer != null && buffer.Length > 0)
			{
				//Console.WriteLine("UDP: " + Encoding.ASCII.GetString(buffer));
				Console.WriteLine("UDP - " + Encoding.Unicode.GetString(buffer));
			}			  
		}
		else{
              if (tcpStream == null)
                tcpStream = tcpClient.GetStream();

              tcpStream.Write(buffer, 0, buffer.Length);
			  
                    // получаем ответ
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = tcpStream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (tcpStream.DataAvailable);
 
                    message = builder.ToString();
                    Console.WriteLine("TCP - " + message);
			  
		}
		
/*
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
*/		  
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