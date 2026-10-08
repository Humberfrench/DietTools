using Dietcode.Core.Password.Abstractions;
using Dietcode.Core.Password.Models;
using Dietcode.Core.Password.Scoring;
using Microsoft.AspNetCore.Mvc;

namespace TesteApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidatePasswordController : ControllerBase
    {
        private readonly IPasswordValidationService _passwordValidationService;
        private readonly IPasswordSecurityService _passwordSecurityService;

        public ValidatePasswordController(
            IPasswordValidationService passwordValidationService,
            IPasswordSecurityService passwordSecurityService)
        {
            _passwordValidationService = passwordValidationService;
            _passwordSecurityService = passwordSecurityService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hi!");
        }

        // Senha vai no corpo do POST, nunca na query string — ver
        // Dietcode.Core.Password/README.md ("API HTTP própria"): query string
        // pode aparecer em logs, proxies, histórico e APM.
        [HttpPost]
        public async Task<ActionResult<PasswordValidationResult>> Verify(
            [FromBody] ValidatePasswordRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Password))
                return BadRequest("Informe a senha no corpo da requisição (campo \"password\").");

            var context = new PasswordValidationContext
            {
                UserName = request.UserName,
                Email = request.Email
            };

            var resultado = await _passwordValidationService.ValidateAsync(request.Password, context, cancellationToken);

            return Ok(resultado);
        }

        // Só a força por pontuação/entropia — não consulta o HIBP, não sabe
        // se a senha foi vazada.
        [HttpPost("strength")]
        public ActionResult<PasswordStrengthResult> Strength([FromBody] ValidatePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Password))
                return BadRequest("Informe a senha no corpo da requisição (campo \"password\").");

            var resultado = request.Password.AsSpan().AnalyzePassword();

            return Ok(resultado);
        }

        // Integração dos dois: vazamento (HIBP) + regras locais + força por
        // pontuação, numa chamada só.
        [HttpPost("security")]
        public async Task<ActionResult<PasswordSecurityResult>> Security(
            [FromBody] ValidatePasswordRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Password))
                return BadRequest("Informe a senha no corpo da requisição (campo \"password\").");

            var context = new PasswordValidationContext
            {
                UserName = request.UserName,
                Email = request.Email
            };

            var resultado = await _passwordSecurityService.CheckAsync(request.Password, context, cancellationToken);

            return Ok(resultado);
        }
    }

    public sealed class ValidatePasswordRequest
    {
        public string Password { get; set; } = string.Empty;

        public string? UserName { get; set; }

        public string? Email { get; set; }
    }
}
