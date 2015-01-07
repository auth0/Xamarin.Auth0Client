using System.Threading.Tasks;

namespace Auth0.SDK
{
    public interface IDeviceIdProvider
    {
        /// <summary>
        /// Generates a unique identifier for the device and returns it.
        /// </summary>
        /// <returns>A task in representation of the unique id</returns>
        /// <remarks>Given a single device the returned value should always be the same. 
        /// Given any two devices the values should be different.</remarks>
        Task<string> GetDeviceId();
    }
}