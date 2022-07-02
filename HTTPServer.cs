using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using TcpServer;

namespace HttpServer
{
	class HttpServer
	{ 
		public static void Main (string[] args)
		{
			if(args.Length >= 2){
				if(args[1] == "True" || args[1] == "true" || args[1] == "captcha")
				{
					HttpResponse.UseCaptcha = true;
				}
			}
			
		
			SQLite3.SQLite3Methods.openSQLite3Db();	//open SQLite3 database...

		//	TcpServer.TcpServer.Main(args);	//just run TCPServer on the same "port".
			TCPUDPServer.TCPUDPServer.Main(args);	//just run TCPServer on the same "port".
		}
	}
}