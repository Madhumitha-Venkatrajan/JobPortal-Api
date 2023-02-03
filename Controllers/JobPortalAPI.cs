using Microsoft.AspNetCore.Mvc;
using System.Data;
using JobPortalAPI.DataAccessLayer;
using JobPortalAPI.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using JobPortalAPI.Handler;
using TokenResponse = JobPortalAPI.Model.TokenResponse;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JobPortalAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class JobPortalAPI : ControllerBase
    {
        private readonly IPersonDataAccess _personDataAccess;
        private readonly IJobDataAccess _JobDataAccess;
        private readonly IAppliedJob _appliedJob;
        private readonly JwtSettings _jwtSettings;
        private readonly IRefereshTokenGenerator _refereshTokenGenerator;


        public JobPortalAPI(IPersonDataAccess personDataAccess, IJobDataAccess JobDataAccess,
            IAppliedJob appliedJob, IOptions<JwtSettings> options, IRefereshTokenGenerator refereshTokenGenerator)
        {
            _personDataAccess = personDataAccess;
            _JobDataAccess = JobDataAccess;
            _appliedJob = appliedJob;
            _jwtSettings = options.Value;
            _refereshTokenGenerator = refereshTokenGenerator;

        }


        // GET: api/<JobPortalAPI>
        [HttpGet("{emailID}")]
        [Authorize(Roles = "Job Seeker")]
        public async Task<Person> Get(string emailID)
        {
            try
            {
                var userInfo = await _personDataAccess.GetPerson(emailID);
                return userInfo;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [NonAction]
        public async Task<TokenResponse> TokenAuthenticate(Claim[] claims)
        {
            var token = new JwtSecurityToken(
      claims: claims,
      expires: DateTime.Now.AddMinutes(1),
      signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.securitykey)), SecurityAlgorithms.HmacSha256)
            );
            var jwttoken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponse() { 
                jwttoken = jwttoken, 
                refreshtoken = await _refereshTokenGenerator.GenerateToken(
                    claims.Single(x => x.Type == ClaimTypes.Email).Value, 
                    claims.Single(x => x.Type == ClaimTypes.Role).Value, 
                    jwttoken) 
            };
        }

        [HttpGet("GetToken")]
        
        public async Task<ActionResult> ValidateUser()
        {
            try
            {
                var emailID = Encoding.GetEncoding("iso-8859-1").GetString(Convert.FromBase64String(Request.Headers["Authorization"].
                    ToString().Substring("Basic ".Length).Trim())).Split(":")[0];
                var password = Encoding.GetEncoding("iso-8859-1").GetString(Convert.FromBase64String(Request.Headers["Authorization"].
                    ToString().Substring("Basic ".Length).Trim())).Split(":")[1];

                var userInfo = await _personDataAccess.GetPerson(emailID);
                if (userInfo != null)
                {
                    if (userInfo.Password == password)
                    {
                        /// Generate Token
                        var tokenhandler = new JwtSecurityTokenHandler();
                        var tokenkey = Encoding.UTF8.GetBytes(_jwtSettings.securitykey);
                        var tokendesc = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(
                    new Claim[] { new Claim(ClaimTypes.Email, userInfo.EmailID), new Claim(ClaimTypes.Role, userInfo.RoleName) }
                                                        ),
                            Expires = DateTime.Now.ToUniversalTime().AddMinutes(1),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
                        };
                        var token = tokenhandler.CreateToken(tokendesc);
                        string finaltoken = tokenhandler.WriteToken(token);

                        var response = new TokenResponse()
                        {
                            jwttoken = finaltoken,
                            refreshtoken = await _refereshTokenGenerator.GenerateToken(emailID, userInfo.RoleName,finaltoken)
                        };

                        return Ok(response);
                    }
                }
                return Unauthorized();
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost("RefToken")]
        public async Task<IActionResult> RefToken([FromBody] TokenResponse tokenResponse)
        {

            /// Generate Token
            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenkey = Encoding.UTF8.GetBytes(_jwtSettings.securitykey);
            SecurityToken securityToken;
            var principal = tokenhandler.ValidateToken(tokenResponse.jwttoken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(tokenkey),
                ValidateIssuer = false,
                ValidateAudience = false

            }, out securityToken);

            var token = securityToken as JwtSecurityToken;
            if (token != null && !token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                return Unauthorized();
            }
            var EmailID = principal.Claims.ToList().FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

            var user = await _refereshTokenGenerator.GetRefreshToken(EmailID, tokenResponse.refreshtoken);
            if (user == null)
                return Unauthorized();

            var response = TokenAuthenticate(principal.Claims.ToArray()).Result;

            return Ok(response);
        }

        [Authorize(Roles = "Job Seeker")]
        [HttpGet]
        [Route("allJobs")]
        public async Task<List<Job>> GetAsync()
        {
            try
            {
                List<Job> jobList = await _JobDataAccess.GetJobList();
                return jobList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        
        // POST api/<JobPortalAPI>
        [HttpPost]
        public async Task<IResult> Post(Person person)
        {
            try
            {
                var existingUser = await _personDataAccess.GetPerson(person.EmailID);
                if (existingUser == null)
                {
                    await _personDataAccess.SavePerson(person);
                    return Results.Created("", person);
                }
                return Results.BadRequest("Email already exists.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        [Authorize(Roles = "Job Provider")]
        // POST api/<JobPortalAPI>
        [HttpPost]
        [Route("postJob")]
        public async Task<IResult> PostJob(Job job)
        {
            try
            {
                await _JobDataAccess.SaveJobPost(job);
                return Results.Created("", job);
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        [Authorize(Roles = "Job Seeker")]
        // POST api/<JobPortalAPI>
        [HttpPost]
        [Route("appliedJobs")]
        public async Task<IResult> Post([FromForm] AppliedJob appliedJob)
        {
            try
            {
                //var appliedJob = JsonConvert.DeserializeObject<AppliedJob>(Request.Form["jobDetails"]);
                var file = Request.Form.Files[0];
                using (MemoryStream stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    appliedJob.Resume = stream.ToArray();
                }

                await _appliedJob.SaveApplyJob(appliedJob);
                return Results.Created("", appliedJob); ;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


        // PUT api/<JobPortalAPI>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }


        [Authorize(Roles = "Admin")]
        // DELETE api/<JobPortalAPI>/5
        [HttpDelete]
        [Route("DeleteNullPerson")]
        public void Delete()
        {
            try
            {
                _personDataAccess.Delete();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
