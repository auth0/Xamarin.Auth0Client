using System;
using System.Threading.Tasks;
using MonoTouch.UIKit;

namespace Auth0.SDK
{
	public class DeviceIdProvider : IDeviceIdProvider
	{
		public Task<string> GetDeviceId ()
		{
			return Task.FromResult<string>(UIDevice.CurrentDevice.Name);
		}
	}
}

