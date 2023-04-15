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
			else if(args.Length == 2){
				ServerUdpIP = args[0];
				ServerUdpPort = System.Int32.Parse(args[1]);
			}

			try{
				UDP.Client.Send(ServerUdpIP, ServerUdpPort, "test");
				UDP.Client.Send(ServerUdpIP, ServerUdpPort, new byte[]{0,1,2,3,4,5});
			}
			catch(Exception ex){
				Console.WriteLine(ex);
			}
			System.Console.ReadLine();
		}
	}
}