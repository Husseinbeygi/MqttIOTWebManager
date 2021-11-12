using MQTTnet.Server;
using Serilog;
using System;
using System.Text;

namespace MqttService
{
    public partial class Bootstrapper
    {
        public class LoggerService : ILoggerService
        {
            private readonly ILogger _logger;

            public LoggerService(ILogger logger)
            {
                _logger = logger;

            }

            public void LogMessage(MqttSubscriptionInterceptorContext context, bool successful)
            {
                _logger.Information(
                    successful
                        ? "New subscription: ClientId = {clientId}, TopicFilter = {topicFilter}"
                        : "Subscription failed for clientId = {clientId}, TopicFilter = {topicFilter}",
                    context.ClientId,
                    context.TopicFilter);
            }
            public void LogMessage(MqttApplicationMessageInterceptorContext context)
            {
                var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage.Payload);

                _logger.Information(
                    "Message: ClientId = {clientId}, Topic = {topic}, Payload = {payload}, QoS = {qos}, Retain-Flag = {retainFlag}",
                    context.ClientId,
                    context.ApplicationMessage?.Topic,
                    payload,
                    context.ApplicationMessage?.QualityOfServiceLevel,
                    context.ApplicationMessage?.Retain);
            }
            public void LogMessage(MqttConnectionValidatorContext context, bool showPassword)
            {
                if (showPassword)
                {
                    _logger.Information(
                        "New connection: ClientId = {clientId}, Endpoint = {endpoint}, Username = {userName}, Password = {password}, CleanSession = {cleanSession}",
                        context.ClientId,
                        context.Endpoint,
                        context.Username,
                        context.Password,
                        context.CleanSession);
                }
                else
                {
                    _logger.Information(
                        "New connection: ClientId = {clientId}, Endpoint = {endpoint}, Username = {userName}, CleanSession = {cleanSession}",
                        context.ClientId,
                        context.Endpoint,
                        context.Username,
                        context.CleanSession);
                }
            }

            public void LogMemoryInformation(string serviceName)
            {
                var totalMemory = GC.GetTotalMemory(false);
                var memoryInfo = GC.GetGCMemoryInfo();
                var divider = ByteToKB;
                Log.Information(
                    "Heartbeat for service {ServiceName}: Total {Total}, heap size: {HeapSize}, memory load: {MemoryLoad}.",
                    serviceName, $"{(totalMemory / divider):N3}", $"{(memoryInfo.HeapSizeBytes / divider):N3}", $"{(memoryInfo.MemoryLoadBytes / divider):N3}");
            }

        }
    }
}
