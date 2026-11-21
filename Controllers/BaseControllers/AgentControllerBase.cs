using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestAPICore.Controllers.BaseControllers
{
    [EnableCors("AllowAll")]
    [Route("agent")]
    public class AgentControllerBase : ControllerBase
    {
    }
}
