using System.ComponentModel.DataAnnotations;

namespace JobPortalAPI.Model
{
    public class Person
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }
        [Required()]
        public string EmailID { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        public string RoleName { get; set; }


    }

}
