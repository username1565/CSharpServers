using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
	TCP-server
Start TCP-server on IP:PORT,
listen TCP-port,
and run new thread with TCPServerProc,
after TCP-connection of some TCP-client.
*/
namespace TCP
{
	partial class Client
	{
		//Create new tcpClient, connect to this, send request as string, receive response, then close client, and return response
		public static string Send(
				string			IP
			,	int				port
			,	string			request = null
			,	Encoding		encoding = null
		)
		{
			try{
				TcpClient		tcpClient = new TcpClient(IP, port);
				encoding = (encoding != null) ? encoding : Encoding.ASCII;
				string response = SendTCP(tcpClient, request, encoding);
				tcpClient.Close();
				return response;
			}
			catch(Exception ex){
				Console.WriteLine(ex);
				return null;
			}
		}
		
		//Create new tcpClient, connect to this, send request as bytes, receive ResponseBytes, then close client, and return response
		public static byte[] Send(
				string			IP
			,	int				port
			,	byte[]			RequestBytes
			,	Encoding		encoding = null
		)
		{
			try{
				TcpClient		tcpClient = new TcpClient(IP, port);
				encoding = (encoding != null) ? encoding : Encoding.ASCII;
				string		request			=	encoding.GetString(RequestBytes)			;
				string		response		=	SendTCP(tcpClient, request, encoding)		;
				tcpClient.Close();
				byte[]		ResponseBytes	=	encoding.GetBytes(response)					;
				return		ResponseBytes;
			}
			catch(Exception ex){
				Console.WriteLine(ex);
				return null;
			}
		}
	}
}