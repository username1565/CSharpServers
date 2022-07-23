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

		public static Encoding latin1 = Encoding.GetEncoding("ISO-8859-1");		//binary encoding
		public static Encoding utf8 = Encoding.UTF8;
		
		public static byte[] Combine(byte[] first, byte[] second)
		{
			byte[] bytes = new byte[first.Length + second.Length];
			Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
			Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
			return bytes;
		}
		
		public static string GetLatin1String(byte[] bytes){
			return latin1.GetString(bytes);
		}

		public static string DecodeMessage(string message){
			message = message.Replace("+", " ");	//space changed to "+"
			return message;
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
		
		public static bool UseCaptcha = false; 
		
		public static string DifferentRequests(object[] properties)	//request properties
		{
		//	Console.WriteLine("DifferentRequests...");
			Dictionary <string, string> _headerProperties = (Dictionary <string, string>)properties[2];	//Method, address, http
			Dictionary <string, string> param_value	= (Dictionary <string, string>)properties[3];		//Param_value from GET or POST-request

			string response = "";
			
			string method = _headerProperties["Method"];
			string address = _headerProperties["Address"];
			
			if(
					address.StartsWith(@"/messages/")
			){
				string page = address.Split(new string[]{"/messages/"}, StringSplitOptions.None)[1];
				int _page = System.Convert.ToInt32(page);
				
				response =		@"<a href=""/feedback"">Back</a>"
							+		SQLite3.SQLite3Methods.ShowMessages(_page)
							+	@"<br>"
							+		SQLite3.SQLite3Methods.GetPagesHTML()
				;
				return response;
			}
			if(
					address.StartsWith(@"/message/")
			){
				string id = address.Split(new string[]{"/message/"}, StringSplitOptions.None)[1];
				
				response = SQLite3.SQLite3Methods.ShowMessage(id);
				return response;
			}
			if(
					address == @"/random_captcha"
			){
				try{
					if(method == @"GET"){
						return @"<form action=""/random_captcha"" method=""post"">"+
									captcha.Captcha.AddInputsOfRandomCaptchaToSomeForm()
									+@"<button type=""submit"">Solve</button></form>"
						;
					}
					else if(method == @"POST"){
						/*
						foreach (KeyValuePair<string, string> value in param_value)  
						{
							Console.WriteLine("Key: {0}, Value: {1}", value.Key, value.Value);  
						}						
						*/
						bool result = false;
						
						if(UseCaptcha == true){
							string RandomCaptchaGuid		=	param_value["RandomCaptchaGuid"]		;
							string RandomCaptchaGuess		=	param_value["RandomCaptchaGuess"]	;
				
							result		=	captcha.Captcha.CheckAnswer(RandomCaptchaGuid, RandomCaptchaGuess);
			//				Console.WriteLine("result: "+result);
						}
						else{
							result = true;
						}
						response = result.ToString() + @"<br> <a href=""./random_captcha"">Back</a>";
						return response;
					}
				}
				catch(Exception ex){
					Console.WriteLine("DifferentContent ex"+ex);
					response = "";
				}
			}
			if(
					method == @"POST"
				&&	address == @"/feedback"
			){
					Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + ": New feedback received");
					
				/*
					int i = 0;
					foreach(object key in properties){
						Console.WriteLine(i+"blahblah: "+key);
						i++;
					}
				*/	
					
					//Dictionary<string, string> message = HttpForms.getArgs((string)properties[1]);
					Dictionary<string, string> message = param_value;
					
				
					foreach (KeyValuePair<string, string> value in message)  
					{
						Console.WriteLine("Key: {0}, Value: {1}", value.Key, value.Value);  
					}
				

					
				//	Console.WriteLine("message: "+message);
					
					string email = message["email"];
					string subject = message["subject"];
					string _message = message["message"];
					
				//	Console.WriteLine("message: "+message);
				//	message = DecodeMessage(message);					//replace +
				//	Console.WriteLine("message: "+message);

				//	HttpForms.SaveMessage(email, subject, message);
				//	HttpForms.SaveMessage(message);
				//	HttpForms.SaveMessageSQLite3(message);

					bool saved = HttpForms.SaveMessageSQLite3(message);
					if(!saved){
						return "not saved" + @"<br> <a href=""/feedback"">Back</a>";
					}
					
					
					response = @"<html>
<meta charset=""UTF-8"">
Message received!<br><br>"+
"<div>"+
"<div>"+email+"</div>"+
"<div>"+subject+"</div>"+
"<div>"+_message+"</div>"+
"</div>"+
@"<a href=""/feedback"">Go back</a>"
					;
			}
			else if(
						method == @"GET"
					&&	address == @"/feedback"
			){
				
					SQLite3.SQLite3Methods.GetPages();
					
					response = @"<html>"+
@"<head>
	<meta charset=""UTF-8"">
	<title>Feedback form</title>
</head>
<body>
"
+
@"
	<form id=""feedback_form"" method=""POST"" action=""/feedback"">
		<div>
			<input id=""email"" name=""email"" type=""text"" placeholder=""email@mail.com"" value=""email@email.com""/>
			<br>
			<input id=""subject"" name=""subject"" type=""text"" placeholder=""subject"" value=""subject""/>
			<br>
			<textarea id=""message"" name=""message"" placeholder=""message""/>message</textarea>
			<br>
			<input id=""attachments"" name=""attachments"" type=""file"" multiple />
			<br>
			<input id=""attached_files"" name=""attached_files"" type=""hidden"" />
			<br>
			"
				+	(
						(UseCaptcha == true)
							? captcha.Captcha.AddInputsOfRandomCaptchaToSomeForm()	//add captcha
							: ""
					)
			+@"
			<br>
			<button id=""submit"" type=""submit"" form=""feedback_form"" value=""Submit"">Submit</button>
		</div>
	</form>
	"

	+	SQLite3.SQLite3Methods.ShowMessages(-2)	//show messages
	+	@"<br>"
	+	SQLite3.SQLite3Methods.GetPagesHTML()

	+	@"
<script>
var feedback_form = document.getElementById('feedback_form');
var attachments = document.getElementById('attachments');
var attached_files = document.getElementById('attached_files');
var submit = document.getElementById('submit');

var busy = false;
function upload(filename, dataURL)
{
	busy = true;
	var xhr_interval;
	var xhr = new XMLHttpRequest();
	xhr.open('POST', '/upload');
	xhr.onload = function(){
        console.log(xhr.responseText);
		busy = false;
		submit.disabled = false;
		var namefile = filename.split('\0').join('');
		attached_files.value += ((attached_files.value === '') ? '' : '&' ) + ( namefile +'='+xhr.responseText.trim() ) ;	//rowid of added file
    };
	xhr.send(filename + dataURL);
}

function readFile(file, filename){
	busy = true;
	submit.disabled = true;
	var reader = new FileReader();
	reader.onload = function(e) {
		for(var i = filename.length; i<256; i++){filename += '\0';}		//add padding with nulls, up to 256 chars
		upload(filename, reader.result);								//and upload it
	}
	reader.onerror = function() {
		console.log(reader.error);
	};
	reader.readAsDataURL(file);
}

var i = 0;
var interval;
attachments.addEventListener(
		'change'
	,	function(e){
			attached_files.value = '';
			i = 0;
			interval = setInterval(
				function(){
					if(busy == false){
						if(i<attachments.files.length){
							readFile(attachments.files[i], attachments.files[i].name);		//read and upload it
							i++;
						}
						else{
							clearInterval(interval);
						}
					}
				},
				100
			);
	}
);
</script>
</body>
</html>
"		
;
			}
			else if(
						method == @"GET"
					&&	address == @"/"				
			)
			{
				response = @"<html><head><title>Main page</title></head><body><h1>Main page</h1>Feedback <a href=""/feedback"">here</a>!</body></html>";
			}
			else{
				response = "";
			}

		//	Console.WriteLine(response);
			return response;
		}
		

		//Different HttpResponses
		public static byte[] Response (string request)
		{
			//request = HttpRequest.Request(request);	//the same string

			StringBuilder builder = new StringBuilder ();
			byte[] sendBytes;
			
		//	Console.WriteLine ("");
		//	Console.WriteLine ("Response: "+request);
			
			if(
					request.Contains(@"POST")
				&&	request.Contains(@"/upload")
			){
				string content = request.Split(new string[]{"\r\n\r\n"}, StringSplitOptions.None)[1];
				string filename = (content.Substring(0, 256)).Replace("\0", string.Empty);
				string path = "Attachments" + Path.DirectorySeparatorChar + filename;
				string filecontent = content.Substring(256);

				//filecontent = filecontent.Split(',')[1];
				//byte[] FileContent = Convert.FromBase64String(filecontent);
				//File.WriteAllBytes(@path, FileContent);
				//string response = @"/Attachments/"+filename;
				
				int rowid = SQLite3.SQLite3Methods.AddAttachment(filename, filecontent);
				string response = rowid.ToString();

				builder.Append(AddHeader());
				builder.AppendLine (response);
				sendBytes = enc.GetBytes (builder.ToString ());
				return sendBytes;
			}
			
			object[] properties = HttpRequest.Properties(request);	//header, content and properties of HTTP-response
		//	Console.WriteLine("(string)properties[0]: "+((string)properties[0]));
			Dictionary <string, string> _headerProperties	= (Dictionary <string, string>)properties[2];
			Dictionary <string, string> param_value			= (Dictionary <string, string>)properties[3];
			
		//	Console.WriteLine("props[\"Method\"]: "+(props["Method"]));
			
  
		//	builder.AppendLine (@"HTTP/1.1 200 OK"); 

		//	Console.WriteLine("request: "+request);
		//	PISDA.PISDA.Log("request: "+request);

			string address = _headerProperties["Address"];

			if(
						address != @"/"
					&&	address != @"/feedback"
					&&	!address.Contains(@"/message/")
					&&	!address.Contains(@"/messages/")
					&&	!address.Contains(@"/attachment/")
					&&	!address.Contains(@"/upload")
			)
			{
			//	Console.WriteLine("return file");
				byte[] FileContent = new byte[0];
				
				Console.WriteLine("Return file. address: "+address);
				if(File.Exists(@"www/"+address)){
					FileContent = File.ReadAllBytes(@"www/"+address);
				}
				
				if(
						address.Contains(@".html")
				){
		//			Console.WriteLine("add text/html header");
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
			else if(address.Contains(@"/attachment/")){
				try{
					string id = address.Split(new string[]{"/attachment/"}, StringSplitOptions.None)[1];
				
					//object[] b_response = SQLite3.SQLite3Methods.GetAttachment(id);
					object[] b_response = SQLite3.SQLite3Methods.GetAttachment(id);
					if(b_response[0] == null && b_response[1] == null){
						builder.Append(AddHeader()); //return as text
						sendBytes =	Combine(
												enc.GetBytes (builder.ToString ())	//header-bytes
											,	enc.GetBytes("Attachment "+id+" not found")		//FileContent-bytes
										)
						;					
					}else{
						string filename		= (string)b_response[0];
						byte[] FileContent	= (byte[]) b_response[1];
				//		Console.WriteLine("b_response.Length: "+b_response.Length);
					
						Console.WriteLine("filename: "+filename);
						if(filename.Contains(".txt"))	//if ".txt" - file
						{
							builder.Append(AddHeader()); //return as text
						
							//show text in tad "pre": <pre>TEXT</pre>
							FileContent =	Combine(
												enc.GetBytes("<pre title=\""+filename+"\">")
											,	FileContent
										);
							FileContent =	Combine(
												FileContent
											,	enc.GetBytes("</pre>")
										);
						
						}
						else{
							builder.Append(AddHeader(true, FileContent.Length)); //else, return as file
						}
					
						sendBytes =	Combine(
											enc.GetBytes (builder.ToString ())	//header-bytes
										,	FileContent							//FileContent-bytes
									)
					;					
					}
				}
				catch (Exception ex){
					Console.WriteLine(ex);
					sendBytes = new byte[0];
				}				
			}
			else{
		//		Console.WriteLine("different requests...");
		//		builder.AppendLine (@"Content-Type: text/html");
		//		builder.AppendLine (@"");

				builder.Append(AddHeader());
				builder.AppendLine (DifferentRequests(properties));

			//	Console.WriteLine ("");
			//	Console.WriteLine ("responce...");
			//	Console.WriteLine (builder.ToString ());
				 
				sendBytes = enc.GetBytes (builder.ToString ());
			}

			return sendBytes;
		}
	}
}