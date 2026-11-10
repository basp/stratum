using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Stratum.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
// This class is implicitly instantiated by the test framework.
public class StratumApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}
