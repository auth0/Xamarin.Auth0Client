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
		// ********** 
		// IMPORTANT: these are demo credentials, and the settings will be reset periodically 
		//            You can obtain your own at https://auth0.com when creating a Xamarin App in the dashboard
		// ***********
		private Auth0.SDK.Auth0Client client = new Auth0.SDK.Auth0Client (
			"contoso",
			"HmqDkk9qtDgxsiSKpLKzc51xD75hgiRW",
			"wbNk_qZi9jqHnj_CKpPasaTAFBaQHma3BnSkU2X00LkBVY_UvuIZ2U3PQG25zqpE");

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

				// This uses a specific connection (named sql-azure-database in Auth0 dashboard) which supports username/password authentication
				this.client.LoginAsync ("sql-azure-database", userName, password)
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
