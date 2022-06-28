using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;	//Dictionary

namespace HttpServer
{
	public class HttpRequest
	{
		public static string Request(string request){
			return request;
		}
		
		public static Dictionary<string, string> HeaderProperties(string header){
			Dictionary<string, string> values = new Dictionary<string, string>();
			
			//extract properties from header
			string[] splitted_header = header.Split(new string[]{"\r\n"}, StringSplitOptions.None);
			for(int line = 0; line<splitted_header.Length; line++)
			{
				if(line == 0){
					string[] FirstLine = splitted_header[line].Split(new string[]{" "}, StringSplitOptions.None);

					if(FirstLine.Length>=3){
						values["Method"] = FirstLine[0].ToString();
						values["Address"] = FirstLine[1].ToString();
						values["http"] = FirstLine[2].ToString();
					}
					continue;
				}
				string[] splitted_line = splitted_header[line].Split(new string[]{": "}, StringSplitOptions.None);
				if(splitted_line.Length >= 2){
					string name = splitted_line[0].ToString();
					string value = splitted_line[1].ToString();
					
					if(name=="attachments"){
						continue;
					}
					else{
						values[name] = value.ToString();
					}
				}
			}
			
/*
			foreach (KeyValuePair<string, string> value in values)  
			{
				Console.WriteLine("Key: {0}, Value: {1}", value.Key, value.Value);  
			}
*/

			return values;
		}

		public static object[] Properties(string request){
			//request - it's string
			
			//it's contains 
				string header = "";
				string content = "";
				
			//extract this both:
				string[] splitted = request.Split(new string[]{"\r\n\r\n"}, StringSplitOptions.None);
				
			//and write
				header = splitted[splitted.Length-2];
				content = splitted[splitted.Length-1];
				
				Dictionary<string, string> header_properties = HeaderProperties(header);

//			Console.WriteLine("header: "+header+", content: "+content+", content.Length: "+content.Length);
			
				return new object[]{
						header				//string
					,	content				//string
					,	header_properties	//Dictionary<string, string>
				};
		}
		
	}
}