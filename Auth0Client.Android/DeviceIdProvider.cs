using System;
using Android.Provider;
using System.Threading.Tasks;
using Android.Accounts;
using Android.OS;

namespace Auth0.SDK
{
	public class DeviceIdProvider : IDeviceIdProvider
	{
		public Task<string> GetDeviceId()
		{
			return Task.FromResult<string>(Build.Fingerprint);
		}
	}
}

