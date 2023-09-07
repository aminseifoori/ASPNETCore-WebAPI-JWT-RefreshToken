using ASPNETCore_WebAPI_JWT_RefreshToken.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Users;

namespace ASPNETCore_WebAPI_JWT_RefreshToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService service;

        public UsersController(IUserService _Service)
        {
            service = _Service;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
        {
            if(ModelState.IsValid)
            {
                var result = await service.RegisterUser(createUserDto);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return StatusCode(201);
            }
            return new UnprocessableEntityObjectResult(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDto user)
        {
            if (ModelState.IsValid)
            {
                if (!await service.ValidateUser(user))
                    return Unauthorized();
                var tokenDto = await service.CreateToken(true);
                return Ok(tokenDto);
            }
            return new UnprocessableEntityObjectResult(ModelState);
        }
    }
}
