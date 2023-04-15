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
		
	
		//Local Peers Discovery over UDP multicast request
		public static void UDPMulticastPeersDiscovery()
		{
			string UdpIP = "0.0.0.0";
			int UdpPort = 8081;
			string MultiCastGroupIP = "235.5.5.11";
		
			UDP.Client udpClient_ = new UDP.Client(UdpIP, UdpPort, MultiCastGroupIP);
			UdpClient udpClient = udpClient_.udpClient;	//get UdpClient

			//Run Local Peer Discovery - send UDP-MultiCast request to MultiCastGroupIP
			byte[] buffer;
			IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(MultiCastGroupIP), UdpPort);
			buffer = Encoding.ASCII.GetBytes("Local Peer Discovery, using UDP-MultiCast");	//send this
			udpClient.Send(buffer, buffer.Length, endPoint);
		
			//receive response
			IPEndPoint remoteEP = null; // IPEndPoint of incoming connection
			buffer   = udpClient.Receive(ref remoteEP);

			//if sommebody respond
			if (buffer != null && buffer.Length > 0)
			{
				//Console.WriteLine("remoteEP.Address.ToString(): "+remoteEP.Address.ToString());
				string remoteIP = remoteEP.Address.ToString();	//get IP
				string peer = (remoteIP+":"+UdpPort);			//and join with port to IP:PORT
				
				//add peer to PeersList
				Console.WriteLine("UDP.Client.UDPMulticastPeersDiscovery()."); //Encoding.ASCII.GetString(buffer));
				Console.WriteLine("Client received Multicast: " + peer);
				if(!Peer.IsPeer.Peers.Contains(peer))
				{
					Peer.IsPeer.CheckPeer(peer);	//add peer in PeersList;
					Console.WriteLine("Add alive peer. Peer.IsPeer.Peers.Count: "+Peer.IsPeer.Peers.Count);
				}
			}
			//peers in Peer.LocalPeersList.PeersList
			
			//	if(udpClient != null)
			//		udpClient.Close();

		}
	}
}