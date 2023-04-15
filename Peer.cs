using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Timers;	//interval

	/*
	Peer
		Decentralized networks constains peers.
		Peer - it's server and client together.
		Server - accept request from client, and send response to client.
		Client - send request to server, and accept response from server.

	TCP-peer
		TCP-peer - it's TCP-server and TCP-client together.
		TCP-peer can connect to another known peers,
		from peers-list,
		so need to make and host the public peers,
		and hardcode the peers-list of known peers.
		TCP-peers can supporting Peer Exchange (PEX)

	UDP-peer
		UDP-peer - it's UDP-server and UDP-client together.
		UDP-peer can connect to another known peers,
		from peers-list,
		so need to make and host the public peers,
		and hardcode the peers-list of known peers.
		UDP-peers can supporting Peer Exchange too.
		Also, UDP-peer can supporting Local Peer Discovery (LDP), because UDP-protocol supporting UDP-multicast.
		
	TCPUDP-peer
		TCP-peer and UDP-Peer (with multicast) - together.

	How peer works?
		Raise server-side - TCP/UDP-server with UDP-MultiCastGroupIP
		Raise client-side - TCP and UDP client (with MultiCastGroupIP)
		Try to connect the found TCP peers from peers list (addnode.txt),
		or/and try to make "Local Peer Discovery" to find local peers, using UDP Multicast request,
			From client-side, send UDP-Multicast request to multicast-group IP, and wait responses.
			If somebody in LAN was responded, add his IP:PORT to peer list (Dictionary), and try to connect this peer
		Then process Peer Exchange, to synchronize peers-lists, and try to connect to known peers.
		Try to connect from client to server-side of another peers, and transfer some data (DHT) between peers, after connection.	
	*/
	
namespace Peer
{
	public class Peer
	{
		public static void Main(string[] args)
		{
			string IP = "0.0.0.0";
			int port = 8081;
			string UDPMultiCastGroupIP = "235.5.5.11";

			if(args.Length == 1){
				port = System.Int32.Parse(args[0]);
			}
			else if(args.Length == 2){
				IP = args[0];
				port = System.Int32.Parse(args[1]);
			}
			else if(args.Length == 3){
				IP = args[0];
				port = System.Int32.Parse(args[1]);
				UDPMultiCastGroupIP = args[2];
			}

			try{
				//	Raise server-side - TCP/UDP-server with UDP-MultiCastGroupIP
				//Start TCP Server
				TCP.Server.Start(new string[]{IP, port.ToString()});

				//Start UDP Server
				UDP.Server.Start(IP, port, UDPMultiCastGroupIP);	//UDP-Server with UDPMultiCastGroupIP
				



				Console.WriteLine("Test TCP: ");
				//		Start TCP clients
				string response;
				byte[] ResponseBytes;
				//	Connect, send request, receive response, then disconnect
				response				=	TCP.Client.Send("127.0.0.1", port, "send and close");
				ResponseBytes			=	TCP.Client.Send("127.0.0.1", port, new byte[]{0,1,2,3,4,5});
			
				//	Connect tcpClient, and keep connection alive
				TcpClient tcpClient 	=	TCP.Client.Connect("127.0.0.1", port);
				//	Send requests using connected tcpClient, and receive responses
				response				=	TCP.Client.Send(tcpClient, "test");
				ResponseBytes			=	TCP.Client.Send(tcpClient, new byte[]{0,1,2,3,4,5});

				Console.WriteLine("Test UDP: ");

				//		Start UDP clients
	//			UDP.Client udpClient2 = new UDP.Client(ServerUdpIP, ServerUdpPort, UDPMultiCastGroupIP);

				UDP.Client udpClient = new UDP.Client(IP, port, UDPMultiCastGroupIP);
				udpClient.Send("test");
				udpClient.Send(new byte[]{0,1,2,3,4,5});

				UDP.Client udpClient2 = new UDP.Client(IP, port, UDPMultiCastGroupIP);
				udpClient2.Send("test");
				udpClient2.Send(new byte[]{0,1,2,3,4,5});
				



				//	Try connect TCP addnodes:
				AliveNodes.GetAliveNodes();	//Alive nodes in AliveNodes.AliveAddnodeList
				AliveNodes.ConnectAliveNodes();	//Connect to alive nodes;
				

				//	Show active connections:
				//		TCP:
				Console.WriteLine("TCP.Client.TCPConnectionsList.Count: "+TCP.Client.TCPConnectionsList.Count);
				//		UDP:
				Console.WriteLine("UDP.Client.UDPConnectionsList.Count: "+UDP.Client.UDPConnectionsList.Count);
				
				
				//	Test connections:
				AliveNodes.TestConnections();
				
			
			//Run LDP (Local Peer Discovery)
			LocalPeersDiscovery.DiscoveryPeers();
			IsPeer.ShowActivePeers();

//			//Run interval for local peer discovery
			LocalPeersDiscovery.RunDiscoveryPeersInterval(5);
			
			
			
			
				//	Check peers, before run PEX
			IsPeer.CheckPeers();
		//	Console.WriteLine("IsPeer.TCPPeers.Count: "+IsPeer.TCPPeers.Count);
		//	Console.WriteLine("IsPeer.UDPPeers.Count: "+IsPeer.UDPPeers.Count);
			
			
				//	Run Peer Exchange:
			//	TCP:
		//	LocalPeersList.TryConnectPeersTCP();
			Console.WriteLine("Try TCP PEX");
			PEX_client.TCPPeerExchange();
		
			//	UDP:
			Console.WriteLine("Try UDP PEX");
		//	LocalPeersList.TryConnectPeersUDP();
			PEX_client.UDPPeerExchange();

				
				
			}
			catch(Exception ex){
				Console.WriteLine(ex);
			}

			//Do not close window, after all.
			Console.ReadLine();
		}
	}
}
