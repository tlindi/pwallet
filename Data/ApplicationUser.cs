using Microsoft.AspNetCore.Identity;
using SimpLN.Data.Entities;

namespace SimpLN.Data;

public class ApplicationUser : IdentityUser
{
	public CloudflareSetting CloudflareSetting { get; set; }
	public string? CustomBolt12 { get; set; }
	public bool UseLnUrlP { get; set; }
	public bool UseLnUrlString { get; set; }
	public bool UseCustomBolt12 { get; set; }
}
