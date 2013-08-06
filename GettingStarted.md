This tutorial explains how to integrate [Auth0](http://developers.auth0.com) with a Xamarin application (iOS or Android).  Auth0 helps you:

* Add authentication with [multiple authentication sources](https://docs.auth0.com/identityproviders), either social like **Google, Facebook, Microsoft Account, LinkedIn, GitHub, Twitter**, or enterprise identity systems like **Windows Azure AD, Google Apps, AD, ADFS or any SAML Identity Provider**. 
* Add authentication through more traditional **username/password databases**.
* Add support for **[linking different user accounts](https://docs.auth0.com/link-accounts)** with the same user.
* Support for generating signed [Json Web Tokens](https://docs.auth0.com/jwt) to call your APIs and **flow the user identity** securely.
* Support for integrating with **Windows Azure Mobile Services backends**.

The library is cross-platform, so once you learn it on iOS, you're all set on Android.

## Create a free account in Auth0

1. Go to [Auth0](http://developers.auth0.com) and click Sign Up.
2. Use Google, GitHub or Microsoft Account to login.
3. Create a new Xamarin Application

## Triggering the login manually or integrating the Auth0 widget

There are two options to do the integration: 

1. Using the [Auth0 Login Widget](https://docs.auth0.com/login-widget) inside a Web View (this is the simplest with only a few lines of code required).
2. Creating your own UI (more work, but higher control the UI and overall experience)

To start with, we'd recommend using the __Login Widget__. Here is a snippet of code to copy & paste on your project: 

```csharp
using Auth0.SDK;

var auth0 = new Auth0Client(
	"Login", 
	"{Your Auth0 Subdomain}", 
	"{Your ClientId in Auth0}");

// Attach to Completed event
auth.Completed += (sender, eventArgs) => {
	// We presented the UI, so it's up to us to dimiss it on iOS (ignore this line on Android)
	// DismissViewController (true, null);
	
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

> You can obtain the {subdomain} and the {clientId} from your application's settings page on the Auth0 Dashboard.

> `Xamarin.Auth0Client` is built on top of the `WebRedirectAuthenticator` in the Xamarin.Auth component. All rules for standard authenticators apply regarding how the UI will be displayed.

![](https://docs.auth0.com/img/xamarin.auth0client.png)

### Skipping the Identity Selector

If you know which identity provider you want to use, you can add a `connection` parameter to the constructor and the user will be sent straight to the specified `connection`:

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

## Accessing user information

Upon successful authentication, the `Complete` event will fire. `Auth0Client` will set the `eventArgs.Account.Username` property to that obtained from the Identity Provider. You will also get the following attributes from `eventArgs.Account` property:

* `eventArgs.Account.GetProfile()`: an extension method which returns a `Newtonsoft.Json.Linq.JObject` object (from [Json.NET component](http://components.xamarin.com/view/json.net/)) containing all available user attributes.
* `eventArgs.Account.Properties["id_token"]`: is a Json Web Token (JWT) containing all of the user attributes and it is signed with your client secret. This is useful to call your APIs and flow the user identity.
* `eventArgs.Account.Properties["access_token"]`: the `access_token` that can be used to access Auth0's API. You would use this for example to [link user accounts](link-accounts).

> If you have the __Windows Azure Mobile Services__ (WAMS) add-on enabled, Auth0 will sign the JWT with WAMS `masterkey`. Also the JWT will be compatible with the format expected by WAMS. More on this is explained [here](https://docs.auth0.com/jwt#5).
