using RestAPICore.DB;
using RestAPICore.Entities;
using RestAPICore.Models;


namespace RestAPICore.Repositories.CityRepo
{
    public class CityRepository : BaseEntity, ICityRepository
    {

        private readonly IRedisConnectionFactory _redisConnectionFactory;

        public CityRepository(IRedisConnectionFactory redisConnectionFactory) : base("displaycities2")
        {
            _redisConnectionFactory = redisConnectionFactory ?? throw new ArgumentNullException(nameof(redisConnectionFactory));
        }

        

        public Dictionary<string, string> getCachedCityList(int[] countryIds = null)
        {
           
            string cityListVersion = this.getVersion("citylist");
            string cachedCityListVersion = this.getVersion("displaycities2");
            Dictionary<string, string> cachedCityListData = null;
            string redisKey;
          var client = _redisConnectionFactory.GetDatabase(); ;
            cachedCityListData = new Dictionary<string, string>();

            if (countryIds != null && countryIds.Length > 0)
            {
                dynamic itr;
                foreach (int cid in countryIds)
                {
                    if (cid != 1)
                    {
                        redisKey = this.getRedisKey(new string[] { cid.ToString() });
                    }
                    else
                    {
                        // Currently Hardcoded for Indian Cities
                        redisKey = this.getRedisKey(new string[] { });
                    }
                    itr = client.HashScan(redisKey);
                    foreach (var item in itr)
                    {
                        cachedCityListData.Add(item.Name, item.Value);
                    }
                }
            }
            else
            {
                redisKey = this.getRedisKey(new string[] { });
                var itr = client.HashScan(redisKey);
                foreach (var item in itr)
                {
                    cachedCityListData.Add(item.Name, item.Value);
                }
            }


            if (cachedCityListData == null || cachedCityListData.Count == 0)
            {
                throw new Exception("Invalid City List !");
                //result = this.prepareCachedCityList();
                //if (result==null || result.Count==0)
                //{

                //}                
                //versions.setVersion("displaycities2", cityListVersion);
                //List<StackExchange.Redis.HashEntry> hashEntries = new List<StackExchange.Redis.HashEntry>();
                //foreach(var item in result)
                //{
                //    hashEntries.Add(new StackExchange.Redis.HashEntry(item.Key, item.Value));
                //}
                //client.HashSet(redisKey, hashEntries.ToArray());
            }
            else
            {
                return cachedCityListData;
            }
        
        }

     

        public string getVersion(string name)
        {
            string[] eVars = new string[] { };
            string redisKey = getRedisKey(eVars);
            string version = "";
            var client = _redisConnectionFactory.GetDatabase();
            version = client.HashGet(redisKey, name);
            return version;
        }

        
    }
}
