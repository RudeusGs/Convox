using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using server.Domain.Entities;
using server.Service.Interfaces;
using server.Service.Models.Authenticate;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : BaseController
    {
        private readonly IAuthenticateService _authenticateService;
        private readonly SignInManager<User> _signInManager;
        public AuthenticateController(
            IAuthenticateService authenticateService,
            SignInManager<User> signInManager)
        {
            _authenticateService = authenticateService;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel request)
        {
            var result = await _authenticateService.Register(request);
            return FromApiResult(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel login)
        {
            var result = await _authenticateService.Login(login);
            return FromApiResult(result);
        }
    }
}
