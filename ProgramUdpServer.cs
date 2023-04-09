using System;

namespace Program
{
	partial class Program
	{
		//start one UDP-server
		static void Main(string[] args)
		{
			string IP = "0.0.0.0";
			int port = 8081;
			string MultiCastGroupIP = "235.5.5.11";
			
			if(args.Length == 1){
				port = System.Int32.Parse(args[0]);
			}
			else if(args.Length == 2){
				IP = args[0];
				port = System.Int32.Parse(args[1]);
			}
			else if(args.Length == 3){
				IP = args[0];
				port = System.Int32.Parse(args[1]);
				MultiCastGroupIP = args[2];
			}
		
			try{
				//Start one UDP-server with multicast:
				UDP.Server.Start(new string[]{IP, port.ToString(), MultiCastGroupIP});
			
				//Start one UDP-server without multicast:
				UDP.Server.Start(new string[]{IP, (port+1).ToString()});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			
			Console.ReadLine();
		}
	}
}