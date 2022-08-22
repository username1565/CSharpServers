using System;
using System.Net;
using System.Net.Sockets;

namespace Program
{
	partial class Program
	{
		public static void Main(string[] args){
			
			string TcpIP = "0.0.0.0";
			int TcpPort = 8081;
			
			string TcpClientIP = "127.0.0.1";
			
			if(args.Length == 1){
				TcpPort = System.Int32.Parse(args[0]);
			}
			else if(args.Length == 2){
				TcpIP = args[0];
				TcpPort = System.Int32.Parse(args[1]);
			}
			else{
				TcpIP = "0.0.0.0";
				TcpPort = 8001;
			}
			
				TCP.Server.Start(TCP.Server.TCPServer(TcpIP, TcpPort));
				TCP.Server.Start(TCP.Server.TCPServer(TcpIP, TcpPort+1));

				Console.WriteLine("Press <ENTER> to stop the servers.");	//show this
		
				Console.WriteLine("test many TcpClients");
				
				TcpClient tcpClient		=	TCP.Client.Connect(TcpClientIP, TcpPort);
				TCP.Client.Send(tcpClient, "test"					);
				TCP.Client.Send(tcpClient, new byte[]{0,1,2,3,4,5}	);

				TcpClient tcpClient2	=	TCP.Client.Connect(TcpClientIP, TcpPort+1);
				TCP.Client.Send(tcpClient2, "test"					);
				TCP.Client.Send(tcpClient2, new byte[]{0,1,2,3,4,5}	);

				Console.ReadLine();											//do not close window, and wait input
		}
	}
}