namespace JobPortalAPI.Model
{
    public class RefreshTokenModel
    {
        public string EmailID { get; set; }
        public string RoleName { get; set; }
        public string TokenID { get; set; }
        public string RefreshToken { get; set; }

        public bool IsActive { get; set; }


    }
}
