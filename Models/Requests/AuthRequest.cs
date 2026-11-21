using System.ComponentModel.DataAnnotations;

namespace RestAPICore.Models.Requests
{
    public class AuthRequest
    {
        
        public int ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; } = string.Empty;

        public string? UserId { get; set; } = null;
    }
}
