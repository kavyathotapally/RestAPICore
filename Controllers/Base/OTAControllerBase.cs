using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestAPICore.Controllers.BaseControllers
{
    [EnableCors("AllowAll")]
    [Route("ota")]
    public abstract class OtaControllerBase : ControllerBase
    {
    }
}
