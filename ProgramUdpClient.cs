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
				//Clients with multicast
				UDP.Client.Send("0.0.0.0", ServerUdpPort, "test", System.Text.Encoding.ASCII, MultiCastGroupIP);
				UDP.Client.Send("0.0.0.0", ServerUdpPort, new byte[]{0,1,2,3,4,5}, MultiCastGroupIP);

				UDP.Client udpClient = new UDP.Client("0.0.0.0", ServerUdpPort, MultiCastGroupIP);
				udpClient.Send("test");
				udpClient.Send(new byte[]{0,1,2,3,4,5});
				
				//Client without multicast:
				UDP.Client.Send(ServerUdpIP, ServerUdpPort, "test");
				UDP.Client.Send(ServerUdpIP, ServerUdpPort, new byte[]{0,1,2,3,4,5});

				UDP.Client udpClient2 = new UDP.Client(ServerUdpIP, ServerUdpPort, MultiCastGroupIP);
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