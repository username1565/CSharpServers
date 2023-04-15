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
		//Binary encoding to encode bytes to string, and decode it back. This allow to encode any byte as one char, and decode it back.
		public Encoding BinaryEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");

		//from specified udpClient, send RequestBytes, receive response as bytes, and return this as bytes.
		public byte[] SendUDP(
				byte[]			RequestBytes
		)
		{
			try{
				Console.WriteLine("request: "+BinaryEncoding.GetString(RequestBytes));
				udpClient.Send(RequestBytes, RequestBytes.Length);
				
				IPEndPoint remoteEP = null; //	IPEndPoint of incoming connection
				byte[] ResponseBytes   = udpClient.Receive(ref remoteEP);
				Console.WriteLine("response: "+BinaryEncoding.GetString(ResponseBytes));
				return ResponseBytes;
			}
			catch (Exception ex){
				Console.WriteLine(ex);
				return null;
			}
		}
		
		//from specified udpClient, send request as string, receive response as bytes, and return this as string.
		public string SendUDP(
				string			request = null
			,	Encoding		encoding = null
		)
		{
			encoding = (encoding != null) ? encoding : Encoding.ASCII;
			Console.WriteLine("UDP Client connected to server: "+Program.Convert.IP_PORT((IPEndPoint)udpClient.Client.RemoteEndPoint));
			byte[] RequestBytes = null;
			//RequestBytes with bytes to send
			if(!string.IsNullOrEmpty(request))
			{
				RequestBytes = encoding.GetBytes(request);
			}
			else if(RequestBytes == null)
			{
				RequestBytes = encoding.GetBytes(DateTime.Now.ToString("HH:mm:ss.fff"));
			}
			byte[] ResponseBytes = null;
			try{
				ResponseBytes = SendUDP(RequestBytes);
				string response = encoding.GetString(ResponseBytes);
				if (RequestBytes != null && RequestBytes.Length > 0)
				{
					Console.WriteLine("Client recived UDP: " + response);
				}
				return response;
			}
			catch (Exception ex){
				Console.WriteLine(ex);
				return null;
			}
		}

		//From defined udpClient send in separate thread an UDP request as bytes, recive response from UDP-server, and return this as an obj-parameter - RequestBytes
		private void SendUDPProc(object arg)
		{
		//	Console.WriteLine("UDP client thread started");
			try
			{
				object[] parameters = (object[])(arg);
				byte[] RequestBytes = (byte[])(parameters[0]);
				byte[] ResponseBytes = (byte[])(parameters[1]);

				Console.WriteLine(
						"UDP Client thread started: from "+Program.Convert.IP_PORT((IPEndPoint)(((UdpClient)udpClient).Client).LocalEndPoint)
					+	" to "	+Program.Convert.IP_PORT((IPEndPoint)(((UdpClient)udpClient).Client).RemoteEndPoint)
				);
				
				ResponseBytes = SendUDP(RequestBytes);
				parameters[1] = ResponseBytes;
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode != 10004) // unexpected
					Console.WriteLine("UDPClientProc exception: " + ex);
			}
			catch (Exception ex)
			{
				Console.WriteLine("UDPClientProc exception: " + ex);
			}
		//	Console.WriteLine("UDP client thread finished");
		}
	}
}