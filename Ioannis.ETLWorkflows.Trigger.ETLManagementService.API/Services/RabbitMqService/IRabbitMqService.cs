using Ioannis.ETLWorkflows.Core.Models;

namespace Ioannis.ETLWorkflows.Triggers.ETLManagementService.API.Services.RabbitMqService
{
    public interface IRabbitMqService
    {
        TriggerResponse QueueTriggerRequest(TriggerRequest triggerRequest);
    }
}
