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
		private const string AuthorizeUrl = "https://{0}/authorize?client_id={1}&redirect_uri={2}&response_type=token&connection={3}&scope={4}";
		private const string LoginWidgetUrl = "https://{0}/login/?client={1}&redirect_uri={2}&response_type=token&scope={3}";
		private const string ResourceOwnerEndpoint = "https://{0}/oauth/ro";
		private const string DelegationEndpoint = "https://{0}/delegation";
		private const string UserInfoEndpoint = "https://{0}/userinfo?access_token={1}";
		private const string DefaultCallback = "https://{0}/mobile";

		private readonly string domain;
		private readonly string clientId;

		public Auth0Client (string domain, string clientId)
		{
			this.domain = domain;
			this.clientId = clientId;
		}

		public Auth0User CurrentUser { get; private set; }

		public string CallbackUrl
		{
			get
			{
				return string.Format (DefaultCallback, this.domain);
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
			var endpoint = string.Format (ResourceOwnerEndpoint, this.domain);
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
		/// Get a delegation token.
		/// </summary>
		/// <returns>Delegation token result.</returns>
		/// <param name="targetClientId">Target client ID.</param>
		/// <param name="options">Custom parameters.</param>
		public Task<JObject> GetDelegationToken(string targetClientId, IDictionary<string, string> options = null)
		{
			var id_token = string.Empty;
			options = options ?? new Dictionary<string, string> ();

			// ensure id_token
			if (options.ContainsKey ("id_token")) {
				id_token = options ["id_token"];
				options.Remove ("id_token");
			} else {
				id_token = this.CurrentUser.IdToken;
			}

			if (string.IsNullOrEmpty (id_token)) {
				throw new InvalidOperationException (
					"You need to login first or specify a value for id_token parameter.");
			}

			var endpoint = string.Format (DelegationEndpoint, this.domain);
			var parameters = new Dictionary<string, string> 
			{
				{ "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
				{ "id_token", id_token },
				{ "target", targetClientId },
				{ "client_id", this.clientId }
			};

			// custom parameters
			foreach (var option in options) {
				parameters.Add (option.Key, option.Value);
			}

			var request = new Request ("POST", new Uri(endpoint), parameters);
			return request.GetResponseAsync ().ContinueWith<JObject>(t => 
				{
					try
					{
						var text = t.Result.GetResponseText();
						return JObject.Parse(text);
					}
					catch (Exception)
					{
						throw;
					}
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

		private void SetupCurrentUser(IDictionary<string, string> accountProperties)
		{
			var endpoint = string.Format(UserInfoEndpoint, this.domain, accountProperties["access_token"]);

			var request = new Request ("GET", new Uri(endpoint));
			request.GetResponseAsync ().ContinueWith (t => 
			{
					try
					{
						var text = t.Result.GetResponseText();

						if (t.Result.StatusCode != System.Net.HttpStatusCode.OK)
						{
							throw new InvalidOperationException(text);
						}

						accountProperties.Add("profile", text);
					}
					catch (Exception ex)
					{
						throw ex;
					}
					finally
					{
						this.CurrentUser = new Auth0User(accountProperties);
					}
				}).Wait();
		}

		private WebRedirectAuthenticator GetAuthenticator(string connection, string scope)
		{
			// Generate state to include in startUri
			var chars = new char[16];
			var rand = new Random ();
			for (var i = 0; i < chars.Length; i++) {
				chars [i] = (char)rand.Next ((int)'a', (int)'z' + 1);
			}

			var redirectUri = this.CallbackUrl;
			var authorizeUri = !string.IsNullOrWhiteSpace (connection) ?
           		string.Format(AuthorizeUrl, this.domain, this.clientId, Uri.EscapeDataString(redirectUri), connection, scope) :
               	string.Format(LoginWidgetUrl, this.domain, this.clientId, Uri.EscapeDataString(redirectUri), scope);

			var state = new string (chars);
			var startUri = new Uri (authorizeUri + "&state=" + state);
			var endUri = new Uri (redirectUri);

			var auth = new WebRedirectAuthenticator (startUri, endUri);
			auth.ClearCookiesBeforeLogin = false;

			return auth;
		}
	}
}
