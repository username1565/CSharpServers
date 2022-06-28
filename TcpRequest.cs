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
/*
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
*/

					byte[] data = new byte[64]; // buffer to receive data
                    
					// receive message
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
 
                    Console.WriteLine(message);
					return message;

			
		}
	}
}