using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.Service.lifetimes;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScopedController : ControllerBase
    {
        // Scoped services are created once per request.
        //should be stateful, such as a database context
        //when we need to maintain state across the request
        private readonly IScopedService _service1;
        private readonly IScopedService _service2;

        public ScopedController(IScopedService service1, IScopedService service2)
        {
            _service1 = service1;
            _service2 = service2;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var id1 = _service1.GetOperationID();
            var id2 = _service2.GetOperationID();

            return Ok(new { id1, id2 });  // id1 and id2 will be the same within a single request.
        }
    }
}
