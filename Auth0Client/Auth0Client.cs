using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;

namespace Auth0.SDK
{
	/// <summary>
	/// A simple Client to Authenticate Users with Auth0.
	/// </summary>
	public class Auth0Client : WebRedirectAuthenticator
	{
		static string AuthorizeUrl = "https://{0}.auth0.com/authorize?client_id={1}&scope=openid%20profile&redirect_uri={2}&response_type=token&connection={3}";
		static string LoginWidgetUrl = "https://{0}.auth0.com/login/?client={1}&scope=openid%20profile&redirect_uri={2}&response_type=token";
		static string ResourceOwnerEndpoint = "https://{0}.auth0.com/oauth/ro";
		static string DefaultCallback = "https://{0}.auth0.com/mobile";

		private readonly string subDomain;
		private readonly string clientId;
		private readonly string clientSecret;

		string state; 
		Uri startUrl;

		/// <summary>
		/// Initializes a new instance of the <see cref="Auth0.SDK.Auth0Client"/> class, instructing it to show Auth0 Login Widget that will dynamically display all available connections.
		/// </summary>
		/// <param name="title">A title to show on the login screen</param>
		/// <param name="tenant">Your tenant in Auth0 (usually {tenant}.auth0.com</param>
		/// <param name="clientId">The Client Id for the application defined in Auth0</param>
		/// <param name="callbackurl">The redirect_uri used to detect the end of the authentication transaction</param>
		public Auth0Client (string subDomain, string clientId, Uri callbackurl = null)
			: this(string.Format(LoginWidgetUrl, subDomain, clientId, Uri.EscapeDataString (ValidateCallback(callbackurl, subDomain))), ValidateCallback(callbackurl, subDomain))
		{
			this.subDomain = subDomain;
			this.clientId = clientId;
		}

		public Auth0Client (string subDomain, string clientId, string clientSecret, Uri callbackurl = null)
			: this(string.Format(LoginWidgetUrl, subDomain, clientId, Uri.EscapeDataString (ValidateCallback(callbackurl, subDomain))), ValidateCallback(callbackurl, subDomain))
		{
			this.subDomain = subDomain;
			this.clientId = clientId;
			this.clientSecret = clientSecret;
		}
	
		private Auth0Client(string startUri, string callback )
			: base( null, new Uri( callback ) )
		{
			this.Title = "Login";

			var chars = new char[16];
			var rand = new Random ();
			for (var i = 0; i < chars.Length; i++) {
				chars [i] = (char)rand.Next ((int)'a', (int)'z' + 1);
			}

			this.state = new string (chars);

			this.startUrl = new Uri( startUri + "&state=" + this.state );
		}

		/// <summary>
		/// Logins the async.
		/// </summary>
		/// <param name="title">A title to show on the login screen.</param>
		/// <param name="connection">The name of the connection to use in Auth0. Connection defines an Identity Provider.</param>
		public Task<AuthenticationResult> LoginAsync(string title = "", string connection = "")
		{
			throw new NotImplementedException ();
		}

		/// <summary>
		/// Login using OAuth2 'Resource Owner Password Credentials Grant'.
		/// </summary>
		/// <returns>Authentication result callback.</returns>
		/// <param name="connection">The name of the connection to use in Auth0. Connection defines an Identity Provider.</param>
		/// <param name="userName">User name.</param>
		/// <param name="password">User password.</param>
		public Task<AuthenticationResult> LoginAsync(string connection, string userName, string password)
		{
			var endpoint = string.Format (ResourceOwnerEndpoint, this.subDomain);
			var parameters = new Dictionary<string, string> 
			{
				{ "client_id", this.clientId },
				{ "client_secret", this.clientSecret },
				{ "connection", connection },
				{ "username", userName },
				{ "password", password },
				{ "grant_type", "password" },
				{ "scope", "openid profile" }
			};

			var request = new Request ("POST", new Uri(endpoint), parameters);
			return request.GetResponseAsync ().ContinueWith<AuthenticationResult>(t => 
			{
				var result = new AuthenticationResult(); // TODO: see t.Exception

				try
				{
					var text = t.Result.GetResponseText();
					var data = JObject.Parse(text).ToObject<Dictionary<string, string>>();

					if (data.ContainsKey ("error")) 
					{
						result.Error = new AuthException ("Error authenticating: " + data["error"]);
					} 
					else if (data.ContainsKey ("access_token"))
					{
						result.Success = true;
						result.Auth0AccessToken = data["access_token"];
						result.IdToken = data["id_token"];
						result.User = data["id_token"].ToJson();
					} 
					else 
					{
						result.Error = new AuthException ("Expected access_token in access token response, but did not receive one.");
					}
				}
				catch (AggregateException ex)
				{
					result.Error = ex.Flatten();
				}
				catch (Exception ex)
				{
					result.Error = ex;
				}

				return result;
			});
		}

		public override Task<Uri> GetInitialUrlAsync ()
		{
			var tcs = new TaskCompletionSource<Uri> ();
			tcs.SetResult (startUrl);
			return tcs.Task;
		}

		protected override void OnRedirectPageLoaded (Uri url, IDictionary<string, string> query, IDictionary<string, string> fragment)
		{
			var all = new Dictionary<string, string> (query);
			foreach (var kv in fragment)
				all [kv.Key] = kv.Value;

			//
			// Check for forgeries in the final redirect.
			//
			if (all.ContainsKey ("state")) {
				if (all ["state"] != state ) {
					OnError ("Invalid state from server. Possible forgery!");
					return;
				}
			}

			//
			// Look for the access_token
			//
			if (fragment.ContainsKey ("access_token")) {
				//
				// We found an access_token
				//
				OnRetrievedAccountProperties (fragment);
				return;
			}
		
			OnError ("Expected access_token in response, but did not receive one.");
		}

		protected virtual void OnRetrievedAccountProperties (IDictionary<string, string> accountProperties)
		{
			var account = new Account (string.Empty, accountProperties);
			var profile = account.GetProfile();
			account.Username = (string)profile["name"];

			OnSucceeded(account);
		}

		private static string ValidateCallback(Uri callbackUrl, string tenant)
		{
			return callbackUrl == null ? 
				string.Format(DefaultCallback, tenant) : callbackUrl.AbsoluteUri;
		}
	}

	public class AuthenticationResult
	{
		public bool Success { get; set; }

		public string Auth0AccessToken { get; set; }

		public string IdToken { get; set; }

		public JObject User { get; set; }

		public Exception Error { get; set; }
	}

	public static class Extensions
	{
		public static JObject ToJson(this string jsonString)
		{
			var decoded = Encoding.Default.GetString(jsonString.Base64UrlDecode());
			return JObject.Parse(decoded);
		}

		public static JObject GetProfile(this Account account)
		{
			if (!account.Properties.ContainsKey ("id_token")) 
			{
				return new JObject();
			}

			var jwt = account.Properties["id_token"].Split ('.')[1];
			var decoded = Encoding.Default.GetString(jwt.Base64UrlDecode());

			return JObject.Parse(decoded);
		}

		public static byte[] Base64UrlDecode(this string input)
		{
			var output = input;
			output = output.Replace('-', '+'); 	// 62nd char of encoding
			output = output.Replace('_', '/'); 	// 63rd char of encoding

			switch (output.Length % 4) 			// Pad with trailing '='s
			{
				case 0: break; 					// No pad chars in this case
				case 2: output += "=="; break; 	// Two pad chars
				case 3: output += "="; break; 	// One pad char
				default: throw new System.InvalidOperationException("Illegal base64url string!");
			}

			return Convert.FromBase64String(output); // Standard base64 decoder
		}
	}
}
