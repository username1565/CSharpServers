using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using Chaos.NaCl;
using NDB;
using nboard;
using NServer;
using System.Text.RegularExpressions;
//using System.Net;
//wanted to download captcha pack from here:
//https://github.com/Karasiq/nanoboard/releases/download/v1.2.0/ffeaeb19.nbc
//but github using redirect, and no any proxy added, for downloading this anonymously.

//			For captcha-generator:
using System.Drawing.Imaging;			//to save PNG with option ImageFormat.Png to save bmp as PNG...
using System.Drawing.Drawing2D;			//to using HatchBrush and HatchStyle
using captcha;							//to load a ByteStringExt.cs, to use Stringify()
//using Newtonsoft.Json;

namespace captcha
{
    /*
        Class encapsulates captcha retriveal (from pack) and verification.

        Each post has it's designated captcha. 
        It is determined by post's SHA256 hash (signature tag and it's contents
        are excluded from hash calculation).
        This hash should match POW filter - at least three consecutive bytes
        with values from zero to one, starting from fourth byte of a hash - 
        otherwise post is considered to be invalid (without POW).
        First three bytes of hash is captcha index (wrapped by amount of captchas
        in the pack).
        The code extracts relevant captcha by index extracted from post's hash,
        and captcha is public ed25519 key, 32-byte seed encrypted by XORing with
        SHA512(UTF-8(captcha answer + public key in hexstring form))					//Here can be the long chain of different hashes, like sha512hash(sha256hash(SCrypt(Keccak(blake256b(value))))). See WarpWallet: https://keybase.io/warp/		
        and captcha image (1-bit) 50x20 pixels (column by column, each bit 
        represents a pixel (1 - black, 0 - white).
		Each block of captcha-pack-file contains ed25519_public_key(32 bytes) + encrypted_seed(32 bytes) + 1000 bits captcha image(125 bytes * 8 = 1000 bits).
		This file can be easy regenerated and just changed, but so hard to solve all captchas there for humans and computers.
		Also, can be changed at any time, the default parameter "dummyMessage";
    */
    public class Captcha
    {
        private static SHA256 _sha = null;

		public static SHA256 _sha_public   // property
		{
			get { return _sha; }  						// get method
			set { if(_sha == null){ _sha = value; } }	// set method
		}

        private const int PowByteOffset = 3;
        private const int PowLength = 3;
        private const int PowTreshold = 1;	//max value after null, to be accept this is consecutive nulls. Example: 000 so hard to be calculated, so "010", "100", "011", etc, is accepted too as "000"
        private const int CaptchaBlockLength = 32 + 32 + 125;
        private const string DataUriPngPrefix = "data:image/png;base64,";
        private const string CaptchaImageFileSuffix = ".png";
        public const string SignatureTag = "sign";
        private const string PowTag = "pow";
		
		public static string captcha_file = "captcha.nbc";
		public static string original_captcha_file_sha256_hash = "0732888283037E2B17FFF361EAB73BEC26F7D2505CDB98C83C80EC14B9680413";
		public static string captcha_downloading_url = "http://some_url_to_download_captcha/";	//This value can be customized in config-3.json, without hardcoding this.

		public static bool captcha_checked = false;		public static bool bypassValidation = false;
		public static bool IsCaptchaValid = false;
		public static bool captcha_found = false;

        private static string _packFile = null;
        public static string _packFile_public		{
			get { if(_packFile == null) { _packFile = captcha_file; } return _packFile; }  								// get method
		}

		//public static long size = new FileInfo(captcha_file).Length;	//byte length of captcha-pack-file
        //public static int count = (int) (size / CaptchaBlockLength);				//number of captchas, inside file
		
		
		private static string SHA256CheckSum(string filePath)
		{
			Console.Write("Wait calculating SHA256-hash for \""+filePath+"\"... ");
			using (SHA256 SHA256 = SHA256Managed.Create())
			{
				using (FileStream fileStream = File.OpenRead(filePath)){
					string sha256hash = ToHex(SHA256.ComputeHash(fileStream), true);
					Console.Write("Done!\n");
					//Console.WriteLine(sha256hash);	//show hash
					return sha256hash;
				}
			}
		}

		private static string ToHex(byte[] bytes, bool upperCase)
		{
			StringBuilder result = new StringBuilder(bytes.Length * 2);
			for (int i = 0; i < bytes.Length; i++)
				result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
			return result.ToString();
		}
		
//		private static void download_captcha_file(string url, string filename){
			/*
			using (WebClient wc = new WebClient())
			{
				//wc.DownloadProgressChanged += wc_DownloadProgressChanged;
				wc.DownloadFileAsync (
					new System.Uri(url),	// Param1 = Link of file
					filename				// Param2 = Path to save
				);
			}
			*/
			
//			Console.WriteLine("url: "+url+", filename: "+filename);
/*
	long fileSize = 0;
    int bufferSize = 1024;
    bufferSize *= 1000;
    long existLen = 0;
    
    System.IO.FileStream saveFileStream;
    if (System.IO.File.Exists(filename))
    {
        System.IO.FileInfo destinationFileInfo = new System.IO.FileInfo(filename);
        existLen = destinationFileInfo.Length;
    }

    if (existLen > 0)
        saveFileStream = new System.IO.FileStream(filename,
                                                  System.IO.FileMode.Append,
                                                  System.IO.FileAccess.Write,
                                                  System.IO.FileShare.ReadWrite);
    else
        saveFileStream = new System.IO.FileStream(filename,
                                                  System.IO.FileMode.Create,
                                                  System.IO.FileAccess.Write,
                                                  System.IO.FileShare.ReadWrite);
 
    System.Net.HttpWebRequest httpReq;
    System.Net.HttpWebResponse httpRes;
    httpReq = (System.Net.HttpWebRequest) System.Net.HttpWebRequest.Create(url);
    httpReq.AddRange((int) existLen);
    System.IO.Stream resStream;
    httpRes = (System.Net.HttpWebResponse) httpReq.GetResponse();
    resStream = httpRes.GetResponseStream();
 
    fileSize = httpRes.ContentLength;
 
    int byteSize;
    byte[] downBuffer = new byte[bufferSize];
 
    while ((byteSize = resStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
    {
        saveFileStream.Write(downBuffer, 0, byteSize);
    }
*/

/*
            bool _collectAvail = false;
            AggregatorMain.Run(new string[0], url, filename);
//            Aggregator.ParseImage(new string[0], url, filename);
            ThreadPool.QueueUserWorkItem(o => 
            {
                while(AggregatorMain.Running) 
                {
                    Thread.Sleep(1000);
                }

                _collectAvail = true;
            });
            //return _collectAvail;
*/			
/*
		DateTime startTime = DateTime.UtcNow;
        WebRequest request = WebRequest.Create(url);
        WebResponse response = request.GetResponse();
        using (Stream responseStream = response.GetResponseStream()) {
            using (Stream fileStream = File.OpenWrite(filename)) { 
                byte[] buffer = new byte[4096];
                int bytesRead = responseStream.Read(buffer, 0, 4096);
                while (bytesRead > 0) {       
                    fileStream.Write(buffer, 0, bytesRead);
                    DateTime nowTime = DateTime.UtcNow;
                    if ((nowTime - startTime).TotalMinutes > 5) {
                        throw new ApplicationException(
                            "Download timed out");
                    }
                    bytesRead = responseStream.Read(buffer, 0, 4096);
                }
            }
        }
*/
		
/*
			WebClient webClient = new WebClient();
			webClient.DownloadFile(url, filename);			
*/
/*
			WebClient webClient = new WebClient();
			//webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
			//webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
			webClient.DownloadFileAsync(new Uri(url), filename);
*/
  
/*
			using (var client = new WebClient())
			{
				client.DownloadFile(new System.Uri(url), filename);
			}			
*/

//}

		
/*
		public static bool verify_captcha_hash(){
            try{
				_packFile = Configurator.Instance.GetValue("captcha_pack_file", captcha_file);
				captcha_downloading_url = Uri.UnescapeDataString(Configurator.Instance.GetValue("captcha_url", captcha_downloading_url));
			}catch//(Exception ex)	//need to show exception
			{
				//Console.WriteLine("exception: "+ex);	//when nboard compiled as NBPack, configurator instance is not available.
			}
			
			string captcha_file_hash = "";
			if(!File.Exists(_packFile)){
				Console.WriteLine(_packFile+" does not exists.");

				//download_captcha_file(captcha_downloading_url, captcha_file);	//github using redirect with temporary links, so cann't download. Method was been removed.

				return false;
			}
			else{
				captcha_file_hash = SHA256CheckSum(_packFile);
				if(captcha_file_hash!=original_captcha_file_sha256_hash){
					Console.WriteLine(
								"(captcha_file_hash == original_captcha_file_sha256_hash): "+(captcha_file_hash==original_captcha_file_sha256_hash)
						+"\n"+	"captcha_file_hash:\n"+captcha_file_hash
						+"\n"+	"original_captcha_file_sha256_hash:\n"+original_captcha_file_sha256_hash
					);

					//download_captcha_file(captcha_downloading_url, captcha_file);	//just leave this here.

					return false;
				}else{
					Console.Write("Hash OK? "+(captcha_file_hash==original_captcha_file_sha256_hash)+". ");
					return true;
				}
			}
		}
*/		
        static Captcha()
        {
			/*
            //_packFile = Configurator.Instance.GetValue("captcha_pack_file", "captcha.nbc");
            try{
				_packFile = Configurator.Instance.GetValue("captcha_pack_file", captcha_file);
            }
			catch//(Exception ex)
			{
				//Console.WriteLine("Captcha.cs. Captcha. Error get captcha-filename from configurator: Exception:"+ex+"\nNow, captcha file is \"captcha.nbc\"");
				_packFile = "captcha.nbc";
			}
			*/
			_sha = SHA256.Create();
			//captcha_found = File.Exists(_packFile);			
        }

