using Serilog;
using static MqttService.Bootstrapper;

namespace MqttService.Logger
{
    public class LoggerServiceFactory
    {
        public static ILoggerService LoggerService()
        {
            return new LoggerService(Log.ForContext("Type", nameof(Bootstrapper)));
        }
    }
}
