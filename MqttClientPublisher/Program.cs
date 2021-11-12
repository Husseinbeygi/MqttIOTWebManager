    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Options;
    using Serilog;
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    namespace MqttClientPublisher
    {
        class Program
        {
            static void Main(string[] args)
            {
                // Create TCP based options using the builder.
                var options = new MqttClientOptionsBuilder()
                    .WithClientId("Client1")
                    .WithTcpServer("localhost",1883)
                    .WithCredentials("pub", "123")
                    .WithTls()
                    .WithCleanSession()
                    .Build();
                // Create a new MQTT client.
                var factory = new MqttFactory();
                var mqttClient = factory.CreateMqttClient();

                mqttClient.ConnectAsync(options, CancellationToken.None);

                while (mqttClient.IsConnected)
                {

                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic("test/test")
                        .WithPayload(DateTime.Now.ToLongTimeString())
                        .WithExactlyOnceQoS()
                        .WithRetainFlag()
                        .Build();

                         mqttClient.PublishAsync(message, CancellationToken.None); // Since 3.0.5 with CancellationToken


                }

            }
        }
    }
