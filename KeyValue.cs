using System;						//System.Console, System.Exception.
using System.Collections;			//Hashtable
using System.Collections.Generic;	//List, HashSet
using System.Linq;					//Keys.Cast<string>.ToList()

namespace Storage
{
	public class KeyValue
	{
		public static bool UseSQLite3 = false; //or "true" to use
		
		public static void Main_(string[] args){
			Test(args);	//run tests
		}
	
		public static void Test(string[] args){
			try{
				KeyValue hashtable = new KeyValue(args);	//create new hashtable
				hashtable.Reset(true);

				hashtable.Count(true);
				hashtable.Add("key1", "value1", false, true);
				hashtable.Add("key2", "value2", false, true);
				hashtable.Add("key3", "value'", false, true);
				hashtable.Count(true);
				hashtable.Keys(true);

				hashtable.GetValue("key1", true);
				hashtable.GetValue("key2", true);
				hashtable.GetValue("key3", true);

				hashtable.Remove("key1", true);
				hashtable.Add("key1", "value5", true);
			
				hashtable.GetValue("key1", true);
				hashtable.GetValue("key2", true);
				hashtable.GetValue("key3", true);
				
				hashtable.Contains("key100", true);
				hashtable.ContainsKey("key1", true);
				hashtable.Search("lue", true);
				
				hashtable.Reset(true);
				hashtable.Count(true);
				hashtable.Keys(true);
				
				
			}
			catch(Exception ex){
				Console.WriteLine(ex);
			}
		}
		
		/*
			In memory "key-value"-storage.
			
			https://docs.microsoft.com/en-us/dotnet/api/system.collections.hashtable?view=net-6.0
			
			.NET Framework only:
			For very large Hashtable objects,
			you can increase the maximum capacity
			to 2 billion elements
			on a 64-bit system
			by setting the enabled attribute of the <gcAllowVeryLargeObjects> configuration element to true
			in the run-time environment.
		*/

		//constructor
		public KeyValue(string[] args){
			if(args.Length == 1){	//if filename only
				UseSQLite3 = false;
				new KeyValue(args[0]);
			}
			else{	//if HashTableName, KeyName, ValueName too
				UseSQLite3 = true;
				new KeyValue(args[0], args[1], args[2], args[3]); //and load hashtable from storage
			}
		}

		//constructor
		public KeyValue(
				string DbFileName = null
			,	string KeyValueTableName = null
			,	string KeyName = null
			,	string ValueName = null
		){
			Console.WriteLine("KeyValue: "+DbFileName+", "+KeyValueTableName+", "+KeyName+", "+ValueName);
			if(String.IsNullOrEmpty(KeyValueTableName)){
				UseSQLite3 = false;
			}
			else{
				UseSQLite3 = true;
			}
			Console.WriteLine("UseSQLite3 = "+UseSQLite3);
			
			//on initialize object, just initialize this
			hashtable = Load(DbFileName, KeyValueTableName, KeyName, ValueName); //and load hashtable from storage
		}
		
		private const int CacheLimit = 100000; // how many records to keep in memory (reduce DB read operations)

		public Hashtable hashtable = new Hashtable();	//cache
		/*
		Methods:
		//	Count( show=false )
		//	Keys( show=false )
		//	ContainsKey( key , show=false )
		//	Contains( key , show=false )
		//	GetValue( key , show=false )
		//	Search( searchstring, caseSensetive=false, show=false )
		//	Add( key, value, replace=false, show=false )
		//	Remove( key, show=false )
		//	Reset( show=false )
		//	Load()
		//	Save()
		*/
		
		//	ShowAndReturn( value, show=false )
		//		Show object, and return this as is
		public object ShowAndReturn(object value, bool show = false){
			if(show){
				if(value == null){
					Console.WriteLine("null");
				}
				else{
					Console.WriteLine((string)value.ToString());
				}
			}
			return value;
		}
		
		//	Count( show=false )
		//		Get number of keys in hashtable storage
		public int Count(
			bool show = false
		)
		{
			if(UseSQLite3 == true){
				int count = SQLite3.Count();
				if(show == true){
					Console.WriteLine("count: "+count);
				}
				return count;
			}
			else{
				return (int)ShowAndReturn(hashtable.Count, show);
			}
		}
		
		//	Keys( show=false )
		//		Get list of keys in hashtable storage
		public List<string> Keys(
			bool show = false
		){
			List<string> result = new List<string>();
			if(UseSQLite3 == true){
				result = SQLite3.Keys();
			}
			else{
				result = hashtable.Keys.Cast<string>().ToList();
			}
			if(show){
				foreach(string key in result){
					Console.WriteLine("key: "+key);
				}
			}
			return result;
		}
		
