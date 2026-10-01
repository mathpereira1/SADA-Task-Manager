using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SadaTaskManager.Application.Interfaces;
using SadaTaskManager.Infrastructure.Persistence;
using SadaTaskManager.Infrastructure.Repositories;

namespace SadaTaskManager.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("SadaTaskManagerDb"));

		services.AddScoped<ITaskItemRepository, TaskItemRepository>();

		return services;
	}
}
