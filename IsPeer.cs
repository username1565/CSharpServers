using System;						//Console.WriteLine()
using System.Collections.Generic;	//List<string>

/*
	Check is IP:PORT an available peer, or not? true/false
	Add any avaliable peer in Peers
	Add available TCP-peers in TCPPeers
	Add available UDP-peers (with multicast, or not) in UDPPeers
*/
namespace Peer
{
	public class IsPeer
	{
		public static HashSet<string> Peers			=	new	HashSet<string>();	//peers TCP or UDP (with multicast not), does not matter.
		public static HashSet<string> TCPPeers		=	new	HashSet<string>();	//TCP only peers
		public static HashSet<string> UDPPeers		=	new	HashSet<string>();	//UDP only peers (with multicast, or not)
	
/*
		//Check is alive peer: true/false.
		public static bool IsAnyAlivePeer(string IP, int port){
			try{
				if(TCP.Client.Send(IP, port, "Are you peer?") == "Yes, I'm peer."){			//first request - TCP
					return true;
				}
			}catch{
				try{
					if(UDP.Client.Send(IP, port, "Are you peer?") == "Yes, I'm peer."){		//second request - UDP, if fail.
						return true;
					}
				}catch{}
			}
			return false;
		}
		
		public static bool AddAnyAlivePeer(string IP, int port){
			if(IsAnyAlivePeer(IP, port)){
				string peer = IP+":"+port.ToString();
				Peers.Add(peer);	//Add any alive peer, here
			}
		}
*/
		
		//	With 2 requests, check is current peer alive or not:
		//	Result: 
		//		Not an alive peer (0)
		//	,	alive TCP-peer (1)
		//	,	alive UDP-peer (2)
		//	,	alive TCP and UDP peer (3)
		
		public static int WhatIsAlivePeer(string IP, int port){
			string peer = IP+":"+port.ToString();	//string peer = "IP:PORT"
			
			int isPeer = 0;		//0 - if not alive
			
			try{
				if(TCP.Client.Send(IP, port, "Are you peer?") == "Yes, I'm peer."){		//first request - TCP
					isPeer = 1;		//1 - if alive TCP peer
				}
			}catch{}
			
			try{
				if(UDP.Client.Send(IP, port, "Are you peer?") == "Yes, I'm peer."){		//second request - UDP
					if(isPeer == 0){		//if not TCP-peer
						isPeer = 2;				//UDP only peer
					}
					else if(isPeer == 1){	//if it was been TCP-peer too
						isPeer = 3; 			//TCP and UDP peer
					}
				}
			}catch{}
			
			return isPeer;	//0-3
		}
		
		//Check is peer alive (with 2 requests), and add this peer to lists, if alive;
		public static int CheckPeer(string IP, int port){
			string peer = "";
			int AlivePeer = WhatIsAlivePeer(IP, port);	//2 requests - TCP and UDP
			if( AlivePeer > 0 ){
				peer = IP+":"+port.ToString();
				Peers.Add(peer);	//Add any alive peer, here
			}
			if( AlivePeer == 1 ){		//if TCP
				TCPPeers.Add(peer);			//Add TCP-peer
			}
			else if( AlivePeer == 2 ){	//if UDP
				UDPPeers.Add(peer);			//Add UDP-peer
			}
			else if( AlivePeer == 3 ){	//if TCPUDP
				TCPPeers.Add(peer);			//Add TCP-peer
				UDPPeers.Add(peer);			//Add UDP-peer
			}
			return AlivePeer;
		}

		public static int CheckPeer(string IPPORT){
			object[]	ipPORT	=	Program.Convert.IP_PORT(IPPORT)			;
			string		IP		=	(string)	ipPORT[0]	;
			int			port	=	(int)		ipPORT[1]	;
			return CheckPeer(IP, port);
		}
		
		//	Another case to check TCP/UDP servers:
		//is TCP server or not? true/false
		public static bool IsTCPServer(string IP, int port){
			string peer = IP+":"+port.ToString();
			if(TCPPeers.Contains(peer)){
				return true;
			}
			try{
				string response_string = TCP.Client.Send(IP, port, "Are you TCP server?");	//TCP-server only
				if(response_string == "Yes, I'm TCP server."){
					Peers.Add(peer);	//Add any peer
					TCPPeers.Add(peer);	//Add TCP-peer
					return true;
				}
				return false;
			}
			catch//	(Exception ex)
			{
			//	Console.WriteLine(ex);
				return false;
			}
		}

		//is UDP server or not? true/false
		public static bool IsUDPServer(string IP, int port){
			string peer = IP+":"+port.ToString();
			if(UDPPeers.Contains(peer)){
				return true;
			}
			try{
				string response_string = UDP.Client.Send(IP, port, "Are you UDP server?");	//UDP-server only
				if(response_string == "Yes, I'm UDP server."){
					Peers.Add(peer);	//Add any peer
					UDPPeers.Add(peer);	//Add UDP-peer
					return true;
				}
				return false;
			}
			catch//	(Exception ex)
			{
			//	Console.WriteLine(ex);
				return false;
			}
		}

		//Check many peers in specified, or current peers-list
		public static void CheckPeers(HashSet<string> Peers = null){
			if(Peers == null){
				Peers = IsPeer.Peers;
			}
			
			foreach(string peer in Peers){
				string[] IP_PORT = peer.Split(':');
				string IP = IP_PORT[0];
				int port = System.Int32.Parse(IP_PORT[1]);
				
				if(!TCPPeers.Contains(peer) && IsTCPServer(IP, port)){
					Peers.Add(peer);
					TCPPeers.Add(peer);
				}
				if(!UDPPeers.Contains(peer) && IsUDPServer(IP, port)){
					Peers.Add(peer);
					UDPPeers.Add(peer);
				}
			}
			ShowActivePeers();
		}
		
		public static void ShowActivePeers(){
			Console.WriteLine("IsPeer.ShowActivePeers(): ");
			//show found peers:
			int peer_number;

			peer_number = 0;
			foreach (var peer in TCPPeers)
			{
				Console.WriteLine("TCP peer №"+(++peer_number) + ": "+ peer);
			}

			peer_number = 0;
			foreach (var peer in UDPPeers)
			{
				Console.WriteLine("UDP peer №"+(++peer_number) + ": "+ peer);
			}
		}
	}
}