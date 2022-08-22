using System;

namespace Program
{
	partial class Program
	{
		//start one UDP-server
		static void Main(string[] args)
		{
			string IP = "127.0.0.1";
			int port = 8081;
			string MultiCastGroupIP = null;
			
			if(args.Length >= 3){
				IP = args[0];
				port = System.Int32.Parse(args[1]);
				MultiCastGroupIP = args[2];
			}
			if(args.Length >= 2){
				IP = args[0];
				port = System.Int32.Parse(args[1]);
			}
		
			try{
				//Start one UDP-server with MultiCast group
				UDP.Server.Start("0.0.0.0", port, "235.5.5.11");
				UDP.Server.Start("0.0.0.0", port+1, "235.5.5.12");
				
	//			UDP.Client udpClient2 = new UDP.Client(ServerUdpIP, ServerUdpPort+1, MultiCastGroupIP);

				UDP.Client udpClient = new UDP.Client(IP, port, "235.5.5.11");
				udpClient.Send("test");
				udpClient.Send(new byte[]{0,1,2,3,4,5});

				UDP.Client udpClient2 = new UDP.Client(IP, port+1, "235.5.5.12");
				udpClient2.Send("test");
				udpClient2.Send(new byte[]{0,1,2,3,4,5});
				
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			
			Console.ReadLine();
		}
	}
}