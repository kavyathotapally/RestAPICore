using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestAPICore.Controllers.BaseControllers;
using RestAPICore.Models;
using RestAPICore.Models.Response;
using RestAPICore.Services.Interfaces;

namespace RestAPICore.Controllers.TY
{
    [Authorize]
    public class CityTYController : TYControllerBase
    {

        private readonly ICityService _cityService;

        public CityTYController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpPost("CityList")]
        public CityListResponse<string[]> Get()
        {
            CityListResponse<string[]> cityListResponse = _cityService.getTyCityList();
            return cityListResponse;
        }

    }
}
