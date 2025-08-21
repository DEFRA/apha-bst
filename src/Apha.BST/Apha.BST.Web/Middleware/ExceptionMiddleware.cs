using Microsoft.Data.SqlClient;

namespace Apha.BST.Web.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
                string ErrorType = "BST.GENERAL_EXCEPTION";


                if (ex is UnauthorizedAccessException)
                { 
                    errorCode = "403 - Forbidden";
                    _logger.LogError(ex, "[{ErrorType:l}] Error type [{ErrorCode:l}]: {Message}", ErrorType, errorCode, ex.Message);                    
                    context.Response.Redirect("/Error/AccessDenied");
                    return;
                }

                else if (ex is SqlException)
                {
                    ErrorType = "BST.SQLException";
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
