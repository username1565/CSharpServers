using System;
using System.Net;
using System.Net.Sockets;

namespace Program
{
	partial class Program
	{
		static void Main21(string[] args)
		{		
			string	TcpUdpServerIP = "127.0.0.1";
			int		TcpUdpServerPort = 8081;
			string	UdpMultiCastGroupIP = "235.5.5.11";
			
			if(args.Length >= 3){
				TcpUdpServerIP = args[0];
				TcpUdpServerPort = System.Int32.Parse(args[1]);
				UdpMultiCastGroupIP = args[2];
			}

			try{

				//Start TCP Servers
				TCP.Server.Start(new string[]{TcpUdpServerIP, TcpUdpServerPort.ToString()});
				TCP.Server.Start(new string[]{TcpUdpServerIP, (TcpUdpServerPort+1).ToString()});

				//Start UDP Servers
				UDP.Server.Start(TcpUdpServerIP, TcpUdpServerPort, UdpMultiCastGroupIP);
				UDP.Server.Start(TcpUdpServerIP, TcpUdpServerPort+1, UdpMultiCastGroupIP);


				Console.WriteLine("Test TCP: ");
				//		Start TCP clients
				string response;
				byte[] ResponseBytes;
				//	Connect, send request, receive response, then disconnect
				response				=	TCP.Client.Send(TcpUdpServerIP, TcpUdpServerPort, "send and close");
				ResponseBytes			=	TCP.Client.Send(TcpUdpServerIP, TcpUdpServerPort, new byte[]{0,1,2,3,4,5});
			
				//	Connect tcpClient, and keep connection alive
				TcpClient tcpClient 	=	TCP.Client.Connect(TcpUdpServerIP, TcpUdpServerPort);
				//	Send requests using connected tcpClient, and receive responses
				response				=	TCP.Client.Send(tcpClient, "test");
				ResponseBytes			=	TCP.Client.Send(tcpClient, new byte[]{0,1,2,3,4,5});

				Console.WriteLine("Test UDP: ");

				//		Start UDP clients
	//			UDP.Client udpClient2 = new UDP.Client(ServerUdpIP, ServerUdpPort+1, UdpMultiCastGroupIP);

				UDP.Client udpClient = new UDP.Client(TcpUdpServerIP, TcpUdpServerPort, UdpMultiCastGroupIP);
				udpClient.Send("test");
				udpClient.Send(new byte[]{0,1,2,3,4,5});

				UDP.Client udpClient2 = new UDP.Client(TcpUdpServerIP, TcpUdpServerPort+1, UdpMultiCastGroupIP);
				udpClient2.Send("test");
				udpClient2.Send(new byte[]{0,1,2,3,4,5});
			}
			catch(Exception ex){
				Console.WriteLine(ex);
			}
			System.Console.ReadLine();
		}
	}
}