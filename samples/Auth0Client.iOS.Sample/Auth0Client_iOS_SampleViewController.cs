using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Auth0Client.iOS.Sample
{
	public partial class Auth0Client_iOS_SampleViewController : UIViewController
	{
		// You can obtain {tenant} and {clientID} from your settings page in the Auth0 Dashboard (https://app.auth0.com/#/settings)
		private const string Tenant = "iaco2";
		private const string ClientID = "XviE9dLlREjXZduIzTqtsGsiZELjls8z";

		public Auth0Client_iOS_SampleViewController () : base ("Auth0Client_iOS_SampleViewController", null)
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
			this.lblUserName.Text = string.Empty;
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}

		partial void loginWithWidgetButtonClick (NSObject sender)
		{
			// This will show all connections enabled in Auth0, and let the user choose the identity provider
			var client = new Auth0.SDK.Auth0Client(
				"Auth0", 						// title
				Tenant, 						// tenant
				ClientID, 						// clientID
				"https://localhost/client");	// callback

			this.wireLogin(client);
		}

		partial void loginWithConnectionButtonClick (NSObject sender)
		{
			// This uses a specific connection: google-oauth2
			var client = new Auth0.SDK.Auth0Client(
				"Auth0", 						// title
				Tenant, 						// tenant
				ClientID, 						// clientID
				"google-oauth2",				// connection name
				"https://localhost/client");	// callback

			this.wireLogin(client);
		}

		private void wireLogin (Auth0.SDK.Auth0Client client)
		{
			client.Completed += (object sender, Xamarin.Auth.AuthenticatorCompletedEventArgs e) => 
			{
				// We presented the UI, so it's up to us to dimiss it on iOS
				DismissViewController (true, null);

				if (e.IsAuthenticated) 
				{
					// All the information gathered from a successful authentication is available in e.Account
					this.ShowResult(
						accessToken: (string)e.Account.Properties["access_token"],
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
			PresentViewController(client.GetUI(), true, null);
		}

		private void ShowResult(string accessToken = "", string idToken = "", string userName = "", string error = "")
		{
			this.txtAccessToken.Text = accessToken;
			this.txtIdToken.Text = idToken;
			this.lblUserName.Text = 
				string.IsNullOrEmpty(error) ? string.Format("Hi {0}!", userName) : "Error: " + error;

			this.lblUserName.SizeToFit ();
		}
	}
}
