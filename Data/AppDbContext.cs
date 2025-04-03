using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpLN.Data.Entities;

namespace SimpLN.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	public DbSet<CloudflareSetting> CloudflareSettings { get; set; }
	public DbSet<PaymentEntity> Payment { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<CloudflareSetting>()
			.HasOne(cf => cf.ApplicationUser)
			.WithOne(u => u.CloudflareSetting)
			.HasForeignKey<CloudflareSetting>(cf => cf.UserId);

		modelBuilder.Entity<PaymentEntity>()
			.HasOne(bs => bs.ApplicationUser);
	}
}