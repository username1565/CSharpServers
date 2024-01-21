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
		private static string	IP						=	"0.0.0.0"			;	//IP to listen TCP and UDP port
		private static int		port					=	8082				;	//port
		private static string	UDPMultiCastGroupIP		=	"235.5.5.11"		;	//Multicast Group IP
		private static string	localhost				=	"127.0.0.1"			;

		public static void Main(string[] args)
		{
			Console.WriteLine(
				@"Usage:
	Use SQLite3 database:
Peer.exe IP port MulticastGroupIP PeerDiscoveryInterval SyncDHTInterval DBFilePath TableName KeyName ValueName
	or use txt-file instead:
Peer.exe IP port MulticastGroupIP PeerDiscoveryInterval SyncDHTInterval TXTFilePath
	or disable LDP
Peer.exe IP port someIP 0 SyncDHTInterval DBFilePath TableName KeyName ValueName
"
			);
			Console.WriteLine("Press any key, to continue...");
			Console.ReadKey();
			
			//port
			//IP, port
			//IP, port, MulticastGroupIP
			
			if(args.Length == 1){		//TCP and UDP IP (0.0.0.0, by default)
				port = System.Int32.Parse(args[0]);
			}
			else if(args.Length > 1){	//port to bind TCP and UDP servers both
				IP = args[0];
				port = System.Int32.Parse(args[1]);
			}
			
			//next 7 arguments:
			//	UDPMultiCastGroupIP
			//	PeerDiscoveryInterval,
			//	SyncDHTInterval,
			//	sqlite3 path,
			//	TableOrViewName,
			//	KeyName,
			//	ValueName
			if(args.Length > 2){	//Multicast Group IP
				UDPMultiCastGroupIP = args[2];
			}
			
			//intervals
			if(args.Length > 3){
				LocalPeersDiscovery.PeerDiscoveryInterval = System.Int32.Parse(args[3]);	//PeerDiscoveryInterval, seconds
				Console.WriteLine("New LocalPeersDiscovery.PeerDiscoveryInterval = "+LocalPeersDiscovery.PeerDiscoveryInterval);
			}
			if(args.Length > 4){
				DHT.DHT_client.SyncDHTInterval = System.Int32.Parse(args[4]);	//SyncDHTInterval, seconds
				Console.WriteLine("New DHT.DHT_client.SyncDHTInterval = "+DHT.DHT_client.SyncDHTInterval);
			}
			
			//DHT args
			if(args.Length > 5){
				DHT.DHT.DBFilePath = args[5];	//sqlite3 or txt Database Path
			}
			if(args.Length > 6){
				Storage.KeyValue.UseSQLite3 = true;	//enable SQLite3
				DHT.DHT.HashTableName = args[6];	//HashTable or View Name in SQLite3 database
			}
			if(args.Length > 7){
				DHT.DHT.KeyName = args[7];		//KeyName in HashTable in SQLite3 database
			}
			if(args.Length > 8){
				DHT.DHT.ValueName = args[8];	//ValueName in HashTable in SQLite3 database
			}
			
			Addnode.DefaultPort = port;
			
		//raise KeyValue HashTable for DHT, with previous args.
		
		//DHT.DHT.RunDHT(args[4], args[5], args[6], args[7]);		//raise KeyValue HashTable for DHT, with previous args.
		//DHT.DHT.RunDHT(args.Skip(5).ToArray()); //last 4 args
		DHT.DHT dht = new DHT.DHT();	//just raise with already defined.
		

			try{
				//	Raise server-side - TCP/UDP-server with UDP-MultiCastGroupIP
				Console.WriteLine("\n\n" +	"Start servers: ");
				//Start TCP Server
				TCP.Server.Start(new string[]{IP, port.ToString()});
				
				UDP.Server.Start(IP, port, UDPMultiCastGroupIP);	//UDP-Server with UDPMultiCastGroupIP				

				//Start UDP Server
			//	UDP.Client.UDPMulticastServer = UDP.Server.UDPServer(null, port, UDPMultiCastGroupIP);	//UDP-Server with UDPMultiCastGroupIP
			//	UDP.Client.UDPMulticastServer = UDP.Server.UDPServer(IP, port, UDPMultiCastGroupIP);	//UDP-Server with UDPMultiCastGroupIP
				System.Threading.Thread.Sleep(500);
			//	Console.WriteLine("UDP.Client.UDPMulticastServer: "+UDP.Client.UDPMulticastServer);



				Console.WriteLine("\n\n" +	"Test TCP clients: ");
				//		Start TCP clients
				string response;
			//	byte[] ResponseBytes;
			
				//	Connect, send request, receive response, then disconnect
				response				=	TCP.Client.Send(localhost, port, "send and close");
			//	ResponseBytes			=	TCP.Client.Send(localhost, port, new byte[]{0,1,2,3,4,5});
				Console.WriteLine(response == "SEND AND CLOSE");
			
				//	Connect tcpClient, and keep connection alive
				TcpClient tcpClient 	=	TCP.Client.Connect(localhost, port);
				//	Send requests using connected tcpClient, and receive responses
				response				=	TCP.Client.Send(tcpClient, "test2");
			//	ResponseBytes			=	TCP.Client.Send(tcpClient, new byte[]{0,1,2,3,4,5});
				Console.WriteLine(response == "TEST2");

				
				Console.WriteLine("\n\n" +	"Test UDP clients: ");
				//		Start UDP clients
	//			UDP.Client udpClient2 = new UDP.Client(ServerUdpIP, ServerUdpPort, UDPMultiCastGroupIP);

				UDP.Client udpClient = new UDP.Client(localhost, port);
				response = udpClient.Send("test");
			//	udpClient.Send(new byte[]{0,1,2,3,4,5});
				Console.WriteLine(response == "TEST");
				

				UDP.Client udpClient2 = new UDP.Client(localhost, port);
				response = udpClient2.Send("test2");
			//	udpClient2.Send(new byte[]{0,1,2,3,4,5});
				Console.WriteLine(response == "TEST2");


				Console.WriteLine("\n\n" +	"Test UDP Multicast: ");
				UDP.Client udpClient3 = new UDP.Client(localhost, port, UDPMultiCastGroupIP);
				response = udpClient3.Send("test");
			//	udpClient.Send(new byte[]{0,1,2,3,4,5});
				Console.WriteLine(response == "TEST");
				

				UDP.Client udpClient4 = new UDP.Client(localhost, port, UDPMultiCastGroupIP);
				response = udpClient4.Send("test2");
			//	udpClient2.Send(new byte[]{0,1,2,3,4,5});
				Console.WriteLine(response == "TEST2");
				


				Console.WriteLine("\n\n" +	"Check alive nodes from known TCP-addnodes (addnodes.txt): ");
				//	Try connect TCP addnodes:
				HashSet<string> KnownAddnodes = Addnode.ReadAddNodes();
				HashSet<string> AliveAddnodes = AliveNodes.GetAliveNodes(KnownAddnodes);	//Alive nodes in AliveNodes.AliveAddnodeList

				Console.WriteLine("\n\n" +	"Try connect alive nodes: ");
				AliveNodes.ConnectAliveNodes();	//Connect to alive nodes;

				Console.WriteLine("\n\n" +	"Show active connections: ");
				//	Show active connections: 
				//		TCP:
				Console.WriteLine("TCP.Client.TCPConnectionsList.Count: "+TCP.Client.TCPConnectionsList.Count);
				//		UDP:
				Console.WriteLine("UDP.Client.UDPConnectionsList.Count: "+UDP.Client.UDPConnectionsList.Count);
				
				
				Console.WriteLine("\n\n" +	"Test connections: ");
				//	Test connections:
				AliveNodes.TestConnections();
				
			

				Console.WriteLine("\n\n" +	"Run LDP (Local Peer Discovery): ");
				//Run LDP (Local Peer Discovery)
				LocalPeersDiscovery.DiscoveryPeers();
				IsPeer.ShowActivePeers();

				Console.WriteLine("\n\n" +	"Run interval for LDP (Local Peer Discovery)... ");
//				//Run interval for LDP (Local Peer Discovery)
				LocalPeersDiscovery.RunDiscoveryPeersInterval();	//seconds
			
			
			
			
			//	Console.WriteLine("\n\n" +	"Check peers, before run PEX: ");
				//	Check peers, before run PEX
			//	IsPeer.CheckPeers();
			//	Console.WriteLine("IsPeer.TCPPeers.Count: "+IsPeer.TCPPeers.Count);
			//	Console.WriteLine("IsPeer.UDPPeers.Count: "+IsPeer.UDPPeers.Count);
			
			
				//	Run Peer Exchange:
			//	TCP:
		//	LocalPeersList.TryConnectPeersTCP();
			Console.WriteLine("\n\n" +	"Try TCP PEX");
			PEX_client.TCPPeerExchange();
		
			//	UDP:
			Console.WriteLine("\n\n" +	"Try UDP PEX");
		//	LocalPeersList.TryConnectPeersUDP();
			PEX_client.UDPPeerExchange();

				
			//	Console.WriteLine("\n\n" +	"Check peers, before run DHT-sync: ");
				//	Check peers, before run DHT-sync
			//	IsPeer.CheckPeers();

			//	Console.WriteLine("\n\n" +	"Sync DHT TCP: ");
				//	Sync DHT TCP:
			//	DHT.DHT_client.TCPSyncDHT();

			//	Console.WriteLine("\n\n" +	"Sync DHT UDP: ");
				//	Sync DHT UDP:
			//	DHT.DHT_client.UDPSyncDHT();
				
			//	Console.WriteLine("\n\n" +	"Sync DHT TCP UDP: ");
				//	Sync DHT TCP UDP:
			//	DHT.DHT_client.TCPUDPSyncDHT();
				
				DHT.DHT_client.RunSyncDHTByInterval(60);	//run DHT syncrhonization with peers, by interval.
				
			}
			catch(Exception ex){
				Console.WriteLine(ex);
			}

			//Do not close window, after all.
			Console.ReadLine();
		}
	}
}
