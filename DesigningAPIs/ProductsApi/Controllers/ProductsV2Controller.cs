using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProductsApi.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/products")]
    //[Route("api/v{version:apiVersion}/products")]
    [Asp.Versioning.ApiVersion("2")]
    [Asp.Versioning.AdvertiseApiVersions("2")]
    [ApiController]
    public class ProductsV2Controller : ControllerBase
    {
        public ProductsV2Controller()
        {

        }

        [HttpGet]
        [Produces("application/vnd.example.v2+json")]
        [Consumes("application/vnd.example.v2+json")]
        public IActionResult Get()
        {
            return Ok("Version 2");
        }
    }
}
