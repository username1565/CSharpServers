using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace HttpServer
{
	class HttpResponse
	{ 
		static Encoding enc = Encoding.UTF8;	//HTTPServer Encoding.
		
		public static byte[] Combine(byte[] first, byte[] second)
		{
			byte[] bytes = new byte[first.Length + second.Length];
			Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
			Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
			return bytes;
		}
		
		public static byte[] Response (string request)
		{
			//request = HttpRequest.Request(request);	//the same string

			byte[] sendBytes;
			
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

				sendBytes =	Combine(
											enc.GetBytes (builder.ToString ())	//header-bytes
										,	FileContent							//FileContent-bytes
									)
				;
			}
			else{
				builder.AppendLine (@"Content-Type: text/html");
				builder.AppendLine (@"");
				builder.AppendLine (@"<html><head><title>Hello world!</title></head><body><h1>Hello world!</h1>Hi!</body></html>");

			//	Console.WriteLine ("");
			//	Console.WriteLine ("responce...");
			//	Console.WriteLine (builder.ToString ());
				 
				sendBytes = enc.GetBytes (builder.ToString ());
			}

			return sendBytes;
		}
	}
}