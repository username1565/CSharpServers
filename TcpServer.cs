using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
	TCP-server
Start TCP-server on IP:PORT,
listen TCP-port,
and run new thread with TCPServerProc,
after TCP-connection of some TCP-client.
*/
namespace TCP
{
	partial class Server
	{
		public static void Start(
			string[] args
		)
		{
			IPAddress TcpIP = IPAddress.Any;
			int TcpPort = 8081;
		
			if(args.Length == 1){
				TcpPort = System.Int32.Parse(args[0]);
			}
			else if(args.Length == 2){
				TcpIP = IPAddress.Parse(args[0]);
				TcpPort = System.Int32.Parse(args[1]);
			}
			
			TcpListener tcpServer = new TcpListener(TcpIP, TcpPort);	//start this on IP:PORT
			Start(tcpServer);
		}

		public static TcpListener TCPServer(string TcpServerIP, int TcpServerPort){
			TcpListener tcpServer = new TcpListener(IPAddress.Parse(TcpServerIP), TcpServerPort);	//start this on IP:PORT
			return tcpServer;
		}

		//start tcpServer
		public static void Start(
			TcpListener tcpServer = null
		)
		{
			//start new thread on accept TCP-connection from some client
			var tcpThread          = new Thread(new ParameterizedThreadStart(TCPServerProc));
			
			//run this in background
			tcpThread.IsBackground = true;
			
			//set thread-name
			tcpThread.Name         = "TCP server thread";

			//start thread
			tcpThread.Start(tcpServer);
		}
	}
}