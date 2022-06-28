using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Mti
{
	class HttpServer
	{ 
		static Encoding enc = Encoding.UTF8;

		public static void Main (string[] args)
		{
			Console.WriteLine ("start up http server...");
			  
			IPAddress ipAddress = Dns.GetHostEntry ("localhost").AddressList [0];
			TcpListener listener = new TcpListener (ipAddress, 8080);
			listener.Start (); 

			while (true) {
				TcpClient client = listener.AcceptTcpClient (); 
				Console.WriteLine ("request incoming...");
			 
				NetworkStream stream = client.GetStream (); 
				string request = ToString (stream); 
			
				Console.WriteLine ("");
				Console.WriteLine (request);
  
				StringBuilder builder = new StringBuilder ();
				builder.AppendLine (@"HTTP/1.1 200 OK"); 
				builder.AppendLine (@"Content-Type: text/html");
				builder.AppendLine (@"");
				builder.AppendLine (@"<html><head><title>Hello world!</title></head><body><h1>Hello world!</h1>Hi!</body></html>"); 
 
				Console.WriteLine ("");
				Console.WriteLine ("responce...");
				Console.WriteLine (builder.ToString ());
				 
				byte[] sendBytes = enc.GetBytes (builder.ToString ());
				stream.Write (sendBytes, 0, sendBytes.Length);

				stream.Close ();
				client.Close ();
			}
		}
		 
		public static string  ToString (NetworkStream stream)
		{
			MemoryStream memoryStream = new MemoryStream ();
			byte[] data = new byte[256];
			int size;
			do {
				size = stream.Read (data, 0, data.Length);
				if (size == 0) {
					Console.WriteLine ("client disconnected...");
					Console.ReadLine ();
					return  null; 
				} 
				memoryStream.Write (data, 0, size);
			} while ( stream.DataAvailable); 
			return enc.GetString (memoryStream.ToArray ());
		}
	}
}