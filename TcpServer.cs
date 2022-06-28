using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TcpServer
{
	class TcpServer
	{
		public static void Main (string[] args)
		{
		
			int port = 8080;
			
			if(args.Length>=1){
				int TryGetPort = 0;
				if(int.TryParse(args[0], out TryGetPort))
				{
					port = TryGetPort;
				}
				Console.WriteLine("port: "+port);
			}
			
			//IPAddress ipAddress = Dns.GetHostEntry ("localhost").AddressList [0];
			IPAddress ipAddress = IPAddress.Any;
			
			Console.WriteLine ("start up http server..."+" ipAddress: "+ipAddress+", port: "+port);
			
			TcpListener listener = new TcpListener (ipAddress, port);
			listener.Start (); 

			while (true) {
				TcpClient client = listener.AcceptTcpClient (); 
				Console.WriteLine ("TCP request incoming...");
			 
				NetworkStream stream = client.GetStream (); 
				string request = TcpRequest.ToString (stream); 
			
				byte[] response = TcpResponse.Response(request);
				stream.Write (response, 0, response.Length);

				stream.Close ();
				client.Close ();
			}
		}

	}
}