using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class LoginSuccessDTO
    {
        [Required]
        public string Token { get; set; } = "";
    }
}
