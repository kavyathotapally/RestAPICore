using System.Collections.Generic;

namespace RestAPICore.Repositories.CityRepo
{
    public interface ICityRepository
    {
         Dictionary<string, string> getCachedCityList(int[] countryIds = null);

       
    }
}