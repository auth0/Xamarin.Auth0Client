# Xamarin.Auth0Client
A cross-platform API for authenticating users with the [Auth0 platform](http://auth0.com).
* This component builds on top of the [Xamarin.Auth](https://github.com/xamarin/Xamarin.Auth) framework.
* Please read [Getting Started](https://github.com/auth0/Xamarin.Auth0Client/blob/master/GettingStarted.md) to learn how to use the library.

## Generate and publish new Xamarin Component

1. Build __Xamarin.Auth0Client.sln__ solution (with __RELEASE mode__)
2. Download the [xamarin-component command line tool](https://components.xamarin.com/submit/xpkg) and run the following script (replacing `Auth0Client-X.Y.xam` with specific version number):

```
mono xamarin-component.exe create-manually Auth0Client-X.Y.Z.xam \
  --name="Auth0 SDK" \
  --summary="Add authentication with different sources, either social like Google, Facebook, Twitter, or enterprise like WAAD, Google Apps, AD, ADFS or any SAML Provider." \
  --publisher="Auth0" \
  --website="http://auth0.com" \
  --details="Details.md" \
  --license="License.md" \
  --getting-started="GettingStarted.md" \
  --icon="icons/Auth0Client_128x128.png" \
  --icon="icons/Auth0Client_512x512.png" \
  --library="ios":"src/Auth0Client.iOS/bin/classic/Release/Auth0Client.iOS.dll" \
  --library="ios-unified":"src/Auth0Client.iOS/bin/unified/Release/Auth0Client.iOS.dll" \
  --library="android":"src/Auth0Client.Android/bin/Release/Auth0Client.Android.dll" \
  --library="ios":"src/Auth0Client.iOS/bin/classic/Release/Xamarin.Auth.iOS.dll" \
  --library="ios-unified":"src/Auth0Client.iOS/bin/unified/Release/Xamarin.Auth.iOS.dll" \
  --library="android":"src/Auth0Client.Android/bin/Release/Xamarin.Auth.Android.dll" \
  --library="ios":"src/Auth0Client.iOS/bin/classic/Release/Newtonsoft.Json.dll" \
  --library="ios-unified":"src/Auth0Client.iOS/bin/unified/Release/Newtonsoft.Json.dll" \
  --library="android":"src/Auth0Client.Android/bin/Release/Newtonsoft.Json.dll" \
  --sample="iOS Sample (Classic). Demonstrates Auth0 authentication on iOS (Classic).":"./Xamarin.Auth0Client.iOS.Sample-Classic.sln" \
  --sample="iOS Sample (Unified API). Demonstrates Auth0 authentication on iOS (Unified API).":"./Xamarin.Auth0Client.iOS.Sample.sln" \
  --sample="Android Sample. Demonstrates Auth0 authentication on Android.":"./Xamarin.Auth0Client.Android.Sample.sln"
```

> For detailed instructions, see the [component packaging guidelines](https://components.xamarin.com/guidelines).

* This will create a component package named __Auth0Client-X.Y.Z.xam__, which will include libraries and samples for iOS and Android, along with the required supplementary files.
* You are ready to upload the new `Auth0Client-X.Y.Z.xam` package to https://components.xamarin.com/admin/new/877

## Issue Reporting

If you have found a bug or if you have a feature request, please report them at this repository issues section. Please do not report security vulnerabilities on the public GitHub issue tracker. The [Responsible Disclosure Program](https://auth0.com/whitehat) details the procedure for disclosing security issues.