		//	ContainsKey( key , show=false )
		//		Check is key contains or not
		public bool ContainsKey(
				string key
			,	bool show = false
		){
			if(UseSQLite3 == true){
				return (bool)ShowAndReturn(SQLite3.ContainsKey(key), show);
			}
			else{
				return (bool)ShowAndReturn(hashtable.ContainsKey(key), show);
			}
		}
		
		//	Contains( key , show=false )
		//		The same, check is key contains or not
		public bool Contains(
				string key
			,	bool show = false
		){
			return ContainsKey(key, show);
		}
		
		//Add value to cache
		public void CacheValue(string key, string value){
			//reset cache, if need
			if(hashtable.Count >= CacheLimit){
				hashtable = new Hashtable();
			}
			//cashing
			if(!hashtable.ContainsKey(key)){
				hashtable[key] = value;
			}
		}

		//	GetValue( key , show=false )
		//		Get value by key from hashtable
		public string GetValue(
				string key
			,	bool show = false
		){
			if(hashtable.ContainsKey(key)){
				return (string)ShowAndReturn((string)hashtable[key], show);
			}
			else{
				string value = SQLite3.GetValue(key);
				CacheValue(key, value);
				return (string)ShowAndReturn((string)value, show);
			}
		}
		
		//	Search( searchstring, caseSensetive=false, show=false )
		//		Get list of keys, where part of value is found
		public List<string> Search(
				string searchstring
			,	bool caseSensetive = false
			,	bool show = false
		){
			
			//Search in cache:
			List<string> FoundKeys = new List<string>();
			if(caseSensetive == true){
				searchstring = searchstring.ToLower();
			}
			foreach(DictionaryEntry record in hashtable){
				string value = (string)record.Value;
				if(caseSensetive == true){
					value = value.ToLower();
				}
				if(value.Contains(searchstring)){
					FoundKeys.Add((string)record.Key);
				}
			}
			if(show == true){
				foreach(string key in FoundKeys){
					Console.WriteLine("key: "+key);
				}
			}
			
			if(UseSQLite3 == true)
			{
				//SQLite search
				List<string> keys = SQLite3.SearchString(searchstring, caseSensetive);
				foreach (string key in keys){
					if(show == true){
						Console.WriteLine("key: "+key);
						FoundKeys.Add(key);
					}
				}
			}
			return FoundKeys;
		}

		//	Add( key, value, replace=false, show=false )
		//		Add key-value in hashtable
		public bool Add(
				string key
			,	string value
			,	bool replace = false
			,	bool show = false
		){
			try{
				if(
						replace == false
					&&	hashtable.ContainsKey(key)
				){
					return (bool)ShowAndReturn(false, show);
				}
				
				if(UseSQLite3 == true){
					int result = SQLite3.Add(key, value, replace);
					if(result == -1){//if not add - false
						return (bool)ShowAndReturn(false, show);
					}
					CacheValue(key, value);
					if(show == true){
						Console.WriteLine("Add result: "+result);
					}
					return (bool)ShowAndReturn(true, show);
				}
				else{
					hashtable[key] = value;
					Save();
					return (bool)ShowAndReturn(true, show);
				}
			}
			catch{
				return (bool)ShowAndReturn(false, show);
			}
		}
		
		//	Remove( key, show=false )
		//		Remove from hashtable the key-value pair, by key
		public bool Remove(
				string key
			,	bool show = false
		){
			try{
				if(UseSQLite3 == true){
					SQLite3.Remove(key);
					if(hashtable.ContainsKey(key)){
						hashtable.Remove(key);
					}
				}
				else{
					hashtable.Remove(key);
					Save();
				}
				return (bool)ShowAndReturn(true, show);
			}
			catch{
				return (bool)ShowAndReturn(false, show);
			}

		}
		
		//	Reset( show=false )
		//		Clear hashtable
		public bool Reset(
				bool show = false
		){
			try{
				hashtable = new Hashtable();
				if(UseSQLite3 == true){
					SQLite3.Reset();
				}
				return (bool)ShowAndReturn(true, show);
			}
			catch{
				return (bool)ShowAndReturn(false, show);
			}
		}
		
		//Load hashtable from storage
		public Hashtable Load(
				string DbFileName = null
			,	string KeyValueTableName = null
			,	string KeyName = null
			,	string ValueName = null
		){
			if(UseSQLite3 == true){
				SQLite3.UseSQLite3 = true;

				SQLite3.KeyValueTableName = KeyValueTableName;
				SQLite3.KeyName = KeyName;
				SQLite3.ValueName = ValueName;

				SQLite3.openSQLite3Db(DbFileName);
				hashtable = new Hashtable();
				return hashtable; //return empty hashtable, to use direct SQL-requests for DB.
			}
			else{
				return Storage.Load(DbFileName);
			}
		}
		
		//Save hashtable in storage
		public void Save(){
			if(UseSQLite3 == true){
			//	SQLite3.Save(hashtable);
			//Each change are already saved
			}
			else{
				Storage.Save(hashtable);
			}
		}
	}
}