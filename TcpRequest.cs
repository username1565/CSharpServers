using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
//using System.Diagnostics;
/*
	Read tcpRequest from client's TCP-stream, and return this as string, decoded with specified encoding.
*/
namespace TCP
{
	partial class Server
	{
		public static string Request(
				NetworkStream stream
			,	Encoding encoding = null
		)
		{
			string		message	=	""				;
			int			count	=	0				;
			byte[]     	buffer	=	new byte[2048]	;
			encoding = encoding == null ? Encoding.ASCII : encoding;

			if (stream.DataAvailable && stream.CanRead)
			{
				while ((count = stream.Read(buffer, 0, buffer.Length)) != 0)
				{
					message = encoding.GetString(buffer, 0, count);
				//	Console.WriteLine("Server received TCP message: "+message);
					return message;
				}
			}
			return message;
		}
	}
}