using JobPortalAPI.Model;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace JobPortalAPI.Handler
{
    public class RefereshTokenGenerator : IRefereshTokenGenerator
    {
        public static string sqlDataSource = @"Server=localhost,1433\\Catalog=JobPortal;Database=JobPortal;User=JobPortalApi;Password=MyLocalhostApi@2023";

        public async Task<string> GenerateToken(string emailID, string roleName,string finaltoken)
        {
            var refreshTokenModel = new RefreshTokenModel { EmailID = emailID, RoleName = roleName, TokenID= finaltoken };
            var randomnumber = new byte[32];
            using (var ramdomnumbergenerator = RandomNumberGenerator.Create())
            {
                ramdomnumbergenerator.GetBytes(randomnumber);
                refreshTokenModel.RefreshToken = Convert.ToBase64String(randomnumber);
                using (SqlConnection conn = new SqlConnection(sqlDataSource))
                {
                    await conn.OpenAsync();
                    if (await DoesUserHaveRefreshToken(emailID))
                    {
                        //update refreshToken
                        await UpdateNewRefreshToken(refreshTokenModel, conn);
                    }
                    else
                    {
                       // refreshTokenModel.TokenID = new Random().Next().ToString();
                        await CreateNewRefreshToken(refreshTokenModel, conn);
                    }
                    conn.Close();
                }
            }
            return refreshTokenModel.RefreshToken;
        }

        private async Task<bool> DoesUserHaveRefreshToken(string emailID)
        {
            string query = "SELECT * FROM AppRefreshToken" +
                           " WHERE EmailID = @EmailID";

            SqlDataReader myReader;

            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                await myConn.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(query, myConn))
                {
                    myCommand.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@EmailID", Value = emailID });
                    myReader = await myCommand.ExecuteReaderAsync();
                    return myReader.HasRows;
                }
                
                myConn.Close();
            }
            return false;
        }

        private static async Task CreateNewRefreshToken(RefreshTokenModel refreshTokenModel, SqlConnection conn)
        {
            string query = "INSERT INTO AppRefreshToken(EmailID,RoleName,TokenID,RefreshToken,IsActive) " +
         "VALUES(@EmailID,@RoleName,@TokenID,@RefreshToken,@IsActive)";

            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@EmailID", Value = refreshTokenModel.EmailID });
            cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@RoleName", Value = refreshTokenModel.RoleName });
            cmd.Parameters.AddWithValue("@TokenID", SqlDbType.VarChar).Value = refreshTokenModel.TokenID;
            cmd.Parameters.AddWithValue("@RefreshToken", SqlDbType.VarChar).Value = refreshTokenModel.RefreshToken;
            cmd.Parameters.Add(new SqlParameter("@IsActive", true));
            int rows = await cmd.ExecuteNonQueryAsync();
        }

        private static async Task UpdateNewRefreshToken(RefreshTokenModel refreshTokenModel, SqlConnection conn)
        {
            string query = "UPDATE AppRefreshToken" +
                " SET RefreshToken = @RefreshToken WHERE EmailID = @EmailID";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@EmailID", Value = refreshTokenModel.EmailID });
            cmd.Parameters.AddWithValue("@RefreshToken", SqlDbType.VarChar).Value = refreshTokenModel.RefreshToken;
            int rows = await cmd.ExecuteNonQueryAsync();
        }


        public async Task<bool> GetRefreshToken(string emailID,string refreshtoken)
        {

            string query = "SELECT * FROM AppRefreshToken" +
                           " WHERE EmailID = @EmailID AND RefreshToken = @RefreshToken";
            SqlDataReader myReader;
            var refreshTokenModel = new RefreshTokenModel();

            using (SqlConnection myConn = new SqlConnection(sqlDataSource))
            {
                await myConn.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(query, myConn))
                {
                    myCommand.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@EmailID", Value = emailID });
                    myCommand.Parameters.Add(new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@RefreshToken", Value = refreshtoken });
                    myReader = await myCommand.ExecuteReaderAsync();
                    return myReader.HasRows;
                }
                myReader.Close();
                myConn.Close();
            }
            return false;
        }
    }
}
