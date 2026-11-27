using static RestAPICore.Auth.Auth;
using RestAPICore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using static RestAPICore.Entities.AgentDetails;
using RestAPICore.Entities;

namespace RestAPICore.Auth
{
    public class AuthPrincipal : IPrincipal
    {
        public static string getClientUser(IPrincipal principal)


        {


            //D9500E8EDF371F68EF6B0F8A1C78D27D | 333 - S | 201706141230 | 0 | FFFF
            if (principal.Identity != null && principal.Identity.Name != null)
            {
                return principal.Identity.Name.Split('|')[1];
            }
            else
            {
                return null;
            }
        }
        public static string getClientId(IPrincipal principal)
        {
            if (principal.Identity != null && principal.Identity.Name != null)
            {

                return principal.Identity.Name.Split('|')[3];
            }
            else
            {
                return null;
            }
        }
        public AuthPrincipal(string strAccessToken)
        {
            AccessToken accessToken = AccessToken.check(strAccessToken);
            if (accessToken == null)
            {
                throw new Exception("Invalid Access Token !");
            }
            this.Identity = new AuthPrincipal.AuthIdentity(accessToken);
        }
        public IIdentity Identity
        {
            get; set;
        }
        public bool IsInRole(string role)
        {
            return true;
        }
        public class AuthIdentity : IIdentity
        {
            public AuthIdentity(AccessToken accessToken)
            {
                this.accessToken = accessToken;
            }
            private AccessToken accessToken;
            private AgentFields objAgentDetails = null;
            string IIdentity.Name
            {
                get
                {
                    return accessToken.clientId;
                }
            }

            string IIdentity.AuthenticationType
            {
                get
                {
                    return "key";
                }
            }

            bool IIdentity.IsAuthenticated
            {
                get
                {
                    return true;
                }
            }
            public string ClientUser
            {
                get
                {
                    return this.accessToken.userId;
                }
            }
            public AgentFields agentDetails
            {
                get
                {
                    if (objAgentDetails == null)
                    {
                        int agentId = Agent.getSubAgentId(this.accessToken.clientId);
                        AgentDetails entAgentDetails = new AgentDetails();
                        objAgentDetails = entAgentDetails.getAgentDetails(agentId);
                    }
                    return objAgentDetails;
                }
            }
            public string channel
            {
                get
                {
                    if (!string.IsNullOrEmpty(this.accessToken.userId))
                    {
                        string[] parts = this.accessToken.userId.Split(' ');
                        return parts.Length > 1 && !string.IsNullOrEmpty(parts[0]) ? parts[0] : null;
                    }
                    return null;
                }
            }

            // wallet username or mobile number
            public string username
            {
                get
                {
                    return accessToken.username;
                }
            }
            public string mobileNo
            {
                get
                {
                    return accessToken.mobileNo;
                }
            }
            public string platform
            {
                get
                {
                    return accessToken.platform;
                }
            }
            public string clientId
            {
                get
                {
                    return accessToken.clientId;
                }
            }

            public string tySource
            {
                get
                {
                    return accessToken.tySource;
                }
            }
            public int tySourceVersion
            {
                get
                {
                    return accessToken.tySourceVersion;
                }
            }
            
        }
    }
}