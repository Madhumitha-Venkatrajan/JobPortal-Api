using JobPortalAPI.Model;

namespace JobPortalAPI.Handler
{
    public interface IRefereshTokenGenerator
    {
        public Task<string> GenerateToken(string emailID, string roleName, string finaltoken);
        public Task<bool> GetRefreshToken(string emailID, string refreshtoken);


    }
}
