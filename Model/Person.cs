using System.ComponentModel.DataAnnotations;

namespace JobPortalAPI.Model
{
    public class Person
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }
        
        public string? EmailID { get; set; }
      
        public string? Password { get; set; }
        
        public int RoleID { get; set; }

        public string? RoleName { get; set; }
    }

}
