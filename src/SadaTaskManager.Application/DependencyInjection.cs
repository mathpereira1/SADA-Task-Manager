using Microsoft.Extensions.DependencyInjection;
using SadaTaskManager.Application.Interfaces;
using SadaTaskManager.Application.Services;

namespace SadaTaskManager.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddScoped<ITaskItemService, TaskItemService>();

		return services;
	}
}
