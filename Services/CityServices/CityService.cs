using RestAPICore.DB;
using RestAPICore.Entities;
using RestAPICore.Models;
using RestAPICore.Models.Response;
using RestAPICore.Repositories.CityRepo;
using RestAPICore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace RestAPICore.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        private readonly IRedisConnectionFactory _redisConnectionFactory;

        public CityService(
    ICityRepository cityRepository,
    IRedisConnectionFactory redisConnectionFactory)
        {
            _cityRepository = cityRepository
                ?? throw new ArgumentNullException(nameof(cityRepository));

            _redisConnectionFactory = redisConnectionFactory
                ?? throw new ArgumentNullException(nameof(redisConnectionFactory));
        }


        public CityListResponse<CityList> getAgentCityList()


        {
            CityListResponse<CityList> response = new CityListResponse<CityList>();
            List<AgentCityItem> result = new List<AgentCityItem>();
            Dictionary<string, string> dictionary = _cityRepository.getCachedCityList();
             try
            {
                foreach (var keyVal in dictionary)
                {
                    //var item = listAllCities[i];                    
                    //    var item = Jil.JSON.Deserialize<Domain.Entities.CityDetails.CachedCityDetails>(keyVal.Value);
                    var item = JsonSerializer.Deserialize<CachedCityDetails>(keyVal.Value);
                    
                    string cityId = item.city_id;
                  
                    result.Add(new AgentCityItem()
                    {
                        City = keyVal.Key,
                        CityId = Convert.ToInt32(cityId),
                        IsPackage = item.is_package!=null ? item.is_package.Value==1:false
                    });
                }
                response.Data = new CityList
                {
                    Cities = result.ToArray()
                }; ;
                response.Success = true;
                return response;
            }
        
            catch (Exception ex)
            {
                response.Success = false;
                return response;
            }
            ;
  
            }

       

        public CityListResponse<string[]> getTyCityList()
        {
            CityListResponse<string[]> response2 = new CityListResponse<string[]>();
            try
            {
                List<string> finalResult = new List<string>();

                Dictionary<string, string> dictResult =
                    _cityRepository.getCachedCityList();

                List<string> TopCityList = new List<string>()
        {
            "Delhi", "Pune", "Bengaluru", "Ahmedabad",
            "Chandigarh", "Jaipur", "Indore", "Chennai",
            "Hyderabad", "Mumbai", "Lucknow", "Nagpur"
            // Keep the remaining cities from your original list here
        };

                finalResult.AddRange(TopCityList);

                foreach (var keyVal in dictResult)
                {
                    var item = JsonSerializer.Deserialize<CachedCityDetails>(
                        keyVal.Value);

                    if (item == null)
                        continue;

                    if (item.is_package == null || item.is_package.Value == 0)
                    {
                        if (!TopCityList.Contains(keyVal.Key))
                        {
                            finalResult.Add(keyVal.Key);
                        }
                    }
                }

                response2.Data = finalResult.ToArray();

                response2.Success = true;
            }
            catch (Exception ex)
            {
                response2.Success = false;
                response2.Error = new ErrorDetails
                {
                    Code = 500,
                    Msg = "An error occurred while fetching the city list.",
                    ProvMsg = ex.Message,
                    TraceId = Guid.NewGuid().ToString()
                };

              //  var gdsError = Providers.ErrorMapping.getDefaultError(ex);

                //response2.Error = ErrorDetails();
                //    Code = Convert.ToInt32(gdsError.error),
                //    Msg = gdsError.error.ToString(),
                //    ProvMsg = gdsError.provMsg,
                //    TraceId = gdsError.traceId
                //};
            }

            return response2;
        }

        public CityListResponse<AgentCityItem[]> getOTACityList(int? agentId = null)
        {
            CityListResponse<AgentCityItem[]> res = new CityListResponse<AgentCityItem[]>();
            List<AgentCityItem> result = new List<AgentCityItem>();
            var entityAgentCountry =  new AgentCountryDetails(_redisConnectionFactory);

            try
            {
                var agentCountries = entityAgentCountry.getAgentCountryIds(agentId);
                Dictionary<string, string> dictResult = _cityRepository.getCachedCityList(agentCountries);
                foreach (var keyVal in dictResult)
                {
                    //var item = listAllCities[i];                    
                    var item = JsonSerializer.Deserialize<CachedCityDetails>(keyVal.Value);
                    if (item.is_package == null || item.is_package.Value == 0)
                    {
                        string cityId = item.city_id;
                        
                        result.Add(new AgentCityItem()
                        {
                            State = item.state,
                            City = keyVal.Key,
                            CityId = Convert.ToInt32(item.city_id),
                            IsPackage = null,
                            StateCode = null,
                            LatLong = null
                        });
                    }

                }

                res.Data = result.ToArray();
                res.Success = true;

            }
            catch (Exception ex)
            {
                res.Success = false;


            }
            return res;
        }
    };
}

        

    
    