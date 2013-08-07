using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Auth0.SDK;
using Newtonsoft.Json.Linq;

namespace Auth0Client.Android.Sample
{
	[Activity (Label = "Auth0Client - Android Sample", MainLauncher = true)]
	public class MainActivity : Activity
	{
		// You can obtain {subDomain}, {clientID} and {clientSecret} from your settings page in the Auth0 Dashboard
		private Auth0.SDK.Auth0Client client = new Auth0.SDK.Auth0Client (
			"iaco2",
			"XviE9dLlREjXZduIzTqtsGsiZELjls8z",
			"g-Xznc-5ccEqgTxEQZrLeE_8bCixQL4-_hDraMgty8ZGBSmz9KnYtWzqUlqFmhpy");

		private readonly TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			this.SetContentView(Resource.Layout.Main);

			var loginWidget = this.FindViewById<Button> (Resource.Id.loginWidget);
			loginWidget.Click += delegate {
				// This will show all connections enabled in Auth0, and let the user choose the identity provider
				this.client.LoginAsync (this)					// current context
					.ContinueWith(
						task => this.ShowResult (task), 
						this.scheduler);
			};

			var loginConnection = this.FindViewById<Button> (Resource.Id.loginConnection);
			loginConnection.Click += delegate {
				// This uses a specific connection: google-oauth2
				this.client.LoginAsync (this, "google-oauth2")	// current context and connection name
					.ContinueWith(
						task => this.ShowResult (task), 
						this.scheduler);
			};
		}

		private void ShowResult(Task<Auth0User> taskResult)
		{
			Exception error = taskResult.Exception != null ? taskResult.Exception.Flatten () : null;

			if (error == null && taskResult.IsCanceled) {
				error = new Exception ("Authentication was canceled.");
			}

			this.FindViewById<TextView>(Resource.Id.userProfileLbl).Text = error == null ?
				taskResult.Result.Profile.ToString() :
				error.InnerException != null ? error.InnerException.Message : error.Message;
		}
	}
}
