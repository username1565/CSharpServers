using System.Collections.Generic;	//Dictionary

namespace TextServer
{
	public class Responses
	{
		public static Dictionary<string, string> server = new Dictionary<string, string>{
		//		{"request", "response"}
		//	,	{"", ""}
				{"Are you peer?", "Yes, I'm peer."}
		};

		public static string Response(string request){
			string response = "";
			if(server.ContainsKey(request)){
				response = server[request];
			}
			else if(request.Contains("Peer Exchange")){
				response = Peer.PEX_server.PeerExchange(request);
			}
			else if(request.StartsWith("DHT synchronization. "))
			{
				return DHT.DHT_server.SyncDHT(request);
			}
			else{
				// return this request, as an UPPER-cased text.
				response = request.Trim().ToUpper();
			}
			return response;
		}		
	}
}