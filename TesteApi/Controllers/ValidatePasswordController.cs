using Microsoft.AspNetCore.Mvc;

namespace TesteApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidatePasswordController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hi!");
        }

        [HttpPost]
        public IActionResult Verify(string password)
        {
            return Ok("Hi!");
        }

    }
}
