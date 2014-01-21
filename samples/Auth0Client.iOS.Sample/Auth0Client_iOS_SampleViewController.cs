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
			"contoso",
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

		private void LoginWithWidgetButtonClick ()
		{
			// This will show all connections enabled in Auth0, and let the user choose the identity provider
			this.client.LoginAsync (this)					// current controller
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

		private void ShowResult(Task<Auth0User> taskResult)
		{
			Exception error = taskResult.Exception != null ? taskResult.Exception.Flatten () : null;

			if (error == null && taskResult.IsCanceled) {
				error = new Exception ("Authentication was canceled.");
			}

			this.resultElement.Value = error == null ?
				taskResult.Result.Profile.ToString () :
				error.InnerException != null ? error.InnerException.Message : error.Message;

			this.ReloadData ();
			this.loadingOverlay.Hide ();
		}
	}
}
