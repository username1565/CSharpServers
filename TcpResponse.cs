using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TcpServer
{
	class TcpResponse
	{
		public static Encoding latin1 = Encoding.GetEncoding("ISO-8859-1");	//Encoding for TCP-responses, to return bytes from string;
		
		public static byte[] GetLatin1Bytes(string text){
			return latin1.GetBytes(text);
		}
	
		//return TcpResponse, as bytes.
		public static byte[] Response(string request)
		{
			//if HTTP-request
			if(
					request.StartsWith("GET")		//GET
				||	request.StartsWith("POST")		//or POST
			){
				return HttpServer.HttpResponse.Response(request);	//return bytes of HTTPResponse
			}
			//else if TCP-request
			else{
				return new byte[0];
			}
		}
	}
}