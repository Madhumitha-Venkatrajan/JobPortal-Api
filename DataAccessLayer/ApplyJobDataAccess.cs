using JobPortalAPI.Model;
using System.Data;
using System.Data.SqlClient;

namespace JobPortalAPI.DataAccessLayer
{
    public class ApplyJobDataAccess : IAppliedJob
    {

        public static string sqlDataSource = @"Data Source=localhost;Initial Catalog=jobPortal;Integrated Security=True";

        public async Task SaveApplyJob(AppliedJob appliedJob)
        {
            string query = "INSERT INTO AppliedJob(JobID,EmailID,Experience,Skills,Resume) " +
                   "VALUES(@JobID,@EmailID,@Experience,@Skills,@Resume)";

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.Int, ParameterName = "@JobID", Value = appliedJob.JobID, });
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@EmailID", Value = appliedJob.EmailID, });
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@Experience", Value = appliedJob.Experience, });
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@Skills", Value = appliedJob.Skills, });
                    cmd.Parameters.AddWithValue("@Resume", SqlDbType.VarBinary).Value = appliedJob.Resume;
                    int rows = await cmd.ExecuteNonQueryAsync();
                }
                conn.Close();
            }
        }
    }
}
