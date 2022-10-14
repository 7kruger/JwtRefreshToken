using JwtRefreshToken.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JwtRefreshToken.Models.Repositories;

public class UserServiceRepository : IUserServiceRepository
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly ApplicationDbContext _db;

	public UserServiceRepository(ApplicationDbContext db)
	{
		this._db = db;
	}

	public UserRefreshToken AddUserRefreshTokens(UserRefreshToken user)
	{
		_db.UserRefreshToken.Add(user);
		return user;
	}

	public void DeleteUserRefreshTokens(string username, string refreshToken)
	{
		var item = _db.UserRefreshToken.FirstOrDefault(x => x.UserName == username && x.RefreshToken == refreshToken);
		if (item != null)
		{
			_db.UserRefreshToken.Remove(item);
		}
	}

	public UserRefreshToken GetSavedRefreshTokens(string username, string refreshToken)
	{
		return _db.UserRefreshToken.FirstOrDefault(x => x.UserName == username && x.RefreshToken == refreshToken && x.IsActive == true);
	}

	public int SaveCommit()
	{
		return _db.SaveChanges();
	}

	public async Task<bool> IsValidUserAsync(User users)
	{
		//var u = _userManager.Users.FirstOrDefault(o => o.UserName == users.Name);
		//var result = await _userManager.CheckPasswordAsync(u, users.Password);
		var user = await _db.Users.FirstOrDefaultAsync(u => u.Name == users.Name && u.Password == users.Password);
		return user != null ? true : false;
	}
}
