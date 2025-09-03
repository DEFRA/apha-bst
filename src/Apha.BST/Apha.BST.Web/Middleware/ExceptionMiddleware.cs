using Amazon.Runtime;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;

namespace Apha.BST.Web.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Items.ContainsKey("ExceptionHandled"))
            {
                // Avoid recursive re-entry
                await _next(context);
                return;
            }
            try
            {
                await _next(context); // Continue down the pipeline
            }
            catch (Exception ex)
            {
                context.Items["ExceptionHandled"] = true; // Flag it as handled
                string errorCode;
                string defaultErrorType = "BST.GENERAL_EXCEPTION";
                string errorType = _configuration["ExceptionTypes:General"] ?? defaultErrorType;
               

                if (ex is UnauthorizedAccessException)
                {
                    errorCode = "403 - Forbidden";
                    errorType = _configuration["ExceptionTypes:Authorization"] ?? defaultErrorType;
                    _logger.LogError(ex, "[{ErrorType:l}] Error [{ErrorCode:l}]: {Message}", errorType, errorCode, ex.Message);
                    context.Response.Redirect("/Error/AccessDenied");
                    return;
                }
                else if (ex is AuthenticationFailureException)
                {
                    errorType = _configuration["ExceptionTypes:Authorization"] ?? defaultErrorType;
                    errorCode = "403 - Forbidden";
                }
                else if (ex is SqlException)
                {
                    errorType = _configuration["ExceptionTypes:Sql"] ?? defaultErrorType;
                    errorCode = "500 - SQL Server Error";
                }
                else
                {
                    errorCode = "500 - Internal Server Error";
                }
                _logger.LogError(ex, "[{ErrorType:l}] Error [{ErrorCode:l}]: {Message}", errorType, errorCode, ex.Message);


                await HandleExceptionAsync(context, ex, "/Error", errorCode);

            }

        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string errorPath, string errorCode)
        {
            context.Response.StatusCode = errorCode.StartsWith("403") ? 403 : 500;

            context.Features.Set<IExceptionHandlerFeature>(new ExceptionHandlerFeature
            {
                Error = exception,
                Path = context.Request.Path
            });

            var originalPath = context.Request.Path;
            context.Request.Path = errorPath;

            try
            {
                // Just re-run the middleware pipeline again
                await _next(context);
            }
            finally
            {
                context.Request.Path = originalPath;
            }
        }


    }
}
