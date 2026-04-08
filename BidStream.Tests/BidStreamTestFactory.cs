using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BidStream.Data;
using Microsoft.Extensions.Configuration;

namespace BidStream.Tests;
public class BidStreamTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "JwtSettings:SecretKey", "SuperSecretTestKeyThatIsAtLeast32BytesLong!" },
                { "JwtSettings:Issuer", "TestIssuer" },
                { "JwtSettings:Audience", "TestAudience" },
                { "JwtSettings:ExpiryMinutes", "15" }
            });
        });

        builder.ConfigureServices(services =>
        {
            // 1. Find the real MariaDB DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            // 2. Remove it so we don't hit the real database
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // 3. Add a fresh In-Memory database for testing
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("BidStreamTestDb");
            });
        });
    }
}