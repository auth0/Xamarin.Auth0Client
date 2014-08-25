using System;
using System.Drawing;
using System.Threading.Tasks;
using Android.Content;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;

namespace Auth0.SDK
{
	public partial class Auth0Client
	{
		/// <summary>
		/// Log a user into an Auth0 application given a connection name.
		/// </summary>
		/// <param name="context" type="Android.Content.Context">
		/// Context used to launch login UI.
		/// </param>
		/// <param name="connection" type="string">
		/// The name of the connection to use in Auth0. Connection defines an Identity Provider.
		/// </param>
		/// <param name="scope" type="string">
		/// Space delimited, case sensitive list of OAuth 2.0 scope values.
		/// </param>
		/// <returns>
		/// Task that will complete when the user has finished authentication.
		/// </returns>
		public Task<Auth0User> LoginAsync (
			Context context, 
			string connection = "", 
			bool withRefreshToken = false, 
			string scope = "openid")
		{
			return this.SendLoginAsync(context, connection, withRefreshToken, scope);
		}

		private async Task<Auth0User> SendLoginAsync(
			Context context, 
			string connection, 
			bool withRefreshToken,
			string scope)
		{
			// Launch server side OAuth flow using the GET endpoint
			scope = IncreaseScopeWithOfflineAccess(withRefreshToken, scope);

			var tcs = new TaskCompletionSource<Auth0User> ();
			var auth = await this.GetAuthenticator (connection, scope);

			auth.Error += (o, e) =>
			{
				var ex = e.Exception ?? new AuthException (e.Message);
				tcs.TrySetException (ex);
			};

			auth.Completed += (o, e) =>
			{
				if (!e.IsAuthenticated) 
				{
					tcs.TrySetCanceled();
					return;
				}

				this.SetupCurrentUser (e.Account.Properties);
				tcs.TrySetResult (this.CurrentUser);
			};

			Intent intent = auth.GetUI (context);
			context.StartActivity (intent);

			return await tcs.Task;
		}
	}
}
