using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPICore.Controllers.BaseControllers;
using RestAPICore.Models;
using RestAPICore.Models.Requests;
using RestAPICore.Models.Response;
using RestAPICore.Services.Interfaces;

namespace RestAPICore.Controllers.AgentControllers
{
    [Authorize]
    public class CityAgentController: AgentControllerBase
    {

        private readonly ICityService _cityService;

        public CityAgentController(ICityService cityService)
        {
            _cityService = cityService            ;
        }

        [HttpPost("CityList")]
        public CityListResponse<CityList> Get()
        {
            CityListResponse<CityList> cityListResponse = _cityService.getAgentCityList();
            return cityListResponse;
        }
       

    }
}
