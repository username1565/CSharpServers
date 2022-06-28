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
		
		public static string DecodeQueryString(string QueryString){
			string decoded = QueryString.Replace("+", " ");			//replace "+" to " "
			decoded = System.Uri.UnescapeDataString(decoded);		//Decode urlencoded string
			decoded = System.Web.HttpUtility.HtmlDecode(decoded);	//decode HTML entities
			return decoded;											//return
		}
		
		//parse QueryString, from content of GET or POST-request
		public static Dictionary<string, string> GetParamValue(string GET_STRING){
			Dictionary<string, string> KeyValue = new Dictionary<string, string>();
			string[] parameters = GET_STRING.Split('&');	//split by "&"
			for(var i = 0; i<parameters.Length; i++){
				string [] param_value = parameters[i].Split('=');
				string key = "";
				string value = "";
				if( param_value.Length == 2 ){
					key = DecodeQueryString(param_value[0]);
					value = DecodeQueryString(param_value[1]);
					//add this into dictionary
				}
				else if(param_value.Length == 1){
					key = DecodeQueryString(param_value[0]);
				}
				KeyValue[key] = value;
			}
			//and return dictionary.
			return KeyValue;
		}

		//return properties from Header
		public static Dictionary<string, string> HeaderProperties(string header){
			Dictionary<string, string> values = new Dictionary<string, string>();
			
			//extract properties from header
			string[] splitted_header = header.Split(new string[]{"\r\n"}, StringSplitOptions.None);	//spolit by "\r\n"
			for(int line = 0; line<splitted_header.Length; line++)	//for each line
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
					
					if(name=="attachments"){	//skip filenames of attachments
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

		//return properties of request
		public static object[] Properties(string request){
			//request - it's string
			
			//it's contains 
				string header = "";
				string content = "";
				
			//extract this both:
				string[] splitted = request.Split(new string[]{"\r\n\r\n"}, StringSplitOptions.None);
				
			//and write
				header = splitted[0];
				content = splitted[1];
				
				Dictionary<string, string> header_properties = HeaderProperties(header);
				Dictionary<string, string> param_value = new Dictionary<string, string>();
				
				if(content.Contains("=")){
					param_value = GetParamValue(content);
				}
			
			//return object with properties of request
				return new object[]{
						header						//string
					,	content						//string
					,	header_properties			//Dictionary<string, string>
					,	param_value					//param_value
				};
		}
				
/*
				Dictionary<string, string> header_properties = HeaderProperties(header);

//			Console.WriteLine("header: "+header+", content: "+content+", content.Length: "+content.Length);
			
				return new object[]{
						header				//string
					,	content				//string
					,	header_properties	//Dictionary<string, string>
				};
		}
*/
		
	}
}