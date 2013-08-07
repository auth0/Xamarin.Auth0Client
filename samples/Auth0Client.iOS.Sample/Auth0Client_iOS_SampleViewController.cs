using System;
using System.Drawing;
using System.Threading.Tasks;
using Auth0.SDK;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Auth0Client.iOS.Sample
{
	public partial class Auth0Client_iOS_SampleViewController : DialogViewController
	{
		// You can obtain {subDomain}, {clientID} and {clientSecret} from your settings page in the Auth0 Dashboard (https://app.auth0.com/#/settings)
		private const string SubDomain = "iaco2";
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
			var client = new Auth0.SDK.Auth0Client (
				SubDomain,								// subDomain
				ClientID,								// clientID
				ClientSecret);							// client secret

			client.LoginAsync (this)					// current controller
				.ContinueWith(task => {
					this.InvokeOnMainThread(() => {
						this.DismissViewController (true, null);
						this.ShowResult (task);
					});	
				});
		}

		private void LoginWithConnectionButtonClick ()
		{
			// This uses a specific connection: google-oauth2
			var client = new Auth0.SDK.Auth0Client (
				SubDomain,								// subDomain
				ClientID,								// clientID
				ClientSecret);							// client secret

			client.LoginAsync (this, "google-oauth2")	// current controller and connection name
				.ContinueWith(task => {
					this.InvokeOnMainThread(() => {
						this.DismissViewController (true, null);
						this.ShowResult (task);
					});
				});
		}

		private void LoginWithUsernamePassword ()
		{
			// Show loading animation
			this.loadingOverlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
			this.View.Add (this.loadingOverlay);

			var connection = "dbtest.com";				// connection name
			var client = new Auth0.SDK.Auth0Client (
				SubDomain,								// subDomain
				ClientID,								// clientID
				ClientSecret);							// client secret

			client.LoginAsync (connection, this.userNameElement.Value, this.passwordElement.Value)
				.ContinueWith (task => {
					this.InvokeOnMainThread(() => {
                   		this.loadingOverlay.Hide ();
						this.ShowResult (task);
					});
				  });
		}

		private void ShowResult(Task<Auth0User> taskResult)
		{
			Exception error = taskResult.Exception != null ? taskResult.Exception.Flatten () : null;

			if (error == null && taskResult.IsCanceled) {
				error = new Exception ("Authentication was canceled.");
			}

			this.resultSectionRow.Caption = error != null ?
				string.Format (
					"ERROR: {0}", 
					error.InnerException != null ? 
						error.InnerException.Message : 
						error.Message) :
				string.Format (
					"Hi {0}!{3}{3}access_token: {1}{3}{3}id_token: {2}", 
					taskResult.Result.Profile["name"], 
					taskResult.Result.Auth0AccessToken, 
					taskResult.Result.IdToken, 
					Environment.NewLine);
		}
	}
}
