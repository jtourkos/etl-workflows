using System;
using Ioannis.ETLWorkflows.Core;
using Ioannis.ETLWorkflows.Core.Models;

namespace Ioannis.ETLWorkflows.Triggers.ETLManagementService.API.Services.RabbitMqService
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly IRabbitMQClient<TriggerRequest> _rabbitMqClient;

        public RabbitMqService(IRabbitMQClient<TriggerRequest> rabbitMqClient)
        {
            _rabbitMqClient = rabbitMqClient ?? throw new ArgumentNullException(nameof(rabbitMqClient));
        }

        public TriggerResponse QueueTriggerRequest(TriggerRequest triggerRequest)
        {
            try
            {
                _rabbitMqClient.PublishMessage(triggerRequest);

                return new TriggerResponse() { Status = QueueTriggerRequestStatus.Queued };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new TriggerResponse() { Status = QueueTriggerRequestStatus.FailedToQueue };
            }
        }
    }
}
