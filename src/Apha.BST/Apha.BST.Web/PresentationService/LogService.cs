namespace Apha.BST.Web.PresentationService
{
    public class LogService : ILogService
    {
        private readonly ILogger<LogService> _logger;
        private readonly IConfiguration _configuration;

        public LogService(ILogger<LogService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void LogGeneralException(Exception ex, string context)
        {
            string errorCode= "500 - Internal Server Error";
            string generalErrorType = _configuration["ExceptionTypes:General"] ?? "BSTDefaultGeneralException";
            _logger.LogError(ex, "[{ErrorType:l}] Error [{ErrorCode:l}] [{Context:l}]: {Message}", generalErrorType,errorCode ,context, ex.Message);
        }

        public void LogSqlException(Exception ex, string context)
        {
            string errorCode= "500 - SQL Server Error";
            string sqlErrorType = _configuration["ExceptionTypes:Sql"] ?? "BSTDefaultGeneralException";
            _logger.LogError(ex, "[{ErrorType:l}] Error [{ErrorCode:l}] [{Context:l}]: {Message}", sqlErrorType,errorCode,context, ex.Message);
        }
    }
}
