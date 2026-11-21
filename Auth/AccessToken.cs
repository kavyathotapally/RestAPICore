using Microsoft.Extensions.Configuration;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace RestAPICore.Auth
{
    public class AccessToken
    {
        /// <summary>
        /// Check accessToken Validity and expiry
        /// </summary>
        /// <param name="inputAccessToken"></param>
        /// <returns></returns>
        public static AccessToken check(string inputAccessToken)
        {
            AccessToken accessToken = new AccessToken(inputAccessToken);
            if (accessToken.checkValidity() && accessToken.checkExpiry())
            {
                return accessToken;
            }
            else
            {
                return null;
            }

        }
        /// <summary>
        /// Create new accessToken for the given `clientId` and `userId` based on `permsCode`
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <param name="permsCode"></param>
        /// <returns></returns>
        public static AccessToken Create(
            string clientId
            , string userId = Authorization.ADMIN_USER_ID
            , string permsCode = Authorization.ADMIN_PERMS_CODE)
        {
            AccessToken accessToken = new AccessToken(clientId, userId, permsCode);
            //string strAccessToken = accessToken.getAccessToken();
            //accessToken.acc
            return accessToken;
        }

        public const string DELIM = "|";
        public const string SECONDARY_DELIM = " ";
        private string strClientId;
        private DateTime dtExpiry;
        private string strUserId;
        private string strPermsCode;
        private static string serverSecret = "salty secret";
        private static int expiryMinutes = 7 * 24 * 60;

        public static void Configure(IConfiguration configuration)
        {
            serverSecret = configuration["Auth:ServerSecret"] ?? serverSecret;

            if (!int.TryParse(configuration["Auth:ExpiryMinutes"], out expiryMinutes) || expiryMinutes <= 0)
            {
                expiryMinutes = 7 * 24 * 60;
            }
        }

        // wallet user details
        private string strUsername;
        private string strMobileNo;
        private string strPlatform;

        private string clientHash;
        private AccessToken(string accessToken)
        {
            this.readAccessToken(accessToken);
        }

        private AccessToken(string clientId, string userId, string permsCode)
        {
            this.strClientId = clientId;
            this.strUserId = userId;
            this.strPermsCode = permsCode;
            this.dtExpiry = Utils.getNow() + new TimeSpan(0, getExpiryMinutes(), 0);
        }

        /// <summary>
        /// Publicly Readable Client Id
        /// </summary>
        public string clientId
        {
            get { return this.strClientId; }
        }

        /// <summary>
        /// Publicly Readable User Id
        /// </summary>
        public string userId
        {
            get { return this.strUserId; }
        }

        /// <summary>
        /// Publicly Readable Perms Code
        /// </summary>
        public string permsCode
        {
            get { return this.strPermsCode; }
        }

        /// <summary>
        /// Publicly Readable wallet username
        /// </summary>
        public string username
        {
            get { return this.strUsername; }
        }

        /// <summary>
        /// Publicly Readable wallet mobile number
        /// </summary>
        public string mobileNo
        {
            get { return this.strMobileNo; }
        }

        /// <summary>
        /// Publicly Readable wallet plateform
        /// </summary>
        public string platform
        {
            get { return this.strPlatform; }
        }

        public string tySource { get; set; } = "";
        public int tySourceVersion { get; set; } = 0;
        public string tySessionId { get; set; } = "";



        /// <summary>
        /// Create AccessToken
        ///     1. Generate hash of payload
        ///     2. Add payload inputs to client hash
        /// </summary>
        /// <returns></returns>
        public string getValue()
        {
            string clientHash = this.generateClientHash();
            string[] accessTokenParts = new string[] {
                clientHash
                ,this.strClientId
                ,this.dtExpiry.ToString("yyyyMMddHHmm")
                ,this.strUserId
                ,this.strPermsCode
            };
            return String.Join(DELIM, accessTokenParts);
        }


        private bool checkExpiry()
        {
            //Code to Bypass token time validity for TY. This is placed due to a bug in TY which led to caching of
            //access token on client machine.
            if (this.strClientId == "333-S")
            {
                return true;
                //long tokenTime = Convert.ToInt64(this.dtExpiry.ToString("yyyyMMddHHmm"));
                //if(tokenTime > 201905010000 && tokenTime < 201905120000)
                //{
                //    return true;
                //}
            }

            return Utils.getNow() < this.dtExpiry;
        }
        private bool checkValidity()
        {
            string outputHash = this.generateClientHash();
            return outputHash == this.clientHash;
        }
        private void readAccessToken(string accessToken)
        {
            string[] accessTokenParts = accessToken.Split(DELIM.ToArray());
            if (accessTokenParts.Length > 4)
            {
                this.clientHash = accessTokenParts[0];
                this.strClientId = accessTokenParts[1];
                string strExpiry = accessTokenParts[2];
                int yyyy = Convert.ToInt32(strExpiry.Substring(0, 4));
                int MM = Convert.ToInt32(strExpiry.Substring(4, 2));
                int dd = Convert.ToInt32(strExpiry.Substring(6, 2));
                int HH = Convert.ToInt32(strExpiry.Substring(8, 2));
                int mm = Convert.ToInt32(strExpiry.Substring(10, 2));

                this.dtExpiry = new DateTime(yyyy, MM, dd, HH, mm, 0);
                this.strUserId = accessTokenParts[3];
                this.strPermsCode = accessTokenParts[4];
                try
                {
                    if (this.strClientId == "333-S")
                    {
                        string[] tySourceDetailsArr = this.strUserId.Split(SECONDARY_DELIM.ToArray());
                        if (tySourceDetailsArr.Length == 1)
                        {
                            this.tySource = tySourceDetailsArr[0];
                        }
                        else if (tySourceDetailsArr.Length == 2)
                        {
                            this.tySource = tySourceDetailsArr[0];
                            this.tySessionId = tySourceDetailsArr[1];
                        }
                        else if (tySourceDetailsArr.Length == 3)
                        {
                            this.tySource = tySourceDetailsArr[0];
                            if (!String.IsNullOrEmpty(tySourceDetailsArr[1]))
                            {
                                this.tySourceVersion = Convert.ToInt32(tySourceDetailsArr[1]);
                            }
                            this.tySessionId = tySourceDetailsArr[2];
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Do nothing
                }

                // for wallet username and mobile number
                string mnumber = accessTokenParts.ElementAtOrDefault(5);
                string uname = accessTokenParts.ElementAtOrDefault(6);
                string platform = accessTokenParts.ElementAtOrDefault(7);
                this.strUsername = String.IsNullOrEmpty(uname) ? "" : uname;
                this.strMobileNo = String.IsNullOrEmpty(mnumber) ? "" : mnumber;
                this.strPlatform = String.IsNullOrEmpty(platform) ? "TY" : platform;
            }
            else
            {
                throw new Exception("Invalid Access Token");
            }
        }
        private string generateClientHash()
        {
            //Generating client hash
            MD5 md5 = MD5.Create();
            string[] payloadParts = new string[] {
                this.strClientId
                ,this.dtExpiry.ToString("yyyyMMddHHmm")
                ,this.strUserId
                ,this.strPermsCode
                ,getServerSecret()
            };
            string payload = String.Join(DELIM, payloadParts);
            byte[] bytePayload = Encoding.ASCII.GetBytes(payload);
            byte[] hash = md5.ComputeHash(bytePayload);

            //Convert client hash to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().ToUpper();
        }
        private static string getServerSecret()
        {
            return serverSecret;
        }
        private static int getExpiryMinutes()
        {
            return expiryMinutes;
        }

    }
}

