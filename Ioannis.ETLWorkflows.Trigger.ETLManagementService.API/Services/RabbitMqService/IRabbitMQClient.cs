using System;

namespace Ioannis.ETLWorkflows.Triggers.ETLManagementService.API.Services.RabbitMqService
{
    public interface IRabbitMQClient<in T> : IDisposable
    {
        void PublishMessage(T message);
    }
}