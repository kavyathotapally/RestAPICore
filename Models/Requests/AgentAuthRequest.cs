namespace RestAPICore.Models.Requests
{
    public class AgentAuthRequest
    {
        public string LoginId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public int? SubAgentId { get; set; }
    }
}
