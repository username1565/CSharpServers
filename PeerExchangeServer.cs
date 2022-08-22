using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Peer
{
	public class PEX_server
	{
		public static char PeersSeparator = ';'; //List of peers, separator

		public static string ReturnPeersList(){
			//return list of peers, joined with PeersSeparator
			return string.Join(PeersSeparator.ToString(), (IsPeer.Peers));
		}
		
		public static void UpdatePeersList(string RemotePeers)
		{
			//update peers, from list of peers, separated PeersSeparator
			string peersList = RemotePeers;
			string[] peers = peersList.Split(PeersSeparator);
			foreach(string peer in peers){
				if(!(IsPeer.Peers).Contains(peer)){
					(IsPeer.Peers).Add(peer);
				}
			}
		}
	
		public static string PEXStub = "Peer Exchange: ";

		public static string PeerExchange(string request){
			if(request.Contains(PEXStub)){
				string RemotePeers = request.Substring(PEXStub.Length); //remove "Peer Exchange - response: "
				UpdatePeersList(RemotePeers);							//update and add remote peers
				return PEXStub + ReturnPeersList();						//return local peers
			}
			else{
				return null;
			}
		}
	}
}