namespace RestAPICore.Entities
{
    public class AgentUserDetails : BaseEntity
    {
        public AgentUserDetails() : base("agentuserdetails1")
        {

        }
        public class AgentUserFields
        {
            public int sub_agent_user_id { get; set; }
            public int sub_agent_id { get; set; }
            public string login_id { get; set; }
            public string password { get; set; }
            public string sub_agent_user_name { get; set; }
            public bool is_active { get; set; }
            public bool is_regular_user { get; set; }
            public int commission_rule { get; set; }
            public decimal comm_share_pct { get; set; } // comm_pct
            public decimal comm_cap_pct { get; set; } //lower

        }
    }
    }
