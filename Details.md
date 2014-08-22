[Auth0](http://developers.auth0.com) is a cloud service that works as a Single Sign On hub between your apps and authentication sources. By adding Auth0 to your Xamarin app you can:

* Add authentication with [multiple authentication sources](https://docs.auth0.com/identityproviders), either social like **Google, Facebook, Microsoft Account, LinkedIn, GitHub, Twitter, Box, 37Signals**, or enterprise identity systems like **Windows Azure AD, Google Apps, AD, ADFS or any SAML Identity Provider**.
* Add authentication through more traditional [username/password databases](https://docs.auth0.com/mysql-connection-tutorial).
* Add support for [linking different user accounts](https://docs.auth0.com/link-accounts) with the same user.
* Support for generating signed [Json Web Tokens](https://docs.auth0.com/jwt) to call your APIs and **flow the user identity** securely.
* Support for integrating with third party APIs **(AWS, Windows Azure Mobile Services, Firebase, Salesforce, and more!)**.
* Analytics of how, when and where users are logging in.
* Pull data from other sources and add it to the user profile, through [JavaScript rules](https://docs.auth0.com/rules).

The library is cross-platform, so once you learn it on iOS, you're all set on Android.

## Authentication with Widget

```csharp
using Auth0.SDK;

var auth0 = new Auth0Client(
	"{domain}",
	"{clientID}");

// 'this' could be a Context object (Android) or UIViewController, UIView, UIBarButtonItem (iOS)
var user = await auth0.LoginAsync(this);
/*
- get user email => user.Profile["email"].ToString()
- get facebook/google/twitter/etc access token => user.Profile["identities"][0]["access_token"]
- get Windows Azure AD groups => user.Profile["groups"]
- etc.
*/
```

* You can obtain the {domain} and {clientID} from your application's settings page on the Auth0 Dashboard. You need to subscribe to Auth0 to get these values. The sample will not work with invalid or missing parameters. You can get a free subscription for testing and evaluation.

## Authentication with your own UI

```csharp
var user = await auth0.LoginAsync(this, "google-oauth2"); // connection name here
```

* connection names can be found on Auth0 dashboard. E.g.: `facebook`, `linkedin`, `somegoogleapps.com`, `saml-protocol-connection`, etc.

## Authentication with specific user name and password

```csharp
var user = await auth0.LoginAsync(
  "sql-azure-database", // connection name here
  "jdoe@foobar.com",    // user name
  "1234");             	// password
```

Get more details on [our Xamarin tutorial](https://docs.auth0.com/xamarin-tutorial).
