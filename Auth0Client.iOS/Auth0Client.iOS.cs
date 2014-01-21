using System;
using System.Drawing;
using System.Threading.Tasks;
using MonoTouch.UIKit;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;

namespace Auth0.SDK
{
	public partial class Auth0Client
	{
		/// <summary>
		/// Log a user into an Auth0 application given a connection name.
		/// </summary>
		/// <param name="viewController" type="MonoTouch.UIKit.UIViewController">
		/// UIViewController used to display modal login UI on iPhone/iPods.
		/// </param>
		/// <param name="connection" type="string">
		/// The name of the connection to use in Auth0. Connection defines an Identity Provider.
		/// </param>
		/// <param name="scope" type="string">
		/// Space delimited, case sensitive list of OAuth 2.0 scope values.
		/// </param>
		/// <returns>
		/// Task that will complete when the user has finished authentication.
		/// </returns>
		public Task<Auth0User> LoginAsync (UIViewController viewController, string connection = "", string scope = "openid")
		{
			return this.SendLoginAsync(default(RectangleF), viewController, connection, scope);
		}

		/// <summary>
		/// Log a user into an Auth0 application given a connection name.
		/// </summary>
		/// <param name="rectangle" type="System.Drawing.RectangleF">
		/// The area in <paramref name="view"/> to anchor to.
		/// </param>
		/// <param name="view" type="MonoTouch.UIKit.UIView">
		/// UIView used to display a popover from on iPad.
		/// </param>
		/// <param name="connection" type="string">
		/// The name of the connection to use in Auth0. Connection defines an Identity Provider.
		/// </param>
		/// <param name="scope" type="string">
		/// Space delimited, case sensitive list of OAuth 2.0 scope values.
		/// </param>
		/// <returns>
		/// Task that will complete when the user has finished authentication.
		/// </returns>
		public Task<Auth0User> LoginAsync (RectangleF rectangle, UIView view, string connection = "", string scope = "openid")
		{
			return this.SendLoginAsync(rectangle, view, connection, scope);
		}

		/// <summary>
		/// Log a user into an Auth0 application given a connection name.
		/// </summary>
		/// <param name="barButtonItem" type="MonoTouch.UIKit.UIBarButtonItem">
		/// UIBarButtonItem used to display a popover from on iPad.
		/// </param>
		/// <param name="connection" type="string">
		/// The name of the connection to use in Auth0. Connection defines an Identity Provider.
		/// </param>
		/// <param name="scope" type="string">
		/// Space delimited, case sensitive list of OAuth 2.0 scope values.
		/// </param>
		/// <returns>
		/// Task that will complete when the user has finished authentication.
		/// </returns>
		public Task<Auth0User> LoginAsync (UIBarButtonItem barButtonItem, string connection = "", string scope = "openid")
		{
			return this.SendLoginAsync(default(RectangleF), barButtonItem, connection, scope);
		}

		private Task<Auth0User> SendLoginAsync(RectangleF rect, object view, string connection, string scope)
		{
			// Launch server side OAuth flow using the GET endpoint
			var tcs = new TaskCompletionSource<Auth0User> ();
			var auth = this.GetAuthenticator (connection, scope);

			UIViewController c = auth.GetUI();

			UIViewController controller = null;
			UIPopoverController popover = null;

			auth.Error += (o, e) =>
			{
				if (controller != null) {
					controller.DismissViewController (true, null);
				}

				if (popover != null) {
					popover.Dismiss (true);
				}

				var ex = e.Exception ?? new AuthException (e.Message);
				tcs.TrySetException (ex);
			};

			auth.Completed += (o, e) =>
			{
				if (controller != null) {
					controller.DismissViewController (true, null);
				}

				if (popover != null) {
					popover.Dismiss (true);
				}

				if (!e.IsAuthenticated) {
					tcs.TrySetCanceled();
				}
				else
				{
					this.SetupCurrentUser (e.Account.Properties);
					tcs.TrySetResult (this.CurrentUser);
				}
			};

			controller = view as UIViewController;
			if (controller != null)
			{
				controller.PresentViewController (c, true, null);
			}
			else
			{
				UIView v = view as UIView;
				UIBarButtonItem barButton = view as UIBarButtonItem;

				popover = new UIPopoverController (c);

				if (barButton != null) {
					popover.PresentFromBarButtonItem (barButton, UIPopoverArrowDirection.Any, true);
				} 
				else {
					popover.PresentFromRect (rect, v, UIPopoverArrowDirection.Any, true);
				}
			}

			return tcs.Task;
		}
	}
}
