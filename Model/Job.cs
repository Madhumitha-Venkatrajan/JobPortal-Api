namespace JobPortalAPI.Model
{
    public class Job
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string FullTime { get; set; }

        public string Salary { get; set; }
        public string Location { get; set; }

        public int JobID { get; set; }
       // public string JobDescription {get; set;}

    }
}
