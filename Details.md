Xamarin.Auth0Client helps you authenticate users with any [Auth0 supported identity provider](https://docs.auth0.com/identityproviders), via the OpenId Connect protocol (built on top of OAuth2). The library is cross-platform, so once you learn it on iOS, you're all set on Android.

```csharp
using Xamarin.Auth0Client;

var auth = new Auth0Client (
	title: "Login",
	tenant: "{your tenant name in Auth0}"
	clientId: "{your client id in Auth0}",
	callbackUrl: "http://localhost/client");

auth.Completed += (sender, eventArgs) => {
	DismissViewController (true, null);
	if (eventArgs.IsAuthenticated) {
		// Use eventArgs.Account to do wonderful things

		var jwt = (string)eventArgs.Properties["id_token"];

		//Use jwt to call an API
	}
}

PresentViewController (auth.GetUI (), true, null);
```

The above example will display the [__Auth0 Login Widget__](https://docs.auth0.com/login-widget). 

![]()

If you add the `connection` parameter to the constructor, then Auth0 will redirect the user to the specified `connection`:

```csharp
using Xamarin.Auth0Client;

var auth = new Auth0Client (
	title: "Login",
	tenant: "{your tenant name in Auth0}"
	clientId: "{your client id in Auth0}",
	connection: "google-oauth2",
	callbackUrl: "http://localhost/client");

```

It's that easy to authenticate users!
