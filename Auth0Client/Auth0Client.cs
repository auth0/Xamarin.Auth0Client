using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoTouch.UIKit;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;

namespace Auth0.SDK
{
	/// <summary>
	/// A simple client to Authenticate Users with Auth0.
	/// </summary>
	public class Auth0Client
	{
		private static string ResourceOwnerEndpoint = "https://{0}.auth0.com/oauth/ro";

		private readonly string subDomain;
		private readonly string clientId;
		private readonly string clientSecret;

		public Auth0Client (string subDomain, string clientId, string clientSecret)
		{
			this.subDomain = subDomain;
			this.clientId = clientId;
			this.clientSecret = clientSecret;
		}

		/// <summary>
		/// Login with OAuth2 'Implicit Granting'
		/// </summary>
		/// <returns>Authentication result callback.</returns>
		/// <param name="title">A title to show on the login screen.</param>
		/// <param name="connection">The name of the connection to use in Auth0. Connection defines an Identity Provider.</param>
		/// <param name="callbackUrl">The redirect_uri used to detect the end of the authentication transaction</param>
		public void LoginAsync(UIViewController controller, Action<AuthenticationResult> onComplete, string title = "", string connection = "", Uri callbackUrl = null)
		{
			var result = new AuthenticationResult();
			var authenticator = new Auth0Authenticator (
				title,
				this.subDomain,
				this.clientId,
				connection,
				callbackUrl);

			authenticator.Completed += (object sender, AuthenticatorCompletedEventArgs e) => {
				if (e.IsAuthenticated) {
					result.Success = true;
					result.Auth0AccessToken = e.Account.Properties["access_token"];
					result.IdToken = e.Account.Properties["id_token"];
					result.User = e.Account.Properties["id_token"].Split ('.')[1].ToJson();
				}

				onComplete(result);
			};

			authenticator.Error += (object sender, AuthenticatorErrorEventArgs e) => {
				result.Error = e.Exception;

				onComplete(result);
			};

			// Present the login UI
			controller.PresentViewController (authenticator.GetUI (), true, null);
		}

		/// <summary>
		/// Login with OAuth2 'Resource Owner Password Credentials Grant'.
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
				var result = new AuthenticationResult();

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
						result.User = data["id_token"].Split ('.')[1].ToJson();
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
	}

	public class AuthenticationResult
	{
		public bool Success { get; set; }

		public string Auth0AccessToken { get; set; }

		public string IdToken { get; set; }

		public JObject User { get; set; }

		public Exception Error { get; set; }
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