        public string ImageDataUri
        {
            get
            {
                return LoadImageAsDataUri();
            }
        }

        private readonly byte[] _publicKey;
        private readonly byte[] _encryptedSeed;
        private readonly byte[] _imageBits;
        private string _imageDataUri = null;

        public byte[] _publicKey_public 			{get { return _publicKey; 		}		}
        public byte[] _encryptedSeed_public			{get { return _encryptedSeed; 	}		}
        public byte[] _imageBits_public				{get { return _imageBits; 		}		}
        public string _imageDataUri_public			{get { return _imageDataUri; 	}		}

        public Captcha(byte[] publicKey, byte[] encryptedSeed, byte[] imageBits)
        {
            _publicKey = publicKey;
            _encryptedSeed = encryptedSeed;
            _imageBits = imageBits;
        }

        public string LoadImageAsDataUri()
        {
            if (_imageDataUri != null) return _imageDataUri;
/*
            var bitmap = BitmapConvert.Convert(_imageBits);
            var imageFile = Guid.NewGuid().ToString() + CaptchaImageFileSuffix;
            bitmap.Save(imageFile);
            var uri = DataUriPngPrefix + Convert.ToBase64String(File.ReadAllBytes(imageFile));
            File.Delete(imageFile);
            _imageDataUri = uri;			
            return _imageDataUri;
*/
			//do this all, without save file on disk, and delete it.
			using (var ms = new MemoryStream())
			{
				using (var bitmap = BitmapConvert.Convert(_imageBits))
				{
					bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					var uri = DataUriPngPrefix + Convert.ToBase64String(ms.GetBuffer());
					_imageDataUri = uri;			
					return _imageDataUri;
				}
			}

        }

        public string AddSignatureToThePost(string post, string guess)
        {
            var dec_seed = ByteEncryptionUtil.WrappedXor(_encryptedSeed, guess + _publicKey.Stringify());
            var privateKey = Ed25519.ExpandedPrivateKeyFromSeed(dec_seed);
            var signature = Ed25519.Sign(Encoding.UTF8.GetBytes(post), privateKey);
            return post + "[" + SignatureTag + "=" + signature.Stringify() + "]";
        }

        public bool CheckSignature(string postWithSignature)
        {
            var post = Encoding.UTF8.GetBytes(postWithSignature.ExceptSignature());
            var sign = postWithSignature.Signature();
            return Ed25519.Verify(sign, post, _publicKey);
        }

        public bool CheckGuess(string guess)
        {
            var dec_seed = ByteEncryptionUtil.WrappedXor(_encryptedSeed, guess + _publicKey.Stringify());
            var privateKey = Ed25519.ExpandedPrivateKeyFromSeed(dec_seed);
            var dummyMessage = new byte[]{(byte)0};
            var signature = Ed25519.Sign(dummyMessage, privateKey);
            //return Ed25519.Verify(signature, dummyMessage, _publicKey);
            bool ok = Ed25519.Verify(signature, dummyMessage, _publicKey);
			if(ok == true){
				/*
				if(NDB.PostDb.UseSQLite3 == true){
					SQLite3Methods.SaveCaptchaAnswer(_publicKey.Stringify(), _encryptedSeed.Stringify(), guess);
				}
				*/
			}
			return ok;
        }

        public static Captcha GetCaptchaForPost(string post, string captchaPackFilename = "")
        {
            captchaPackFilename = (captchaPackFilename == "") ? _packFile : captchaPackFilename;
            var size = new FileInfo(captchaPackFilename).Length;
            int count = (int) (size / CaptchaBlockLength);
            return GetCaptchaForIndex(CaptchaIndex(post, count), captchaPackFilename);
        }

        public static Captcha GetCaptchaForIndex(int captchaIndex, string captchaPackFilename = "")
        {
			captchaPackFilename = (captchaPackFilename == "") ? _packFile : captchaPackFilename;
            if (captchaIndex == -1) return null;
            var publicKey = FileUtil.Read(captchaPackFilename, captchaIndex * CaptchaBlockLength, 32);
            var encryptedSeed = FileUtil.Read(captchaPackFilename, captchaIndex * CaptchaBlockLength + 32, 32);
            var image = FileUtil.Read(captchaPackFilename, captchaIndex * CaptchaBlockLength + 32 + 32, 125);
            return new Captcha(publicKey, encryptedSeed, image);
        }
		
/*
		public static string GetCaptchaAsJsonByIndex(string IndexOfCaptcha, string captchaPackFilename = ""){
			captchaPackFilename = (captchaPackFilename == "") ? _packFile : captchaPackFilename;
			int CaptchaIndexInt = ByteStringExt.parse_number(IndexOfCaptcha);
			Captcha CaptchaByIndex =  GetCaptchaForIndex(CaptchaIndexInt, captchaPackFilename);
			return JsonConvert.SerializeObject(CaptchaByIndex, Formatting.Indented);
		}
*/

