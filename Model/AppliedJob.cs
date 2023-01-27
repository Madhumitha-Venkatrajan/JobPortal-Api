using System.Text.Json.Serialization;

namespace JobPortalAPI.Model
{
    public class AppliedJob
    {
        public int JobID { get; set; }

        public string EmailID { get; set; }

        public string Experience { get; set;}
        
        public string Skills { get; set;}
        
        public byte[] Resume { get; set; }


    }
}
