using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDP
{
	partial class Server
	{
		//get request-string from byte[] data
		public static string Request(byte[] data, Encoding encoding){
			string message = encoding.GetString(data);
		//	Console.WriteLine("Request received: "+message);
			return message;
		}
	
		//Response as string
		public static string Response(string request)
		{
			if(request == "Are you UDP server?"){
				return "Yes, I'm UDP server.";
			}
			string response = TextServer.Responses.Response(request);
		//	Console.WriteLine("Response sent:"+response);
			return response;
		}
		
		//Response as bytes
		public static byte[] Response(string request, Encoding encoding)
		{
			return encoding.GetBytes(Response(request));
		}

		public static void RequestResonse(
				UdpClient server
			,	Encoding encoding
		)
		{
			IPEndPoint remoteEP = null;
			byte[] buffer   = server.Receive(ref remoteEP);
					
		/*
			Console.WriteLine(
					"UDP Client connected: from "+Program.Convert.IP_PORT(remoteEP)
				+	" to "	+Program.Convert.IP_PORT((IPEndPoint)((server.Client).LocalEndPoint))
			);
		*/

			if (buffer != null && buffer.Length > 0)
			{
				string message = Request(buffer, encoding);
			//	Console.WriteLine("UDP received: " + message);
				
				//Send resonse for UDP-request
				byte[] response = Response(message, encoding);
				server.Send(response, response.Length, remoteEP.Address.ToString(), remoteEP.Port); // отправка
			//	Console.WriteLine("UDP sent: " + encoding.GetString(response));
			}
		}
	}
}