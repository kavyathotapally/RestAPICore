using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestAPICore.Controllers.BaseControllers;
using RestAPICore.Entities;
using RestAPICore.Models;
using RestAPICore.Models.Response;
using RestAPICore.Services.Interfaces;

namespace RestAPICore.Controllers.Base
{
    [Authorize]
    public class CityOTAController : OtaControllerBase
    {

        private readonly ICityService _cityService;

        public CityOTAController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpPost("CityList")]
        public CityListResponse<AgentCityItem[]> Get()
        {
            string? client = User.Identity?.Name;
            int agentId = Agent.getSubAgentId(client);
           
            CityListResponse<AgentCityItem[]> cityListResponse = _cityService.getOTACityList(agentId);
            return cityListResponse;
        }

    }
}
