using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Auth0.SDK
{
	public class Auth0User
	{
		public Auth0User()
		{
		}

		public Auth0User(IDictionary<string, string> accountProperties)
		{
			this.Auth0AccessToken = accountProperties ["access_token"];
			this.IdToken = accountProperties ["id_token"];
			this.Profile = accountProperties ["id_token"].Split ('.') [1].ToJson ();
		}

		public string Auth0AccessToken { get; set; }

		public string IdToken { get; set; }

		public JObject Profile { get; set; }
	}

	internal static class Extensions
	{
		internal static JObject ToJson(this string jsonString)
		{
			var decoded = Encoding.Default.GetString(jsonString.Base64UrlDecode());
			return JObject.Parse(decoded);
		}

		internal static byte[] Base64UrlDecode(this string input)
		{
			var output = input;
			output = output.Replace('-', '+'); 	// 62nd char of encoding
			output = output.Replace('_', '/'); 	// 63rd char of encoding

			switch (output.Length % 4) 			// Pad with trailing '='s
			{
				case 0: break; 					// No pad chars in this case
				case 2: output += "=="; break; 	// Two pad chars
				case 3: output += "="; break; 	// One pad char
				default: throw new InvalidOperationException("Illegal base64url string!");
			}

			return Convert.FromBase64String(output); // Standard base64 decoder
		}
	}
}
