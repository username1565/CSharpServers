using System;											//System.Console
using System.IO;										//MemoryStream
using System.Text;										//Encoding
using System.Collections.Generic;						//Dictionary
using System.Runtime.Serialization.Formatters.Binary;	//BinaryFormatter
using System.Linq;										//Dictionary keys to List<string>
//using System.Uri;										//Uri.EscapeDataString(String), Uri.UnescapeDataString(String)

namespace DHT
{
	public class DHT
	{
		public static Encoding encoding = Encoding.GetEncoding("ISO-8859-1");	//binary encoding, to save bytes.
	
		//Hash table is key-value dictionary
		public static Dictionary<string, string> HashTable = new Dictionary<string, string>(){
		//	{"key", "value"}
			{"key", "value"}
		};
		
		public static int Count(){
			return DHT.HashTable.Count;
		}

		public static List<string> KeysList(){
			return HashTable.Keys.ToList();
		}
		
		public static string ListTostring(List<string> list){
			return string.Join(";", list.ToArray());
		}
		
		public static List<string> NewKeysFound = new List<string>();
		
		public static List<string> NewKeys(string keysstring){
			List<string> keys = keysstring.Split(';').ToList();
			for(int i = 0; i<keys.Count; i++){
				if(HashTable.ContainsKey(keys[i])){
					keys.Remove(keys[i]);
				}
			}
			//if keys is not existing, new found keys, now
			
			for(int i = 0; i<keys.Count; i++){
				if(!NewKeysFound.Contains(keys[i])){
					NewKeysFound.Add(keys[i]);				//add new found keys in NewKeysFound
				}
			}
			return keys;
		}
		
		public static string DictionaryTostring(Dictionary < string, string > dictionary)
		{
			string dictionarystring = "{";
			foreach(KeyValuePair < string, string > keyValues in dictionary)
			{
				dictionarystring += System.Uri.EscapeDataString(keyValues.Key) + ":" + System.Uri.EscapeDataString(keyValues.Value) + ",";
			}
			return dictionarystring.TrimEnd(',', ' ') + "}";
		}
		
		public static Dictionary<string, string> DictionaryFromString( string dictionarystring )
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			
			dictionarystring = dictionarystring.Substring(1, dictionarystring.Length-1);
			string[] keyvalues = dictionarystring.Split(',');
			for(int i = 0; i<keyvalues.Length; i++){
				string[] key_value = keyvalues[i].Split(':');
				string key = System.Uri.EscapeDataString(key_value[0]);
				string value = System.Uri.EscapeDataString(key_value[1]);
				dictionary[key] = value;
			}
			return dictionary;
		}

		public static string GetDataByKeys(string keys){
			List<string> Keys = keys.Split(';').ToList();
			Dictionary <string, string> data = new Dictionary<string, string>();
			foreach(string key in Keys){
				if(HashTable.ContainsKey(key)){
					data[key] = HashTable[key];
				}
			}
			return DictionaryTostring(data);
		}
		
