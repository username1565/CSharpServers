using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
	UDPServerProc
Thread to Start UDP Server.
Start this.
Receive UDP-datagramm, when someone sent this.
Read data of request
Send data of response, by request.

When Multicast request accepted, UDP-packets sending for Multicast group, and UDP-server too.
Need to skip second UDP-request, then.
*/

//Server-side
namespace UDP
{
	partial class Server
	{
		private static void UDPServerProc(object arg)
		{
			object[]	parameters			=	(object[])arg;
			UdpClient	server				=	(UdpClient) (parameters[0]);
			string		MultiCastGroupIP	=	(string)	(parameters[1]);
		
			Console.WriteLine(
				"UDP server thread started: "+Program.Convert.IP_PORT((IPEndPoint)((server.Client).LocalEndPoint))
				+ ( (server.Client.MulticastLoopback == true) ? " with multicast group IP " + MultiCastGroupIP : "" )
			);

			try
			{
				for(;;)
				{
					UDP.Server.RequestResponse(server, Encoding.ASCII);
				}
			}
			catch (SocketException ex)
			{
				if(ex.ErrorCode != 10004) // unexpected
					Console.WriteLine("UDPServerProc exception: " + ex);
			}
			catch (Exception ex)
			{
				Console.WriteLine("UDPServerProc exception: " + ex);
			}

			Console.WriteLine("UDP server thread finished");
			
			Console.WriteLine("Run again...");
			UDPServerProc(arg);
			
		}
	}
}