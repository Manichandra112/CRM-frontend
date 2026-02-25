namespace CRM_Backend.Services.Implementations
{
    using CRM_Backend.Services.Interfaces;
    using System.Security.Cryptography;
    using System.Text;

    public class DeviceFingerprintService : IDeviceFingerprintService
    {
        public string Generate(string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                userAgent = "UNKNOWN";

            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(userAgent));
            return Convert.ToHexString(hash);
        }
    }
}
