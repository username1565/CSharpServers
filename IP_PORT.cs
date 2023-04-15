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
			return ip_port;	//string "IP:PORT"
		}
		
		public static object[] IP_PORT(string IPPORT){
			string[]	IP_PORT	=	IPPORT.Split(':');
			string		IP		=	IP_PORT[0];
			int			port	=	System.Int32.Parse(IP_PORT[1]);
			return new object[]{IP, port};			//string, int
		}
		
		public static object[] IPAddress_PORT(object[] IPPORT)
		{
			IPAddress	IP		=	IPAddress.Parse((string)IPPORT[0]);
			int			port	=	(int)(IPPORT[1]);
			return new object[]{IP, port};			//IPAddress, int
		}

		public static string IP_PORT(string IP, int PORT)
		{
			return IP+":"+PORT.ToString();				//string "IP:PORT"
		}

		public static string IP_PORT(IPAddress IP, int PORT)
		{
			return IP.ToString()+":"+PORT.ToString();	//string "IP:PORT"
		}
	}
}
