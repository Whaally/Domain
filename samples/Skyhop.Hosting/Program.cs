using JasperFx.Events.Daemon;
using Marten;
using Serilog;
using Whaally.Domain;
using Whaally.Domain.Infrastructure.OrleansHost;

namespace Skyhop.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            var builder = WebApplication
                .CreateBuilder(args);

            var flags = builder
                .Configuration
                .GetSection("Flags")
                .Get<Flags>();
            
            /*
             * The `AddDomain` call registers all required classes with the service collection, while also discovering
             * and registering any user-implemented domain components such as commands, events, services and sagas.
             *
             * Normally this works correctly when a type from the `Skyhop.Domain` library had been referenced, but as
             * that isn't the case, the library is not available through AppDomain.CurrentDomain.GetAssemblies() either.
             * To work around this we can either:
             * 
             * 1. Manually load this assembly ahead of time
             * 2. Manually load all relevant parts
             *
             * I'm lazy so I'd rather write a single line of code.
             */

            builder.Services.AddDomain(
                options =>
                {
                    options.Assembly = "Skyhop.Domain";
                    options.AggregateHandlerFactory = services =>
                        new OrleansAggregateHandlerFactory(services.GetRequiredService<IClusterClient>());
                });
            
            /*
             * Add Marten for event persistence and projections.
             */
            var martenBuilder = builder.Services.AddMarten(options =>
            {
                options.Connection(builder.Configuration.GetConnectionString("PostgreSQL")!);
                options.Events.MetadataConfig.EnableAll();
            });
            
            /*
             * For integration tests we want to disable the projection daemon running with this instance. Instead we'll
             * manually wait for the projection daemon to finish execution during integration tests.
             */
            if (!flags?.DisableProjectionDaemon ?? true)
                martenBuilder.AddAsyncDaemon(DaemonMode.HotCold);

            /*
             * Asp.net stuff. Not that exciting.
             */
            builder.Services.AddMvc();
            
            builder.Services.AddOrleans(siloBuilder =>
            {
                siloBuilder.UseLocalhostClustering();
                
                siloBuilder
                    .AddActivityPropagation()
                    .AddCustomStorageBasedLogConsistencyProvider()
                    .AddMemoryGrainStorage("PubSubStore");
            });

            var app = builder
                .Build();

            // Construct the definitive logger
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            app.MapControllers();
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unhandled exception occurred during bootstrapping");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
