using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LumeAI.Data
{
    // Essa classe é pra configurar a geração das migrations, pois o DbContext não consegue pegar a string de conexão do appsettings.json normalmente nesse projeto de console
    public class LumeAIDataContextFactory : IDesignTimeDbContextFactory<LumeAIDataContext>
    {
        public LumeAIDataContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("CONNECTION_STRING");

            var optionsBuilder = new DbContextOptionsBuilder<LumeAIDataContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new LumeAIDataContext(optionsBuilder.Options);
        }
    }
}
