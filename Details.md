[Auth0](http://developers.auth0.com) is a cloud service that works as a Single Sign On hub between your apps and authentication sources. By adding Auth0 to your Xamarin app you can:

* Add authentication with [multiple authentication sources](https://docs.auth0.com/identityproviders), either social like **Google, Facebook, Microsoft Account, LinkedIn, GitHub, Twitter**, or enterprise identity systems like **Windows Azure AD, Google Apps, AD, ADFS or any SAML Identity Provider**. 
* Add authentication through more traditional **[username/password databases](https://docs.auth0.com/mysql-connection-tutorial)**.
* Add support for **[linking different user accounts](https://docs.auth0.com/link-accounts)** with the same user.
* Support for generating signed [Json Web Tokens](https://docs.auth0.com/jwt) to call your APIs and **flow the user identity** securely.
* Support for integrating with **Windows Azure Mobile Services backends**.
* Analytics of how, when and where users are logging in.
* Pull data from other sources and add it to the user profile, through [JavaScript rules](https://docs.auth0.com/rules).

The library is cross-platform, so once you learn it on iOS, you're all set on Android.

## Authentication with Widget

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

## Authentication with your own UI

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

Get more details on [our Xamarin tutorial](https://docs.auth0.com/xamarin-tutorial).
