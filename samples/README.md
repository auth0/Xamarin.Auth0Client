# How to run the samples?

### Samples should work out of the box because they are using DEMO API keys. 

---

If you want to try with your own credentials you should:

1. Go to <https://developers.auth0.com>
2. Signup using GitHub, Google or Microsoft Account.
3. Choose your subdomain (e.g. mycompany.auth0.com).
4. Create a new application from Apps / APIs section.
5. Copy paste the client id / client secret used to initialize the `Auth0Client` class on `MainActivity.cs` or the iOS Controller.
6. Register the following Callback URL on the Application Settings section:

```
https://{YOUR_SUBDOMAIN}.auth0.com/mobile
```
