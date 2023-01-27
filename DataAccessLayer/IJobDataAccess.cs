using JobPortalAPI.Model;

namespace JobPortalAPI.DataAccessLayer
{
    public interface IJobDataAccess
    {
        public Task<List<Job>> GetJobList();
        public Task SaveJobPost(Job job);
    }
}
