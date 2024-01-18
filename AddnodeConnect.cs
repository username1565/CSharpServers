using System;
using System.IO;	//File.
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;	//HashSet

namespace Peer
{
	/*
		Load addnodes from "addnodes.txt" into AddnodeList
		For each node in AddnodeList, check is this alive TCP-server, and add to AliveNodeList, and Peers.
	*/
	public class Addnode
	{
		public static string addnodesFile = "addnodes.txt";
		
		public static int DefaultPort = 8081;
		
		public static HashSet<string> AddnodeList = new HashSet<string>(){		//HashSet known peers (nodes)
			"127.0.0.1:"+DefaultPort
		};
		
		public static HashSet<string> ReadAddNodes()
		{
			Console.WriteLine("Peer.Addnode.ReadAddNodes(): ");
		
			if(!File.Exists(@addnodesFile)){		//if not exists
				File.WriteAllText(addnodesFile, "");	//create
			}
			
			string[] addnodes = File.ReadAllLines(@addnodesFile);
			for(int i = 0; i<addnodes.Length; i++)
			{
				string addnode = addnodes[i];
				if(addnode.Contains("addnode="))
				{
					string node = addnode.Split('=')[1];
					
					//check port
					string[] IP_PORT = node.Split(':');
					if(IP_PORT.Length == 1){	//if IP only
						//add default port
						//node is IP_PORT
						node = IP_PORT[0]+":"+DefaultPort.ToString();
					}
					AddnodeList.Add(node);	//and add node to the list.
					
					if(IP_PORT[1] != DefaultPort.ToString()){	//if port is not default
						AddnodeList.Add(IP_PORT[0]+":"+DefaultPort.ToString());	//Add this node with default port, too.
					}
				}
			}
			Console.WriteLine("Peer.Addnode.AddnodeList.Count: "+Addnode.AddnodeList.Count);
			return AddnodeList;
			//Known nodes in Addnode.AddnodeList
		}
		
		public static void SaveAddNodes()
		{
			if(AddnodeList.Count == 0){return;}
			string Addnodes = "";
			foreach(string node in AddnodeList){
				Addnodes += "addnode="+node+"\n";
			}
			Addnodes = Addnodes.Substring(0, Addnodes.Length-1);
			File.WriteAllText(addnodesFile, Addnodes);
		}
		
		public static void SaveAddNodes(HashSet<string> AliveAddnodeList)
		{
			foreach(string node in AliveAddnodeList){
				AddnodeList.Add(node);
			}
			SaveAddNodes();
		}		
	}
	
	public class AliveNodes
	{
		public static HashSet<string> AliveAddnodeList = new HashSet<string>(){};	//HashSet alive nodes

		public static bool TryConnectTCP(string node, string IP, int port){
			bool isAliveTCP = IsPeer.IsTCPServer(IP, port);
			if(isAliveTCP){
				AliveAddnodeList.Add(node);
				return true;
			}
			return false;
		}

		public static bool TryConnectUDP(string node, string IP, int port){
			bool isAliveUDP = IsPeer.IsUDPServer(IP, port);
			if(isAliveUDP){
				AliveAddnodeList.Add(node);
				return true;
			}
			return false;
		}
		
		public static HashSet<string> GetAliveNodes(HashSet<string> AddnodeList = null)
		{
			if(AddnodeList == null)	//if addnode list not specified
			{
				if(Addnode.AddnodeList.Count == 0){	//and if there is no any hardcoded addnodes.
					Addnode.ReadAddNodes();				//load addnodes.
				}
				AddnodeList = Addnode.AddnodeList;		//or/and use this list
			}
			
			Console.WriteLine("Peer.AliveNodes.GetAliveNodes(). AddnodeList.Count: "+AddnodeList.Count);
			foreach(string node in AddnodeList){
				string[]	IP_PORT	=	node.Split(':');
				string		IP		=	IP_PORT[0];
				int			port	=	System.Int32.Parse(IP_PORT[1]);
				
				try{
					TryConnectTCP(node, IP, port);
					continue;
				}
				catch{
					TryConnectUDP(node, IP, port);
				}
			}
			Console.WriteLine("AliveAddnodeList.Count: "+AliveAddnodeList.Count);
			Addnode.SaveAddNodes(AliveAddnodeList);	//save this to "addnode.txt".
			return AliveAddnodeList;
			//Alive nodes in AliveNodes.AliveAddnodeList
		}
		
		public static bool TestConnections(){
			try{
				string response = null;
				foreach(TcpClient tcpClient in TCP.Client.TCPConnectionsList){
					Console.WriteLine("TCP server: "+tcpClient.Client.RemoteEndPoint.ToString());
					response = TCP.Client.Send(tcpClient, "test"	);
					Console.WriteLine(response == "TEST");
					response = TCP.Client.Send(tcpClient, "test2"	);
					Console.WriteLine(response == "TEST2");
				}
				foreach(UDP.Client udpClient in UDP.Client.UDPConnectionsList){
					Console.WriteLine("UDP server: "+udpClient.udpClient.Client.RemoteEndPoint.ToString());
					response = udpClient.Send("test");
					Console.WriteLine(response == "TEST");
					response = udpClient.Send("test2");
					Console.WriteLine(response == "TEST2");
				}
				return true;
			}
			catch{
				return false;
			}
		}

		public static void ConnectAliveNodes(){
			foreach(string node in AliveAddnodeList){
				string[]	IP_PORT	=	node.Split(':');
				string		IP		=	IP_PORT[0];
				int			port	=	System.Int32.Parse(IP_PORT[1]);
				
				try{
					bool isAliveTCP = IsPeer.IsTCPServer(IP, port);
					if(isAliveTCP){
						AliveAddnodeList.Add(node);

						TCP.Client.TryConnect(node);
						IsPeer.TCPPeers.Add(node);
						IsPeer.Peers.Add(node);
					}
				}catch{
				}
				
				try{
					bool isAliveUDP = IsPeer.IsUDPServer(IP, port);
					if(isAliveUDP){
						//UDP.Clients.AddUdpClient(IP, port);
						UDP.Client.TryConnect(node);
						AliveAddnodeList.Add(node);
						IsPeer.UDPPeers.Add(node);
						IsPeer.Peers.Add(node);
					}
				}catch{
				}
			}
			//TestConnections();
		}
		
		
	}
}