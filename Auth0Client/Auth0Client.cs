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

		private readonly string tenant;
		private readonly string clientId;
		private readonly string connection;

		string state; 
		Uri startUrl;

		/// <summary>
		/// Initializes a new instance of the <see cref="Auth0.SDK.Auth0Client"/> class, instructing it to use a specific connection (e.g. 'google-oauth2', 'amazon', etc)
		/// </summary>
		/// <param name="title">A title to show on the login screen</param>
		/// <param name="tenant">Your tenant in Auth0 (usually {tenant}.auth0.com</param>
		/// <param name="clientId">The Client Id for the application defined in Auth0</param>
		/// <param name="connection">The name of the connection to use in Auth0. Connection defines an Identity Provider</param>
		/// <param name="callbackurl">The redirect_uri used to detect the end of the authentication transaction</param>
		public Auth0Client (string title, string tenant, string clientId, string connection, Uri callbackurl = null) 
			: this( title, string.Format(AuthorizeUrl, tenant, clientId, Uri.EscapeDataString (ValidateCallback(callbackurl, tenant)), connection), ValidateCallback(callbackurl, tenant))
		{
			this.clientId = clientId;
			this.tenant = tenant;
			this.connection = connection;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Auth0.SDK.Auth0Client"/> class, instructing it to show Auth0 Login Widget that will dynamically display all available connections.
		/// </summary>
		/// <param name="title">A title to show on the login screen</param>
		/// <param name="tenant">Your tenant in Auth0 (usually {tenant}.auth0.com</param>
		/// <param name="clientId">The Client Id for the application defined in Auth0</param>
		/// <param name="callbackurl">The redirect_uri used to detect the end of the authentication transaction</param>
		public Auth0Client (string title, string tenant, string clientId, Uri callbackurl = null)
			: this( title, string.Format(LoginWidgetUrl, tenant, clientId, Uri.EscapeDataString (ValidateCallback(callbackurl, tenant))), ValidateCallback(callbackurl, tenant))
		{
			this.clientId = clientId;
			this.tenant = tenant;
		}
	
		private Auth0Client(string title, string startUri, string callback )
			: base( null, new Uri( callback ) )
		{
			this.Title = title;

			var chars = new char[16];
			var rand = new Random ();
			for (var i = 0; i < chars.Length; i++) {
				chars [i] = (char)rand.Next ((int)'a', (int)'z' + 1);
			}

			this.state = new string (chars);

			this.startUrl = new Uri( startUri + "&state=" + this.state );
		}

		public override Task<Uri> GetInitialUrlAsync ()
		{
			var tcs = new TaskCompletionSource<Uri> ();
			tcs.SetResult (startUrl);
			return tcs.Task;
		}

		/// <summary>
		/// Authenticate user from an specified connection with OAuth2 'Resource Owner Password Credentials Grant'
		/// </summary>
		/// <param name="clientSecret">The Client Secret for the application defined in Auth0</param>
		/// <param name="userName">User name</param>
		/// <param name="password">User password</param>
		public void Authenticate(string clientSecret, string userName, string password)
		{
			var endpoint = string.Format (ResourceOwnerEndpoint, tenant);
			var parameters = new Dictionary<string, string> 
			{
				{ "grant_type", "password" },
				{ "client_id", this.clientId },
				{ "client_secret", clientSecret },
				{ "connection", this.connection },
				{ "username", userName },
				{ "password", password },
				{ "scope", "openid profile" }
			};

			var request = new Request ("POST", new Uri(endpoint), parameters);
			request.GetResponseAsync ().ContinueWith(t => 
			{
				try
				{
					var text = t.Result.GetResponseText();
					var data = JObject.Parse(text).ToObject<Dictionary<string, string>>();

					if (data.ContainsKey ("error")) 
					{
						throw new AuthException ("Error authenticating: " + data["error"]);
					} 
					else if (data.ContainsKey ("access_token"))
					{
						this.OnRetrievedAccountProperties(data);
					} 
					else 
					{
						throw new AuthException ("Expected access_token in access token response, but did not receive one.");
					}
				}
				catch (Exception ex)
				{
					this.OnError(ex);
				}
			});
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

	public static class Extensions
	{
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
