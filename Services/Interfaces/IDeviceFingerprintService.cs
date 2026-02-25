namespace CRM_Backend.Services.Interfaces
{
    public interface IDeviceFingerprintService
    {
        string Generate(string userAgent);
    }
}
