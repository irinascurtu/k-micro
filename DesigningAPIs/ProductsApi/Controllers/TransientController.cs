using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.Service.lifetimes;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransientController : ControllerBase
    {
        // Transient services are created each time they are requested.
        //should be small and stateless, utility classes
        private readonly ITransientService _service1;
        private readonly ITransientService _service2;

        public TransientController(ITransientService service1, ITransientService service2)
        {
            _service1 = service1;
            _service2 = service2;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var id1 = _service1.GetOperationID();
            var id2 = _service2.GetOperationID();

            return Ok(new { id1, id2 });  // id1 and id2 will be different.
        }
    }

}
