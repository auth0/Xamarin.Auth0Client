Xamarin.Auth0Client helps you authenticate users with any [Auth0 supported identity provider](https://docs.auth0.com/identityproviders), via the OpenId Connect protocol (built on top of OAuth2). The library is cross-platform, so once you learn it on iOS, you're all set on Android.

```csharp
using Auth0.SDK;

var auth = new Auth0Client (
	"Login", // title
	"{your tenant name in Auth0}"
	"{your client id in Auth0}");

auth.Completed += (sender, eventArgs) => {
	DismissViewController (true, null);
	
	if (eventArgs.IsAuthenticated) {
		// Use eventArgs.Account to do wonderful things

		var jwt = (string)eventArgs.Properties["id_token"];

		// Use jwt to call an API
	}
}

PresentViewController (auth.GetUI (), true, null);
```

The above example will display the [__Auth0 Login Widget__](https://docs.auth0.com/login-widget). 

![](http://puu.sh/3RUxd.jpg)

If you add the `connection` parameter to the constructor, then Auth0 will redirect the user to the specified `connection`:

```csharp
using Auth0.SDK;

var auth = new Auth0Client (
	"Login", // title
	"{your tenant name in Auth0}"
	"{your client id in Auth0}",
	"google-oauth2");
```

It's that easy to authenticate users!
