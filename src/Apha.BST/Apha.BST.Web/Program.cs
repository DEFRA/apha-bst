using Apha.BST.Application.Mappings;
using Apha.BST.Application.Validation;
using Apha.BST.DataAccess.Data;
using Apha.BST.Web.Extensions;
using Apha.BST.Web.Mappings;
using Apha.BST.Web.Middleware;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Host.UseSerilog((ctx, lc) =>
    {
        lc.WriteTo.Console();
        string srvpath = ctx.Configuration.GetValue<string>("AppSettings:LosgPath") ?? string.Empty;
        string logpath = $"{(ctx.HostingEnvironment.IsDevelopment() ? "Logs" : srvpath)}\\Logsample.log";
        lc.WriteTo.File(logpath, Serilog.Events.LogEventLevel.Verbose, rollingInterval: RollingInterval.Day);
        //lc.WriteTo.File(logpath, Serilog.Events.LogEventLevel.Error, rollingInterval: RollingInterval.Day);

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

// Add database context
//builder.Services.AddDbContext<WeatherForecastDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("WeatherForecastConnectionString")
//    ?? throw new InvalidOperationException("Connection string 'WeatherForecastConnectionString' not found.")));

builder.Services.AddDbContext<BSTContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BSTConnectionString")
    ?? throw new InvalidOperationException("Connection string 'BSTConnectionString' not found.")));


builder.Services.AddAutoMapper(typeof(EntityMapper).Assembly);
 // Inside builder.Services configuration:
builder.Services.AddAutoMapper(typeof(ViewModelMapper));
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Services and Repositories
builder.Services.AddApplicationServices();

// Register Authentication services
builder.Services.AddAuthenticationServices(builder.Configuration);

var app = builder.Build();

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
/*
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error");
    //// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();

    //Fallback global handler (useful if custom middleware is not present or fails). Usually used only in production
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

            context.Response.ContentType = "application/json";

            switch (exception)
            {
                case BusinessValidationErrorException validationEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(validationEx.Errors);
                    break;

                // You can handle other exception types here

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Status = "error",
                        Message = "An unexpected error occurred.",
                        Details = exception?.Message
                    });
                    break;
            }
        });
    });
    app.UseHsts();
}
*/
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    //pattern: "{controller=Persons}/{action=Index}/{id?}");

app.Run();