        public static bool PostHasSolvedCaptcha(string post, bool bypassValidation = false)
        {
			if(bypassValidation) {
				//Console.WriteLine("Captcha.cs: PostHasSolvedCaptcha - bypassValidation = "+bypassValidation+" now.");
				return true;
			}
            var captcha = GetCaptchaForPost(post);
            if (captcha == null) return false;
            bool isCaptchaSolved = captcha.CheckSignature(post);
			if(!isCaptchaSolved){
				File.AppendAllText(@"invalidcaptchas.txt", "Captcha not solved for post: "+post);
			}
			return isCaptchaSolved;
        }

		//This need to accelerate computing POW and captcha-index, as first three bytes of POW-hash.
        public static string ExceptXmg(string post)
        {
		//	Console.WriteLine("Captcha.cs. ExceptXmg. Post:"+post);
		//	Console.WriteLine("replyTo", post.Substring(0, 32));
        //    var matches = Regex.Matches(post, "\\[xmg=[^\\]]*\\]");
            var matches = Regex.Matches(post, "\\[(i|x)mg=[^\\]]*\\]");	//except xmg and img and tag too
            foreach (Match m in matches)
            {
				string hash = _sha.ComputeHash(Encoding.UTF8.GetBytes(m.Value)).Stringify();
			//	Console.WriteLine("ExceptXmg: hash: "+hash);
                post = post.Replace(m.Value, hash);
            }
            return post;
        }

/*
		private static Dictionary<string, string> PostHashesWhereFileTagNotExcepted = new Dictionary<string>{
			//	{"post hash with excepted file-tag", "old post-hash to replace on this"}
				{"","082a06592e3b241e3c49063524a3ac25"}
			,	{"","4dcf53a58e839e8fddce6959222ed675"}
			,	{"","c19a06c128f78e5946014883bc018dd9"}
			,	{"","946d50e33c413a93d7c963424e0846d9"}
			,	{"","e55aa8b07a49805e6f450f264762fcf2"}
			,	{"",""}
			,	{"",""}
			,	{"",""}
			,	{"",""}
		};
*/		

		//[file]-tag is the part of message.
		//If this will be replaced, on hash (to accelerate computing the hash), then old versions of nanoboard will not validate new posts.
		//Also captcha indes depending from hash, after ExceptSignature, and ExceptXmg.
		//If ExceptFile will be added, will be different captcha-indexes for posts with [file]-tag inside.
		
		//Except [file]-tags with attachments.
        public static string ExceptFile(string post)
        {
            var matches = Regex.Matches(post, @"\[file.*?\].*?\[/file\]");		//match files
            foreach (Match m in matches)										//for each match
            {
			//	Console.WriteLine("ExceptFile: hash: "+_sha.ComputeHash(Encoding.UTF8.GetBytes(m.Value)).Stringify());
                post = post.Replace(m.Value, _sha.ComputeHash(Encoding.UTF8.GetBytes(m.Value)).Stringify());	//replace matched value to hash
            }
            return post;	//and return post.
        }

        private static byte[] ComputeHash(string post)
        {
            return _sha.ComputeHash(Encoding.UTF8.GetBytes(post));
        }

		//List with three default posts, without POW.
		public static List<string> FirstThreePosts = new List<string>{
				"00000000000000000000000000000000{Welcome to Nanoboard}"
			,	"f682830a470200d738d32c69e6c2b8a4[b]КАТЕГОРИИ[/b]\nЧтобы создать новую категорию, ответьте на это сообщение.\nОтветьте на одну из категорий, чтобы создать там тред."
			,	"bdd4b5fc1b3a933367bc6830fef72a35[b]Бред/Разное[/b]"
		};
		
        public static bool PostHasValidPOW(string post)
        {
			if(FirstThreePosts.Contains(post)){return false;}	//exclude FirstThreePosts, then return false, and do nothing;
		//	Console.WriteLine("Captcha.cs. PostHasvalidPOW. post: "+post);
            post = post.ExceptSignature();
        //    var hash = ComputeHash(ExceptXmg(post));
            var hash = ComputeHash(ExceptFile(ExceptXmg(post)));
            bool isPowValid = hash.MaxConsecZeros(PowByteOffset, PowTreshold) >= PowLength;
			if(!isPowValid){
				File.AppendAllText(@"invalidPOW.txt", "invalid POW for post:\n"+post+"\n\n");
			}
			return isPowValid;
        }

        public static byte[] returnPOWHash(string post, int max = 0)
        {
		//	if(max == 0){max = count;}
			if(max==0){return new byte[]{0};}						//% max, when max = 0 return error. max == null when captcha-pack-file not found, and file.Length == 0
            post = post.ExceptSignature();
        //    var hash = ComputeHash(ExceptXmg(post));	//Need ExceptFile here, or not?? Maybe not, else, captchas will be not ok.
            var hash = ComputeHash(ExceptFile(ExceptXmg(post)));	//Need ExceptFile here, or not?? Maybe not, else, captchas will be not ok.
//			Console.WriteLine("CaptchaIndex. hash = ComputeHash(ExceptXmg(post)) = "+BitConverter.ToString(ComputeHash(ExceptXmg(post))).Replace("-", ""));
            if (hash.MaxConsecZeros(PowByteOffset, PowTreshold) < PowLength) {	return new byte[]{1}; }
			return hash;
        }
		
		public static string returnPOWHashString(string post, int max = 0){
		//	if(max == 0){max = count;}
			if(max == 0){return "0";}						//% max, when max = 0 return error. max == null when captcha-pack-file not found, and file.Length == 0
			string hexString = BitConverter.ToString(returnPOWHash(post, max)).Replace("-", "");
			return hexString;
		}

        public static int CaptchaIndex(string post, int max = 0)
        {
		//	if(max == 0){max = count;}
			if(max == 0){return 0;}						//% max, when max = 0 return error. max == null when captcha-pack-file not found, and file.Length == 0
            post = post.ExceptSignature();
        //    var hash = ComputeHash(ExceptXmg(post));	//Need ExceptFile here, or not?? Maybe not, else, captchas will be not ok.
            var hash = ComputeHash(ExceptFile(ExceptXmg(post)));	//Need ExceptFile here, or not?? Maybe not, else, captchas will be not ok.
//			Console.WriteLine("CaptchaIndex. hash = ComputeHash(ExceptXmg(post)) = "+BitConverter.ToString(ComputeHash(ExceptXmg(post))).Replace("-", ""));
            if (hash.MaxConsecZeros(PowByteOffset, PowTreshold) < PowLength) return -1;
//			Console.WriteLine("hash[0]: "+hash[0]+", hash[1]: "+hash[1]+", hash[2]: "+hash[2]+", max: "+max);
//			Console.WriteLine("POW is ok... return captcha-index: "+((hash[0] + hash[1] * 256 + hash[2] * 256 * 256) % max));
            return (hash[0] + hash[1] * 256 + hash[2] * 256 * 256) % max;
        }
		
