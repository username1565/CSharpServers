using System;

namespace Program
{
	partial class Program
	{
		//start one UDP-server
		static void Main5(string[] args)
		{
			string IP = "0.0.0.0";
			int port = 8081;
			
			if(args.Length == 1){
				port = System.Int32.Parse(args[0]);
			}
			else if(args.Length == 2){
				IP = args[0];
				port = System.Int32.Parse(args[1]);
			}
		
			try{
				//Start one UDP-server
				UDP.Server.Start(new string[]{IP, port.ToString()});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			
			Console.ReadLine();
		}
	}
}