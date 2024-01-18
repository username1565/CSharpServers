using System;				//System.Console, 	System.Exception
using System.IO;			//System.IO.File
using System.Collections;	//Hashtable

/* Key-Value storage - save key-value hashtable on disk, and read valus from disk */
namespace Storage
{
	public class Storage
	{
		public static string HashTableFileName = "hashtable.txt";
		
		public static Hashtable Load(string DBFileName = null){
			if(!String.IsNullOrEmpty(DBFileName)){ //if not null
				HashTableFileName = DBFileName;	//set this, on load
			}
			if(!File.Exists(HashTableFileName)){
				File.WriteAllText(HashTableFileName, "");
			}
			string HashtableString = File.ReadAllText(HashTableFileName);
			return Converters.HashtableFromString(HashtableString);
		}
		
		public static void Save(Hashtable hashtable){
			string HashtableString = Converters.HashtableToString(hashtable);
			File.WriteAllText(HashTableFileName, HashtableString);
		}
	}
	
	//HashTable converters.
	public partial class Converters
	{
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
	}	
}