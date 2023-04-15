using System;

namespace Program
{
	partial class Program
	{
		//start one UDP-server
		static void Main(string[] args)
		{
			string IP = "0.0.0.0";
			int ServerUdpPort = 8081;
			string MultiCastGroupIP = null;
			
			if(args.Length == 1){
				ServerUdpPort = System.Int32.Parse(args[0]);
			}
			else if(args.Length == 2){
				IP = args[0];
				ServerUdpPort = System.Int32.Parse(args[1]);
			}
			else if(args.Length == 3){
				IP = args[0];
				ServerUdpPort = System.Int32.Parse(args[1]);
				MultiCastGroupIP = args[2];
			}
		
			try{
				//Start few UDP-server with/out MultiCast group
				UDP.Server.Start(IP, ServerUdpPort);
				UDP.Server.Start(IP, ServerUdpPort+1);
				
				UDP.Server.Start("0.0.0.0", ServerUdpPort+2, "235.5.5.11");
				UDP.Server.Start("0.0.0.0", ServerUdpPort+3, "235.5.5.12");

				//Start few UDPClients with/out MultiCast group
				UDP.Client.Send("127.0.0.1", ServerUdpPort, "test", System.Text.Encoding.ASCII);
				UDP.Client.Send("127.0.0.1", ServerUdpPort, new byte[]{0,1,2,3,4,5});
				
				UDP.Client.Send("127.0.0.1", ServerUdpPort+1, "test", System.Text.Encoding.ASCII);
				UDP.Client.Send("127.0.0.1", ServerUdpPort+1, new byte[]{0,1,2,3,4,5});
				
				UDP.Client.Send("0.0.0.0", ServerUdpPort+2, "test", System.Text.Encoding.ASCII, "235.5.5.11");
				UDP.Client.Send("0.0.0.0", ServerUdpPort+2, new byte[]{0,1,2,3,4,5}, "235.5.5.11");

				UDP.Client udpClient = new UDP.Client("0.0.0.0", ServerUdpPort+3, "235.5.5.12");
				udpClient.Send("test");
				udpClient.Send(new byte[]{0,1,2,3,4,5});

				UDP.Client udpClient2 = new UDP.Client("0.0.0.0", ServerUdpPort+3, "235.5.5.12");
				udpClient2.Send("test");
				udpClient2.Send(new byte[]{0,1,2,3,4,5});
				
				//UDP Without Multicast
				UDP.Client udpClient3 = new UDP.Client("127.0.0.1", ServerUdpPort);
				udpClient3.Send("test");
				udpClient3.Send(new byte[]{0,1,2,3,4,5});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			
			Console.ReadLine();
		}
	}
}