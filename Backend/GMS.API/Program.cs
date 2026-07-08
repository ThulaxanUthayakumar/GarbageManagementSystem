using Serilog;

// Bootstrap logger: captures startup failures before configuration is loaded.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Garbage Management System API");

    var builder = WebApplication.CreateBuilder(args);

    // Full Serilog pipeline, configured from appsettings.json (console + rolling file).
    builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Garbage Management System API",
            Version = "v1",
            Description = "REST API for municipal garbage collection, complaint management, scheduling and reporting."
        });
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "GMS API v1");
        });
    }

    app.UseHttpsRedirection();

    app.MapControllers();

    // Liveness probe - also proves the Phase 1 skeleton runs end to end.
    app.MapGet("/health", () => Results.Ok(new
    {
        status = "Healthy",
        service = "GarbageManagementSystem.API",
        timestampUtc = DateTime.UtcNow
    }))
    .WithTags("Health");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Garbage Management System API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
