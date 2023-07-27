using log4net.Core;
using log4net;
using System.Reflection;
using log4net.Config;
using System.Runtime.CompilerServices;

namespace ResignationAPI.Repository.IRepository
{
    public interface ILoggingRepository
    {
        public void LogError(string message, [CallerFilePath] string filename = "");
       
    }
}
