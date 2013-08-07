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
			"{subDomain}",
			"{clientID}",
			"{clientSecret}");

		private readonly TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
		private ProgressDialog progressDialog;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			this.SetContentView(Resource.Layout.Main);

			this.progressDialog = new ProgressDialog (this);
			this.progressDialog.SetMessage ("loading...");

			var loginWithWidget = this.FindViewById<Button> (Resource.Id.loginWithWidget);
			loginWithWidget.Click += delegate {
				// This will show all connections enabled in Auth0, and let the user choose the identity provider
				this.client.LoginAsync (this)					// current context
					.ContinueWith(
						task => this.ShowResult (task), 
						this.scheduler);
			};

			var loginWithConnection = this.FindViewById<Button> (Resource.Id.loginWithConnection);
			loginWithConnection.Click += delegate {
				// This uses a specific connection: google-oauth2
				this.client.LoginAsync (this, "google-oauth2")	// current context and connection name
					.ContinueWith(
						task => this.ShowResult (task), 
						this.scheduler);
			};

			var loginWithUserPassword = this.FindViewById<Button> (Resource.Id.loginWithUserPassword);
			loginWithUserPassword.Click += delegate {
				this.progressDialog.Show();

				var userName = this.FindViewById<EditText> (Resource.Id.txtUserName).Text;
				var password = this.FindViewById<EditText> (Resource.Id.txtUserPassword).Text;

				// This uses a specific connection which supports username/password authentication
				this.client.LoginAsync ("dbtest.com", userName, password)
					.ContinueWith (
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

			this.FindViewById<TextView>(Resource.Id.txtResult).Text = error == null ?
				taskResult.Result.Profile.ToString() :
				error.InnerException != null ? error.InnerException.Message : error.Message;

			if (this.progressDialog.IsShowing) {
				this.progressDialog.Hide();
			}
		}
	}
}
