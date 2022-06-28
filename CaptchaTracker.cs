using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Linq;
//using NDB;
using captcha;

namespace NServer
{
    class CaptchaTracker
    {
		//static captchas for posts (depends from value of hash of post)
        public static Dictionary<string, captcha.Captcha>		Captchas				=	new Dictionary<string, captcha.Captcha>();
//        public static Dictionary<string, string>				Posts					=	new Dictionary<string, string>();

		//random captchas for uploading PNG's in "/download"-folder, and maybe, another actions.
        public static Dictionary<string, captcha.Captcha>		RandomCaptchas					=	new Dictionary<string, captcha.Captcha>();	//Rsndom captcha by guid
		public static Dictionary<string, long>					TimeOfGenerationRandomCaptcha	=	new Dictionary<string, long>();				//DateTime of generated captchas, to remove old captchas. See Captcha.ClearCaptchasIfNeed
    }
}
