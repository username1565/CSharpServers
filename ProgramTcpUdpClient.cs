using System;

namespace Program
{
	partial class Program
	{
		static string	TcpUdpServerIP = "127.0.0.1";
		static int		TcpUdpServerPort = 8081;
		static string	UdpMultiCastGroupIP = "235.5.5.11";
		
		static void Main(string[] args)
		{		
			if(args.Length >= 3){
				TcpUdpServerIP = args[0];
				TcpUdpServerPort = System.Int32.Parse(args[1]);
				UdpMultiCastGroupIP = args[2];
			}

			try{
				//start TCP client
				string response;
				byte[] ResponseBytes;
				//	Connect, send request, receive response, then disconnect
				response				=	TCP.Client.Send(TcpUdpServerIP, TcpUdpServerPort, "send and close");
				ResponseBytes			=	TCP.Client.Send(TcpUdpServerIP, TcpUdpServerPort, new byte[]{0,1,2,3,4,5});
			
				//Start UDP client
				UDP.Client.Send(TcpUdpServerIP, TcpUdpServerPort, "test");
				UDP.Client.Send(TcpUdpServerIP, TcpUdpServerPort, new byte[]{0,1,2,3,4,5});
			}
			catch(Exception ex){
				Console.WriteLine(ex);
			}
			System.Console.ReadLine();
		}
	}
}