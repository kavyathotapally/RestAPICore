namespace RestAPICore.Common

{
    public class Utils
    {
        public static DateTime getNow()
        {
            DateTime ISTDate = DateTime.UtcNow + new TimeSpan(hours: 5, minutes: 30, seconds: 0);
            return ISTDate;
        }
        public static string toHexString(int intValue, int placeCount = 4)
        {
            return intValue.ToString('X' + placeCount.ToString()).ToUpper();
        }
        public static int HextoInt(string hexValue)
        {
            return int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
        }


        private string MD5(string password)
        {
            byte[] textBytes = System.Text.Encoding.Default.GetBytes(password);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }
    }
}

