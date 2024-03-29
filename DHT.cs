using System;											//System.Console
using System.IO;										//MemoryStream
using System.Text;										//Encoding
using System.Collections.Generic;						//Dictionary
using System.Runtime.Serialization.Formatters.Binary;	//BinaryFormatter
using System.Linq;										//Dictionary keys to List<string>
//using System.Uri;										//Uri.EscapeDataString(String), Uri.UnescapeDataString(String)
using System.Collections;								//Hashtable
using Storage;	//KeyValue hashtable
using System.Timers;	//interval

namespace DHT
{
	public class DHT
	{
		public static Encoding encoding = Encoding.GetEncoding("ISO-8859-1");	//binary encoding, to save bytes.
	
	//	public static KeyValue hashtable = new KeyValue("Hashtable.txt");	//SimpleTXTStorage
	//	public static KeyValue hashtable = new KeyValue("Hashtable.db3", "KeyValue", "key", "value");	//SQLite3Storage
	
		//define this values, from arguments.
		public static string DBFilePath		= null;
		public static string HashTableName	= null;
		public static string KeyName		= null;
		public static string ValueName		= null;

		public static KeyValue hashtable = null;
		
		public DHT(){ //args may be already defined
			//Set this value in KeyValue-Method
			//Storage.KeyValue.UseSQLite3 = true;	//use SQLite3 db, with defined values.
			hashtable = new KeyValue(DBFilePath, HashTableName, KeyName, ValueName);	//use sqlite, with values
		}
		
		public static void RunDHT(string[] args){
			hashtable = new KeyValue(args);	//use sqlite, with values
		}
	
		public static void RunDHT(
				string DBFilePath = null
			,	string HashTableName = null
			,	string KeyName = null
			,	string ValueName = null
		){
			hashtable = new KeyValue(DBFilePath, HashTableName, KeyName, ValueName);	//use sqlite, with values
		}

		public static int Count(){
			return hashtable.Count();
		}

		public static List<string> KeysList(){
			return hashtable.Keys();
		}
		
		public static string ListTostring(List<string> list){
			return string.Join(";", list.ToArray());
		}
		
		public static List<string> NewKeysFound = new List<string>();
		
