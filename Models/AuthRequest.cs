using System.ComponentModel.DataAnnotations;

namespace RestAPICore.Models
{
    public class AuthRequest
    {
        
        public int ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; } = string.Empty;

        public string? UserId { get; set; } = null;
    }

    public class AuthAgentResponse
    {
        public string? accessToken { get; set; }
      //  public _OldAuth? OldAuth { get; set; }
        

    }
}
