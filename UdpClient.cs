using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//Client-side
namespace UDP
{
	partial class Client
	{
		public	UdpClient	udpClient			=	null	;
		public	string		UdpServerIP			=	null	;
		public	int			UdpServerPort		=	0		;
		public	string		MultiCastGroupIP	=	null	;

		public Client(
				string			UseUdpServerIP
			,	int				UseUdpServerPort
			,	string			SetMultiCastGroupIP = null
		)
		{
			UdpServerIP = UseUdpServerIP;
			UdpServerPort = UseUdpServerPort;
			udpClient = new UdpClient();
			try
			{
				if(UdpServerIP == "127.0.0.1" || SetMultiCastGroupIP == null){
					udpClient.Client.MulticastLoopback = false;
					udpClient.Connect(UdpServerIP, UdpServerPort);	//Connect this to IP:PORT
				}else{
					MultiCastGroupIP = SetMultiCastGroupIP;
				}
			}
			catch (Exception ex){
				Console.WriteLine(ex);
			}
		}

		//Client
		public byte[] Send(
				byte[] RequestBytes = null
		)
		{
			try
			{
				byte[] ResponseBytes = new byte[0];
				object[] parameters = new object[]{RequestBytes, ResponseBytes};

				//start new thread
				var udpClientThread          = new Thread(new ParameterizedThreadStart(SendUDPProc));
			
				//run this in background
				udpClientThread.IsBackground = true;
			
				//set thread-name
				udpClientThread.Name         = "UDP client thread";
			
				//start thread with parameters
				udpClientThread.Start(parameters);
				
				//join 
				udpClientThread.Join();
				
				//extract resonse from parameters
				ResponseBytes = (byte[])parameters[1];
				
				//return response
				return ResponseBytes;
			}
			catch (Exception ex){
				Console.WriteLine(ex);
				return null;
			}
		}

		//Send request as string, and return response string
		public string Send(
				string request
			,	Encoding encoding = null
		)
		{
			encoding = (encoding != null) ? encoding : Encoding.ASCII;

			try{
				byte[] RequestBytes = encoding.GetBytes(request);
				byte[] ResponseBytes = null;
				ResponseBytes = Send(RequestBytes);
				string response = encoding.GetString(ResponseBytes);
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
			,	string			MultiCastGroupIP = null
		)
		{
			UDP.Client udpClient = new Client(IP, port, MultiCastGroupIP);	//create new UdpClient
			return udpClient.Send(RequestBytes);			//and send bytes for this
		}		

		//Create new tcpClient, connect to this, send request as string, receive response, then close client, and return response
		public static string Send(
				string			IP
			,	int				port
			,	string			request = null
			,	Encoding		encoding = null
			,	string			MultiCastGroupIP = null
		)
		{
			UDP.Client udpClient = new Client(IP, port, MultiCastGroupIP);	//create new UdpClient
			encoding = (encoding != null) ? encoding : Encoding.ASCII;
			return udpClient.Send(request, encoding);		//and send string for this
		}
	}
}
