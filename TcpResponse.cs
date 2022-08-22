using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

/*
	By tcp-request, as string, return TCP Response as string or bytes (with specified encoding)
*/
namespace TCP
{
	partial class Server
	{
		public static string Response(string message)
		{
			if(message == "Are you TCP server?"){
				return "Yes, I'm TCP server.";
			}
			return TextServer.Responses.Response(message);
		}
		
		public static byte[] Response(	
				string message
			,	Encoding encoding
		){
		//	Console.WriteLine("Try generate TCP response...");
			byte[] response = encoding.GetBytes(Response(message));
		//	Console.WriteLine("Done...");
			return response;
		}
	}
}