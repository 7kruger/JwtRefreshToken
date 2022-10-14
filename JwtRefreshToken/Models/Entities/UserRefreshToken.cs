using System.ComponentModel.DataAnnotations;

namespace JwtRefreshToken.Models.Entities;

public class UserRefreshToken
{
	[Key]
	public int Id { get; set; }
	[Required]
	public string UserName { get; set; }
	[Required]
	public string RefreshToken { get; set; }
	public bool IsActive { get; set; } = true;
}
