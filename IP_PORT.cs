using System;
using System.Net;
using System.Net.Sockets;

namespace Program
{
	partial class Convert
	{
/*
	Convert IPEndPoint to string "IP:PORT"
*/	
		public static string IP_PORT(IPEndPoint endPoint){
			string ip_port = endPoint.Address.ToString()+":"+endPoint.Port.ToString();
			return ip_port;
		}
	}
}
