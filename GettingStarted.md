This tutorial explains how to integrate [Auth0](http://developers.auth0.com) with a Xamarin application (iOS or Android).  Auth0 helps you:

* Add authentication with [multiple authentication sources](https://docs.auth0.com/identityproviders), either social like **Google, Facebook, Microsoft Account, LinkedIn, GitHub, Twitter**, or enterprise identity systems like **Windows Azure AD, Google Apps, AD, ADFS or any SAML Identity Provider**. 
* Add authentication through more traditional **[username/password databases](https://docs.auth0.com/mysql-connection-tutorial)**.
* Add support for **[linking different user accounts](https://docs.auth0.com/link-accounts)** with the same user.
* Support for generating signed [Json Web Tokens](https://docs.auth0.com/jwt) to call your APIs and **flow the user identity** securely.
* Support for integrating with **Windows Azure Mobile Services backends**.
* Analytics of how, when and where users are logging in.
* Pull data from other sources and add it to the user profile, through [JavaScript rules](https://docs.auth0.com/rules).

The library is cross-platform, so once you learn it on iOS, you're all set on Android.

## Create a free account in Auth0

1. Go to [Auth0](http://developers.auth0.com) and click Sign Up.
2. Use Google, GitHub or Microsoft Account to login.
3. Create a new Xamarin Application

There are three options to do the integration: 

1. Using the [Auth0 Login Widget](https://docs.auth0.com/login-widget) inside a Web View (this is the simplest with only a few lines of code required).
2. Creating your own UI (more work, but higher control the UI and overall experience).
3. Using specific user name and password.

## Option 1: Authentication using Login Widget

To start with, we'd recommend using the __Login Widget__. Here is a snippet of code to copy & paste on your project: 

```csharp
using Auth0.SDK;

var auth0 = new Auth0Client(
	"{subDomain}",
	"{clientID}",
	"{clientSecret}");

// 'this' could be a Context object (Android) or UIViewController, UIView, UIBarButtonItem (iOS)
auth0.LoginAsync (this)
	 .ContinueWith(t => { 
	 /* 
	    Use t.Result to do wonderful things, e.g.: 
	      - get user email => t.Result.Profile["email"].ToString()
	      - get facebook/google/twitter/etc access token => t.Result.Profile["identities"][0]["access_token"]
	      - get Windows Azure AD groups => t.Result.Profile["groups"]
	      - etc.
	*/ });
```

- You can obtain the {subDomain}, {clientID} and the {clientSecret} from your application's settings page on the Auth0 Dashboard.

- `Xamarin.Auth0Client` is built on top of the `WebRedirectAuthenticator` in the Xamarin.Auth component. All rules for standard authenticators apply regarding how the UI will be displayed.

![](https://docs.auth0.com/img/xamarin.auth0client.png)

## Option 2: Authentication with your own UI

If you know which identity provider you want to use, you can add a `connection` parameter to the constructor and the user will be sent straight to the specified `connection`:

```csharp
auth0.LoginAsync (this, "google-oauth2") // connection name here
	 .ContinueWith(t => { /* Use t.Result to do wonderful things */ });
```

- connection names can be found on Auth0 dashboard. E.g.: `facebook`, `linkedin`, `somegoogleapps.com`, `saml-protocol-connection`, etc.

## Option 3: Authentication with specific user name and password

```csharp
auth0.LoginAsync (
	"sql-azure-database", 		// connection name here
	"jdoe@foobar.com", 			// user name
	"1234")						// password
	 .ContinueWith(t => 
	 { 
	 	/* Use t.Result to do wonderful things */ 
 	 });
```

## Accessing user information

The `Auth0User` has the following properties:

* `Profile`: returns a `Newtonsoft.Json.Linq.JObject` object (from [Json.NET component](http://components.xamarin.com/view/json.net/)) containing all available user attributes (e.g.: `user.Profile["email"].ToString()`).
* `IdToken`: is a Json Web Token (JWT) containing all of the user attributes and it is signed with your client secret. This is useful to call your APIs and flow the user identity (or Windows Azure Mobile Services, see below).
* `Auth0AccessToken`: the `access_token` that can be used to access Auth0's API. You would use this for example to [link user accounts](https://docs.auth0.com/link-accounts).

- If you want to use __Windows Azure Mobile Services__ (WAMS) you should create a WAMS app in Auth0 and set the Master Key that you can get on the Windows Azure portal. Then you have change your Xamarin app to use the client id and secret of the WAMS app just created and set the callback of the WAMS app to be` https://{subDomain}.auth0.com/mobile`. Finally, you have to set the `MobileServiceAuthenticationToken` property of the `MobileServiceUser` with the `IdToken` property of `Auth0User`.
