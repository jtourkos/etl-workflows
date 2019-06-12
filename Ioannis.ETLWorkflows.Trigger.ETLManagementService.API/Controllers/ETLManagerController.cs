using System;
using System.Threading.Tasks;
using Ioannis.ETLWorkflows.Core;
using Ioannis.ETLWorkflows.Core.Models;
using Ioannis.ETLWorkflows.Triggers.ETLManagementService.API.Services.RabbitMqService;
using Microsoft.AspNetCore.Mvc;

namespace Ioannis.ETLWorkflows.Triggers.ETLManagementService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ETLManagerController : ControllerBase
    {
        private readonly IRabbitMqService _rabbitMqService;

        public ETLManagerController(IRabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService ?? throw new ArgumentNullException(nameof(rabbitMqService));
        }

        [HttpPost]
        public async Task<ActionResult> Trigger([FromBody] TriggerRequest triggerRequest)
        {
            var response =  _rabbitMqService.QueueTriggerRequest(triggerRequest);

            if (response.Status == QueueTriggerRequestStatus.Queued)
            {
                return Ok(response.Status.ToString());
            }

            throw new Exception("Failed to queue request.");
        } 
    }
}
