using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

/*
	TCPServerProc
Thread to Start TCP Server.
Start this.
Accept TCP-client, when someone connected.
Read tcpRequest from client's stream.
Write tcpResponse by client's request.
*/
//Server-side
namespace TCP
{
	partial class Server
	{
		
		private static bool isClientConnected(TcpClient tcpClient){
			bool connected = (
					tcpClient.Connected
				&&	tcpClient.Client.Poll(01, SelectMode.SelectWrite)
				&&	tcpClient.Client.Poll(01, SelectMode.SelectRead)
				&&	!tcpClient.Client.Poll(01, SelectMode.SelectError)
			//	&&	tcpClient.Client.Poll(0, SelectMode.SelectRead)
			);
		//	Console.WriteLine("isClientConnected. connected: "+connected);
			return connected;
		}
		
		private static bool IsSocketConnected(Socket socket)
		{
			try
			{
				return !(socket.Poll(01, SelectMode.SelectRead) && socket.Available == 0);
			}
			catch (SocketException) { return false; }
		}

		private static Encoding encoding = Encoding.ASCII;

		private static void AcceptedClient(object arg){
		
			object[] parameters = (object[])arg;
			TcpClient client = (TcpClient)(parameters[0]);
			TcpListener server = (TcpListener)(parameters[1]);
		
		//	Console.WriteLine("TCP Server "+Program.Convert.IP_PORT((IPEndPoint)server.LocalEndpoint)+" accepted client: "+Program.Convert.IP_PORT((IPEndPoint)client.Client.RemoteEndPoint));

			NetworkStream stream = client.GetStream();
					
			string tcpRequest;
			byte[] tcpResponse;
			while(true){
				try{
					int interval = 20;
					int tries = 0;
					int maxtries = (client.ReceiveTimeout / interval);
					while (!stream.DataAvailable)
					{
						if(!IsSocketConnected(client.Client)){
					//		Console.WriteLine("closeClient");
							goto closeClient;
						}
						Thread.Sleep(interval);
					//	Console.WriteLine("try read"+tries);
						tries += 1;
						if(tries > maxtries){	//if too many tries
							goto closeClient;	//close client
						}
					}
					//uncomment client.ReceiveTimeout
				
					//Read TCP-request
					tcpRequest = TCP.Server.Request(stream, encoding);
					Console.WriteLine("TCP request: " + tcpRequest);
					
					//Write TCP-response
					tcpResponse = TCP.Server.Response(tcpRequest, encoding);	//else get response bytes
					stream.Write(tcpResponse, 0, tcpResponse.Length);			//write this
					Console.WriteLine("TCP response: " + encoding.GetString(tcpResponse));			//show this
				}
				//on error
				catch{
					goto closeClient;	//break from cycle
				}
			}
			closeClient:
				Console.WriteLine("Close NetworkStream...");
				stream.Close();	//Close NetworkStream
				Console.WriteLine("Close client...");
				client.Close();	//close client in the end of thread
		}
	
		private static void TCPServerProc(object arg)
		{
			Console.WriteLine("TCP server thread started: "+Program.Convert.IP_PORT((IPEndPoint)((TcpListener)arg).LocalEndpoint));

			try
			{
				TcpListener server = (TcpListener)arg;

				server.Start();

				for(;;)
				{
				//	Console.WriteLine("AcceptClient");
					TcpClient client = server.AcceptTcpClient();
				
					client.ReceiveTimeout = 5000000;	//milliseconds
				/*
					// Gets the receive time out using the ReceiveTimeout public property.
					if (client.ReceiveTimeout > 5000000)	//or set this value
					{
						Console.WriteLine("The receive time out limit was successfully set " + client.ReceiveTimeout.ToString());
					}
				*/
					
					object[] parameters = new object[]{client, server};
					
					//start new thread on accept TCP-connection from some client
					var tcpClientThread          = new Thread(new ParameterizedThreadStart(AcceptedClient));
			
					//run this in background
					tcpClientThread.IsBackground = true;
			
					//set thread-name
					tcpClientThread.Name         = "accepted TCP client "+Program.Convert.IP_PORT((IPEndPoint)client.Client.RemoteEndPoint)+" thread";

					//start thread
					tcpClientThread.Start(parameters);
				}
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode != 10004) // unexpected
					Console.WriteLine("TCPServerProc exception: " + ex);
			}
			catch (Exception ex)
			{
				Console.WriteLine("TCPServerProc exception: " + ex);
			}

			Console.WriteLine("TCP server thread finished");

			Console.WriteLine("Run again...");
			TCPServerProc(arg);
		}
	}

}