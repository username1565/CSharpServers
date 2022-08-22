using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//Client-side
namespace TCP
{
	partial class Client
	{
		//Binary encoding to encode bytes to string, and decode it back. This allow to encode any byte as one char, and decode it back.
		public static Encoding BinaryEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");

		//From specified connected tcpClient, send request as string, receive response, and return this response as string
		public static string SendTCP(
				TcpClient		tcpClient
			,	string			request
			,	Encoding		encoding = null
		)
		{
				encoding					=	(encoding != null) ? encoding : BinaryEncoding	;
				
				//Get client's NetworkStream
				NetworkStream tcpStream = tcpClient.GetStream();
			
				byte[] RequestBytes = encoding.GetBytes(request);
				
				//send RequestBytes
				tcpStream.Write(RequestBytes, 0, RequestBytes.Length);
				Console.WriteLine("Client sent: "+request);

				try{
					byte[] data = new byte[64];
					StringBuilder builder = new StringBuilder();
					int bytes = 0;
					do
					{
						bytes = tcpStream.Read(data, 0, data.Length);
						builder.Append(encoding.GetString(data, 0, bytes));
					}
					while (tcpStream.DataAvailable && tcpClient.Connected);						
 
					string response = builder.ToString();
					Console.WriteLine("TCP Client received - " + response);
					

				//	Do not close tcpStream, because server close connection with client, and do not listen stream, then.
				//		tcpStream.Close();
				//	Do not close tcpClient, to let use this again, for next response...
				//		tcpClient.Close();
						
						return response;
				}
				catch (Exception ex){
					Console.WriteLine(ex);
					return null;
				}
		}
	}
}