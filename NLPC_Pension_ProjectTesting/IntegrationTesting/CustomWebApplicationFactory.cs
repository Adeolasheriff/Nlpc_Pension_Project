using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Nlpc_Pension_Project.Infrastructure.AppDbContext;

public class RealDatabaseWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Replace real connection string with test one
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            // Add ApplicationDbContext with real test DB
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    GetConnectionStringFromAppSettings());
            });
        });

        builder.UseEnvironment("Development");
    }

    private static string GetConnectionStringFromAppSettings()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        return configuration.GetConnectionString("DefaultConnection");
    }
}