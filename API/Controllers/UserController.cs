using API.Models;
using BLL.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class UserController
	{
        public IIdentityService _identityService;
        public UserController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

		[HttpPost("Register")]
		public async Task<ActionResult<string>> RegisterByPhone([FromBody] RegisterUserRequest registerModel)
		{
			var result = await _identityService.Register(registerModel.Username,
				registerModel.Password,
				registerModel.Contact);
			
			return new ActionResult<string>(result.Id.ToString());
		}
		[HttpGet("Login/{contact}")]
		public async Task<ActionResult<string>> Login(string contact, string password)
		{
			var result = await _identityService.Login(contact, password);

			return new ActionResult<string>(result);
		}
	}
}
