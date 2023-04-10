using System.Net;
using System.Net.Sockets;

namespace Program
{
	partial class Program
	{
		static void Main15(string[] args)
		{
			string	ServerTcpIP		=	"127.0.0.1"		;
			int		ServerTcpPort	=	8081			;
			
			if(args.Length == 1){
				ServerTcpPort = System.Int32.Parse(args[0]);
			}
			else if(args.Length == 2){
				ServerTcpIP = args[0];
				ServerTcpPort = System.Int32.Parse(args[1]);
			}
			
			string response;
			byte[] ResponseBytes;
			//	Connect, send request, receive response, then disconnect
			response				=	TCP.Client.Send(ServerTcpIP, ServerTcpPort, "send and close");
			ResponseBytes			=	TCP.Client.Send(ServerTcpIP, ServerTcpPort, new byte[]{0,1,2,3,4,5});
			
			System.Console.ReadLine();
		}
	}
}