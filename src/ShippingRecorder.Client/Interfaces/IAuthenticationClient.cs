using System.Threading.Tasks;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IAuthenticationClient
    {
        Task<string> AuthenticateAsync(string username, string password);
        void ClearAuthentication();
        Task<bool> IsTokenValidAsync();
    }
}
