using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Auth0.SDK
{
    public class TokenValidator
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static bool HasExpired(string jwt)
        {
            if (jwt == null)
            {
                throw new ArgumentNullException("jwt");
            }

            var parts = jwt.Split(new []{ "." }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3)
            {
                throw new ArgumentException("jwt is not well formed. Check http://jwt.io");
            }

            var partialBody = parts[1];

            // padding
            partialBody = partialBody.PadRight(partialBody.Length + (4 - partialBody.Length % 4) % 4, '='); ;

            var body = Convert.FromBase64String(partialBody);
            var obj = JObject.Parse(Encoding.UTF8.GetString(body, 0, body.Length));
            JToken exp;
            if (obj.TryGetValue("exp", out exp))
            {
                double seconds = exp.Value<double>();
                var expDate = Epoch.AddSeconds(seconds);

                return expDate <= DateTime.UtcNow;
            }

            return false;
        }
    }
}