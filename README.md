# Xamarin.Auth0Client

A cross-platform API for authenticating users with the [Auth0 platform](https://developers.auth0.com)

This component builds on top of the [Xamarin.Auth](https://github.com/xamarin/Xamarin.Auth) framework.

Please read [Getting Started](https://github.com/auth0/Xamarin.Auth0Client/blob/master/GettingStarted.md) to learn how to use the library.

## Generate Xamarin Component

Build __Xamarin.Auth0Client.sln__ solution, [download](https://components.xamarin.com/submit/xpkg) the xamarin-component command line tool and run the following script:

    mono xpkg/xamarin-component.exe create-manually Auth0Client-0.1.xam \ 
    --name="Auth0 SDK" \ 
    --summary="Add login with Google, Facebook, Twitter, Amazon, GitHub, LinkedIn; Windows Azure AD, Google Apps, AD, and SAML; or username/pwd database. One API for all." \ 
    --publisher="Auth0" \ 
    --website="http://auth0.com" \ 
    --details="Details.md" \ 
    --license="License.md" \ 
    --getting-started="GettingStarted.md" \ 
    --icon="icons/Auth0Client_128x128.png" \ 
    --icon="icons/Auth0Client_512x512.png" \ 
    --library="ios":"bin/Auth0Client.iOS.dll" \ 
    --library="android":"bin/Auth0Client.Android.dll" \ 
    --sample="iOS Sample. Demonstrates Auth0 authentication on iOS.":"samples/Auth0Client.iOS.Sample.sln" \ 
    --sample="Android Sample. Demonstrates Auth0 authentication on Android":"samples/Auth0Client.Android.Sample.sln"

This will create a component package named __Auth0Client-0.1.xam__, which will include libraries and samples for iOS and Android, along with the required supplementary files.

For detailed instructions, see the [component packaging guidelines](https://components.xamarin.com/guidelines).
