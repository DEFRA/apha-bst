namespace Apha.BST.Web.PresentationService
{
    public class LogService : ILogService
    {
        private readonly ILogger<LogService> _logger;

        public LogService(ILogger<LogService> logger)
        {
            _logger = logger;
        }

        public void LogGeneralException(Exception ex, string context)
        {
            _logger.LogError(ex, "[BST.GENERAL_EXCEPTION] Error in [{Context:l}]: {Message}", context, ex.Message);
        }

        public void LogSqlException(Exception ex, string context)
        {
            _logger.LogError(ex, "[BST.SQLException] Error in [{Context:l}]: {Message}", context, ex.Message);
        }
    }
}
