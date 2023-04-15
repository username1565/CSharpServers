using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Timers;	//interval

namespace Peer
{
	public class LocalPeersDiscovery
	{
		//set interval for Peer Discovery
		public static int PeerDiscoveryInterval = 1; //seconds to repeat send UDP MultiCast request to discovery peers.
		
		//Method to run Peer Discovery
		public static void DiscoveryPeers(){
			Console.WriteLine("\n\n" + "Peer.LocalPeersDiscovery.DiscoveryPeers() - send multicast");
			UDP.Client.UDPMulticastPeersDiscovery();	//send MultiCast UDP request, to discovery peers in LAN
		}
		
		//Define timer, to repeat Peer Discovery
		public static System.Timers.Timer aTimer = null;	

		//trigger this time, when timer elapsed
		public static void DiscoveryPeers(object source, ElapsedEventArgs e)
		{
			DiscoveryPeers();
			IsPeer.ShowActivePeers();
		}
		
		//Method to run Peer Discovery
		public static void RunDiscoveryPeersInterval(int setPeerDiscoveryInterval = -1){
			if(setPeerDiscoveryInterval != -1){
				PeerDiscoveryInterval = setPeerDiscoveryInterval;
			}
			
			//run interval
			aTimer = new System.Timers.Timer( PeerDiscoveryInterval * 1000 );
			aTimer.Elapsed += new ElapsedEventHandler(DiscoveryPeers);
			aTimer.AutoReset = true;
			aTimer.Enabled = true;	
		}
	}
}