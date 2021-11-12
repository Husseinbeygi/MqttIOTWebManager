using DeviceManagment.Domain.Device;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using MqttService.Configuration;
using MqttService.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MqttService
{
    public partial class Bootstrapper : BackgroundService
    {
        
        private readonly ILoggerService _logger;
        private readonly MqttConfiguration _mqttConfiguration;
        private static double ByteToKB => 1048576.0;
        private readonly string serviceName;

        public Bootstrapper(MqttConfiguration mqttConfiguration, string serviceName)
        {
            _mqttConfiguration = mqttConfiguration;
            this.serviceName = serviceName;
            _logger = LoggerServiceFactory.LoggerService();
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await LogHealthMonitor(cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred: {Exception}", ex);
                }
            }
        }
        private async Task LogHealthMonitor(CancellationToken cancellationToken)
        {
            this.LogMemoryInformation();
            await Task.Delay(_mqttConfiguration.DelayInMilliSeconds, cancellationToken);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            GuardForInvalidMqttConfiguration();

            ConfigAndStartMqttService();

            await base.StartAsync(cancellationToken);
        }

        private void GuardForInvalidMqttConfiguration()
        {
            if (!_mqttConfiguration.IsValid())
            {
                throw new Exception("The Configuration is not Vaild.");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

        }
        private void ConfigAndStartMqttService()
        {
            var options = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(_mqttConfiguration.Port)
                .WithEncryptedEndpointPort(_mqttConfiguration.TlsPort)
                .WithConnectionValidator(ClientValidator())
                .WithSubscriptionInterceptor(SubscriptionInterceptor())
                .WithApplicationMessageInterceptor(ApplicationMessageInterceptor());

            var mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.StartAsync(options.Build());

        }

        private Action<MqttApplicationMessageInterceptorContext> ApplicationMessageInterceptor()
        {
            return a =>
            {
                a.AcceptPublish = true;
                _logger.LogMessage(a);

            };
        }

        private Action<MqttSubscriptionInterceptorContext> SubscriptionInterceptor()
        {
            return s =>
            {
                s.AcceptSubscription = true;
                _logger.LogMessage(s, false);

            };
        }

        private Action<MqttConnectionValidatorContext> ClientValidator()
        {
            return v =>
            {
                var UsersList = new List<User>();
                UsersList.Add(new User("pub", "123"));

                var CurrentUser = UsersList.FirstOrDefault(x => x.UserName == v.Username);
                if (CurrentUser == null)
                {
                    BadUseNameorPassword(v);
                    return;
                }

                if (v.Username != CurrentUser.UserName)
                {
                    BadUseNameorPassword(v);
                    return;
                }

                if (v.Password != CurrentUser.Password)
                {
                    BadUseNameorPassword(v);
                    return;
                }

                v.ReasonCode = MqttConnectReasonCode.Success;
                _logger.LogMessage(v, false);

            };
        }

        private void BadUseNameorPassword(MqttConnectionValidatorContext v)
        {
            v.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            _logger.LogMessage(v, true);
        }

        private void LogMemoryInformation()
        {

            _logger.LogMemoryInformation(this.serviceName);
        }

    }
}
