using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Peer
{
	public class PEX_client
	{
		public static List<string> peersLists = new List<string>(){"5.5.5.5"};
	
		//use PEX-server's methods
		public static string ReturnPeersList(){
			return PEX_server.ReturnPeersList();
		}
		
		public static void UpdatePeersList(string RemotePeers){
			PEX_server.UpdatePeersList(RemotePeers);
		}
		
		public static string PeerExchange(string response){
			return PEX_server.PeerExchange(response);
		}
		
	
		public static void TCPPeerExchange(){
			Console.WriteLine("PeerExchangeClient.cs: TCPPeerExchange(). IsPeer.TCPPeers.Count: "+IsPeer.TCPPeers.Count);
			try{
				foreach(string tcpPeer in IsPeer.TCPPeers)
				{
					string[] IP_PORT = tcpPeer.Split(':');
					string IP = IP_PORT[0];
					int port = System.Int32.Parse(IP_PORT[1]);
					
					string request = "Peer Exchange: "+ReturnPeersList();			//send client's peers
					Console.WriteLine("TCP sent PEX request: "+request);
					string response_string = TCP.Client.Send(IP, port, request);	//receive server's peers
					Console.WriteLine("TCP received PEX response: "+response_string);
					PeerExchange(response_string);									//Add server's peers.
				}
			}
			catch(Exception ex){
				Console.WriteLine(ex);
			}
		}

		public static void UDPPeerExchange(){
			Console.WriteLine("PeerExchangeClient.cs: UDPPeerExchange(). IsPeer.UDPPeers.Count: "+IsPeer.UDPPeers.Count);
			try{
				foreach(string udpPeer in IsPeer.UDPPeers)
				{
					string[] IP_PORT = udpPeer.Split(':');
					string IP = IP_PORT[0];
					int port = System.Int32.Parse(IP_PORT[1]);
					
					string request = "Peer Exchange: "+ReturnPeersList();
					Console.WriteLine("UDP send PEX request: "+request);
					string response_string = UDP.Client.Send(IP, port, request);
					Console.WriteLine("UDP received PEX response: "+response_string);
					PeerExchange(response_string);
				}
			}
			catch(Exception ex){
				Console.WriteLine(ex);
			}		
		}

/*
		public static void UDPPeerExchange(){
		//	Console.WriteLine("Try PEX: UDP.Client.UDPConnectionsList.Count: "+UDP.Client.UDPConnectionsList.Count);
			foreach(UdpClient ConnectedUDPClient in UDP.Client.UDPConnectionsList){
			//	Console.WriteLine("Peer Exchange - request");
				string response = (string)UDP.Client.Connect(ConnectedUDPClient, PEX_server.PEXStub)[0];
				Console.WriteLine("1 PEX response: "+response);

				Console.WriteLine("PEX received peers: "+UDP.Client.LastResponse);
				peersLists.Add(UDP.Client.LastResponse);	//get peers there
				
				foreach(string peersList in peersLists){
					string[] peers = peersList.Split(';');
					foreach(string peer in peers){
						if(!(LocalPeersList.PeersList).Contains(peer)){
							(LocalPeersList.PeersList).Add(peer);
						}
					}
				}
				
				//send my peers there
				string myPeers = string.Join(";", (LocalPeersList.PeersList));
				string sendMyPeers = "Peer Exchange - response: " + myPeers;
				Console.WriteLine("sendMyPeers: "+sendMyPeers);
				response = (string)UDP.Client.Connect(ConnectedUDPClient, sendMyPeers)[0];
				Console.WriteLine("2 PEX response: "+response);
			}
		}
*/		
		public static void PeerExchange(){
			
		}
	}
}