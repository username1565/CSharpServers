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
/*
	Here is constains two lists with connected clients:
	TCP.Client.TCPConnectionsList - HashSet with TcpClient, with opened active TCP connections.
	UDP.Client.UDPConnectionsList - HashSet with UDP.Client, with opened active UDP connection.
	
	For each client with active connections,
	there is possible to send string or bytes, and receive responses.
*/
	partial class Client
	{
		public static int MaxTcpConnections = 10;

		public static HashSet<TcpClient> TCPConnectionsList = new HashSet<TcpClient>();
		
		public static TcpClient TryConnect(IPAddress IP, int port){
			TcpClient tcpClient;
			try{
				tcpClient = TCP.Client.Connect(IP, port);
				TCPConnectionsList.Add(tcpClient);
				return tcpClient;
			}
			catch
			{
				return null;
			}
		}
		
		public static TcpClient TryConnect(string IPPORT){
			string[]	IP_PORT	=	IPPORT.Split(':');
			IPAddress	IP		=	IPAddress.Parse(IP_PORT[0]);
			int			port	=	System.Int32.Parse(IP_PORT[1]);
			return TryConnect(IP, port);
		}
	}
}

namespace UDP
{
	partial class Client
	{
	
		public static int MaxTcpConnections = 10;

		public static HashSet<UDP.Client> UDPConnectionsList = new HashSet<UDP.Client>();
		
		public static UDP.Client TryConnect(
				IPAddress IP
			,	int port
			,	string MulticastGroupIP = null
		){
			UDP.Client udpClient;
			try{
				udpClient = new UDP.Client(IP.ToString(), port, MulticastGroupIP);
				UDPConnectionsList.Add(udpClient);
			//	udpClient = UDP.Clients.AddUdpClient(IP.ToString(), port);
			//	Console.WriteLine("UDP.Clients.UdpClients.Count: "+UDP.Clients.UdpClients.Count);
				return udpClient;
			}
			catch
			{
				return null;
			}
		}
		
		public static UDP.Client TryConnect(string IPPORT){
			string[]	IP_PORT	=	IPPORT.Split(':');
			IPAddress	IP		=	IPAddress.Parse(IP_PORT[0]);
			int			port	=	System.Int32.Parse(IP_PORT[1]);
			return TryConnect(IP, port);
		}
		

		public static bool TryAddUDPConnection(IPAddress IP, int port){
		
			//if too many connections
			if(UDPConnectionsList.Count >= MaxTcpConnections){
				Console.WriteLine("ActiveConnections.cs. TryConnect. UDPConnectionsList.Count: "+UDPConnectionsList.Count+" is over MaxTcpConnections: "+MaxTcpConnections);
				return false;	//do not connect
			}
			
			//Check remote endpoints of connected clients
			foreach(UDP.Client ConnectedUdpClient in UDPConnectionsList){
				IPEndPoint connectedEP = (IPEndPoint)ConnectedUdpClient.udpClient.Client.RemoteEndPoint;
				
				//If already connected
				if(
						connectedEP.Address.ToString() == IP.ToString()
					&&	connectedEP.Port == port
				)
				{
					return true;	//do not connect, because already connected
				}
			}
			
			//else, try UDP connect			
			UDP.Client udpClient = UDP.Client.TryConnect(IP, port);
			
			if(udpClient != null){
				
				if( ! UDPConnectionsList.Contains( udpClient ) )
				{
					//Console.WriteLine("Connection added");
					UDPConnectionsList.Add(udpClient);
				}
				Console.WriteLine("UDPConnectionsList.Count: "+UDPConnectionsList.Count);
				
				return true;
			}else{
				return false;
			}
		}
		
		public static bool TryAddUDPConnection(string IPPORT){
			string[]	IP_PORT	=	IPPORT.Split(':');
			IPAddress	IP		=	IPAddress.Parse(IP_PORT[0]);
			int			port	=	System.Int32.Parse(IP_PORT[1]);
			return TryAddUDPConnection(IP, port);
		}
	}
}	