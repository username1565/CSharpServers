using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;	//Dictionary

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
		
		public static string AddHeader(
				bool isBinary = false
			,	int contentLength = 0
		)
		{
			string header = "HTTP/1.1 200 OK" + "\r\n"
				+ "Content-Type: " +
					(
						(isBinary == true)
						? "application/octet-stream"
						: "text/html"
					) + "\r\n"
				+ (
					(contentLength != 0)
						? "Content-Length: " + contentLength.ToString() + "\r\n"
						: ""
				)
				+ "\r\n"
			;
			return header;
		}

		//Different HttpResponses
		public static byte[] Response (string request)
		{
			//request = HttpRequest.Request(request);	//the same string

			byte[] sendBytes;
			
		//	Console.WriteLine ("");
		//	Console.WriteLine (request);
			
			object[] properties = HttpRequest.Properties(request);	//header, content and properties of HTTP-response
		//	Console.WriteLine("(string)properties[0]: "+((string)properties[0]));
			Dictionary <string, string> props = (Dictionary <string, string>)properties[2];
			Console.WriteLine("props[\"Method\"]: "+(props["Method"]));
			
  
			StringBuilder builder = new StringBuilder ();
		//	builder.AppendLine (@"HTTP/1.1 200 OK"); 

		//	Console.WriteLine("request: "+request);

			if(!request.StartsWith("GET / ")){
				string details = request.Split(new string[]{"\r\n"}, StringSplitOptions.None)[0];
				string address = details.Split(new string[]{" "}, StringSplitOptions.None)[1];
				byte[] FileContent = new byte[0];
				
				if(File.Exists(@"www/"+address)){
					FileContent = File.ReadAllBytes(@"www/"+address);
				}
					
				if(address.Contains(".html")){
		//			builder.AppendLine (@"Content-Type: text/html;");
					builder.Append(AddHeader());
				}
				else{
		//			builder.AppendLine (@"Content-Type: application/octet-stream;");
					builder.Append(AddHeader(true, FileContent.Length));
				}
		//		builder.AppendLine (@"Content-Length: "+FileContent.Length.ToString());
		//		builder.AppendLine (@"");

				sendBytes =	Combine(
											enc.GetBytes (builder.ToString ())	//header-bytes
										,	FileContent							//FileContent-bytes
									)
				;
			}
			else{
		//		builder.AppendLine (@"Content-Type: text/html");
		//		builder.AppendLine (@"");

				builder.Append(AddHeader());
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