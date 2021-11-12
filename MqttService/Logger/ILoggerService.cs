using MQTTnet.Server;

namespace MqttService
{
    public partial class Bootstrapper
    {
        public interface ILoggerService
        {
            public void LogMessage(MqttSubscriptionInterceptorContext context, bool successful);
            public void LogMessage(MqttApplicationMessageInterceptorContext context);
            public void LogMessage(MqttConnectionValidatorContext context, bool showPassword);
            public void LogMemoryInformation(string serviceName);
        }
    }
}
