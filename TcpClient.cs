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
				TcpClient		tcpClient = Connect(IP, port);
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
				TcpClient		tcpClient = Connect(IP, port);
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

		//Connect to TCP server, and return TcpClient
		
		//string, int
		public static TcpClient Connect(
				string TcpIP
			,	int TcpPort
			,	int ConnectionTimeout = 1	//seconds
		){
			TcpClient tcpClient = null;
			try
			{
				tcpClient = new TcpClient();	//start this on IP:PORT

				//inactive node connection timeout.
				IAsyncResult ar = tcpClient.BeginConnect(TcpIP, TcpPort, null, null);  
				System.Threading.WaitHandle wh = ar.AsyncWaitHandle;
				try 
				{
					if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(ConnectionTimeout), false))  
					{  
						tcpClient.Close();  
						throw new TimeoutException();  
					}
					tcpClient.EndConnect(ar);  
				}
				catch //(Exception ex)
				{
					//Console.WriteLine(ex);
				//	Console.WriteLine(TcpIP+":"+TcpPort+" not respond within "+ConnectionTimeout + " seconds...");
					wh.Close();
					return null;
				}
				finally 
				{
					wh.Close();
				}
			}
			catch (Exception ex){
				Console.WriteLine(ex);
				return null;
			}
			return tcpClient;
		}
		
		//IPAddress, int
		public static TcpClient Connect(IPAddress TcpIP, int TcpPort){
			return Connect(TcpIP.ToString(), TcpPort);
		}

		//string string
		public static TcpClient Connect(string TcpIP, string TcpPort){
			return Connect(TcpIP, System.Int32.Parse(TcpPort));
		}

		//IPAddress string
		public static TcpClient Connect(IPAddress TcpIP, string TcpPort){
			return Connect(TcpIP.ToString(), System.Int32.Parse(TcpPort));
		}

		//From specified TcpClient, send string, receive response string from server, and return this response-string.
		public static string Send(
				TcpClient	tcpClient = null
			,	string request = null
			,	Encoding encoding = null
		)
		{
			if(request == null || tcpClient == null){ return null; }

			encoding					=	(encoding != null) ? encoding : BinaryEncoding	;

			string response = "";
			try
			{
				object[] parameters = new object[]{tcpClient, request, response, encoding};
				//start new thread on accept TCP-connection from some client
				var tcpClientThread          = new Thread(new ParameterizedThreadStart(SendTCPProc));
			
				//run this in background
				tcpClientThread.IsBackground = true;
			
				//set thread-name
				tcpClientThread.Name         = "TCP client thread";
			
				//start thread with parameters
				tcpClientThread.Start(parameters);
				
				tcpClientThread.Join();
				
				response = (string)parameters[2];
				return response;
			}
			catch //(Exception ex)
			{
			//	Console.WriteLine(ex);
				return null;
			}
		}
		
		//From specified TcpClient, send bytes, receive response bytes, and return this ResponseBytes.
		public static byte[] Send(
				TcpClient	tcpClient = null
			,	byte[]		RequestBytes = null
			,	Encoding	encoding = null
		)
		{
			if(RequestBytes == null || tcpClient == null){ return null; }
			encoding					=	(encoding != null) ? encoding : BinaryEncoding	;
		
			string		request			=	encoding.GetString(RequestBytes)			;
			string		response		=	Send(tcpClient, request)					;
			byte[]		ResponseBytes	=	encoding.GetBytes(response)					;
			return		ResponseBytes;
		}
	}
}