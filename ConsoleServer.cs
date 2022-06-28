using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPUDPServer
{
    public class ClientObject
    {
        public TcpClient client;
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
        }
 
        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64]; // буфер для получаемых данных
                while (true)
                {
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
 
                    string message = builder.ToString();
 
                    Console.WriteLine("TCP: " + message);
                    // отправляем обратно сообщение в верхнем регистре
                    //message = message.Substring(message.IndexOf(':') + 1).Trim().ToUpper();
                    message = message.Substring(message.IndexOf(':') + 1).Trim();
                    data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
	
  public class TCPUDPServer
  {
	public static Encoding latin1 = Encoding.GetEncoding("ISO-8859-1");	//binary encoding can encode all bytes as string, and decode it back

	public static void Main(string[] args)
    {
	
      TcpListener tcpServer = null;
      UdpClient   udpServer = null;
      int         port      = 8082;

			//program accept port, in args[0]
			if(args.Length>=1){
				int TryGetPort = 0;
				if(int.TryParse(args[0], out TryGetPort))
				{
					port = TryGetPort;
				}
				Console.WriteLine("port: "+port);
			}
	  
      Console.WriteLine(string.Format("Starting TCP and UDP servers on port {0}...", port));

      try
      {
        udpServer = new UdpClient(port);
        tcpServer = new TcpListener(IPAddress.Any, port);

        var udpThread          = new Thread(new ParameterizedThreadStart(UDPServerProc));
        udpThread.IsBackground = true;
        udpThread.Name         = "UDP server thread";
        udpThread.Start(udpServer);

        var tcpThread          = new Thread(new ParameterizedThreadStart(TCPServerProc));
        tcpThread.IsBackground = true;
        tcpThread.Name         = "TCP server thread";
        tcpThread.Start(tcpServer);

        Console.WriteLine("Press <ENTER> to stop the servers.");
        Console.ReadLine();
      }
      catch (Exception ex)
      {
        Console.WriteLine("Main exception: " + ex);
      }
      finally
      {
        if (udpServer != null)
          udpServer.Close();

        if (tcpServer != null)
          tcpServer.Stop();
      }

      Console.WriteLine("Press <ENTER> to exit.");
      Console.ReadLine();
    }

    private static void UDPServerProc(object arg)
    {
      Console.WriteLine("UDP server thread started");

      try
      {
        UdpClient server = (UdpClient)arg;
        IPEndPoint remoteEP;
        byte[] buffer;

        for(;;)
        {
          remoteEP = null;
          buffer   = server.Receive(ref remoteEP);

          if (buffer != null && buffer.Length > 0)
          {
            //Console.WriteLine("UDP: " + Encoding.ASCII.GetString(buffer));
			Console.WriteLine("UDP: " + Encoding.Unicode.GetString(buffer));
			server.Send(buffer, buffer.Length, remoteEP.Address.ToString(), remoteEP.Port); // отправка
          }
        }
      }
      catch (SocketException ex)
      {
        if(ex.ErrorCode != 10004) // unexpected
          Console.WriteLine("UDPServerProc exception: " + ex);
      }
      catch (Exception ex)
      {
        Console.WriteLine("UDPServerProc exception: " + ex);
      }

      Console.WriteLine("UDP server thread finished");
    }

    private static void TCPServerProc(object arg)
    {
      Console.WriteLine("TCP server thread started");

      try
      {
        TcpListener server = (TcpListener)arg;
        //byte[]      buffer = new byte[2048];
        //int         count; 

        server.Start();

        for(;;)
        {
          TcpClient client = server.AcceptTcpClient();

/*
          using (var stream = client.GetStream())
          {
            while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
              Console.WriteLine("TCP: " + Encoding.ASCII.GetString(buffer, 0, count));
            }
          }
          client.Close();
*/
                    ClientObject clientObject = new ClientObject(client);	//and start work with client
 
                    // создаем новый поток для обслуживания нового клиента
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));	//in new thread
                    clientThread.Start();														//start thread

        }
      }
      catch (SocketException ex)
      {
        if (ex.ErrorCode != 10004) // unexpected
          Console.WriteLine("TCPServerProc exception: " + ex);
      }
      catch (Exception ex)
      {
        Console.WriteLine("TCPServerProc exception: " + ex);
      }

      Console.WriteLine("TCP server thread finished");
    }
  }
}