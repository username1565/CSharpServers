using System;
using System.Net;
using System.Net.Sockets;

namespace Program
{
	partial class Program
	{
		static void Main16(string[] args)
		{
			string ServerUdpIP = "127.0.0.1";
			int ServerUdpPort = 8081;
			string MultiCastGroupIP = "235.5.5.11";

			if(args.Length == 1){
				ServerUdpPort = System.Int32.Parse(args[0]);
			}
			else if(args.Length == 2){
				ServerUdpIP = args[0];
				ServerUdpPort = System.Int32.Parse(args[1]);
			}
			else if(args.Length == 3){
				ServerUdpIP = args[0];
				ServerUdpPort = System.Int32.Parse(args[1]);
				MultiCastGroupIP = args[2];
			}

			try{
				UDP.Client.Send(ServerUdpIP, ServerUdpPort, "test");
				UDP.Client.Send(ServerUdpIP, ServerUdpPort, new byte[]{0,1,2,3,4,5});
//				UDP.Client udpClient = new UDP.Client(ServerUdpIP, ServerUdpPort, MultiCastGroupIP);
//				udpClient.Send("test");
//				udpClient.Send(new byte[]{0,1,2,3,4,5});
			}
			catch(Exception ex){
				Console.WriteLine(ex);
			}
			System.Console.ReadLine();
		}
	}
}