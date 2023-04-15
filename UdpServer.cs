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
			IPAddress	UdpServerIP			=	null;
			int			UdpServerPort		=	0;
			string		MultiCastGroupIP	=	null;
			
			if(args.Length == 1){
				UdpServerIP = IPAddress.Any;
				UdpServerPort = System.Int32.Parse(args[0]);
			}
			else if(args.Length == 2){
				UdpServerIP = IPAddress.Parse(args[0]);
				UdpServerPort = System.Int32.Parse(args[1]);
			}
			else if(args.Length == 3){
				UdpServerIP = IPAddress.Parse(args[0]);
				UdpServerPort = System.Int32.Parse(args[1]);
				MultiCastGroupIP = args[2];
			}
			else{
				UdpServerIP = IPAddress.Any;
				UdpServerPort = 8081;
			}
		
			//Start UDP-client
			UdpClient udpServer = UDPServer(UdpServerIP.ToString(), UdpServerPort,  MultiCastGroupIP);
			Start(udpServer, MultiCastGroupIP);
		}

		//Create UdpClient for UDPServer, and return this.
		public static UdpClient UDPServer(
				string UdpServerIP
			,	int UdpServerPort
			,	string MultiCastGroupIP = null
		){
			UdpClient udpServer = null;
			
			udpServer = new UdpClient();
			IPEndPoint ep = new IPEndPoint(IPAddress.Parse( UdpServerIP ), UdpServerPort ); // endpoint where server is listening (testing localy)
			udpServer.Client.Bind(ep);

			if(MultiCastGroupIP != null){
				//Join this with multicast-group
				udpServer.Client.MulticastLoopback = true;
				udpServer.JoinMulticastGroup(IPAddress.Parse(MultiCastGroupIP));
			}
			else{
				udpServer.Client.MulticastLoopback = false;
			}
			return udpServer;
		}

	
		//Start UDPServer with specified UdpClient, in separate thread.
		public static void Start(
				UdpClient udpServer
			,	string MultiCastGroupIP = null
		)
		{
			//set parameters
			object[] parameters = new object[]{udpServer, MultiCastGroupIP};
			
			//start new thread on accept UDP-datagramm
			var udpThread          = new Thread(new ParameterizedThreadStart(UDPServerProc));
			
			//run this in background
			udpThread.IsBackground = true;
			
			//set thread-name
			udpThread.Name         = "UDP server thread";
			
			//start thread
			udpThread.Start(parameters);
		}
		
		//Start UDPServer with specified UdpClient, in separate thread.
		public static void Start(
				string IP
			,	int port
			,	string MultiCastGroupIP = null
		)
		{
			UdpClient server = UDPServer(IP, port, MultiCastGroupIP);
			Start(server, MultiCastGroupIP);
		}
		
	}
}
