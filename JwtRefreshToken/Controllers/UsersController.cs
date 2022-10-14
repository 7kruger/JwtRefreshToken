using JwtRefreshToken.Models.Entities;
using JwtRefreshToken.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtRefreshToken.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly IJWTManagerRepository jWTManager;
	private readonly IUserServiceRepository userServiceRepository;

	public UsersController(IJWTManagerRepository jWTManager, IUserServiceRepository userServiceRepository)
	{
		this.jWTManager = jWTManager;
		this.userServiceRepository = userServiceRepository;
	}

	[HttpGet]
	public List<string> Get()
	{
		var users = new List<string>
		{
			"Satinder Singh",
			"Amit Sarna",
			"Davin Jon"
		};

		return users;
	}

	[AllowAnonymous]
	[HttpPost]
	[Route("authenticate")]
	public async Task<IActionResult> AuthenticateAsync(User usersdata)
	{
        var validUser = await userServiceRepository.IsValidUserAsync(usersdata);

        if (!validUser)
        {
            return Unauthorized("Incorrect username or password!");
        }

        var token = jWTManager.GenerateToken(usersdata.Name);

		if (token == null)
		{
			return Unauthorized("Invalid Attempt!");
		}

		// saving refresh token to the db
		UserRefreshToken obj = new UserRefreshToken
		{
			RefreshToken = token.RefreshToken,
			UserName = usersdata.Name
		};

		userServiceRepository.AddUserRefreshTokens(obj);
		userServiceRepository.SaveCommit();
		return Ok(token);
	}

	[AllowAnonymous]
	[HttpPost]
	[Route("refresh")]
	public IActionResult Refresh(Token token)
	{
		var principal = jWTManager.GetPrincipalFromExpiredToken(token.AccessToken);
		var username = principal.Identity?.Name;

		//retrieve the saved refresh token from database
		var savedRefreshToken = userServiceRepository.GetSavedRefreshTokens(username, token.RefreshToken);

		if (savedRefreshToken.RefreshToken != token.RefreshToken)
		{
			return Unauthorized("Invalid attempt!");
		}

		var newJwtToken = jWTManager.GenerateRefreshToken(username);

		if (newJwtToken == null)
		{
			return Unauthorized("Invalid attempt!");
		}

		// saving refresh token to the db
		UserRefreshToken obj = new UserRefreshToken
		{
			RefreshToken = newJwtToken.RefreshToken,
			UserName = username
		};

		userServiceRepository.DeleteUserRefreshTokens(username, token.RefreshToken);
		userServiceRepository.AddUserRefreshTokens(obj);
		userServiceRepository.SaveCommit();

		return Ok(newJwtToken);
	}
}
