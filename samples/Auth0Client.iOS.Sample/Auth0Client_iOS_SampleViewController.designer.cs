// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Auth0Client.iOS.Sample
{
	[Register ("Auth0Client_iOS_SampleViewController")]
	partial class Auth0Client_iOS_SampleViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITextField txtAccessToken { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtIdToken { get; set; }

		[Action ("loginWithConnectionButtonClick:")]
		partial void loginWithConnectionButtonClick (MonoTouch.Foundation.NSObject sender);

		[Action ("loginWithWidgetButtonClick:")]
		partial void loginWithWidgetButtonClick (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (txtAccessToken != null) {
				txtAccessToken.Dispose ();
				txtAccessToken = null;
			}

			if (txtIdToken != null) {
				txtIdToken.Dispose ();
				txtIdToken = null;
			}
		}
	}
}
