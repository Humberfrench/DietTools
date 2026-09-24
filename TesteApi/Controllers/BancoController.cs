using Microsoft.AspNetCore.Mvc;
using TesteApi.Domain;
using TesteApi.Interfaces;

namespace TesteApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BancoController : ControllerBase
    {
        private readonly IBancoRepository _bancoRepository;
        public BancoController(IBancoRepository bancoRepository)
        {
            _bancoRepository = bancoRepository;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<Banco>> Get()
        {
            var bancos = await _bancoRepository.ObterTodos();

            return bancos;
        }
    }
}
