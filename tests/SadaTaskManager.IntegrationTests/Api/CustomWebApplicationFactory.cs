using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SadaTaskManager.Infrastructure.Persistence;

namespace SadaTaskManager.IntegrationTests.Api;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
	private readonly string _databaseName = Guid.NewGuid().ToString();

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureServices(services =>
		{
			ServiceDescriptor? dbContextDescriptor = services.SingleOrDefault(
				descriptor => descriptor.ServiceType == typeof(IDbContextOptionsConfiguration<AppDbContext>));

			if (dbContextDescriptor != null)
			{
				services.Remove(dbContextDescriptor);
			}

			services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_databaseName));
		});
	}
}
