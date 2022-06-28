using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;			//Thread. ThreadStart
using TcpServer;

namespace TcpServer
{
	class TcpServer
	{
		public static Encoding encoding = Encoding.UTF8;
		//public static Encoding encoding = Encoding.GetEncoding("ISO-8859-1");	//binary encoding
	
        private static int port = 8888;
        static TcpListener listener;
        public static void Main(
			string[] args	//port in args[0]
		)
        {
			//program accept port, in args[0]
			if(args.Length>=1){
				int TryGetPort = 0;
				if(int.TryParse(args[0], out TryGetPort))
				{
					port = TryGetPort;
				}
				Console.WriteLine("port: "+port);
			}

            try
            {
				//default IP is localhost
				//IPAddress ipAddress = Dns.GetHostEntry ("localhost").AddressList [0];

				//Use 0.0.0.0
				IPAddress ipAddress = IPAddress.Any;

				//listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);

				listener = new TcpListener(ipAddress, port);
                listener.Start();
				
				Console.WriteLine ("start up http server..."+" ipAddress: "+ipAddress+", port: "+port);
                Console.WriteLine("Wait connections...");
 
				//start listen
                while(true)
                {
					//when client connected, accept client
                    TcpClient client = listener.AcceptTcpClient();
					//and start work with client
                    ClientObject clientObject = new ClientObject(client);
 
                    // Create new thread to work with client
					//in new thread
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
					//start thread
                    clientThread.Start();
                }
            }
            catch(Exception ex)
            {
				//or write this
                Console.WriteLine(ex.Message);
            }
            finally
            {
				//stop listener
                if(listener!=null)
                    listener.Stop();
            }
        }

	
/*
		public static void Main (string[] args)
		{
		
			int port = 8080;
			
			if(args.Length>=1){
				int TryGetPort = 0;
				if(int.TryParse(args[0], out TryGetPort))
				{
					port = TryGetPort;
				}
				Console.WriteLine("port: "+port);
			}
			
			//IPAddress ipAddress = Dns.GetHostEntry ("localhost").AddressList [0];
			IPAddress ipAddress = IPAddress.Any;
			
			Console.WriteLine ("start up http server..."+" ipAddress: "+ipAddress+", port: "+port);
			
			TcpListener listener = new TcpListener (ipAddress, port);
			listener.Start (); 

			while (true) {
				TcpClient client = listener.AcceptTcpClient (); 
				Console.WriteLine ("TCP request incoming...");
			 
				NetworkStream stream = client.GetStream (); 
				string request = TcpRequest.ToString (stream); 
			
				byte[] response = TcpResponse.Response(request);
				stream.Write (response, 0, response.Length);

				stream.Close ();
				client.Close ();
			}
		}
*/
	}
}