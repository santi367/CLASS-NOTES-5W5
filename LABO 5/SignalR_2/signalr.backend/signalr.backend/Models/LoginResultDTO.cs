using System.ComponentModel.DataAnnotations;

namespace signalr.backend.Models
{
    public class LoginResultDTO
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }
}
