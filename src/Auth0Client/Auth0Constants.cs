using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;
using System.Security.Policy;

namespace Auth0.SDK
{
	/// <summary>
	/// All the constants used by the Auth0Client
	/// </summary>
	public class Auth0Constants
	{
        public const string AuthorizeUrl = "https://{0}/authorize?client_id={1}&redirect_uri={2}&response_type=token&connection={3}&scope={4}";
        public const string LoginWidgetUrl = "https://{0}/login?client={1}&redirect_uri={2}&response_type=token&scope={3}";
        public const string ResourceOwnerEndpoint = "https://{0}/oauth/ro";
		public const string AccessTokenEndpoint = "https://{0}/oauth/access_token";
        public const string DelegationEndpoint = "https://{0}/delegation";
        public const string UserInfoEndpoint = "https://{0}/userinfo?access_token={1}";
        public const string DefaultCallback = "https://{0}/mobile";
	}
}
