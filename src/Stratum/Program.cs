using System.Text.Json.Serialization;

namespace Stratum;

using Endpoints;

// ReSharper disable once ClassNeverInstantiated.Global
// This class is implicitly instantiated by the runtime.
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args);
        builder
            .Services
            .AddOpenApi();
        builder
            .Services
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.DefaultIgnoreCondition = 
                    JsonIgnoreCondition.WhenWritingNull;
            });
        
        var app = builder.Build();
        app
            .MapGet("/", LandingPage.Get)
            .WithName(EndpointName.LandingPage);
        app
            .MapGet("/conformance", Conformance.Get) 
            .WithName(EndpointName.Conformance);
        app
            .MapGet("/collections", Collections.Get)
            .WithName(EndpointName.Collections);
        app
            .MapOpenApi("/api")
            .WithName(EndpointName.ServiceDescription);
        
        app.Run();
    }
}
