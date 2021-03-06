using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;			//Thread. ThreadStart
using TcpServer;

namespace TcpServer
{
    public class ClientObject
    {
        public TcpClient client;
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
        }
 
        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64]; // буфер для получаемых данных
                while (true)
                {
				//	Console.WriteLine("Receiving message...");
				
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                    //    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
						builder.Append(TcpServer.encoding.GetString(data, 0, bytes));
					//	Console.WriteLine("data..."+bytes);
						if(bytes == 0) {break;}
                    }
                    while (stream.DataAvailable);
 
                    string message = builder.ToString();


					if(message==""){
							Console.WriteLine("break...");
							stream.Close ();
							client.Close ();
							break;
					}

					if(
							message.StartsWith("GET")
						||	message.StartsWith("POST")
					){
					//	Console.WriteLine("TCP: "+message);

						//read request
						//string request = TcpRequest.ToString (stream); 

						//and return response by request
						//byte[] response = TcpResponse.Response(request);
						byte[] response = HttpServer.HttpResponse.Response(message);
						stream.Write (response, 0, response.Length);

					//		Console.WriteLine("CloseStream and client");
							stream.Close ();
							client.Close ();
							break;
							
					}
                    else{
						message = Encoding.Unicode.GetString(TcpServer.encoding.GetBytes(message));	//change encoding to Unicode
						Console.WriteLine("TCP: "+message);
						
						// отправляем обратно сообщение в верхнем регистре
						//message = message.Substring(message.IndexOf(':') + 1).Trim().ToUpper();
						message = message.Substring(message.IndexOf(':') + 1).Trim();
						data = Encoding.Unicode.GetBytes(message);
						stream.Write(data, 0, data.Length);
					}
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
}