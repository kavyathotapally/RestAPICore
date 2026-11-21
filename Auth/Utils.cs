namespace RestAPICore.Auth
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
    }
}

