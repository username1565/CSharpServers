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
		public static void UDPMulticastPeersDiscovery(
			int ReceiveTimeout = 500
		)
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
		
			//Receive responses:
			IPEndPoint remoteEP = null; // IPEndPoint of incoming connection
			string response = "";

			//Multicast Client can receive many resposes, from different interfaces.
			//set Timeout to receive responses
			udpClient.Client.ReceiveTimeout = ReceiveTimeout;
			bool receiving = true;
			while(receiving){
				try{
					remoteEP = null; // IPEndPoint of incoming connection
					buffer   = udpClient.Receive(ref remoteEP);

					//if sommebody respond
					if (buffer != null && buffer.Length > 0)
					{
						response = udpClient_.BinaryEncoding.GetString(buffer);
					//	Console.WriteLine("receiving response: "+response);
						//and work with the current response
						if(response == "Local Peer Discovery, using UDP-MultiCast".ToUpper()){
							string peer = remoteEP.ToString();
							Console.WriteLine("UDP.Client.UDPMulticastPeersDiscovery(). Client received Multicast from " + peer);

							//add peer to PeersList
							if(!Peer.IsPeer.Peers.Contains(peer))
							{
								Peer.IsPeer.CheckPeer(peer);	//add peer in PeersList;
								Console.WriteLine("Add alive peer. Peer.IsPeer.Peers.Count: "+Peer.IsPeer.Peers.Count);
							}
						}
					}
				}
				catch{	//if no any response, within ReceiveTimeout
					receiving = false;
				}
			}
			
			if(udpClient != null)
			{
				udpClient.Close();
			}
			//peers in Peer.LocalPeersList.PeersList
		}
	}
}