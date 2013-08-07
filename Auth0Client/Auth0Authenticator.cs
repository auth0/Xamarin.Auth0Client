using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace Auth0.SDK
{
	public class Auth0Authenticator : WebRedirectAuthenticator
	{
		private static string AuthorizeUrl = "https://{0}.auth0.com/authorize?client_id={1}&scope=openid%20profile&redirect_uri={2}&response_type=token&connection={3}";
		private static string LoginWidgetUrl = "https://{0}.auth0.com/login/?client={1}&scope=openid%20profile&redirect_uri={2}&response_type=token";
		private static string DefaultCallback = "https://{0}.auth0.com/mobile";

		private readonly string state; 
		private readonly Uri startUrl;

		/// <summary>
		/// Initializes a new instance of the <see cref="Auth0.SDK.Auth0Authenticator"/> class, instructing it to use a specific connection (e.g. 'google-oauth2', 'amazon', etc)
		/// </summary>
		/// <param name="title">A title to show on the login screen</param>
		/// <param name="subDomain">Your subdomain in Auth0 (usually {subdomain}.auth0.com</param>
		/// <param name="clientId">The Client Id for the application defined in Auth0</param>
		/// <param name="connection">The name of the connection to use in Auth0. Connection defines an Identity Provider</param>
		/// <param name="callbackUrl">The redirect_uri used to detect the end of the authentication transaction</param>
		public Auth0Authenticator (string title, string subDomain, string clientId, string connection, Uri callbackUrl = null) 
			: base (null, ValidateCallback(callbackUrl, subDomain))
		{
			this.Title = string.IsNullOrWhiteSpace(title) ? "Login" : title;

			var chars = new char[16];
			var rand = new Random ();
			for (var i = 0; i < chars.Length; i++) {
				chars [i] = (char)rand.Next ((int)'a', (int)'z' + 1);
			}

			var redirectUri = ValidateCallback (callbackUrl, subDomain).AbsoluteUri;
			var startUri = !string.IsNullOrWhiteSpace (connection) ?
				string.Format (AuthorizeUrl, subDomain, clientId, Uri.EscapeDataString (redirectUri), connection) :
				string.Format (LoginWidgetUrl, subDomain, clientId, Uri.EscapeDataString (redirectUri));

			this.state = new string (chars);
			this.startUrl = new Uri (startUri + "&state=" + this.state);
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
			{
				all [kv.Key] = kv.Value;
			}

			// Check for forgeries in the final redirect
			if (all.ContainsKey ("state")) {
				if (all ["state"] != state ) {
					this.OnError ("Invalid state from server. Possible forgery!");
					return;
				}
			}

			// Look for the access_token
			if (fragment.ContainsKey ("access_token")) {
				// We found an access_token
				this.OnRetrievedAccountProperties (fragment);
				return;
			}

			this.OnError ("Expected access_token in response, but did not receive one.");
		}

		protected virtual void OnRetrievedAccountProperties (IDictionary<string, string> accountProperties)
		{
			this.OnSucceeded (string.Empty, accountProperties);
		}

		private static Uri ValidateCallback(Uri callbackUrl, string subDomain)
		{
			return callbackUrl == null ? 
				new Uri(string.Format(DefaultCallback, subDomain)) : callbackUrl;
		}
	}
}
