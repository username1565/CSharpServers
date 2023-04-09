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
			string MultiCastGroupIP = null;
			
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
				//Start few UDP-server with MultiCast group
				UDP.Server.Start(IP, port);
				UDP.Server.Start(IP, port+1);
				
				UDP.Server.Start("0.0.0.0", port+2, "235.5.5.11");
				UDP.Server.Start("0.0.0.0", port+3, "235.5.5.12");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			
			Console.ReadLine();
		}
	}
}