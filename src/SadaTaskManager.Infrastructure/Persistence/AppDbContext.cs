using Microsoft.EntityFrameworkCore;
using SadaTaskManager.Domain;
using SadaTaskManager.Infrastructure.Persistence.Configurations;

namespace SadaTaskManager.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public DbSet<TaskItem> TaskItems { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new TaskItemConfiguration());
	}
}
