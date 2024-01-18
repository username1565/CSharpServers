using System.Collections.Generic;	//List, HashSet
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Data;			//DataSet and DataTable
//using System.Data.SQLite;
using Mono.Data.Sqlite;
using System.Text.RegularExpressions; //Regex
using System.Collections;	//Hashtable

namespace Storage
{
	public class SQLite3
	{
/*
	Methods to working with this database
	And tests of this methods, in Main().
	Methods will be runned from another places.
	
	If "database.sqlite3" still not exists,
	to create this, compile current program, and run this,
	or create this from "database.sqlite3.sql", using command:
		$ sqlite3 database.sqlite3 < database.sqlite3.sql

	Requirements:
		System.Data.SQLite.dll
		PISDA.cs
		database.sqlite3 or database.sqlite3.sql
		
		- open database or create this.
		- run tests of different methods
*/	

		public static bool UseSQLite3 = true;
		
		//Define this from arguments.
		public static string KeyValueTableName = null;
		public static string KeyName = null;
		public static string ValueName = null;

		public static void openSQLite3Db(string DbFile)
		{
			//set db-filename, create if does not exists, and open connection with this db, after create file.
			PISDA.PISDA.DBFileName = DbFile;
		}
		
		public static bool is0(string value){
			return string.IsNullOrEmpty(value);
		}
		
		public static bool is0(object value){
			return ( value == null || Convert.IsDBNull(value) || is0( (string) value.ToString() ) );
		}
		
		public static string ToStr(object value){
			return (!is0(value)) ? (string) value.ToString() : null;
		}
		
		public static int ObjectToInt(object value)
		{
			int test;
			bool result = Int32.TryParse(ToStr(value), out test);
			if (result)
			{
				//Console.WriteLine("Sucess");
				return test;
			}
			else
			{
				//Console.WriteLine("Failure");
				return 0;
			}
		}
		
		//---- Show values

		//log str
		public static void Log(string str){
			PISDA.PISDA.Log(str);
		}

		//log DataTable
		public static void LogDataTable(DataTable table){
			foreach (DataRow row in table.Rows)
			{
				foreach (DataColumn column in table.Columns)
				{
					object item = row[column];
					// read column and item
					Log(item.ToString()+", ");
				}
				Log("\n");
			}
			Log("\n\n");
		}

		//Log DataRow
		public static void LogDataRow(DataRow row){
			if(row==null){return;}
			foreach(object cell in row.ItemArray) {
				Log(cell+"\t");
				//((cell.GetType()).Equals(typeof(byte[])))		//is blob? (byte[])
			}
			Log("\n");			
		}

		//vacuum;
		public static int SQLite3Vacuum(){
			int	result = PISDA.PISDA.ExecuteSQL("vacuum;");
			return result;
		}
		
		
//random sql
		//DataTable table = PISDA.PISDA.ExecuteWithResults(sql, show);
		//PISDA.PISDA.LogDataTable(table);
		

		
		
		
		
		//Key-Velue storage methods:
/*
Count()
Keys()
ContainsKey(key)
GetValue(key)
Search(searchString, caseSensetive)
Add(key, value, replace=false)
Remove(Key)
Reset()
*/		
		//Count
		public static int Count(){
			string sql = "SELECT COUNT(*) from [main].["+KeyValueTableName+"];";
			object result = PISDA.PISDA.ExecuteScalar(sql);
			return System.Int32.Parse(ToStr(result));
		}
		
/*
		//Keys
		public static DataTable Keys(){
			string sql = "SELECT "+KeyName+" from [main].["+KeyValueTableName+"];";
			DataTable result = PISDA.PISDA.GetDataTable(sql);
			return result;
		}
*/

		public static List<string> Keys(){
			string sql = "SELECT "+KeyName+" from [main].["+KeyValueTableName+"];";
			DataTable sqlresult = PISDA.PISDA.GetDataTable(sql);

			List<string> result = new List<string>();
			foreach(DataRow row in sqlresult.Rows){
				result.Add((string)row[KeyName]);
			}
			return result;
		}
		
		//ContainsKey
		public static bool ContainsKey(string key){
			string sql = "SELECT COUNT(*) from [main].["+KeyValueTableName+"] WHERE "+KeyName+"='"+key+"';";
			object result = PISDA.PISDA.ExecuteScalar(sql);
			return (System.Int32.Parse(ToStr(result)) > 0);
		}
		
		//The same, but with name Contains
		public static bool Contains(string key){
			return ContainsKey(key);
		}
		
		//Get value
		public static string GetValue(string key){
			string sql = "SELECT "+ValueName+@" from [main].["+KeyValueTableName+"] WHERE "+KeyName+"= '"+key+"';";
			object result = PISDA.PISDA.ExecuteScalar(sql);
			return ToStr(result);
		}
		
		//Search string
		public static bool CaseSensetiveLike = false;
		
		public static void SetCaseSensetiveLikeIfNeed(bool CaseSensetive = false)
		{
			if(CaseSensetiveLike != CaseSensetive){
				string sql = @"PRAGMA case_sensitive_like = "+((CaseSensetive == true) ? @"true" : @"false") +@";";
				PISDA.PISDA.ExecuteSQL(sql);
				CaseSensetiveLike = CaseSensetive;
			}
		}
		
		public static List<string> SearchString(string search, bool caseSensetive = false )
		{
			SetCaseSensetiveLikeIfNeed();
			
			string sql = @"SELECT * FROM [main].["+KeyValueTableName+@"] WHERE  ["+ValueName+@"] LIKE ('%' || '"+ search  +@"' || '%') "+((caseSensetive == true) ? @"COLLATE NOCASE" : @"")+@";";
			DataTable sqlresult = PISDA.PISDA.GetDataTable(sql);
			
			List<string> result = new List<string>();
			foreach(DataRow row in sqlresult.Rows){
				result.Add((string)row[KeyName]);
			}
			return result;
		}

		//Add value
		public static int Add(string key, string value, bool replace=false){
			value = value.Replace("'", "''");
			string sql = @"INSERT "+
			(
				(replace == true)
					?	@"OR REPLACE"
					:	@"OR IGNORE"
			)
			+@" INTO [main].["+KeyValueTableName+@"] (["+KeyName+@"], ["+ValueName+@"])
				VALUES('"+key+@"', '"+value+@"');"
			;
			
			int result = PISDA.PISDA.ExecuteSQL(sql);
			return result;
		}
		
		//Remove value
		public static int Remove(string key){
			string sql = @"DELETE FROM [main].["+KeyValueTableName+@"] WHERE "+KeyName+@"= '"+key+@"';";
			int result = PISDA.PISDA.ExecuteSQL(sql);
			return result;
		}
		
		//Reset KeyValue
		public static int Reset(){
			string sql = @"DELETE FROM [main].["+KeyValueTableName+@"];";
			int result = PISDA.PISDA.ExecuteSQL(sql);
			return result;
		}

		public static void Main_()
		{
			Console.WriteLine("Test SQLite3Methods.cs");
		}
		
	}

}