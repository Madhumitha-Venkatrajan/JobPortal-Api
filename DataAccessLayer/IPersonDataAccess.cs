using JobPortalAPI.Model;
using System.Threading.Tasks;
namespace JobPortalAPI.DataAccessLayer
    
{
    public interface IPersonDataAccess
    {
        public Task<Person> GetPerson(string emailID);
        public Task SavePerson(Person person, string emailID, string password);
        public Task PersonDetails(Person person);
        public Task UpdatePersonDetails(Person person);

        public Task<bool> DoesUserHaveProfileDetails(string emailID);
        public Task<Person> GetPersonDetails(string emailID);

        public void Delete();

    }
}
