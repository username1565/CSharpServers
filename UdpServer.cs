using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

//UDP-server
namespace UDP
{
	partial class Server
	{
		//Start UDP-server
		public static void Start(
			string [] args
		)
		{
			IPAddress	UdpServerIP		=	IPAddress.Any;
			int			UdpServerPort	=	8081;
			
			if(args.Length == 1){
				UdpServerIP = IPAddress.Any;
				UdpServerPort = System.Int32.Parse(args[0]);
			}
			else if(args.Length == 2){
				UdpServerIP = IPAddress.Parse(args[0]);
				UdpServerPort = System.Int32.Parse(args[1]);
			}
		
			//Start UDP-client
			UdpClient udpServer = new UdpClient();
			IPEndPoint ep = new IPEndPoint(UdpServerIP, UdpServerPort); // endpoint where server is listening (testing localy)
			udpServer.Client.Bind(ep);
			Start(udpServer);
		}

		//Create UdpClient for UDPServer, and return this.
		public static UdpClient UDPServer(
				string UdpServerIP
			,	int UdpServerPort
		){
			UdpClient udpServer = new UdpClient();
			IPEndPoint ep = new IPEndPoint(IPAddress.Parse( UdpServerIP ), UdpServerPort ); // endpoint where server is listening (testing localy)
			udpServer.Client.Bind(ep);
			return udpServer;
		}

	
		//Start UDPServer with specified UdpClient, in separate thread.
		public static void Start(
				UdpClient udpServer
		)
		{
			//start new thread on accept UDP-datagramm
			var udpThread          = new Thread(new ParameterizedThreadStart(UDPServerProc));
			
			//run this in background
			udpThread.IsBackground = true;
			
			//set thread-name
			udpThread.Name         = "UDP server thread";
			
			//start thread
			udpThread.Start(udpServer);
		}
	}
}
