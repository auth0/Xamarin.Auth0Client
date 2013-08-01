using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Auth0Client.Android.Sample
{
	[Activity (Label = "Auth0Client.Android.Sample", MainLauncher = true)]
	public class Activity1 : Activity
	{
		// You can obtain {tenant} and {clientID} from your settings page in the Auth0 Dashboard (https://app.auth0.com/#/settings)
		private const string Tenant = "{tenant}";
		private const string ClientID = "{clientID}";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView(Resource.Layout.Main);

			FindViewById<TextView>(Resource.Id.id_token).Text = string.Empty;
			FindViewById<TextView>(Resource.Id.userName).Text = string.Empty;

			Button loginWidget = FindViewById<Button> (Resource.Id.loginWidget);

			loginWidget.Click += delegate {
				// This will show all connections enabled in Auth0, and let the user choose the identity provider
				var client = new Auth0.SDK.Auth0Client (
					"Auth0", 						// title
					Tenant, 						// tenant
					ClientID, 						// clientID
					"https://localhost/client");	// callback

				this.wireLogin(client);
			};

			Button loginConnection = FindViewById<Button> (Resource.Id.loginConnection);

			loginConnection.Click += delegate {
				// This uses a specific connection: google-oauth2
				var client = new Auth0.SDK.Auth0Client (
					"Auth0", 						// title
					Tenant, 						// tenant
					ClientID, 						// clientID
					"google-oauth2",				// connection name
					"https://localhost/client");	// callback

				this.wireLogin(client);
			};
		}

		private void wireLogin (Auth0.SDK.Auth0Client client)
		{
			client.Completed += (object sender, Xamarin.Auth.AuthenticatorCompletedEventArgs e) => 
			{
				if (e.IsAuthenticated) 
				{
					// All the information gathered from a successful authentication is available in e.Account
					this.ShowResult(
						idToken: (string)e.Account.Properties["id_token"],
						userName: e.Account.Username);
				}
				else
				{
					// The user cancelled
					this.ShowResult(error: "User cancelled");
				}
			};

			client.Error += (object sender, Xamarin.Auth.AuthenticatorErrorEventArgs e) =>
			{
				this.ShowResult(error: e.Message);
			};

			// We're ready to present the login UI
			var intent = client.GetUI (this);
			StartActivityForResult(intent, 42);
		}

		private void ShowResult(string idToken = "", string userName = "", string error = "")
		{
			FindViewById<TextView>(Resource.Id.id_token).Text = 
				!string.IsNullOrEmpty(idToken) ? 
					string.Format("id_token: {0}", idToken) : string.Empty;

			FindViewById<TextView>(Resource.Id.userName).Text = 
				string.IsNullOrEmpty(error) ? 
					string.Format("Hi {0}!", userName) : "Error: " + error;
		}
	}
}
