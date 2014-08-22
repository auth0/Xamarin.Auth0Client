// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using System;
using System.Drawing;
using Auth0.SDK;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Auth0Client.iOS.Sample
{
	[Register ("Auth0Client_iOS_SampleViewController")]
	partial class Auth0Client_iOS_SampleViewController
	{
		internal LoadingOverlay loadingOverlay;
		internal EntryElement userNameElement;
		internal EntryElement passwordElement;
		internal StyledMultilineElement resultElement;

		private void Initialize()
		{
			var loginWithWidgetBtn = new StyledStringElement ("Login with Widget", this.LoginWithWidgetButtonClick) {
				Alignment = UITextAlignment.Center
			};

			var loginWithConnectionBtn = new StyledStringElement ("Login with Google", this.LoginWithConnectionButtonClick) {
				Alignment = UITextAlignment.Center
			};

			var loginBtn = new StyledStringElement ("Login", this.LoginWithUsernamePassword) {
				Alignment = UITextAlignment.Center
			};

			this.resultElement = new StyledMultilineElement (string.Empty, string.Empty, UITableViewCellStyle.Subtitle);

			var login1 = new Section ("Login");
			login1.Add (loginWithWidgetBtn);
			login1.Add (loginWithConnectionBtn);

			var login2 = new Section ("Login with user/password");
			login2.Add (this.userNameElement = new EntryElement ("User", string.Empty, string.Empty));
			login2.Add (this.passwordElement = new EntryElement ("Password", string.Empty, string.Empty, true));
			login2.Add (loginBtn);

			var result = new Section ("Result");
			result.Add(this.resultElement);

			this.Root.Add (new Section[] { login1, login2, result });
		}
	}
}
