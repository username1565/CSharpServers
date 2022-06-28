using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;	//Dictionary
using System.Web;	//HttpUtility.HtmlDecode


namespace HttpServer
{

	public static class DateTimeOffsetExtensions
	{
		private static readonly DateTimeOffset UnixEpoch =
			new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

		public static long ToUnixTimeMicroseconds(this DateTimeOffset timestamp)
		{
			TimeSpan duration = timestamp - UnixEpoch;
			// There are 10 ticks per microsecond.
			return duration.Ticks / 10;
		}
	}

	class HttpForms
	{
		public static Dictionary<string, string> GetPostArgs(string request){
			Dictionary<string, string> PostArgs = new Dictionary<string, string>();
			
			string[] splitted = request.Split(new string[]{"\r\n\r\n"}, StringSplitOptions.None);	//get content from POST-request
			string content = splitted[1];	//here is this
			
			string[] splitted2 = content.Split('&');	//split by "&" to get arg-lines

			//Console.WriteLine("splitted2.Length: "+splitted2.Length);
			for(int i = 0; i<splitted2.Length; i++){		//for each line
				string[] splitted3 = splitted2[i].Split('=');	//extract arg-value
			//	Console.WriteLine("splitted3[0]: "+splitted3[0]);
				PostArgs[splitted3[0]] = splitted3[1];				//write this in dictionary
			}
			
/*
			foreach (KeyValuePair<string, string> value in PostArgs)  
			{
				Console.WriteLine("Key: {0}, Value: {1}", value.Key, value.Value);  
			}			
*/
			
			List<string> attachments = new List<string>{};
			
			//Console.WriteLine("splitted2.Length: "+splitted2.Length);
			for(int i = 0; i<splitted2.Length; i++){		//for each line
				string[] splitted3 = splitted2[i].Split('=');	//extract arg-value
				
				if(splitted3[0] == "attachments"){
					attachments.Add(splitted3[1]);
					continue;
				}
				
			//	Console.WriteLine("splitted3[0]: "+splitted3[0]);
				string key = splitted3[0];
				string value = splitted3[1];
				
				PostArgs[key] = value;		//write this in dictionary
			}
			
			return PostArgs;
		}
		
		public static string GetCurrentUnixTimestamp(){
			DateTimeOffset now = DateTimeOffset.UtcNow;
			Console.WriteLine(now.ToUnixTimeMicroseconds());
			
			string timestamp = now.ToUnixTimeMicroseconds().ToString();
			return timestamp;
		}
		
		public static void SaveMessage(string email, string subject, string message){
			string timestamp = GetCurrentUnixTimestamp();
			string save_message = @""+
@"timestamp: "	+ timestamp + "\r\n" +
@"email: "		+ email		+ "\r\n" +
@"subject: "	+ subject	+ "\r\n" +
@"message: "	+ message	+ "\r\n\r\n"
;

			//File.AppendAllText(@"messages/messages.txt", save_message);
			File.WriteAllText(@"messages/" + timestamp+".txt", save_message);
		}
		
/*
		public static void SaveMessage(Dictionary<string, string> message){
			string timestamp = GetCurrentUnixTimestamp();

			string save_message = "";
			foreach (KeyValuePair<string, string> keypair in message)  
			{
				//Console.WriteLine("Key: {0}, Value: {1}", keypair.Key, keypair.Value);
				save_message += keypair.Key+": "+keypair.Value + "\r\n";
			}
			save_message += "\r\n";

			//File.AppendAllText(@"messages/messages.txt", save_message);
			File.WriteAllText(@"messages/" + timestamp+".txt", save_message);
		}

		public static void SaveMessageSQLite3(Dictionary<string, string> message){
			try{
				SQLite3.SQLite3Methods.AddMessage(message);
			}
			catch (Exception ex){
				Console.WriteLine("SaveMessageSQLite3. ex: "+ex);
				SaveMessage(message);
			}
		}
*/
		public static bool SaveMessage(Dictionary<string, string> message){
			try{
				string timestamp = GetCurrentUnixTimestamp();

				string save_message = "";
				foreach (KeyValuePair<string, string> keypair in message)  
				{
					//Console.WriteLine("Key: {0}, Value: {1}", keypair.Key, keypair.Value);
					save_message += keypair.Key+": "+keypair.Value + "\r\n";
				}
				save_message += "\r\n";

				//File.AppendAllText(@"messages/messages.txt", save_message);
				File.WriteAllText(@"messages/" + timestamp+".txt", save_message);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool SaveMessageSQLite3(Dictionary<string, string> message){
			if(HttpResponse.UseCaptcha == true){
				bool result		=	captcha.Captcha.CheckAnswer(message["RandomCaptchaGuid"], message["RandomCaptchaGuess"]);
				if(result == false){
					return false;
				}
			}
			
			try{
				return SQLite3.SQLite3Methods.AddMessage(message);
			}
			catch (Exception ex){
				Console.WriteLine("SaveMessageSQLite3. ex: "+ex);
				return SaveMessage(message);
			}
		}		
	}
}