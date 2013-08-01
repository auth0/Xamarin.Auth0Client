## 1. Create and configure an Auth0Client

```csharp
using Auth0.SDK;
...
var auth0 = new Auth0Client( "A Title", "{tenant}", "{Your ClientId in Auth0}", "http://localhost/client");
```

> You can obtain {tenant} and {clientId} from your settings page in the [Auth0 Dashboard](https://app.auth0.com/#/settings).

## 2. Authenticate the user

`Auth0Client` is built on top of the `WebRedirectAuthenticator` in the Xamarin.Auth component. All rules for standard authenticators apply regarding how the UI will be displayed.

Before we present the UI, we need to start listening to the `Completed` event which fires when the user successfully authenticates or cancels. You can find out if the authentication succeeded by testing the `IsAuthenticated` property of `eventArgs`:

```csharp
auth.Completed += (sender, eventArgs) => {
	// We presented the UI, so it's up to us to dimiss it on iOS.
	DismissViewController (true, null);

	if (eventArgs.IsAuthenticated) {
		// Use eventArgs.Account to do wonderful things
	} else {
		// The user cancelled
	}
};
```

All the information gathered from a successful authentication is available in `eventArgs.Account`.

Now we're ready to present the login UI from `ViewDidAppear` on iOS:

```csharp
PresentViewController (auth.GetUI (), true, null);
```

The `GetUI` method returns `UINavigationControllers` on iOS, and `Intents` on Android. On Android, we would write the following code to present the UI from `OnCreate`:

```csharp
StartActivity (auth.GetUI (this));
```
It's that easy!


## 3. Retrieve authentication properties

Upon successful authentication, the `Complete` event will fire. `Auth0Client` will set the `username` property to that obtained from the Identity Provider. You will also get:

* `access_token` 
* `id_token`

The `access_token` is a regular OAuth Access Token that can be used to call [Auth0 API](https://docs.auth0.com/api-reference). `id_token` is a Json Web Token (JWT) and it is signed by Auth0. 

> If you have the __Windows Azure Mobile Services__ (WAMS) add-on enabled, Auth0 will sign the JWT with WAMS `masterkey`. Also the JWT will be compatible with the format expected by WAMS.
