using System;
using System.Net;
using System.Net.Sockets;

namespace Program
{
	partial class Program
	{
		static void Main(string[] args)
		{		
			string ServerUdpIP = "127.0.0.1";
			int ServerUdpPort = 8081;
			
			if(args.Length == 1){
				ServerUdpPort = System.Int32.Parse(args[0]);
			}
			if(args.Length == 2){
				ServerUdpIP = args[0];
				ServerUdpPort = System.Int32.Parse(args[1]);
			}

			try{
				UDP.Client udpClient = new UDP.Client(ServerUdpIP, ServerUdpPort);
				udpClient.Send("test");
				udpClient.Send(new byte[]{0,1,2,3,4,5});

				UDP.Client udpClient2 = new UDP.Client(ServerUdpIP, ServerUdpPort+1);
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