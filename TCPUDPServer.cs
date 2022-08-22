using System;

namespace TCPUDP
{
	class Server
	{
		public static void Main19(string[] args){
			string TcpIP;
			string UdpIP;
			int TcpPort = 0;
			int UdpPort = 0;
			string MultiCastGroupIP = "235.5.5.11";
			
			if(args.Length == 1){
				TcpIP = UdpIP = "0.0.0.0";
				TcpPort = UdpPort = System.Int32.Parse(args[0]);
			}
			if(args.Length == 2){
				TcpIP = UdpIP = args[0];
				TcpPort = UdpPort = System.Int32.Parse(args[1]);
			}
			if(args.Length == 3){
				TcpIP = UdpIP = args[0];
				TcpPort = UdpPort = System.Int32.Parse(args[1]);
				MultiCastGroupIP = "235.5.5.11";
			}
			else{
				TcpIP = UdpIP = "0.0.0.0";
				TcpPort = UdpPort = 8081;
			}

			//Start TCP Servers
			TCP.Server.Start(new string[]{TcpIP, TcpPort.ToString()});
			TCP.Server.Start(new string[]{TcpIP, (TcpPort+1).ToString()});

			//Start UDP Servers
			UDP.Server.Start(UdpIP, UdpPort, MultiCastGroupIP);
			UDP.Server.Start(UdpIP, UdpPort+1, MultiCastGroupIP);

			Console.ReadLine();											//do not close window, and wait input
		}
	}
}