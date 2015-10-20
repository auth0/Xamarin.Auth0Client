using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;
using System.Security.Policy;

namespace Auth0.SDK
{
	/// <summary>
	/// A simple client to Authenticate Users with Auth0.
	/// </summary>
	public partial class Auth0Client
	{
		protected string Domain { get; set; }
		protected string ClientId { get; set; }

		public Auth0Client (string domain, string clientId)
		{
			this.Domain = domain;
			this.ClientId = clientId;
			this.DeviceIdProvider = new DeviceIdProvider();
		}

		public Auth0User CurrentUser { get; protected set; }

		/// <summary>
		/// The component used to generate the device's unique id
		/// </summary>
		public IDeviceIdProvider DeviceIdProvider { get; set; }

		public string CallbackUrl
		{
			get
			{
                return string.Format(Auth0Constants.DefaultCallback, this.Domain);
			}
		}

		/// <summary>
		///  Log a user into an Auth0 application given an user name and password.
		/// </summary>
		/// <returns>Task that will complete when the user has finished authentication.</returns>
		/// <param name="connection" type="string">The name of the connection to use in Auth0. Connection defines an Identity Provider.</param>
		/// <param name="userName" type="string">User name.</param>
		/// <param name="password type="string"">User password.</param>
		public async Task<Auth0User> LoginAsync(string connection, 
			string userName, 
			string password, 
			bool withRefreshToken = false,
			string scope = "openid")
		{

            var endpoint = string.Format(Auth0Constants.ResourceOwnerEndpoint, this.Domain);
			var scopeParameter = IncreaseScopeWithOfflineAccess (withRefreshToken, scope);
			var parameters = new Dictionary<string, string> 
			{
				{ "client_id", this.ClientId },
				{ "connection", connection },
				{ "username", userName },
				{ "password", password },
				{ "grant_type", "password" },
				{ "scope",  scopeParameter }
			};

			if (ScopeHasOfflineAccess (scopeParameter)) {
				var deviceId = this.DeviceIdProvider.GetDeviceId ().Result;
				parameters ["device"] = deviceId;
			}
				
			var request = new Request ("POST", new Uri(endpoint), parameters);
			var response = await request.GetResponseAsync();

			try
			{
				var text = response.GetResponseText();
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
		}

		/// <summary>
		/// Verifies if the jwt for the current user has expired.
		/// </summary>
		/// <returns>true if the token has expired, false otherwise.</returns>
		/// <remarks>Must be logged in before invoking.</remarks>
		public bool HasTokenExpired()
		{
			if (string.IsNullOrEmpty(this.CurrentUser.IdToken))
			{
				throw new InvalidOperationException("You need to login first.");
			}

			return TokenValidator.HasExpired(this.CurrentUser.IdToken);
		}

		/// <summary>
		/// Renews the idToken (JWT)
		/// </summary>
		/// <returns>The refreshed token.</returns>
		/// <remarks>The JWT must not have expired.</remarks>
		/// <param name="options">Additional parameters.</param>
		public Task<JObject> RenewIdToken(Dictionary<string, string> options = null)
		{
			if (string.IsNullOrEmpty(this.CurrentUser.IdToken))
			{
				throw new InvalidOperationException("You need to login first.");
			}

			options = options ?? new Dictionary<string, string>();

			if (!options.ContainsKey("scope"))
			{
				options["scope"] = "passthrough";
			}

			return this.GetDelegationToken(
				api: "app", 
				idToken: this.CurrentUser.IdToken, 
				options: options);
		}

        /// <summary>
        /// Renews the idToken (JWT)
        /// </summary>
        /// <returns>The refreshed token.</returns>
        /// <param name="refreshToken" type="string">The refresh token to use. If null, the logged in users token will be used.</param>
        /// <param name="options">Additional parameters.</param>
        public async Task<JObject> RefreshToken(
            string refreshToken = "",
            Dictionary<string, string> options = null)
        {
            var emptyToken = string.IsNullOrEmpty(refreshToken);
            if (emptyToken 
				&& (this.CurrentUser == null 
					|| string.IsNullOrEmpty(this.CurrentUser.RefreshToken)))
            {
                throw new InvalidOperationException(
                    "The current user's refresh token could not be retrieved or no refresh token was provided as parameter.");
            }

            return await this.GetDelegationToken(
				api: "app",
				refreshToken: emptyToken ? this.CurrentUser.RefreshToken : refreshToken,
                options: options);
        }

		/// <summary>
		/// Get a delegation token
		/// </summary>
		/// <returns>Delegation token result.</returns>
		/// <param name="api">The type of the API to be used.</param>
		/// <param name="idToken">The string representing the JWT. Useful only if not expired.</param>
		/// <param name="refreshToken">The refresh token.</param>
		/// <param name="targetClientId">The clientId of the target application for which to obtain a delegation token.</param>
		/// <param name="options">Additional parameters.</param>
		public async Task<JObject> GetDelegationToken(
			string api = "",
			string idToken = "",
			string refreshToken = "",
			string targetClientId = "",
			Dictionary<string, string> options = null)
		{
			if (!(string.IsNullOrEmpty(idToken) || string.IsNullOrEmpty(refreshToken)))
			{
				throw new InvalidOperationException(
					"You must provide either the idToken parameter or the refreshToken parameter, not both.");
			}

			if (string.IsNullOrEmpty(idToken) && string.IsNullOrEmpty(refreshToken))
			{
				if (this.CurrentUser == null || string.IsNullOrEmpty(this.CurrentUser.IdToken)){
					throw new InvalidOperationException(
						"You need to login first or specify a value for idToken or refreshToken parameter.");
				}

				idToken = this.CurrentUser.IdToken;
			}

			options = options ?? new Dictionary<string, string>();
			options["id_token"] = idToken;
			options["api_type"] = api;
			options["refresh_token"] = refreshToken;
			options["target"] = targetClientId;
			options["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer";
			options ["client_id"] = this.ClientId;

            var endpoint = string.Format(Auth0Constants.DelegationEndpoint, this.Domain);

			options = options
				.Where (kvp => !string.IsNullOrEmpty (kvp.Value))
				.ToDictionary (kvp => kvp.Key, kvp => kvp.Value);
				
			var request = new Request ("POST", new Uri(endpoint), options);
			var result = await request.GetResponseAsync ();

			try
			{
				var text = result.GetResponseText();
				var data = JObject.Parse(text);
				JToken temp = null;

				if(data.TryGetValue("id_token", out temp))
				{
					var jwt = temp.Value<string>();

					this.CurrentUser = this.CurrentUser 
						?? new Auth0User() { RefreshToken = refreshToken };
					this.CurrentUser.IdToken = jwt;
				}

				return data;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Log a user out of a Auth0 application.
		/// </summary>
		public void Logout()
		{
			this.CurrentUser = null;
			WebAuthenticator.ClearCookies();
		}

		/// <summary>
		/// Gets the WebRedirectAuthenticator.
		/// </summary>
		/// <returns>The authenticator.</returns>
		/// <param name="connection">Connection name.</param>
		/// <param name="scope">OpenID scope.</param>
		/// <param name="deviceName">The device name to use if gettting a refresh token.</param>
        /// <param name="title">Title displayed by WebRedirectAuthenticator, by default is empty.</param>
        protected virtual async Task<WebRedirectAuthenticator> GetAuthenticator(string connection, string scope, string title = null)
		{
			// Generate state to include in startUri
			var chars = new char[16];
			var rand = new Random ();
			for (var i = 0; i < chars.Length; i++) {
				chars [i] = (char)rand.Next ((int)'a', (int)'z' + 1);
			}

			var redirectUri = this.CallbackUrl;
			var authorizeUri = !string.IsNullOrWhiteSpace (connection) ?
                string.Format(Auth0Constants.AuthorizeUrl, this.Domain, this.ClientId, Uri.EscapeDataString(redirectUri), connection, scope) :
                string.Format(Auth0Constants.LoginWidgetUrl, this.Domain, this.ClientId, Uri.EscapeDataString(redirectUri), scope);

			if (ScopeHasOfflineAccess("offline_access"))
			{
				var deviceId = Uri.EscapeDataString(await this.DeviceIdProvider.GetDeviceId());
				authorizeUri += string.Format("&device={0}", deviceId);
			}

			var state = new string (chars);
			var startUri = new Uri (authorizeUri + "&state=" + state);
			var endUri = new Uri (redirectUri);

			var auth = new WebRedirectAuthenticator (startUri, endUri);
			auth.ClearCookiesBeforeLogin = false;
            auth.Title = title;
			return auth;
		}

		private static string IncreaseScopeWithOfflineAccess(bool withRefreshToken, string scope)
		{
			if (withRefreshToken && !ScopeHasOfflineAccess(scope))
			{
				scope += " offline_access";
			}

			return scope;
		}

		private static bool ScopeHasOfflineAccess(string scope)
		{
			return scope
				.Split(new string[0], StringSplitOptions.RemoveEmptyEntries)
				.Any(e => e.Equals("offline_access", StringComparison.InvariantCultureIgnoreCase));
		}

		protected async void SetupCurrentUser(IDictionary<string, string> accountProperties)
		{
            var endpoint = string.Format(Auth0Constants.UserInfoEndpoint, this.Domain, accountProperties["access_token"]);

			var request = new Request ("GET", new Uri(endpoint));
			var response = await request.GetResponseAsync();

			try
			{
				var text = response.GetResponseText();

				if (response.StatusCode != System.Net.HttpStatusCode.OK)
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
		}
	}
}
