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
		internal EntryElement userNameElement;
		internal EntryElement passwordElement;
		internal MultilineElement resultSectionRow;

		private void Initialize()
		{
			var login1 = new Section ("Login");
			login1.Add (new StyledStringElement ("Login with Widget", this.LoginWithWidgetButtonClick));
			login1.Add (new StyledStringElement ("Login with Google", this.LoginWithConnectionButtonClick));

			var login2 = new Section ("Login with user/password");
			login2.Add (this.userNameElement = new EntryElement ("User", string.Empty, string.Empty));
			login2.Add (this.passwordElement = new EntryElement ("Password", string.Empty, string.Empty, true));
			login2.Add (new StyledStringElement ("Login", this.LoginWithUsernamePassword));

			var result = new Section ("Result");
			result.Add(this.resultSectionRow = new MultilineElement (string.Empty));

			this.Root.Add (login1);
			this.Root.Add (login2);
			this.Root.Add (result);
		}
	}
}
