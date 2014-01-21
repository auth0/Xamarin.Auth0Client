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
			this.Auth0AccessToken = accountProperties.ContainsKey("access_token") ? accountProperties["access_token"] : string.Empty;
			this.IdToken = accountProperties.ContainsKey("id_token") ? accountProperties["id_token"] : string.Empty;
			this.Profile = accountProperties.ContainsKey("profile") ? accountProperties["profile"].ToJson() : null;
		}

		public string Auth0AccessToken { get; set; }

		public string IdToken { get; set; }

		public JObject Profile { get; set; }
	}

	internal static class Extensions
	{
		internal static JObject ToJson(this string jsonString)
		{
			return JObject.Parse(jsonString);
		}
	}
}