		public static long GetGMTInMS(DateTime timestamp)
        {
            var unixTime = timestamp.ToUniversalTime() -
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)unixTime.TotalMilliseconds;
        }
		
		public static string RandomGuid(){	return Guid.NewGuid().ToString();	}

		private static RNGCryptoServiceProvider _RNG = new RNGCryptoServiceProvider();  

		private static int GetNextRnd(int min, int max)
		{
			byte[] rndBytes = new byte[4];
			_RNG.GetBytes(rndBytes);
			int rand = BitConverter.ToInt32(rndBytes, 0); 
			const Decimal OldRange = (Decimal)int.MaxValue - (Decimal)int.MinValue;
			Decimal NewRange = max - min;
			Decimal NewValue = ((Decimal)rand - (Decimal)int.MinValue) / OldRange * NewRange + (Decimal)min;
			return (int)NewValue;
		}

		public static int maxCaptchaIndexes =	//Total number of captchas in captcha-pack-file
			(
				(
					(!File.Exists(_packFile))					//if captcha-pack-file not exists
						?	0									//then set it as 0
						:	(int)								//otherwise, long to int
					(
						(FileInfo)						//FileInfo
								(new FileInfo(_packFile))		//of captcha_file
					)
					.Length								//take file-size
				)
				/
				(
					(25*25*125)							//each captcha contains two 32-bytes key, and 125 byte of image
				)
			)
		;

		public static void ClearCaptchasIfNeed(long unixTimestampNow){
			//If too many keys - clear old guids
			int MaxItemsInDictionaryToClearThis = 50;	//items in dictionary to try clean this
			if((CaptchaTracker.TimeOfGenerationRandomCaptcha).Count > MaxItemsInDictionaryToClearThis){
				List<string> OldItemsToRemove = new List<string>();	//need to create a separate list because keypair can not be removed in foreach cycle.

				foreach (KeyValuePair<string, long> GuidAndTimestamp in (CaptchaTracker.TimeOfGenerationRandomCaptcha)) {
					int timeout = 15;	//seconds for each guid
					if(unixTimestampNow - GuidAndTimestamp.Value > timeout * 1000){
						OldItemsToRemove.Add(GuidAndTimestamp.Key);
					}
				}

				foreach (string OldItemToRemove in OldItemsToRemove)
				{
					(CaptchaTracker.TimeOfGenerationRandomCaptcha)	.Remove(OldItemToRemove);
					(CaptchaTracker.RandomCaptchas)					.Remove(OldItemToRemove);
				}
				OldItemsToRemove.Clear();	//remove all guids from list.
			}
			//else leave this in memory.
		}
		
		//	Generate new random captcha-index, and extract captcha from captcha-pack-file, then add to dictionary, and return guid.
		public static string RandomCaptcha(string guid = ""){
			string	NewRandomGuid				=	guid == "" ? Guid.NewGuid().ToString() : guid;
			if(guid != "" && (CaptchaTracker.RandomCaptchas).ContainsKey(NewRandomGuid)){return guid;}

			int		NewRandomCaptchaIndex 		= 	GetNextRnd(0, maxCaptchaIndexes);
			long	unixTimestampNow			=	GetGMTInMS(DateTime.Now);
			
			captcha.Captcha NewRandomCaptcha	=	captcha.Captcha.GetCaptchaForIndex(NewRandomCaptchaIndex, captcha.Captcha.captcha_file);
			ClearCaptchasIfNeed(unixTimestampNow);	//clear old captchas, if need.
			//then add new captcha
			(CaptchaTracker.RandomCaptchas)					.Add(NewRandomGuid, NewRandomCaptcha);
			(CaptchaTracker.TimeOfGenerationRandomCaptcha)	.Add(NewRandomGuid, unixTimestampNow);
			return NewRandomGuid;
		}

		//	Generate the new captcha, and this not contains in captcha-pack-file, then add to dictionary, and return guid.
		public static string GetNewRandomCaptcha(string guid = ""){
			string	NewRandomGuid				=	guid == "" ? Guid.NewGuid().ToString() : guid;
			if(guid != "" && (CaptchaTracker.RandomCaptchas).ContainsKey(NewRandomGuid)){return guid;}

			long	unixTimestampNow			=	GetGMTInMS(DateTime.Now);
			captcha.Captcha NewRandomCaptcha	=	CaptchaPack_Generator.Captcha_generator.GenerateNewCaptcha(null, _RNG, null);
			ClearCaptchasIfNeed(unixTimestampNow);	//clear old captchas, if need.
			//then add new captcha
			(CaptchaTracker.RandomCaptchas)					.Add(NewRandomGuid, NewRandomCaptcha);
			(CaptchaTracker.TimeOfGenerationRandomCaptcha)	.Add(NewRandomGuid, unixTimestampNow);
			return NewRandomGuid;
		}
		
		public static string GetCaptchaImage(string guid=""){
			if(guid == "" || !(CaptchaTracker.RandomCaptchas).ContainsKey(guid)){return "";}
			captcha.Captcha CaptchaByGuid = (CaptchaTracker.RandomCaptchas)[guid];
			return CaptchaByGuid.ImageDataUri;
		}

		public static string AddInputsOfRandomCaptchaToSomeForm(){
		//	string RandomCaptchaGuid				= RandomCaptcha();				//get random captcha from captcha-pack-file
			string RandomCaptchaGuid				= GetNewRandomCaptcha();		//generate new random captcha
			string RandomCaptchaImage				= GetCaptchaImage(RandomCaptchaGuid);
			string CaptchaSolveForm	=
@"
	<a href=""javascript:void(0)"" onclick=""window.location.reload();"">
		<img title=""Random captcha image"" src="""	+	RandomCaptchaImage 	+
		@"""/>
	</a> 
	<input type=""hidden"" name=""RandomCaptchaGuid"" value="""		+	RandomCaptchaGuid	+
@""" title=""captcha-guid, generated on server."" >
	<input type=""text"" maxlength=""5"" placeholder=""Enter captcha-answer"" name=""RandomCaptchaGuess"" required autocomplete=""off"">
";
			return CaptchaSolveForm;
		}
		
		public static bool CheckAnswer(string guid, string guess){
			try{
				captcha.Captcha CaptchaByGuid = (CaptchaTracker.RandomCaptchas)[guid];

				(CaptchaTracker.TimeOfGenerationRandomCaptcha)	.Remove(guid);
				(CaptchaTracker.RandomCaptchas)				.Remove(guid);			

				return CaptchaByGuid.CheckGuess(guess);
			}
			catch	//(Exception ex)
			{
				//Console.WriteLine("CheckAnswer: "+ex);	//when nanodb server is restarted, Dictionary is empty, and this throws.
				return false;								//so captcha answers is invalid
			}
		}
		
