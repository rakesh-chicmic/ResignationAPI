using log4net.Config;
using log4net.Core;
using log4net;
using System.Reflection;
using ResignationAPI.Repository.IRepository;
using System.Runtime.CompilerServices;

namespace ResignationAPI.Repository
{
    public class LoggingRepository : ILoggingRepository
    {
        // log4net
        public void LogError(string message, [CallerFilePath] string filename = "")
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            ILog _logger = LogManager.GetLogger(filename);
            _logger.Error(message);
        }

    }
}
