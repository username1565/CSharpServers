using System;

namespace Program
{
	partial class Program
	{
		//start few TCP-servers, on different ports
		static void Main3(string[] args)
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
				TCP.Server.Start(new string[]{IP, port.ToString()});
				TCP.Server.Start(new string[]{IP, (++port).ToString()});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			
			Console.ReadLine();
		}
	}
}