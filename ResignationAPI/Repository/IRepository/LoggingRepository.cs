using log4net.Config;
using log4net.Core;
using log4net;
using System.Reflection;

namespace ResignationAPI.Repository.IRepository
{
    public class LoggingRepository : ILoggingRepository
    {
        public void LogError(string message)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
            _logger.Info(message);
        }
    }
}
