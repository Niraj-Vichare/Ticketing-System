using Microsoft.AspNetCore.Mvc;
using RabbitMq.Ticket.Producer.Model;
using RabbitMq.Ticket.Producer.Service;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMq.Ticket.Producer.Controllers
{
    [ApiController]
    public class TicketGenerator : ControllerBase
    {
        private QueueService _queueService; 
        public TicketGenerator(QueueService queueService)
        {
            _queueService = queueService;
        }
        [HttpPost]
        [Route("/Ticket/GeneratorTicket")]
        public async Task<IActionResult> TicketCreation([FromBody] TicketModel ticketModel)
        {
            if(ticketModel == null)
            {
                return BadRequest();
            }
            
            await _queueService.InitializeQueueSetup();

            string json = System.Text.Json.JsonSerializer.Serialize(ticketModel);
            byte[] body = Encoding.UTF8.GetBytes(json);
            bool result =await _queueService.PublishMessage(ticketModel, body);
            if(result)
            {
                return Ok("Ticket is generated successfully.");
            }
            return BadRequest();
        }
    }
}
