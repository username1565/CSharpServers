//using Newtonsoft.Json;
//using NDB;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Data;			//DataSet and DataTable
//using System.Data.SQLite;
using Mono.Data.Sqlite;
//using Newtonsoft.Json;	//JsonConvert.SerializeObject.
using System.Text.RegularExpressions; //Regex
using System.Web;	//HttpUtility.HtmlDecode


namespace SQLite3
{
	public class SQLite3Methods
	{
/*
	open database
	connect to this database
	
	add message
	add attachments for message
	select message
	
*/	
	
/*
	Methods to working with this database "nanodb.exe-source\DataBbase\nanodb.sqlite3"
	And tests of this methods.
	Methods will be runned in PostDbSQLite3.cs and from another places.
	
	If "nanodb.sqlite3" still not exists,
	to create this, compile current program, and run this,
	or create this from nanodb.sqlite3.sql, using command:
		$ sqlite3 nanodb.sqlite3 < nanodb.sqlite3.sql

	Requirements:
		System.Data.SQLite.dll
		PISDA.cs
		nanodb.sqlite3 or nanodb.sqlite3.sql
		
		- open database or create this.
		- run tests of different methods
*/	

		public static bool UseSQLite3 = true;
		public static string sqliteDBFilename = @"feedback.sqlite3";					//SQLite3 DataBase file.
		public static string SqlFileToCreateDB = @"feedback.sqlite3.sql";				//SQL-file, to create this DB, if this not exists.
		
