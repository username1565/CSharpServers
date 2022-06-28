using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using NDB;
using nboard;

namespace captcha
{

    static class ByteStringExt
    {
        public static string Stringify(this byte[] bytes)
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

        public static byte[] Bytify(this string @string)
        {
			//when database is corrupted and damaged,
			//or when some text contains in the end of the post, after [sign=blahblah]some_text
			string sign_value = @string.Split(']')[0];				//just cut signature [sign=blahblah], and working with signature only.
            var bytes = new byte[sign_value.Length / 2];

            for (int i = 0; i < sign_value.Length / 2; i++)
            {
				try{						//try 
					bytes[i] = byte.Parse(sign_value[i * 2] + "" + sign_value[i * 2 + 1], System.Globalization.NumberStyles.HexNumber);
					//Sometimes System.Byte.Parse return incorrect value, when @string contains some text, after signature.
				}
				catch (Exception ex){		//or return some shit, as bytes.
					Console.WriteLine(ex);
					bytes[i] = 0;			//add null byte, just to skip this step.
				}
            }

            return bytes;
        }

        public static int MaxLeadingZeros(this byte[] bytes)
        {
            int len = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0)
                {
                    len += 1;
                }

                else
                {
                    return len;
                }
            }

            return len;
        }

        public static int MaxConsecZeros(this byte[] bytes, int startFrom = 0, int limit = 0)
        {
            int len = 0;
            int max_len = 0;

            for (int i = startFrom; i < bytes.Length; i++)
            {
                if (bytes[i] <= limit)
                {
                    len += 1;
                }

                else
                {
                    if (max_len < len)
                    {
                        max_len = len;
                    }

                    len = 0;
                }
            }

//			Console.WriteLine("hash: "+BitConverter.ToString(bytes).Replace("-", "")+", limit: "+limit+", Math.Max(max_len, len): "+Math.Max(max_len, len));
            return Math.Max(max_len, len);
        }
    }
}