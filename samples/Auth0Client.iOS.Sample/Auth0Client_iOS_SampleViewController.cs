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
		// ********** 
		// IMPORTANT: these are demo credentials, and the settings will be reset periodically 
		//            You can obtain your own at https://auth0.com when creating a Xamarin App in the dashboard
		// ***********
		private Auth0.SDK.Auth0Client client = new Auth0.SDK.Auth0Client (
			"contoso.auth0.com",
			"HmqDkk9qtDgxsiSKpLKzc51xD75hgiRW");

		private readonly TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();

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

		private void RefreshUserId()
		{
			// Show loading animation
			this.loadingOverlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
			this.View.Add (this.loadingOverlay);

			if (this.client.CurrentUser != null) {
				//if logged in, refresh token.
				this.client.RefreshToken ()
					.ContinueWith (
					task => this.ShowResult (this.client.CurrentUser), this.scheduler);

			} else {
				//no user logged in, so show error in results
				SetResultText ("You must be signed in for this feature to work.");
			}
		}
	

		private void LoginWithWidgetButtonClick ()
		{
			// This will show all connections enabled in Auth0, and let the user choose the identity provider
			this.client.LoginAsync (this, // current controller
				withRefreshToken:true, deviceName: "MyTestDevice") //refresh token support. device needed if using refresh.
				.ContinueWith(
					task => this.ShowResult (task), 
					this.scheduler);
		}


		private void LoginWithConnectionButtonClick ()
		{
			// This uses a specific connection: google-oauth2
			this.client.LoginAsync (this, "google-oauth2")	// current controller and connection name
						.ContinueWith(
							task => this.ShowResult (task), 
							this.scheduler);
		}

		private void LoginWithUsernamePassword ()
		{
			// Show loading animation
			this.loadingOverlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
			this.View.Add (this.loadingOverlay);

			// This uses a specific connection (named sql-azure-database in Auth0 dashboard) which supports username/password authentication
			this.client.LoginAsync ("sql-azure-database", this.userNameElement.Value, this.passwordElement.Value)
						.ContinueWith (
							task => this.ShowResult (task),
							this.scheduler);
		}

		private void SetResultText(string text)
		{
			this.resultElement.Value = text;
			this.ReloadData ();
			this.loadingOverlay.Hide ();
		}

		private void ShowResult(Auth0User user)
		{
			var id = user.IdToken;
			var refreshToken = user.RefreshToken;
			var profile = user.Profile.ToString();

			var truncatedId = id.Remove (0, 20);
			truncatedId = truncatedId.Insert (0, "...");

			SetResultText(string.Format ("Id (*Changed): {0}\r\n\r\nRefresh Token: {1}\r\n\r\nProfile:\r\n{2}", 
				truncatedId, refreshToken, profile));
		}

		private void ShowResult(Task<Auth0User> taskResult)
		{
			Exception error = taskResult.Exception != null ? taskResult.Exception.Flatten () : null;

			if (error == null && taskResult.IsCanceled) {
				error = new Exception ("Authentication was canceled.");
			}

			var refreshToken = error == null ?
				taskResult.Result.RefreshToken :
				string.Empty;

			var id = error == null ?
				taskResult.Result.IdToken :
				string.Empty;

			var truncatedId = id.Remove (0, 20);
			truncatedId = truncatedId.Insert (0, "...");

			var profile = error == null ?
				taskResult.Result.Profile.ToString () :
				error.InnerException != null ? error.InnerException.Message : error.Message;
				
			SetResultText(string.Format ("Id: {0}\r\n\r\nRefresh Token: {1}\r\n\r\nProfile:\r\n{2}", 
				truncatedId, refreshToken, profile));
		}
	}
}
