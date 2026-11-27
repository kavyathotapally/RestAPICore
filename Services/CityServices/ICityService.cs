using RestAPICore.Models;
using RestAPICore.Models.Response;

namespace RestAPICore.Services.Interfaces
{
    public interface ICityService
    {
        CityListResponse<CityList> getAgentCityList();

        //  BaseResponse<string[]> getTyCityList();

        CityListResponse<string[]> getTyCityList();

        CityListResponse<AgentCityItem[]> getOTACityList(int? agentId = null);



    }
}