		public static List<string> NewKeys(string keysstring){
			List<string> keys = keysstring.Split(';').ToList();
			for(int i = 0; i<keys.Count; i++){
				if(hashtable.ContainsKey(keys[i])){
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
		
		public static string Encode(string str){
			return System.Uri.EscapeDataString(str);
		}
		
		public static string Decode(string str){
			return System.Uri.UnescapeDataString(str);
		}

		//Convert Hashtable to string
		public static string HashtableToString( Hashtable hashtable , bool show = false)
		{
			string HashtableString = "";
			foreach(DictionaryEntry keyValues in hashtable)
			{
				string line = Encode((string)keyValues.Key) + ":" + Encode((string)keyValues.Value) + "\n";
				HashtableString += line;
				if(show == true){
					Console.WriteLine(line);
				}
			}
			return HashtableString.TrimEnd('\n', ' ');
		}

		//Convert Hashtable from string
		public static Hashtable HashtableFromString( string HashtableString )
		{
			Hashtable hashtable = new Hashtable();
			if(string.IsNullOrEmpty(HashtableString)){
				return hashtable;
			}
			string[] keyvalues = HashtableString.Split('\n');
			if(keyvalues.Length>0){
				for(int i = 0; i<keyvalues.Length; i++){
					string[] key_value = keyvalues[i].Split(':');
					if(key_value.Length>=2){
						string key = Decode(key_value[0]);
						string value = Decode(key_value[1]);
						hashtable.Add(key, value);
					}
				}
			}
			return hashtable;
		}

		public static string GetDataByKeys(string keys){
			List<string> Keys = keys.Split(';').ToList();
			Hashtable data = new Hashtable();
			foreach(string key in Keys){
				if(hashtable.ContainsKey(key)){
					data[key] = hashtable.GetValue(key);
				}
			}
			return HashtableToString(data);
		}
		
		public static string GetDataFromstring(string data_string){
			Hashtable data = HashtableFromString(data_string);
			foreach(DictionaryEntry record in data){
				if(!hashtable.ContainsKey((string)record.Key)){
					hashtable.Add((string)record.Key, (string)record.Value, true);
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
				foreach(string tcpPeer in Peer.IsPeer.TCPPeers)	//for each TCP peer
				{
					string[] IP_PORT = tcpPeer.Split(':');		//get
					string IP = IP_PORT[0];						//IP
					int port = System.Int32.Parse(IP_PORT[1]);	//PORT
					
					string NextRequest = SyncDHT();				//get next request
					string response_string = "";				//string with response
			
					do{
						if(NextRequest == DHTStub+"ok" ){	//if already synchronized
							break;
						}
						Console.WriteLine("TCP DHT sync. request: "+NextRequest);	//show request
						response_string = TCP.Client.Send( IP, port, NextRequest );	//send this to peer
						Console.WriteLine("TCP DHT sync. response: "+response_string);	//show response
						NextRequest = SyncDHT(response_string);	//get next request, by response
					}
					while(!(response_string == null || response_string == DHTStub+"ok"));	//and do sync while not synchronized.
				}
			}
			catch(Exception ex){
				Console.WriteLine(ex);
			}
		}
		
		public static void UDPSyncDHT(){
			try{
				foreach(string udpPeer in Peer.IsPeer.UDPPeers)	//for each alive UDP peer
				{
					string[] IP_PORT = udpPeer.Split(':');		//get
					string IP = IP_PORT[0];						//IP
					int port = System.Int32.Parse(IP_PORT[1]);	//port
					
					string NextRequest = SyncDHT();				//Get request to start sync DHT
					string response_string = "";				//string with response.
					do{
						if(NextRequest == DHTStub+"ok" ){	//if already synchronized
							break;
						}
						Console.WriteLine("UDP DHT sync. request: "+NextRequest);		//show request
						response_string = UDP.Client.Send( IP, port, NextRequest );		//send request to UDP-server to the same IP-port, as TCP peer have.
						Console.WriteLine("UDP DHT sync. response: "+response_string);	//show response
						NextRequest = SyncDHT(response_string);							//get next request.
					}
					while(!(response_string == null || response_string == DHTStub+"ok"));	//and do sync while not synchronized.
				}
			}
			catch(Exception ex){
				Console.WriteLine(ex);
			}
		}
		
		//set interval for sync DHT from peers
		public static int SyncDHTInterval = 60; //seconds to repeat send UDP MultiCast request to discovery peers.

		//Define timer, to repeat synchronizatino from peers.
		public static System.Timers.Timer syncDHTTimer = null;	

		public static void TCPUDPSyncDHT(){
			try{
				TCPSyncDHT();
			//	UDPSyncDHT();
			}
			catch(Exception ex){
				Console.WriteLine("SyncDHTByInterval. Exception: " + ex + ". Try to UDPSyncDHT();");
				UDPSyncDHT();
			}
		}
		//trigger this method, when timer elapsed
		public static void TCPUDPSyncDHT(object source, ElapsedEventArgs e)
		{
			TCPUDPSyncDHT();
		}
		
		//Method to run DHT synchonization from peers.
		public static void RunSyncDHTByInterval(int setSyncDHTInterval = -1){	//set 0 to disable this
			if(setSyncDHTInterval != -1){
				SyncDHTInterval = setSyncDHTInterval;
			}
			if(SyncDHTInterval == 0){	//disable, if 0
				Console.WriteLine("SyncDHTInterval = 0, so sync DHT was been disabled.");
				return;
			}
			
			TCPUDPSyncDHT();	//Run this on first run, before set interval.
			
			//run interval
			syncDHTTimer = new System.Timers.Timer( SyncDHTInterval * 1000 );
			syncDHTTimer.Elapsed += new ElapsedEventHandler(TCPUDPSyncDHT);
			syncDHTTimer.AutoReset = true;
			syncDHTTimer.Enabled = true;	
		}
		
	}
}