using JobPortalAPI.Model;

namespace JobPortalAPI.DataAccessLayer
{
    public interface IAppliedJob
    {
        public Task SaveApplyJob(AppliedJob appliedJob);

    }
}
