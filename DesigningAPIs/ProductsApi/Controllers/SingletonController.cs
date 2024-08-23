using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.Service.lifetimes;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SingletonController : ControllerBase
    {
        //a singleton service is created once and shared across all requests, during the lifetime of the application

        private readonly ISingletonService _service1;
        private readonly ISingletonService _service2;

        public SingletonController(ISingletonService service1, ISingletonService service2)
        {
            _service1 = service1;
            _service2 = service2;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var id1 = _service1.GetOperationID();
            var id2 = _service2.GetOperationID();

            return Ok(new { id1, id2 });  // id1 and id2 will always be the same.
        }
    }
}
