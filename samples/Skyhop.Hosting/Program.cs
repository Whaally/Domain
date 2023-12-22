using Marten;
using Marten.Events.Daemon.Resiliency;
using Orleans.EventSourcing;
using Orleans.Runtime;
using Serilog;
using Whaally.Domain;
using Whaally.Domain.Abstractions.Aggregate;
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
             */
            builder.Services.AddDomain();
        
            var defaultHandler = builder
                .Services
                .SingleOrDefault(q => q.ServiceType.IsAssignableTo(typeof(IAggregateHandlerFactory)));
            
            if (defaultHandler != null)
                builder.Services.Remove(defaultHandler);
            
            /*
             * This registers a custom AggregateHandlerFactory, retrieving IAggregateHandler instances from Orleans.
             */
            builder.Services.AddSingleton<IAggregateHandlerFactory, OrleansAggregateHandlerFactory>();
        
            /*
             * Add Marten for event persistence and projections.
             */
            var martenBuilder = builder.Services.AddMarten(options =>
            {
                options.Connection(builder.Configuration.GetConnectionString("PostgreSQL")!);
                options.Events.MetadataConfig.EnableAll();
            });
                
            martenBuilder.OptimizeArtifactWorkflow();
            
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
                
                siloBuilder.Services
                    // See https://github.com/dotnet/orleans/issues/8157 for more context
                    .AddSingleton<Factory<IGrainContext, ILogConsistencyProtocolServices>>(serviceProvider =>
                    {
                        var factory = ActivatorUtilities.CreateFactory(typeof(ProtocolServices),
                            new[] { typeof(IGrainContext) });
                        return arg1 => (ILogConsistencyProtocolServices)factory(serviceProvider, new object[] { arg1 });
                    });
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
