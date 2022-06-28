using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TcpServer
{
	class TcpRequest
	{
		public static Encoding utf8 = Encoding.UTF8;	//Encoding for TcpRequests.
		
		//read TCP request from bytes to string
		public static string ToString (NetworkStream stream)
		{
			MemoryStream memoryStream = new MemoryStream ();
			byte[] data = new byte[256];
			int size;
			do {
				size = stream.Read (data, 0, data.Length);
				if (size == 0) {
					Console.WriteLine ("client disconnected...");
					Console.ReadLine ();
					return  null; 
				} 
				memoryStream.Write (data, 0, size);
			} while ( stream.DataAvailable); 
			return utf8.GetString (memoryStream.ToArray ());
		}
	}
}