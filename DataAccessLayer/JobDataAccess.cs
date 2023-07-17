using JobPortalAPI.Model;
using System.Data.SqlClient;
using System.Data;

namespace JobPortalAPI.DataAccessLayer
{
    public class JobDataAccess : IJobDataAccess
    {
        public static string sqlDataSource = @"Server=localhost,1433\\Catalog=JobPortal;Database=JobPortal;User=JobPortalApi;Password=MyLocalhostApi@2023";

        public async Task<List<Job>> GetJobList()
        {
            string query = "SELECT * FROM Job ";
            List<Job> jobList = new List<Job>();

            SqlDataReader myReader;

            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                await myConn.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(query, myConn))
                {
                    //  myCommand.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@EmailID", Value = emailID });
                    myReader = await myCommand.ExecuteReaderAsync();

                    if (myReader.HasRows)
                    {
                        while (await myReader.ReadAsync())
                        {
                            var job = new Job()
                            {
                                CompanyID = Convert.ToInt32(myReader["CompanyID"].ToString()),
                                CompanyName = myReader["CompanyName"].ToString(),
                                JobTitle = myReader["JobTitle"].ToString(),
                                JobType = myReader["JobType"].ToString(),
                                Salary = myReader["Salary"].ToString(),
                                Location = myReader["Location"].ToString(),
                                JobID = Convert.ToInt32(myReader["JobID"].ToString())
                            };
                            jobList.Add(job);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                }
                myReader.Close();
                myConn.Close();
            }
            return jobList;
        }


        public async Task<List<Job>> GetJobListJobProvider()
        {
            string query = "SELECT * FROM Job ";
            List<Job> jobList = new List<Job>();

            SqlDataReader myReader;

            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                await myConn.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(query, myConn))
                {
                    //  myCommand.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@EmailID", Value = emailID });
                    myReader = await myCommand.ExecuteReaderAsync();

                    if (myReader.HasRows)
                    {
                        while (await myReader.ReadAsync())
                        {
                            var job = new Job()
                            {
                                CompanyID = Convert.ToInt32(myReader["CompanyID"].ToString()),
                                CompanyName = myReader["CompanyName"].ToString(),
                                JobTitle = myReader["JobTitle"].ToString(),
                                JobType = myReader["JobType"].ToString(),
                                Salary = myReader["Salary"].ToString(),
                                Location = myReader["Location"].ToString(),
                                JobID = Convert.ToInt32(myReader["JobID"].ToString())
                            };
                            jobList.Add(job);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                }
                myReader.Close();
                myConn.Close();
            }
            return jobList;
        }


        public async Task SaveJobPost(Job job)
        {
            string query = "spSaveJobPost";

            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.Int, ParameterName = "@CompanyID", Value = job.CompanyID, });
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@CompanyName", Value = job.CompanyName, });
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@JobTitle", Value = job.JobTitle, });
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@JobType", Value = job.JobType, });
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@Salary", Value = job.Salary, });
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@Location", Value = job.Location, });

                    int rows = await cmd.ExecuteNonQueryAsync();
                }
                conn.Close();
            }
        }
    }
}
