
using RestAPICore.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestAPICore.DB
{
    public class SubAgentUser
    {
        public string userId { get; set; }
        public string password { get; set; }
        public string username { get; set; }
    }
    public class SubAgentUserCommDetails
    {
        public decimal commPct { get; set; }
        public decimal commPctLower { get; set; }
        public int commissionRule { get; set; }
        public int visibility { get; set; }
    }
    
}
