using System;
using System.Drawing;
using Auth0.SDK;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Auth0Client.iOS.Sample
{
	public partial class Auth0Client_iOS_SampleViewController : DialogViewController
	{
		// You can obtain {tenant}, {clientID} and {clientSecret} from your settings page in the Auth0 Dashboard (https://app.auth0.com/#/settings)
		private const string Tenant = "iaco2";
		private const string ClientID = "XviE9dLlREjXZduIzTqtsGsiZELjls8z";
		private const string ClientSecret = "g-Xznc-5ccEqgTxEQZrLeE_8bCixQL4-_hDraMgty8ZGBSmz9KnYtWzqUlqFmhpy";

		public Auth0Client_iOS_SampleViewController (RootElement root) : base(root)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			this.Initialize ();
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}

		private void LoginWithWidgetButtonClick ()
		{
			// This will show all connections enabled in Auth0, and let the user choose the identity provider
			var client = new Auth0.SDK.Auth0Client(
				"Auth0", 						// title
				Tenant, 						// tenant
				ClientID); 						// clientID

			this.WireLogin(client);
		}

		private void LoginWithConnectionButtonClick ()
		{
			// This uses a specific connection: google-oauth2
			var client = new Auth0.SDK.Auth0Client(
				"Auth0", 						// title
				Tenant, 						// tenant
				ClientID, 						// clientID
				"google-oauth2");				// connection name

			this.WireLogin(client);
		}

		private void LoginWithUsernamePassword ()
		{
			var client = new Auth0.SDK.Auth0Client (
				"Auth0", 						// title
				Tenant, 						// tenant
				ClientID,						// clientID
				"dbtest.com");					// connection name

			this.WireLogin(client, true);
		}

		private void WireLogin (Auth0.SDK.Auth0Client client, bool fromUsernamePassword = false)
		{
			client.Completed += (object sender, Xamarin.Auth.AuthenticatorCompletedEventArgs e) => 
			{
				if (e.IsAuthenticated) 
				{
					// All the information gathered from a successful authentication is available in e.Account
					this.ShowResult(
						accessToken: (string)e.Account.Properties["access_token"],
						idToken: (string)e.Account.Properties["id_token"],
						userName: (string)e.Account.GetProfile()["name"]);
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

			if (!fromUsernamePassword) {
				// Present the login UI
				this.PresentViewController (client.GetUI (), true, null);
			} 
			else 
			{
				// Perform authentication based on user/password
				client.Authenticate(
					ClientSecret, 					// client secret
					this.userNameElement.Value, 	// username
					this.passwordElement.Value);	// password

				// TODO: show loading icon
			}
		}

		private void ShowResult(string accessToken = "", string idToken = "", string userName = "", string error = "")
		{
			// We presented the UI, so it's up to us to dimiss it on iOS
			this.DismissViewController (true, null);

			this.resultSectionRow.Caption = !string.IsNullOrWhiteSpace (error) ?
				string.Format ("ERROR: {0}", error) :
				string.Format ("Hi {0}!{3}{3}access_token: {1}{3}{3}id_token: {2}", userName, accessToken, idToken, Environment.NewLine);
		}
	}
}
