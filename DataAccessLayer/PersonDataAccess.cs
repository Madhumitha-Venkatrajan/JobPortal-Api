using System.Data;
using System.Data.SqlClient;
using JobPortalAPI.Model;
namespace JobPortalAPI.DataAccessLayer
{
    public class PersonDataAccess : IPersonDataAccess
    {
        public static string sqlDataSource = @"Data Source=DESKTOP-EJMKR84\SQLEXPRESS;Initial Catalog=jobPortal;Integrated Security=True";

        public async Task<Person> GetPerson(string emailID)
        {

            string query = "SELECT * FROM PersonWithRole"+
                           " WHERE EmailID = @EmailID";
            Person person = null;

            SqlDataReader myReader;

            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                await myConn.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(query, myConn))
                {
                    myCommand.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@EmailID", Value = emailID });
                    myReader = await myCommand.ExecuteReaderAsync();

                    if (myReader.HasRows)
                    {
                        while (await myReader.ReadAsync())
                        {

                            person = new Person()
                            {
                                Name = myReader["Name"].ToString(),
                                PhoneNumber = myReader["PhoneNumber"].ToString(),
                                EmailID = myReader["EmailID"].ToString(),
                                Password = myReader["Password"].ToString(),
                                RoleName=myReader["RoleName"].ToString()
                            };


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


            return person;
        }
        public async Task SavePerson(Person person,string emailID,string password)
        {

            string query = "INSERT INTO Person(Name,PhoneNumber,EmailID,Password,RoleID) " +
                    "VALUES(@Name,@PhoneNumber,@EmailID,@Password,@RoleID)";


            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@Name", Value = person.Name, });
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@PhoneNumber", Value = person.PhoneNumber, });
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@EmailID", Value = emailID, });
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@Password", Value = password, });
                    cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.Int, ParameterName = "@RoleID", Value = person.RoleID});
                    int rows = await cmd.ExecuteNonQueryAsync();

                }
                conn.Close();
            }
        }

        public void Delete()
        {
            string query = "DELETE FROM Person WHERE Name=''";
            using (SqlConnection conn = new SqlConnection(sqlDataSource))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    int rows = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
    }


}