		public static void openSQLite3Db()
		{
			bool fileExist = File.Exists(sqliteDBFilename);
			if (!fileExist)
			{
				//set db-filename, create if does not exists, and open connection with this db, after create file.
				PISDA.PISDA.DBFileName = sqliteDBFilename;

				//read sql-query from the file, to create tables, triggers, and views, if not exists.
				string sqlquery = @File.ReadAllText(SqlFileToCreateDB);

				PISDA.PISDA.NonQuery(sqlquery);					//execute SQL-query to create DataBase
			}

			//set db-filename, create if does not exists, and open connection with this db, after create file.
			PISDA.PISDA.DBFileName = sqliteDBFilename;

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
		
//Begin SQLite-methods

		public static Encoding latin1 = Encoding.GetEncoding("ISO-8859-1");
		public static Encoding utf8 = Encoding.UTF8;

		public static string Decode(string str){
			return HttpUtility.HtmlDecode(System.Uri.UnescapeDataString(str));
		}
		
		//1.	Add file (if hash exists - generate anti-collision id, and re-compute hash).
		public static void AddAttachment(int MessageID, string filename, string FileContent){
			//Console.WriteLine("FileContent");
			//uint FileContentByteLength = (uint) FileContent.Length; //get ByteLength, [0 - 4,294,967,295] (4 bytes)
            //string hexByteLength = UintToHex(FileContentByteLength);

			//string AttachmentID = (ComputeSha256Hash(FileContent) + hexByteLength).ToLower();	//add ByteLength to hash
			
			string base64 = Decode(FileContent);
			base64 = base64.Split(',')[1];
			
			byte[] fileBytes = Convert.FromBase64String(base64);
			
			PISDA.PISDA.AddRow("MessagesAttachments", new object[]{null, MessageID.ToString(), filename, fileBytes}); //add or return existing hash
		}
		
		public static void AddAttachmentsForMessage(int MessageID, Dictionary<string, string> files){
			foreach (KeyValuePair<string, string> keypair in files)  
			{
				string filename = keypair.Key;
				string FileContent = keypair.Value;
				AddAttachment(MessageID, filename, FileContent);
			}
		}
		
		public static bool AddMessage(Dictionary<string, string> message){
			string email		=	"";
			string subject		=	"";
			string messageText	=	"";
			Dictionary<string, string> files = new Dictionary<string, string>();
			
			foreach (KeyValuePair<string, string> keypair in message)  
			{
				//Console.WriteLine("Key: {0}, Value: {1}", keypair.Key, keypair.Value);
				string key = keypair.Key;

				if(key == "email"){
					email = keypair.Value;
				}
				else if(key == "subject"){
					subject = keypair.Value;
				}
				else if(key == "message"){
					//messageText = latin1.GetString(utf8.GetBytes(message["message"])).Replace("'", "''");
					messageText = (@message["message"]).Replace("'", "''");
				}
				else if(
					new string[]{
							"attachments"
						,	"RandomCaptchaGuid"
						,	"RandomCaptchaGuess"
					}.Contains(key)
				){	//skip this values
					continue;
				}
				else{
					files[key] = keypair.Value;
				}
			}
			
			string sql = @"
INSERT OR REPLACE INTO [Messages] (email, subject, message)
VALUES(
		'"+	email		+@"'
	,	'"+	subject 	+@"'
	,	'"+	messageText	+@"'
)
;
";

			try{
				int	MessageID = PISDA.PISDA.ExecuteSQL(sql);
				bool added = (MessageID > 0); //true or false
				
				/*
				Console.WriteLine("files.Count: "+files.Count);
				foreach (KeyValuePair<string, string> keypair in message)  
				{
					Console.WriteLine("key: "+keypair.Key+": value: "+keypair.Value);
				}				
				*/
				
				if(added && files.Count > 0){
					MessageID = Convert.ToInt32(PISDA.PISDA.ExecuteScalar("SELECT MAX(id) FROM [Messages];"));
				//	Console.WriteLine("MessageID: "+MessageID);
					AddAttachmentsForMessage(MessageID, files);
					Console.WriteLine("AddMessage: added: "+added);
					return added;
				}
				else if(added){
					return added;
				}
				else{
					return false;
				}
			}
			catch (Exception ex){
				Console.WriteLine("SQLite3Methods. AddMessage ex: "+ex);
				return false;
			}
		}
		
		public static DataTable GetMessages(){
			string sql = @"SELECT * FROM [Main].[Messages];";
			DataTable result = PISDA.PISDA.GetDataTable(sql);
			return result;
		}

		public static DataTable GetAttachments(){
			string sql = @"SELECT * FROM [Main].[MessagesAttachments];";
			DataTable result = PISDA.PISDA.GetDataTable(sql);
			return result;
		}
		
		
		public static string ShowMessages(){
			string FeedBackForm = "<hr>";
				DataTable messages = GetMessages();
			//	Console.WriteLine("messages.Rows.Count: "+messages.Rows.Count);
				for(int i = messages.Rows.Count-1; i>=0; i--){
					DataRow message = messages.Rows[i];
					FeedBackForm += "<div>";
						FeedBackForm += "<div>";
							FeedBackForm += "id: <a href=\"/message/"+message["id"]+"\">" + message["id"] +"</a>";
						FeedBackForm += "</div>";
						FeedBackForm += "<div>";
							FeedBackForm += "email: " + message["email"];
						FeedBackForm += "</div>";
						FeedBackForm += "<div>";
							FeedBackForm += "subject: " + message["subject"];
						FeedBackForm += "</div>";
						FeedBackForm += "<div>";
							FeedBackForm += "message: " + message["message"];
						FeedBackForm += "</div>";
					FeedBackForm += "</div>";
					FeedBackForm += "<hr>";
				}
			return FeedBackForm;
		}

		//public static object[] GetAttachment(string id){
		public static byte[] GetAttachment(string id){
			string sql = @"SELECT * FROM [Main].[MessagesAttachments] WHERE id="+id+";";
			DataRow attachment = PISDA.PISDA.ReturnDataRow(sql);

		//	return new object[] {
		//			attachment["filename"]	//string
		//		,	attachment["data"]	//byte[]
		//	};

			return (byte[]) attachment["data"];
		}
		
		public static DataTable GetAttachmentsForMessage(string id){
			string sql = @"SELECT * FROM [Main].[MessagesAttachments] WHERE MessageID = "+id+";";
			DataTable result = PISDA.PISDA.GetDataTable(sql);
			return result;
		}
		
		public static DataRow GetMessage(string id){
			string sql = @"SELECT * FROM [Main].[Messages] WHERE id="+id+@";";
			DataRow result = PISDA.PISDA.ReturnDataRow(sql);
			return result;
		}

		public static string ShowMessage(string id){
			string FeedBackForm = @"<html>
	<meta charset=""UTF-8"">
	<head>
	</head>
	<body>
	<a href=""/feedback"">Back</a>
	<hr>";
				DataRow message = GetMessage(id);
					FeedBackForm += "<div>";
						FeedBackForm += "<div>";
							FeedBackForm += "id: <a href=\"/message/"+message["id"]+"\">" + message["id"] +"</a>";
						FeedBackForm += "</div>";
						FeedBackForm += "<div>";
							FeedBackForm += "email: " + message["email"];
						FeedBackForm += "</div>";
						FeedBackForm += "<div>";
							FeedBackForm += "subject: " + message["subject"];
						FeedBackForm += "</div>";
						FeedBackForm += "<div>";
							FeedBackForm += "message: " + message["message"];
						FeedBackForm += "</div>";
					FeedBackForm += "</div>";
					FeedBackForm += "<br><br>Attachments:<br>";
					FeedBackForm += "<div>";
						DataTable attachments = GetAttachmentsForMessage(id);
						if(attachments.Rows.Count != 0){
							for(int i = attachments.Rows.Count-1; i>=0; i--){
								DataRow attachment = attachments.Rows[i];
								//string dataURL = "data:application/octet-stream;base64,"+Convert.ToBase64String((byte[])attachment["data"]);
								string dataURL = "/attachment/"+attachment["id"];
								FeedBackForm += "<a href=\""+dataURL+"\" download=\""+attachment["filename"]+"\">"+attachment["filename"]+"</a><br>";
							}
						}
					FeedBackForm += "</div>";
					FeedBackForm += "<hr>";
					FeedBackForm += "<body>";
					FeedBackForm += "</html>";
			return FeedBackForm;
		}

		
//END SQLite-methods
		
		public static void Main()
		{
			Console.WriteLine("Main...");
		}
		
	}

}