using System.ComponentModel.DataAnnotations;

namespace LoginApp.Models
{
    public class RegisterModel
    {
        [Required] public string FirstName { get; set; } = "";
        [Required] public string LastName { get; set; } = "";
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required] public string Password { get; set; } = "";
        [Required] public string MobileNo { get; set; } = "";
        [Required] public int CountryID { get; set; }
        public string SignupIP { get; set; } = "";
    }
}