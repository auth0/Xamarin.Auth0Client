Xamarin.Auth0Client helps you authenticate users with any [Auth0 supported identity provider](https://docs.auth0.com/identityproviders), via the OpenId Connect protocol (built on top of OAuth2). The library is cross-platform, so once you learn it on iOS, you're all set on Android.

```csharp
using Auth0.SDK;

var auth0 = new Auth0Client(
	"Login", 
	"{Your Auth0 Subdomain}", 
	"{Your ClientId in Auth0}");

// Attach to Completed event
auth.Completed += (sender, eventArgs) => {
	// We presented the UI, so it's up to us to dimiss it on iOS (ignore this line on Android)
	DismissViewController (true, null);
	
	if (eventArgs.IsAuthenticated) {
		// Use **eventArgs.Account** to do wonderful things
	} else {
		// The user cancelled
	}
};

// Show the UI
// iOS      [ViewDidAppear]
UINavigationControllers view = auth0.GetUI();
PresentViewController(view, true, null);

// Android  [OnCreate]
Intents intent = auth0.GetUI(this);
StartActivityForResult(intent, 42);
```

> You can obtain {subdomain} and {clientId} from your settings page in the Auth0 Dashboard.

The above example will display the [__Auth0 Login Widget__](https://docs.auth0.com/login-widget). If you add the `connection` parameter to the constructor, the user will be sent straight to the specified `connection`:

```csharp
using Auth0.SDK;
// ...
var auth = new Auth0Client (
	"Login", 
	"{Your Auth0 Subdomain}", 
	"{Your ClientId in Auth0}",
	"google-oauth2"); // connection name here
```

> connection names can be found on Auth0 dashboard. E.g.: `facebook`, `linkedin`, `somegoogleapps.com`, `saml-protocol-connection`, etc.

It's that easy to authenticate users!
