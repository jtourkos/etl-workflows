using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Ioannis.ETLWorkflows.Triggers.ETLManagementService.API.Services.RabbitMqService
{
    public class RabbitMQClient<T> : IDisposable, IRabbitMQClient<T>
    {
        private const string EXCHANGE_NAME = "gf.etl-workflow.topic";
        private const string ROUTING_KEY = "etl-workflow.trigger";
        private const string ETL_TRIGGERS_QUEUE_NAME = "gf.etl-workflow";

        private bool _disposed = false;
        private readonly IModel _channel;
        private readonly IConnection _connection;

        public RabbitMQClient(RabbitMQClientConfiguration rabbitMqClientConfiguration)
        {
            if (rabbitMqClientConfiguration == null)
            {
                throw new ArgumentNullException(nameof(rabbitMqClientConfiguration));
            }

            var factory = new ConnectionFactory
            {
                HostName = rabbitMqClientConfiguration.HostName,
                UserName = rabbitMqClientConfiguration.UserName,
                Password = rabbitMqClientConfiguration.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(EXCHANGE_NAME, "topic");

            _channel.QueueDeclare(ETL_TRIGGERS_QUEUE_NAME, true, false, false, null);

            _channel.QueueBind(ETL_TRIGGERS_QUEUE_NAME, EXCHANGE_NAME, ROUTING_KEY);
        }

        public void PublishMessage(T message)
        {
            _channel.BasicPublish(EXCHANGE_NAME,
                ROUTING_KEY,
                null,
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool itIsSafeToAlsoFreeManagedObjects)
        {
            if (!_disposed)
            {
                if (itIsSafeToAlsoFreeManagedObjects)
                {
                    // Dispose managed resources.
                    _connection?.Close();

                    // Dispose unmanaged managed resources.
                }

                _disposed = true;
            }
        }

        #endregion
    }
}
