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
            try
            {
                await _next(context); // Continue down the pipeline
            }
            catch (Exception ex)
            {
                string errorCode;
                string ErrorType = _configuration["ExceptionTypes:General"] ?? "BSTDefaultGeneralException";              
               

                if (ex is UnauthorizedAccessException)
                { 
                    errorCode = "403 - Forbidden";
                    _logger.LogError(ex, "[{ErrorType:l}] Error type [{ErrorCode:l}]: {Message}", ErrorType, errorCode, ex.Message);                    
                    context.Response.Redirect("/Error/AccessDenied");
                    return;
                }

                else if (ex is SqlException)
                {
                    ErrorType = _configuration["ExceptionTypes:Sql"] ?? "BSTDefaultGeneralException";
                    errorCode = "500 - SQL Server Error";
                }
                else
                {
                    errorCode = "500 - Internal Server Error";
                }
                _logger.LogError(ex, "[{ErrorType:l}] Error type [{ErrorCode:l}]: {Message}", ErrorType, errorCode, ex.Message);


                // Redirect to generic error page (no code in query string)
                context.Response.Redirect("/Error");

            }
        }
    }
}