		public static string GetDataFromstring(string data_string){
			Dictionary <string, string> data = DictionaryFromString(data_string);
			foreach(KeyValuePair <string, string> record in data){
				if(!HashTable.ContainsKey(record.Key)){
					HashTable[record.Key] = record.Value;
				}
			}
			Console.WriteLine("DHT synchronized. DHT.Count()"+DHT.Count());
			return "ok";
		}
		
	}
	
	public class DHT_server
	{
		//receive keys number from remote client
		//get keys number of DHT on server
		//compare keys number on server's DHT, and remote DHT
		//	if remote larger, send number
		//	if remote lesser, send keys
		
		//receive keys
		//	compare with existing keys, and get newkeys
		//	ask data by newkeys

		public static string DHTStub = "DHT synchronization. ";
		public static string KeysNumStub = "Keys number: ";
		public static string KeysStub = "Keys: ";
		public static string RequestKeysStub = "Request Keys: ";
		public static string DHTDataStub = "DHT data: ";
		
		//Server-side
		public static string SyncDHT(string request){
			string data = "";
			int remoteKeysNum = 0;
			int KeysCount = DHT.Count();

			if(request.StartsWith(DHTStub)){
				data = request.Substring(DHTStub.Length);
				if(data.StartsWith(KeysNumStub)){
					data = data.Substring(KeysNumStub.Length);
					remoteKeysNum = System.Int32.Parse(data);
					if(remoteKeysNum == KeysCount ){
						return DHTStub+"ok";
					}
					else if(remoteKeysNum > KeysCount){
						return DHTStub + KeysNumStub + KeysCount.ToString();
					}
					else if(remoteKeysNum < KeysCount){
						return DHTStub + KeysStub + DHT.ListTostring(DHT.KeysList());	//send server's DHT keys
					}
				}
				else if(data.StartsWith(KeysStub)){
					data = data.Substring(KeysStub.Length);
					List<string> newkeys = DHT.NewKeys(data);
					return DHTStub + RequestKeysStub + DHT.ListTostring(DHT.NewKeysFound);
					//load DHT by keys from newkeys...									<-----------------
				}
				else if(data.StartsWith(RequestKeysStub)){
					data = data.Substring(RequestKeysStub.Length);
					return DHTStub + DHTDataStub + DHT.GetDataByKeys(data);
				}
				else if(data.StartsWith(DHTDataStub)){
					data = data.Substring(DHTDataStub.Length);
					return DHTStub + DHT.GetDataFromstring(data);	//"ok"
				}
				return null;
			}
			return null;
		}
	}
	
	public class DHT_client
	{
		//send keys number
		//receive keys number or keys
		//	if number
		//		send keys
		//	if keys
		//		compare with existing keys, and get newkeys
		//		ask data by newkeys, and save
		
		public static string DHTStub = "DHT synchronization. ";
		public static string KeysNumStub = "Keys number: ";
		public static string KeysStub = "Keys: ";
		public static string RequestKeysStub = "Request Keys: ";
		public static string DHTDataStub = "DHT data: ";
		
		//Client side - return next request as string, after receive data
		public static string SyncDHT(string data = null){
			if(data == null){
				return ( DHTStub+KeysNumStub+DHT.Count().ToString() );
			}
			else if(data.StartsWith(DHTStub)){
				data = data.Substring(DHTStub.Length);
				if(data.StartsWith(KeysNumStub)){
		//Num of server's DHT keys returning when this value is lesser, so need to send DHT keys from client, to server's DHT.
					return (DHTStub + KeysStub + DHT.ListTostring(DHT.KeysList()));	//send server's DHT keys
				}
				else if(data.StartsWith(KeysStub)){
					data = data.Substring(KeysStub.Length);
					List<string> newkeys = DHT.NewKeys(data);
					return (DHTStub + RequestKeysStub + DHT.ListTostring(DHT.NewKeysFound));
					//load DHT by keys from newkeys...									<-----------------
				}
				else if(data.StartsWith(RequestKeysStub)){
					data = data.Substring(RequestKeysStub.Length);
					return DHTStub + DHTDataStub + DHT.GetDataByKeys(data);
				}
				else if(data.StartsWith(DHTDataStub)){
					data = data.Substring(DHTDataStub.Length);
					return DHTStub + DHT.GetDataFromstring(data);	//"ok"
				}
				else{
					return null;
				}
			}
			Console.WriteLine("data: "+data+" return null");
			return null;
		}
		
		
		public static void TCPSyncDHT(){
			try{
				int keysNum = DHT.Count();
				foreach(string tcpPeer in Peer.IsPeer.TCPPeers)
				{
					string[] IP_PORT = tcpPeer.Split(':');
					string IP = IP_PORT[0];
					int port = System.Int32.Parse(IP_PORT[1]);
					
					keysNum = DHT.Count();
					string NextRequest = SyncDHT();
					string response_string = "";
			
					do{
						Console.WriteLine("NextRequest: "+NextRequest);
						if(NextRequest == DHTStub+"ok" ){
							break;
						}
						Console.WriteLine("TCP DHT sync. request: "+NextRequest);
						response_string = TCP.Client.Send( IP, port, NextRequest );
						Console.WriteLine("TCP DHT sync. response: "+response_string);
						NextRequest = SyncDHT(response_string);
					}
					while(!(response_string == null || response_string == DHTStub+"ok"));
					
					keysNum = DHT.Count();
				}
			}
			catch(Exception ex){
				Console.WriteLine(ex);
			}
		}
		
		public static void UDPSyncDHT(){
			try{
				int keysNum = DHT.Count();
				foreach(string tcpPeer in Peer.IsPeer.TCPPeers)
				{
					string[] IP_PORT = tcpPeer.Split(':');
					string IP = IP_PORT[0];
					int port = System.Int32.Parse(IP_PORT[1]);
					
					keysNum = DHT.Count();
					string NextRequest = SyncDHT();
					DHT.HashTable["key2"] = "value2";
					string response_string = "";
					do{
						Console.WriteLine("NextRequest: "+NextRequest);
						if(NextRequest == DHTStub+"ok" ){
							break;
						}
						Console.WriteLine("TCP DHT sync. request: "+NextRequest);
						response_string = UDP.Client.Send( IP, port, NextRequest );
						Console.WriteLine("TCP DHT sync. response: "+response_string);
						NextRequest = SyncDHT(response_string);
					}
					while(!(response_string == null || response_string == DHTStub+"ok"));

					
					keysNum = DHT.Count();
				}
			}
			catch(Exception ex){
				Console.WriteLine(ex);
			}
		}
	}
}