using Microsoft.AspNetCore.Mvc;

namespace Api_Macoratti.Controllers
{
    // [ApiVersion("1.0", Deprecated = true)] // versão obsoleta -> aparece no header
    // [ApiVersion("1.0")]
    // [ApiVersion("3.0")]
    // [Route("api/v{v:apiVersion}/teste")] // -> versao informada no startup
    // [Route("api/teste")] // -> acesso pelo header do postman
    [Route("api/teste1")]
    [ApiController]
    public class TesteV1Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("<html><body><h2>Teste v1 Controller - versao 1.0 </h2></body></html>", "text/html");
        }
        // [HttpGet, MapToApiVersion("3.0")] // https://localhost:5001/api/v3/teste
        // public IActionResult GetVersao2()
        // {
        //     return Content("<html><body><h2>Teste v1 Controller - versao 3.0 </h2></body></html>", "text/html");
        // }
    }
}