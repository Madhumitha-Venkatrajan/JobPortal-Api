using JobPortalAPI.Model;
using System.Threading.Tasks;
namespace JobPortalAPI.DataAccessLayer
    
{
    public interface IPersonDataAccess
    {
        public Task<Person> GetPerson(string emailID);
        public Task SavePerson(Person person, string emailID, string password);
        public void Delete();

    }
}
