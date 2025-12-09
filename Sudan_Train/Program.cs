using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SchoolProject.Core.MiddleWare;
using Serilog;
using Sudan_Train.Core;
using Sudan_Train.Infrastructure;
using Sudan_Train.Infrastructure.context;
using Sudan_Train.Infrastructure.Seeder;
using Sudan_Train.Service;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connection to SQL Server
builder.Services.AddDbContext<ApplicationDBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("dbcontext"));
});

#region Dependency Injections

// Service Registration
builder.Services.AddInfrastructureDependencies()
                .AddServiceDependencies(builder.Configuration)
                .AddCoreDependencies()
                .AddServiceRegisteration(builder.Configuration);

#endregion

#region Localization

// Localization
builder.Services.AddControllersWithViews();
builder.Services.AddLocalization(opt => { opt.ResourcesPath = ""; });

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    List<CultureInfo> supportedCultures = new List<CultureInfo>
    {
        new CultureInfo("en-US"),
        new CultureInfo("ar-EG")
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

#endregion

#region CORS

var CORS = "_cors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORS,
        policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin();
        });
});

#endregion

#region Serilog

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Services.AddSerilog();

#endregion

var app = builder.Build();

#region Database Initialization & Seeding

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var databaseSeeder = services.GetRequiredService<DatabaseSeeder>();
        await databaseSeeder.InitializeAsync();

        var roleSeeder = services.GetRequiredService<RoleSeeder>();
        await roleSeeder.SeedAsync();

        var userSeeder = services.GetRequiredService<UserSeeder>();
        await userSeeder.SeedAsync();

        Log.Information("Database initialization and seeding completed successfully.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while initializing the database.");
    }
}

#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect root to Swagger in Development
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

#region Localization Middleware
var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(options!.Value);
#endregion

app.UseMiddleware<ErrorHandlerMiddleware>(); // Error Handling Middleware

app.UseHttpsRedirection();

app.UseCors(CORS);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
