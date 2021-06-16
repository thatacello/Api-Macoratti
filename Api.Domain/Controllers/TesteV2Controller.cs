using Microsoft.AspNetCore.Mvc;

namespace Api_Macoratti.Controllers
{
    // https://localhost:5001/api/v2/teste
    // [ApiVersion("2.0")]
    // [Route("api/v{v:apiVersion}/teste")] // -> versao informada no startup
    // [Route("api/teste")]
    [Route("api/teste2")]
    [ApiController]
    public class TesteV2Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("<html><body><h2>Teste v2 Controller - versao 2.0 </h2></body></html>", "text/html");
        }
    }
}