		//parse partial data from the form of port-request. See FileHandler.cs UploadPNG
		public static bool ValidateRandomCaptchaInputs(string FormOrPartialForm){
			//Dictionary<string, string> ParamValue = NServer.DbApiHandler.GetParamValue(FormOrPartialForm);
			Dictionary<string, string> ParamValue = HttpServer.HttpRequest.GetParamValue(FormOrPartialForm);
			string RandomCaptchaGuid		=	ParamValue["RandomCaptchaGuid"]		;
			string RandomCaptchaGuess		=	ParamValue["RandomCaptchaGuess"]	;
				
			bool result		=	CheckAnswer(RandomCaptchaGuid, RandomCaptchaGuess);
			return result;
		}

        public static string AddPow(string post)
        {
            post = post.ExceptSignature();
        //    var xpost = ExceptXmg(post);
            var xpost = ExceptFile(ExceptXmg(post));
            byte[] hash = null;
            var buffer = new byte[128];
            var rand = new RNGCryptoServiceProvider();
            var trash = "";

            while (hash == null || hash.MaxConsecZeros(PowByteOffset, PowTreshold) < PowLength)
            {
                rand.GetBytes(buffer);
                trash = "["+PowTag+"=" + buffer.Stringify() + "]";
                hash = ComputeHash(xpost + trash);		//Here can be the long chain of different hashes, like sha512hash(sha256hash(SCrypt(Keccak(blake256b(value))))). See WarpWallet: https://keybase.io/warp/
            }

//			Console.WriteLine("Catcha.cs. AddPow. trash: "+trash+", hash: "+BitConverter.ToString(hash).Replace("-", "")+", hash.MaxConsecZeros(PowByteOffset, PowTreshold): "+hash.MaxConsecZeros(PowByteOffset, PowTreshold));
            return post + trash;
        }
    }//end class captcha.Captcha.
}//end namespace Captcha.


//-----BEGIN comment in Captcha.cs-----
/*	//-----Captcha_Pack_Generator.cs-----
	//	This code can working autonomous
	//	and this can be compiled in standalone exe,
	//	after uncomment the commented code:
	//Usings:
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

using System.Security.Cryptography;
using System.Linq;

	//	Before using Chaos.NaCl, need to add Chaos.NaCl.dll, as reference, for compilation this:
using Chaos.NaCl;

	//	Compile a standalone exe:
	//	DirWithCSFile>%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe /t:exe /reference:"../Chaos.NaCl.dll" /out:"../Captcha_Pack_Generator.exe" Captcha_Pack_Generator.cs
//-----END comment in Captcha.cs-----
*/

namespace CaptchaPack_Generator
{

//-----BEGIN comment in Captcha.cs-----
/*	// The following commented code make this program full and independent:

	//ByteStringExt.cs - need to show bytearrays as hex, stringify
    public static class ByteStringExt
    {
        public static string Stringify(this byte[] bytes) //show bytes as hex-string
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
		
		//int.Parse and Int32.Parse working bad for me.		See issue: https://github.com/nanoboard/nanoboard/issues/5
		//So this function was been writed, to make this code more independent...
        public static int parse_number(this string string_number)//this function return (int)number from (string)"number". Negative numbers supporting too.
        {	if(string_number=="" || string_number == null){Console.WriteLine("NbPack.cs. parse_number. string_number is empty or null: (string_number == \"\"): "+string_number+", (string_number == null): "+(string_number == null)); return 0;}
			string test = (new System.Text.RegularExpressions.Regex(@"\D")).Replace(string_number, "");
            int test_length = test.Length;
            int number = 0;
            for(int i = ((char)test[0]=='-')?1:0; i < test_length; i++){
                number += ((int)Char.GetNumericValue((char)test[i])*(int)Math.Pow(10,test_length-i-1));
			}
            number = ((char)test[0]=='-'?(0-number):(number));
            return number;
        }
	}

	//ByteEncryptionUtil.cs - need to encrypt generated Ed25519-seed, and decrypt it by publicKey and captcha_answer.
    public static class ByteEncryptionUtil
    {
        public static byte[] WrappedXor(byte[] input, string key)
        {
			//one sha hash generated in this case. There can be the chains of different hashes, to make trying the brute-force captcha-value so difficult.
            byte[] sha = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
            byte[] output = new byte[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (byte) (input[i] ^ sha[i & 63]);
            }
            return output;
        }
    }
	
//FileUtil.cs	- to read-write bytes in the specified offset of file or/and append the bytes and text there.
    public static class FileUtil
    {
        public static object _lock = new object();

        // Appends bytes to the end of file
        public static int Append(string path, string @string)
        {
            lock (_lock) //sometimes .db3 file busy by another process, when program try to append string there. lock it.
            {
				return Append(path, System.Text.Encoding.UTF8.GetBytes(@string));
			}
        }
	
        // Appends bytes to the end of file
        public static int Append(string path, byte[] bytes)
        {
            lock (_lock)
            {
                long pos = 0;
                using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None))
                {
                    pos = stream.Position;
                    stream.Write(bytes, 0, bytes.Length);
					stream.Close();
					stream.Dispose();
                }
                return (int)pos;
            }
        }

        // Writes bytes at specific file offset, overwrites existing bytes
        public static void Write(string path, byte[] bytes, int offset)
        {
            lock (_lock)
            {
                using (Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    stream.Seek(offset, SeekOrigin.Begin);
                    stream.Write(bytes, 0, bytes.Length);
					stream.Close();
					stream.Dispose();
                }
            }
        }
		
        // Reads bytes from file using specific offset and length
        public static byte[] Read(string path, int offset, int length)
        {
            var bytes = new byte[length];
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            {
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Read(bytes, 0, length);
				stream.Close();
				stream.Dispose();
            }
            return bytes;
        }		
    }//end class FileUtil
//-----END comment in Captcha.cs-----
*/

	//Generate random captcha text, and return captcha image for this text, with b/w image in Captcha_Generator.imageBits (static) or created_captcha_object._imagebits (non-static)
    public class Captcha_generator
    {
//        private static Random Randomizer = new Random(DateTime.Now.Second);	//old code
		private static Random Randomizer = null;		//make this more crypto-strength, and create this object once in Program class.
        public string Text { get; set; }				//Text of generated captcha, empty by default.
        public byte[] ImageAsByteArray { get; set; }	//not compressed image, as bytes, with many collors, empty by default.
        public static byte[] imageBits { get; set; }	//The static field with compressed b/w captcha image. This is available as "Captcha.imageBits", after creating captcha object. Empty by default.
		public byte[] _imageBits { get; set; }			//do the previous field non-static, and now this available as "captcha_object._imageBits" for already created object.
		public static string DataUriPngPrefix = "data:image/png;base64,";
		public string _imageBits_dataURI { get; set; }

        public Captcha_generator(Random set_defined_Randomizer) // Captcha-object constructor
        {
			Randomizer = set_defined_Randomizer;		//set already predefined randomizer
            Text = GetRandomText();						//Generate random captcha-text.
            ImageAsByteArray = CreateCaptcha(Text);		//create static "imageBits" too.
			_imageBits = imageBits;						//copy compressed image from static "imageBits" to non-static field "_imageBits", after creating "imageBits".
			//set _imageBits_dataURI to be able to open this in browser-tab
			using (var ms = new MemoryStream())
			{
				using (var bitmap = Convert(_imageBits))
				{
					bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
					var uri = DataUriPngPrefix + System.Convert.ToBase64String(ms.GetBuffer());
					_imageBits_dataURI = uri;
				}
			}
		}

