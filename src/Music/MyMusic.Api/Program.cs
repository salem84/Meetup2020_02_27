using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MyMusic.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build()
                //.MigrateDbContext<ApplicationDbContext>((context, services) =>
                //{
                //    var logger = services.GetRequiredService<ILogger<Program>>();
                //    var configuration = services.GetRequiredService<IConfiguration>();
                //    var connectionString = configuration.GetConnectionString("MenuDatabaseConnectionString");
                //    logger.LogInformation(connectionString);
                //})
                .Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
