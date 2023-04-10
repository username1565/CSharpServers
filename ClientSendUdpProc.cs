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

				IPEndPoint remoteEP = null;		//	IPEndPoint of incoming connection
				byte[] ResponseBytes   = null;	//	response bytes
				string response = "";
				
				//if multicast-client
				if(udpClient.Client.MulticastLoopback == true){
					//send multicast-request, and receive many responses:
					IPEndPoint multicastEP = new IPEndPoint(IPAddress.Parse(MultiCastGroupIP), UdpServerPort);
					udpClient.Send(RequestBytes, RequestBytes.Length, multicastEP);	//send to server's multicast EndPoint
					
					//Receive responses:
					//Multicast Client can receive many resposes, from different interfaces.
					//set Timeout to receive responses
					udpClient.Client.ReceiveTimeout = 20;
					bool receiving = true;
					while(receiving){
						try{
							remoteEP = null; // IPEndPoint of incoming connection
							ResponseBytes   = udpClient.Receive(ref remoteEP);
							response = BinaryEncoding.GetString(ResponseBytes);

						//	Console.WriteLine("receiving response: "+response);
							//and work with the current response
						}
						catch{	//if no any response, within ReceiveTimeout
							receiving = false;
						}
					}
					Console.WriteLine("response: "+response);
					return ResponseBytes;	//return last response
				}
				else{ //if UDP-client without multicast:
					
					//set timeout to receive response:
					udpClient.Client.ReceiveTimeout = 5000;
				
					//just send UDP-request
					udpClient.Send(RequestBytes, RequestBytes.Length);

					try{
						//and receive response
						remoteEP = null; // IPEndPoint of incoming connection
						ResponseBytes   = udpClient.Receive(ref remoteEP);
						response = BinaryEncoding.GetString(ResponseBytes);
						//	Console.WriteLine("receiving response: "+response);
							//and work with the current response
					}
					catch{
					}
					Console.WriteLine("response: "+response);
					return ResponseBytes;
				}
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

		//from defined udpClient send in separate thread an UDP request as bytes, and return ResponseBytes
		private void SendUDPProc(object arg)
		{
		//	Console.WriteLine("UDP client thread started");
			try
			{
				object[] parameters = (object[])(arg);
				byte[] RequestBytes = (byte[])(parameters[0]);
				byte[] ResponseBytes = (byte[])(parameters[1]);
				
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