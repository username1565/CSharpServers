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
		
		public static byte[] Combine(byte[] first, byte[] second)
		{
			byte[] bytes = new byte[first.Length + second.Length];
			Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
			Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
			return bytes;
		}
		
		public static void Main (string[] args)
		{
			
			int port = 8080;
			
			//program accept port, in args[0]
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
			TcpListener listener = new TcpListener (ipAddress, port);
			listener.Start (); 
			Console.WriteLine ("start up http server..."+" ipAddress: "+ipAddress+", port: "+port);

			while (true) {
				TcpClient client = listener.AcceptTcpClient (); 
				Console.WriteLine ("request incoming...");
			 
				NetworkStream stream = client.GetStream (); 
				string request = ToString (stream); 
			
				Console.WriteLine ("");
				Console.WriteLine (request);
  
				StringBuilder builder = new StringBuilder ();
				builder.AppendLine (@"HTTP/1.1 200 OK"); 
				
				Console.WriteLine("request: "+request);
				if(!request.StartsWith("GET / ")){
					string details = request.Split(new string[]{"\r\n"}, StringSplitOptions.None)[0];
					string address = details.Split(new string[]{" "}, StringSplitOptions.None)[1];
					byte[] FileContent = new byte[0];
					
					if(File.Exists(@"www/"+address)){
						FileContent = File.ReadAllBytes(@"www/"+address);
					}
					
					if(address.Contains(".html")){
						builder.AppendLine (@"Content-Type: text/html;");
					}
					else{
						builder.AppendLine (@"Content-Type: application/octet-stream;");
					}
					builder.AppendLine (@"Content-Length: "+FileContent.Length.ToString());
					builder.AppendLine (@"");

					byte[] sendBytes =	Combine(
												enc.GetBytes (builder.ToString ())	//header-bytes
											,	FileContent							//FileContent-bytes
										)
					;
					stream.Write (sendBytes, 0, sendBytes.Length);
				}
				else{
					builder.AppendLine (@"Content-Type: text/html");
					builder.AppendLine (@"");
					builder.AppendLine (@"<html><head><title>Hello world!</title></head><body><h1>Hello world!</h1>Hi!</body></html>");

				//	Console.WriteLine ("");
				//	Console.WriteLine ("responce...");
				//	Console.WriteLine (builder.ToString ());
				 
					byte[] sendBytes = enc.GetBytes (builder.ToString ());
					stream.Write (sendBytes, 0, sendBytes.Length);
				}

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