using System;						//System.Console, System.Exception.
using System.Collections;			//Hashtable
using System.Collections.Generic;	//List, HashSet
using System.Linq;					//Keys.Cast<string>.ToList()

namespace Storage
{
	public class KeyValue
	{
		public static void Main_(string[] args){
			Test();	//run tests
		}
	
		public static void Test(){
			try{
				KeyValue hashtable = new KeyValue();	//create new hashtable

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
		public KeyValue(){
			//on initialize object, just initialize this
			hashtable = Load(); //and load hashtable from storage
		}
		
		public Hashtable hashtable = new Hashtable();
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
			return (int)ShowAndReturn(hashtable.Count, show);
		}
		
		//	Keys( show=false )
		//		Get list of keys in hashtable storage
		public List<string> Keys(
			bool show = false
		){
			List<string> result = hashtable.Keys.Cast<string>().ToList();
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
			return (bool)ShowAndReturn(hashtable.ContainsKey(key), show);
		}
		
		//	Contains( key , show=false )
		//		The same, check is key contains or not
		public bool Contains(
				string key
			,	bool show = false
		){
			return ContainsKey(key, show);
		}
		
		//	GetValue( key , show=false )
		//		Get value by key from hashtable
		public string GetValue(
				string key
			,	bool show = false
		){
			return (string)ShowAndReturn((string)hashtable[key], show);
		}
		
		//	Search( searchstring, caseSensetive=false, show=false )
		//		Get list of keys, where part of value is found
		public List<string> Search(
				string searchstring
			,	bool caseSensetive = false
			,	bool show = false
		){
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
			if(
					replace == false
				&&	hashtable.ContainsKey(key)
			){
				return (bool)ShowAndReturn(false, show);
			}
			else{
				hashtable[key] = value;
				Save();
				return (bool)ShowAndReturn(true, show);
			}
		}
		
		//	Remove( key, show=false )
		//		Remove from hashtable the key-value pair, by key
		public bool Remove(
				string key
			,	bool show = false
		){
			try{
				hashtable.Remove(key);
				Save();
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
				return (bool)ShowAndReturn(true, show);
			}
			catch{
				return (bool)ShowAndReturn(false, show);
			}
		}
		
		//Load hashtable from storage
		public Hashtable Load(){
			return Storage.Load();
		}
		
		//Save hashtable in storage
		public void Save(){
			Storage.Save(hashtable);
		}
	}
}