		//Method to generate random captcha text
        private static string GetRandomText()
        {
            string text = "";													//define empty string
            const string chars = "?abcefgijknopqrsvxyz3478";					//use string with chars, allowed in captcha. chars.Length = 24
            for (int i = 0; i < 5; i++){										//number of captcha symbols = 5
				text += chars.Substring(Randomizer.Next(0, chars.Length), 1);		//generate this 5 symbols
			}
            return text;														//return captcha-text
        }

		//Convert many-colors-bitmap to compressed b/w image as bytearray where pixels are encoded as bits:
        public static byte[] Convert(Bitmap bmp)
        {
            var bytes = new byte[bmp.Width * bmp.Height / 8];
            int bii = 0;
            int byi = 0;
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    var pix = bmp.GetPixel(x, y);
                    if (pix.R < 128)	//it so fast!
                    {
                        bytes[byi] |= (byte)(1 << bii);
                    }
                    bii += 1;
                    if (bii >= 8)
                    {   
                        bii = 0;
                        byi += 1;
                    }
                }
            }
            return bytes;
        }
		
		//Convert the bytearray with bits-pixels of compressed b/w image, convert it back -> to b/w bitmap
        public static Bitmap Convert(byte[] bytes, int width = 50, int height = 20)
        {
            var bmp = new Bitmap(width, height);
            int bii = 0;
            int byi = 0;
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    var color = Color.White;
                    if ((bytes[byi] & (byte)(1 << bii)) != 0)
                    {
                        color = Color.Black;
                    }
                    bii += 1;
                    if (bii >= 8)
                    {   
                        bii = 0;
                        byi += 1;
                    }
                    bmp.SetPixel(x, y, color);
                }
            }
            return bmp;
        }

		//return captcha image from specified/generated text, and set not compressed image, and compressed image-bits as bytearray Captcha_generator.imageBits
        private static byte[] CreateCaptcha(string text)
        {
            byte[] byteArray = null;
            Font[] fonts = { 
				//old code
//				new Font("Arial", 24, FontStyle.Bold), 
//				new Font("Courier New", 22, FontStyle.Bold), 
//				new Font("Calibri", 20, FontStyle.Bold),
//				new Font("Tahoma", 24, FontStyle.Italic | FontStyle.Bold)

				//fix font-size for image 50x20
				new Font("Arial", 8+Randomizer.Next(6), FontStyle.Bold), 
				new Font("Courier New", 8+Randomizer.Next(6), FontStyle.Bold), 
				new Font("Calibri", 8+Randomizer.Next(6), FontStyle.Bold),
				new Font("Tahoma", 8+Randomizer.Next(6), FontStyle.Italic | FontStyle.Bold)
				//and here, can be added many-many another fonts
			};
			int rotate = 0;	//degress of rotation, after generate each symbol
			bool rotated = (Randomizer.Next(2) == 1);	//random true/false
            using (var bmp = new Bitmap(50, 20))	//for image 50x20 (1000 pixels)
            {
                using (var graphic = Graphics.FromImage(bmp))	//write captcha-image
                {
					using (var hb = new HatchBrush(HatchStyle.DarkUpwardDiagonal, Color.White, Color.White)) graphic.FillRectangle(hb, 0, 0, bmp.Width, bmp.Height); //background is white
					for (int i = 0; i < text.Length; i++)												//for each symbol in captcha-text
                    {
                        var point = new PointF((i * 9)+Randomizer.Next(1)-Randomizer.Next(2), 10);		//select point to draw symbol
						rotate = Randomizer.Next(4);													//generate random degress of rotation
                        graphic.RotateTransform((rotated == true) ? rotate : 0-rotate);					//rotate or rotate-back
						rotated = !rotated;																//change rotation bool true/false/true/false
                        graphic.DrawString(																//and draw the current symbol
							text.Substring(i, 1),															//take this symbol
							fonts[Randomizer.Next(0, fonts.Length)],										//select random font
							Brushes.Black,																	//write this as black
							point,																			//in selected point
							new StringFormat { LineAlignment = StringAlignment.Center }						//write this symbol as string
						);
                    }
                }
                using (var stream = new MemoryStream()) {	//after generate many-colors image with captcha
					imageBits = Convert(bmp);					//make the compressed image from that bmp, and save it in static field Captcha.ImageBits, as bytearray
                    bmp.Save(stream, ImageFormat.Png);			//save old bmp in MemoryStream
                    byteArray = stream.ToArray();				//and return this as bytearray
                }
            }
            foreach (var font in fonts) font.Dispose();		// Cleanup Fonts (they are disposable)
            return byteArray;	//and return not compressed image, as bytearray
        }
		
		//	Generate new object captcha.Captcha with publicKey, and encryptedSeed, inside. (captcha.Captcha is public class, to do this)
		public static captcha.Captcha GenerateNewCaptcha(
				Captcha_generator captcha_image = null									//already generated Captcha_Generator-object
			,	System.Security.Cryptography.RNGCryptoServiceProvider rand = null		//new System.Security.Cryptography.RNGCryptoServiceProvider(); to get random bytes with crypto-strength.
			,	Random Randomizer = null												//Random Randomizer = 	new Random(seed); //if Captcha_Generator-object not been generated.
			,	bool verify_captcha_answer_before_writting = true						//this can take some time.
		){
			//Add publicKey, and EncryptedSeed (encrypted with captcha-text), to Captcha_generator-object, verify this, and return Captcha-object.
			
			
			if(rand == null){	//if rand not specified
				rand = new System.Security.Cryptography.RNGCryptoServiceProvider();		//initialize new object, to take crypto-strength random there.
			}
			byte[]	ed25519_seed = new byte[32];	//define the ed25519_seed bytearray with length 32 bytes to store randomly generated Ed25519-seed.
			
			if(captcha_image == null){	//if captcha_image not specified, and not been generated
				if(Randomizer == null){	//and if randomizer not specified
					Randomizer = new Random((int)((long)DateTime.Now.Ticks % (long)(int.MaxValue)));	//create this
					//variants to take nanoseconds, as seed:
					//Console.WriteLine((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds * 100);
					//Console.WriteLine(DateTime.Now.Ticks);
					//Console.WriteLine(DateTimeOffset.Now.ToUnixTimeMilliseconds());
					//(long)(int.MaxValue)
				}
				captcha_image = new Captcha_generator(Randomizer);	//or/and generate new Captcha_generator-object with that Randomizer.
			}
			
			byte[] publicKey = new byte[32];													//define 32-bytes bytearray, to extract public key, as 32 last bytes.
			byte[] encrypted_ed25519_seed = new byte[32];										//define 32-bytes bytearray, to save an encrypted ed25519-seed.
			
			int fail_check = 0;				//define this 0, as start attempt to check captcha-answer
			int max_checks = 100;			//define this to prevent invinity loop in do-while cycle.
			
			//	Start try to generate the valid captcha
			do{
				//		Generate random publicKey, Seed, and EncryptedSeed
				rand.GetBytes(ed25519_seed);																//generate ed25519_seed randomly
				byte[] ed25519_seed_and_publicKey = Ed25519.ExpandedPrivateKeyFromSeed(ed25519_seed);		//compute ed25519 privateKey = ed25519_seed + publicKey
				System.Buffer.BlockCopy(	ed25519_seed_and_publicKey, 				32, publicKey, 	0, 			32		);		//do extract the publicKey
				
				//Encrypt seed by captcha answer
				encrypted_ed25519_seed = ByteEncryptionUtil.WrappedXor(ed25519_seed, 			(captcha_image.Text) 			+ publicKey.Stringify());	//encrypt ed25519_seed.
				
				//build the block with one captcha (32 bytes (publicKey) + 32 bytes (EncryptedSeed) + 1000 bits (Captcha black/white image))
				byte[] captcha_block = new byte[	32	+	32	+	125	];
				
				System.Buffer.BlockCopy(	publicKey, 								0, captcha_block, 	0, 			32		);	//write there the pub
				System.Buffer.BlockCopy(	encrypted_ed25519_seed, 				0, captcha_block, 	32, 		32		);	//encrypted seed
				System.Buffer.BlockCopy(	captcha_image._imageBits, 				0, captcha_block, 	32 + 32, 	125		);	//and 1000 bits of captcha image.
				
				if(verify_captcha_answer_before_writting == true)										//	If, need to test captcha_answer:
				{																						//		do this verification
					byte[] decrypted_ed25519_seed 				=	new byte[32];						//define bytearray to save decrypted_ed25519_seed
					byte[] test_ed25519_seed_and_publicKey		= 	new byte[64];						//define bytearray to save privkey (seed and pubkey)	- 64 bytes
					byte[] test_new_pubkey 						= 	new byte[32];						//define bytearray extract pubkey						- 32 bytes
					
					//decrypt this, just for test, with existing captcha-answer
					decrypted_ed25519_seed = ByteEncryptionUtil.WrappedXor(encrypted_ed25519_seed, 	(captcha_image.Text) 			+ publicKey.Stringify());
					
					//	extract privkey and pubkey from decrypted seed (ed25519-privkey, 64 bytes), as first 32 bytes.
					test_ed25519_seed_and_publicKey = Ed25519.ExpandedPrivateKeyFromSeed(decrypted_ed25519_seed);
					System.Buffer.BlockCopy(	test_ed25519_seed_and_publicKey, 	32, test_new_pubkey, 	0, 	32	);		//do extract the public key
					//	or do this by one command:
					//Ed25519.KeyPairFromSeed(out test_new_pubkey, out test_ed25519_seed_and_publicKey, ByteEncryptionUtil.WrappedXor(encrypted_ed25519_seed, (captcha_image.Text) + publicKey.Stringify()));
					
					if(
							Program.CompareTwoArrays(decrypted_ed25519_seed, ed25519_seed)	//compare decrypted seed with original ed25519_seed
						&&	Program.CompareTwoArrays(test_new_pubkey, publicKey)			//compare pubkey with previous extracted pubkey.
					){						//and if both ok
						break;				//break from cycle.
					}
					else{	//else if both not ok
						fail_check++;																	//add 1 to this value
						Console.WriteLine("Captcha.cs. GenerateNewCaptcha. fail_check: "+fail_check);	//show this.
						//prevent infinity loop in do-while cycle.
						if( fail_check > max_checks ){													//if too many fail_checks
							Console.WriteLine("Something wrong with captcha-generation.");					//show this
							break;																			//break from cycle, and return invalid captcha, then.
						}else{																			//else
							continue;																		//try to generate captcha, and verify aswer, for this, again.
						}
					}
				}
			}
			while(verify_captcha_answer_before_writting == true);										//	do this all, while valid captcha not generated.
			
			return new captcha.Captcha(publicKey, encrypted_ed25519_seed, captcha_image._imageBits);	//	And after all return captcha.Captcha-object.
		}
    }//end class Captcha_generator
	
	public class Program{
		//make this Randomizer more crypto-strength, and define this once
		private static Random Randomizer = 	new Random(
														(
															(int)
															(
																(long)
																(
																	(
																		(long)
																		(DateTime.Now - new DateTime(1970, 1, 1))
																		.TotalMilliseconds
																	)
																	+
																	(
																		(long)
																		(DateTime.Now.Ticks)
																	)
																)
																%
																(long)
																(int.MaxValue)
															)
														)
											)
		;	//end define the Randomizer
	//Define the another public variables, about captcha:
			//189 bytes in block in offsets (captcha_index * 189); 189 = 32 + 32 + 125 (1000 bits for pixels of b/w image 20x50)
			public static string 	@captchaPackFilename 			= "captcha_pack.nbc"	;				//captcha-pack-file
			public static int 		CaptchaBlockLength 				= 189					;				//bytelength of one captcha-block: 32 bytes + 32 bytes + 125 bytes (1000 bits);
		
			//there is possible to logging anwsers, because solved once captcha can be used again, by index.
			public static string 	@captchas_answers_file 			= "answers.bin"			;				//5 bytes with symbols in block in offsets (captcha_index * 5)
			public static int 		CaptchaAnswerLength 			= 5						;				//bytelength of one captcha-answer: 5 symbols = 5 bytes;
			//show dot in console, after each "one_dot" iterations.
			public static int		one_dot							= 1024;														//	show one dot, when this number of captchas was been generated.
			public static int		block_length_to_write			= 16384;													//	block length to write
			public static byte[]	captchas_block_to_write			= new byte[block_length_to_write*CaptchaBlockLength]	;	//	write captchas by blocks 16384 captchas, to minimize the actions of writes.
			public static byte[]	captcha_answers_block_to_write	= new byte[block_length_to_write*CaptchaAnswerLength]	;	//	write captcha-anwers by blocks 16384 answers, to minimize the actions of writes.
			public static bool		save_the_captcha_answers 		= 	false;		//if need to write the generated anwsers (optionally value, and disabled, false, by default).
			
			
	//method to generate captcha-pack-file
		public static void generate_captcha_pack_file(
				bool save_captcha_images_as_files = false
			,	bool verify_captcha_answer_before_writting = true
			,	bool logging = false
			,	int set_number_of_captchas_for_one_dot = 1024
			,	int set_block_length_to_write = 16384
		){
			if ( ( save_captcha_images_as_files == true ) && ( !Directory.Exists("captchas") ) ){Directory.CreateDirectory("captchas");}	//create directory "catpchas", if not exists.
		try{
			one_dot 				= set_number_of_captchas_for_one_dot;
			block_length_to_write 	= set_block_length_to_write;
			
            byte[]	ed25519_seed = new byte[32];								//define the ed25519_seed array with length 32 bytes to store randomly generated Ed25519-seed.
			System.Security.Cryptography.RNGCryptoServiceProvider rand = new System.Security.Cryptography.RNGCryptoServiceProvider();		//initialize object once, to take crypto-strength random there on each iteration.
			Console.WriteLine("Starting to generate captchas...");
			Console.WriteLine("save_captcha_images_as_files = "+save_captcha_images_as_files+", verify_captcha_answer_before_writting = "+verify_captcha_answer_before_writting+", logging = "+logging+", set_number_of_captchas_for_one_dot = "+set_number_of_captchas_for_one_dot+", set_block_length_to_write = "+set_block_length_to_write);			
			Console.WriteLine("Each dot means already was been generated "+one_dot+" captchas,\nand this is contains in bytearray-buffer with size: "+block_length_to_write);
			//Start to generate 1024*1024 different captchas:
			for(int i = 0; i<1048576; i++){
				//create new captcha with public static Randomizer, which was been already defined once. Don't create this Randomizer again and again for each object.	
				Captcha_generator NewCaptcha = new Captcha_generator(Randomizer);

				//	generate in the cycle the many bitmaps 1000 bits = 1000 pixels, black or white, 20x50 with captchas, and save this with answer filename.
				captcha.Captcha NewCaptchaFull = Captcha_generator.GenerateNewCaptcha(NewCaptcha, rand, null, verify_captcha_answer_before_writting);

				if	(	save_captcha_images_as_files 	== 	true	){		//If need to save as files
					save_captchas_as_images(i, NewCaptcha); 				//	Save image in file, and replace "?" to "_"
				}
	
				byte[] captcha_block = new byte[	32	+	32	+	125	];			//build the block with one captcha (32 bytes + 32 bytes + 1000 bits)
	
				System.Buffer.BlockCopy(	NewCaptchaFull._publicKey_public,		0, captcha_block, 	0, 			32		);	//write there the pub
				System.Buffer.BlockCopy(	NewCaptchaFull._encryptedSeed_public,	0, captcha_block, 	32, 		32		);	//encrypted seed
				System.Buffer.BlockCopy(	NewCaptchaFull._imageBits_public, 		0, captcha_block, 	32 + 32, 	125		);	//and 1000 bits of captcha image.

				//Write captcha in largest block
				System.Buffer.BlockCopy(
					captcha_block,											//the current captcha block
					0,														//from 0 offset
					captchas_block_to_write,								//write here
					(i%block_length_to_write) * CaptchaBlockLength,			//in this offset
					CaptchaBlockLength										//all the current byte-block
				);
																			//and...
				System.Buffer.BlockCopy(
					System.Text.Encoding.UTF8.GetBytes(NewCaptcha.Text),	//the captcha answer, as bytes
					0,														//from 0 offset
					captcha_answers_block_to_write,							//write here
					(i%block_length_to_write) * CaptchaAnswerLength,		//in this offset
					CaptchaAnswerLength										//all the current byte-block
				);

				show_progress_and_write_data(i);							//then try to write block in data or/and show progress
			}		//end captcha-generation cycle
		}
		catch(Exception ex){
			Console.WriteLine(ex);
		}
		
		}		//end generate_captcha_pack_file method
		
		public static bool check_captcha_answer_for_index(int index, string captcha_answer){
			byte[] publicKey 		= 	new byte[32];
			byte[] encryptedSeed	=	new byte[32];
//			byte[] image 			=	new byte[125];
			System.Buffer.BlockCopy(captchas_block_to_write, 	(index%block_length_to_write * CaptchaBlockLength), 			publicKey,		0,		32	);	//get public key		from block
            System.Buffer.BlockCopy(captchas_block_to_write, 	(index%block_length_to_write * CaptchaBlockLength) + 32, 		encryptedSeed, 	0,		32	);	//get encrypted_seed	from block
//			System.Buffer.BlockCopy(captchas_block_to_write, 	(index%block_length_to_write * CaptchaBlockLength) + 32 + 32,	image,			0,		125	);	//get bits of compressed captcha-image
			byte[]	new_pubkey 	= 	new byte[32];
			byte[]	new_privkey = 	new byte[32];
			Ed25519.KeyPairFromSeed(out new_pubkey, out new_privkey, ByteEncryptionUtil.WrappedXor(encryptedSeed, captcha_answer + publicKey.Stringify()));
			if(CompareTwoArrays(new_pubkey, publicKey)){
				show_progress_and_write_data(index);								//write blocks in file or show progress.
				//Console.WriteLine("Captcha_generator index = "+index+" is solved!");
				return true;
			}else{
				return false;
			}
		}
		
		//	Save image into file, and put the answer in filename. Replace '?' to '_', because '?' not allowed in filename.			
		public static void save_captchas_as_images(int i, Captcha_generator captcha_image, bool bw = false){
				using(
					Bitmap bmp = (
									(bw == true)
										?//save captcha image as compressed black-white-only PNG, which was been encoded by 1000 bits only.
											Captcha_generator.Convert(captcha_image._imageBits)
										:// or take image of this captcha, as not compressed image-bytearray, and save it as PNG (many colors there)
											(Bitmap)Bitmap.FromStream(new MemoryStream(captcha_image.ImageAsByteArray))
					)
				)
				{
					string current_captcha_filename = (	i.ToString("D6")	+	"="	+	(captcha_image.Text).Replace('?', '_')	+	".png"	);
					bmp.Save("captchas\\"+@current_captcha_filename, ImageFormat.Png);  // Or Png
				}
		}

		public static void show_progress_and_write_data(int index){
			if(index%one_dot == 0 && index!=0){
				Console.Write(".");
			}
			if(index%block_length_to_write == 0 && index!=0){
				Console.Write("\n"+index+" captchas generated, and writted in the files.\n");
				FileUtil.Append(captchaPackFilename, captchas_block_to_write);				//append block in the end of captcha_pack-file
				if(save_the_captcha_answers == true){	FileUtil.Append(captchas_answers_file, captcha_answers_block_to_write);	}	//just append the answer to solve this capthca in text-file.
				captchas_block_to_write				= new byte[block_length_to_write*CaptchaBlockLength];
				captcha_answers_block_to_write		= new byte[block_length_to_write*CaptchaAnswerLength];
			}
		}
		
		//Just simply method to compare bytearrays and to test it
		public static bool CompareTwoArrays(byte[] array1, byte[] array2)
		{
			return !array1.Where((t, i) => t != array2[i]).Any();
		}
		
		//	start Program to generate captchas...
		public static void Main(string[] args){
			bool save_captcha_images_as_files 				=	false;
			bool verify_captcha_answer_before_writting 		=	true;
			bool logging 									=	false;
			int set_one_dot 								=	1024;
			int set_block_size 								=	16384;
			
			if(args.Length>=1){		if (bool.TryParse(args[0]		, out save_captcha_images_as_files))			{}		}
			if(args.Length>=2){		if (bool.TryParse(args[1]		, out verify_captcha_answer_before_writting))	{}		}
			if(args.Length>=3){		if (bool.TryParse(args[2]		, out logging))									{}		}
			if(args.Length>=4){		if (int.TryParse(args[3]		, out set_one_dot))								{set_one_dot 		= (args[3]).parse_number();}		} //sometimes int.Parse, and Int32.Parse working incorrect
			if(args.Length>=5){		if (Int32.TryParse(args[4]		, out set_block_size))							{set_block_size 	= (args[4]).parse_number();}		} //so with parse_number this working good.
			
			generate_captcha_pack_file(save_captcha_images_as_files, verify_captcha_answer_before_writting, logging, set_one_dot, set_block_size);
		}//end Main

	}//end class Program

}//end CaptchaPack_Generator namespace
	//-----Captcha_Pack_Generator.cs-----
