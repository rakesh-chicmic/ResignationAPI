using log4net.Core;
using log4net;
using System.Reflection;
using log4net.Config;

namespace ResignationAPI.Repository.IRepository
{
    public interface ILoggingRepository
    {
        public void LogError(string message);      
    }
}
