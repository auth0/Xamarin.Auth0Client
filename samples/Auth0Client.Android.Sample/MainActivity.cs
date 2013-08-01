using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Auth0Client.Sample
{
	[Activity (Label = "Auth0Client.Sample", MainLauncher = true)]
	public class Activity1 : Activity
	{
		private void wireLogin( Auth0.SDK.Auth0Client client )
		{
			client.Completed += (object sender, Xamarin.Auth.AuthenticatorCompletedEventArgs e) => {
				FindViewById<TextView>(Resource.Id.userName).Text = "User: " + e.Account.Username;
				FindViewById<TextView>(Resource.Id.id_token).Text = "Id Token: " + (string)e.Account.Properties["id_token"];
			};

			client.Error += (object sender, Xamarin.Auth.AuthenticatorErrorEventArgs e) => {
				FindViewById<TextView>(Resource.Id.userName).Text = "Error: " + e.Message;
			};

			var intent = client.GetUI (this);
			StartActivityForResult(intent, 42);
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Button loginWidget = FindViewById<Button> (Resource.Id.loginWidget);

			loginWidget.Click += delegate {

				//This will show all connections enabled in Auth0, and let the user choose the identity provider
				var auth0 = new Auth0.SDK.Auth0Client( "Auth0", "eugeniop", "2iuy7pGx4Q1ITmUwCvgziYtdkclejcqt", "http://localhost/client");
				wireLogin( auth0 );
			};


			Button loginConnection = FindViewById<Button> (Resource.Id.loginConnection);

			loginConnection.Click += delegate {

				//This uses a specific connection: google-ouath2
				var auth0 = new Auth0.SDK.Auth0Client( "Auth0", "eugeniop", "2iuy7pGx4Q1ITmUwCvgziYtdkclejcqt", "google-oauth2", "http://localhost/client");
				wireLogin (auth0);
			};
		}
	}
}


