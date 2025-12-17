using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sudan_Train.MessagingApi.BackgroundServices;
using Sudan_Train.MessagingApi.Configuration;
using Sudan_Train.MessagingApi.Data;
using Sudan_Train.MessagingApi.Services;
using Sudan_Train.MessagingApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Sudan Train Messaging API",
        Version = "v1",
        Description = "API for managing email, SMS, and push notifications"
    });
});

// Configure Database Context (SQL Server)
builder.Services.AddDbContext<MessagingDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MessagingDb"));
});

// Configure Settings
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("SmsSettings"));
builder.Services.Configure<PushSettings>(builder.Configuration.GetSection("PushSettings"));

// Register Core Services
builder.Services.AddSingleton<IMessageQueueService, RabbitMQService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IPushNotificationService, PushNotificationService>();
builder.Services.AddScoped<IMessageTrackingService, MessageTrackingService>();

// Register Background Services (Consumers)
builder.Services.AddHostedService<EmailConsumerService>();
builder.Services.AddHostedService<SmsConsumerService>();
builder.Services.AddHostedService<PushConsumerService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks();

// Configure host options to prevent crash on background service failure
builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Messaging API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Log startup
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Sudan Train Messaging API starting...");
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);

app.Run();
