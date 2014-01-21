using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;

namespace Auth0.SDK
{
	/// <summary>
	/// A simple client to Authenticate Users with Auth0.
	/// </summary>
	public partial class Auth0Client
	{
		private const string AuthorizeUrl = "https://{0}.auth0.com/authorize?client_id={1}&scope=openid%20profile&redirect_uri={2}&response_type=token&connection={3}";
		private const string LoginWidgetUrl = "https://{0}.auth0.com/login/?client={1}&scope=openid%20profile&redirect_uri={2}&response_type=token";
		private const string ResourceOwnerEndpoint = "https://{0}.auth0.com/oauth/ro";
		private const string DefaultCallback = "https://{0}.auth0.com/mobile";

		private readonly string subDomain;
		private readonly string clientId;

		public Auth0Client (string subDomain, string clientId)
		{
			this.subDomain = subDomain;
			this.clientId = clientId;
		}

		public Auth0User CurrentUser { get; private set; }

		public string CallbackUrl
		{
			get
			{
				return string.Format (DefaultCallback, this.subDomain);
			}
		}

		/// <summary>
		///  Log a user into an Auth0 application given an user name and password.
		/// </summary>
		/// <returns>Task that will complete when the user has finished authentication.</returns>
		/// <param name="connection" type="string">The name of the connection to use in Auth0. Connection defines an Identity Provider.</param>
		/// <param name="userName" type="string">User name.</param>
		/// <param name="password type="string"">User password.</param>
		public Task<Auth0User> LoginAsync(string connection, string userName, string password)
		{
			var endpoint = string.Format (ResourceOwnerEndpoint, this.subDomain);
			var parameters = new Dictionary<string, string> 
			{
				{ "client_id", this.clientId },
				{ "connection", connection },
				{ "username", userName },
				{ "password", password },
				{ "grant_type", "password" },
				{ "scope", "openid profile" }
			};

			var request = new Request ("POST", new Uri(endpoint), parameters);
			return request.GetResponseAsync ().ContinueWith<Auth0User>(t => 
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
						this.SetupCurrentUser (data);
					} 
					else 
					{
						throw new AuthException ("Expected access_token in access token response, but did not receive one.");
					}
				}
				catch (Exception ex)
				{
					throw ex;
				}

				return this.CurrentUser;
			});
		}

		/// <summary>
		/// Log a user out of a Auth0 application.
		/// </summary>
		public void Logout()
		{
			this.CurrentUser = null;
			WebAuthenticator.ClearCookies();
		}

		private void SetupCurrentUser (IDictionary<string, string> accountProperties)
		{
			this.CurrentUser = new Auth0User (accountProperties);
		}

		private WebRedirectAuthenticator GetAuthenticator(string connection)
		{
			// Generate state to include in startUri
			var chars = new char[16];
			var rand = new Random ();
			for (var i = 0; i < chars.Length; i++) {
				chars [i] = (char)rand.Next ((int)'a', (int)'z' + 1);
			}

			var redirectUri = this.CallbackUrl;
			var authorizeUri = !string.IsNullOrWhiteSpace (connection) ?
				string.Format (AuthorizeUrl, subDomain, clientId, Uri.EscapeDataString (redirectUri), connection) :
				string.Format (LoginWidgetUrl, subDomain, clientId, Uri.EscapeDataString (redirectUri));

			var state = new string (chars);
			var startUri = new Uri (authorizeUri + "&state=" + state);
			var endUri = new Uri (redirectUri);

			var auth = new WebRedirectAuthenticator (startUri, endUri);
			auth.ClearCookiesBeforeLogin = false;

			return auth;
		}
	}
}
