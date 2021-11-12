using System;

namespace MqttService.Configuration
{
    public class MqttConfiguration
    {
        public int Port { get; set; } = 1883;
        public int DelayInMilliSeconds { get; set; } = 2000;
        public int TlsPort { get; set; } = 8883;

        public bool IsValid()
        {

            if (Port is <= 0 or > 65535)
            {
                throw new Exception("The port is invalid");
            }

            if (DelayInMilliSeconds <= 0)
            {
                throw new Exception("The heartbeat delay is invalid");
            }

            if (TlsPort is <= 0 or > 65535)
            {
                throw new Exception("The TLS port is invalid");
            }

            return true;
        }


    }
}
