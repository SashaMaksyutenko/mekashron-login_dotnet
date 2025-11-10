using System.ComponentModel.DataAnnotations;

namespace LoginApp.Models
{
    public class LoginModel
    {
        [Required] public string Username { get; set; } = "";
        [Required] public string Password { get; set; } = "";
        public string IPs { get; set; } = "";
    }
}