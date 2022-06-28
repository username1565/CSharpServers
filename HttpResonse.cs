using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;	//Dictionary
using HttpServer;

namespace HttpServer
{
	class HttpResponse
	{ 
		static Encoding enc = Encoding.UTF8;	//HTTPServer Encoding.

		public static Encoding latin1 = Encoding.GetEncoding("ISO-8859-1");
		public static Encoding utf8 = Encoding.UTF8;
		
		public static byte[] Combine(byte[] first, byte[] second)
		{
			byte[] bytes = new byte[first.Length + second.Length];
			Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
			Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
			return bytes;
		}
		
		public static string AddHeader(
				bool isBinary = false
			,	int contentLength = 0
		)
		{
			string header = "HTTP/1.1 200 OK" + "\r\n"
				+ "Content-Type: " +
					(
						(isBinary == true)
						? "application/octet-stream"
						: "text/html"
					) + "\r\n"
				+ (
					(contentLength != 0)
						? "Content-Length: " + contentLength.ToString() + "\r\n"
						: ""
				)
				+ "\r\n"
			;
			return header;
		}
		
		public static string DifferentRequests(
				object[] properties					//properties
			,	Dictionary <string, string> props	//Method, address, http
		){
			//Dictionary <string, string> props = (Dictionary <string, string>)properties[2];	//Method, address, http
			
			string response = "";
			
			if(
					props["Method"] == @"POST"
				&&	props["Address"] == @"/feedback"
			){
					Console.WriteLine("Message received");
					
				/*
					int i = 0;
					foreach(object key in properties){
						Console.WriteLine(i+"blahblah: "+key);
						i++;
					}
				*/	
					
					Dictionary<string, string> content = HttpForms.getArgs((string)properties[1]);
					
				/*
					foreach (KeyValuePair<string, string> value in content)  
					{
						Console.WriteLine("Key: {0}, Value: {1}", value.Key, value.Value);  
					}
				*/

					
				//	Console.WriteLine("content: "+content);
					
					string email = content["email"];
					string subject = content["subject"];
					string message = content["message"];
				//	Console.WriteLine("message: "+message);

				//	HttpForms.SaveMessage(email, subject, message);
				//	HttpForms.SaveMessage(content);
					HttpForms.SaveMessageSQLite3(content);
					
					
					
					response = @"<html>
<meta charset=""UTF-8"">
Message received!<br><br>"+
"<div>"+
"<div>"+email+"</div>"+
"<div>"+subject+"</div>"+
"<div>"+message+"</div>"+
"</div>"+
@"<a href=""./feedback"">Go back</a>"
					;
			}
			else if(
						props["Method"] == @"GET"
					&&	props["Address"] == @"/feedback"
			){
				
				
					response = @"<html>"+
@"<head>
	<title>Feedback form</title>
</head>
<body>
	<form id=""feedback_form"" method=""POST"" action=""./feedback"">
		<div>
			<input name=""email"" type=""text"" placeholder=""email@mail.com"" value=""email@email.com""/>
			<br>
			<input name=""subject"" type=""text"" placeholder=""subject"" value=""subject""/>
			<br>
			<textarea name=""message"" placeholder=""message""/>message</textarea>
			<br>
			<input id=""attachments"" name=""attachments"" type=""file"" multiple />
			<br>
			<button type=""submit"" form=""feedback_form"" value=""Submit"">Submit</button>
		</div>
	</form>
	<script>
var form = document.getElementById('feedback_form');
var files = document.getElementById('attachments');

function readFile(file){
			var reader = new FileReader();

			reader.readAsDataURL(file);
				reader.onload = function()
				{
					console.log(reader.result);
					var input = document.createElement('input');
					input.type = 'hidden';
					input.name = file.name;
					input.value = reader.result;
					form.appendChild(input);
				};

			reader.onerror = function() {
				console.log(reader.error);
			};				
}

files.addEventListener(
	'change',
	function(e){
		console.log(e);
		for(var i = 0; i<files.files.length; i++){
			var file = files.files[i];
			console.log(file);
			readFile(file);
		}
	}
);		
	</script>
</body>
</html>";	//page
			}
			else if(
						props["Method"] == @"GET"
					&&	props["Address"] == @"/"				
			)
			{
				response = @"<html><head><title>Main page</title></head><body><h1>Main page</h1>Feedback <a href=""./feedback"">here</a>!</body></html>";
			}
			else{
				response = "";
			}

			Console.WriteLine(response);
			return response;
		}
		

		//Different HttpResponses
		public static byte[] Response (string request)
		{
			//request = HttpRequest.Request(request);	//the same string

			byte[] sendBytes;
			
		//	Console.WriteLine ("");
		//	Console.WriteLine (request);
			
			object[] properties = HttpRequest.Properties(request);	//header, content and properties of HTTP-response
		//	Console.WriteLine("(string)properties[0]: "+((string)properties[0]));
			Dictionary <string, string> props = (Dictionary <string, string>)properties[2];
			Console.WriteLine("props[\"Method\"]: "+(props["Method"]));
			
  
			StringBuilder builder = new StringBuilder ();
		//	builder.AppendLine (@"HTTP/1.1 200 OK"); 

		//	Console.WriteLine("request: "+request);

			if(
						!request.StartsWith("GET / ")
					&&	props["Address"] != @"/feedback"
			)
			{
				string details = request.Split(new string[]{"\r\n"}, StringSplitOptions.None)[0];
				string address = details.Split(new string[]{" "}, StringSplitOptions.None)[1];
				byte[] FileContent = new byte[0];
				
				if(File.Exists(@"www/"+address)){
					FileContent = File.ReadAllBytes(@"www/"+address);
				}
					
				if(address.Contains(".html")){
		//			builder.AppendLine (@"Content-Type: text/html;");
					builder.Append(AddHeader());
				}
				else{
		//			builder.AppendLine (@"Content-Type: application/octet-stream;");
					builder.Append(AddHeader(true, FileContent.Length));
				}
		//		builder.AppendLine (@"Content-Length: "+FileContent.Length.ToString());
		//		builder.AppendLine (@"");

				sendBytes =	Combine(
											enc.GetBytes (builder.ToString ())	//header-bytes
										,	FileContent							//FileContent-bytes
									)
				;
			}
			else{
				Console.WriteLine("different requests...");
		//		builder.AppendLine (@"Content-Type: text/html");
		//		builder.AppendLine (@"");

				builder.Append(AddHeader());
				builder.AppendLine (DifferentRequests(properties, props));

			//	Console.WriteLine ("");
			//	Console.WriteLine ("responce...");
			//	Console.WriteLine (builder.ToString ());
				 
				sendBytes = enc.GetBytes (builder.ToString ());
			}

			return sendBytes;
		}
	}
}