using Apha.BST.Application.Mappings;
using Apha.BST.Application.Validation;
using Apha.BST.DataAccess.Data;
using Apha.BST.Web.Extensions;
using Apha.BST.Web.Mappings;
using Apha.BST.Web.Middleware;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsEnvironment("local"))
{
    builder.Host.UseSerilog((ctx, lc) =>
    {
        lc.WriteTo.Console();
        string srvpath = ctx.Configuration.GetValue<string>("AppSettings:LogsPath") ?? string.Empty;
        string logpath = $"{("Logs")}\\Logsample.log";
        lc.WriteTo.File(logpath, Serilog.Events.LogEventLevel.Verbose, rollingInterval: RollingInterval.Day);
    });
}
else
{
    Serilog.Debugging.SelfLog.Enable(Console.Error);
    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration.UseAwsCloudWatch(builder.Configuration);
    });
}

builder.Services.AddDbContext<BstContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BSTConnectionString")
    ?? throw new InvalidOperationException("Database Connection string 'BSTConnectionString' not found.")));


builder.Services.AddAutoMapper(typeof(EntityMapper).Assembly);
 // Inside builder.Services configuration:
builder.Services.AddAutoMapper(typeof(ViewModelMapper));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Services and Repositories
builder.Services.AddApplicationServices();

// Register Authentication services
builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services.AddHealthChecks();
var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request Scheme: {context.Request.Scheme}");
    Console.WriteLine($"X-Forwarded-Proto Header: {context.Request.Headers["X-Forwarded-Proto"]}");
    await next();
});

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();

}
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapHealthChecks("/health");

// Middleware to log request headers
app.Use(async (context, next) =>
{
    var logObject = new
    {
        Tag = "RequestLog", // Static text for easy CloudWatch search
        Method = context.Request.Method,
        Path = context.Request.Path.ToString(),
        Headers = context.Request.Headers
            .Where(h => !string.Equals(h.Key, "Cookie", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(h => h.Key, h => h.Value.ToString())
    };

    // Serialize to JSON (compact)..
    var json = JsonSerializer.Serialize(logObject);

    Console.WriteLine(json); // One row in CloudWatch
    await next();
});


await app.RunAsync();
