using RabbitMq.Channel.Consumer.Model;
using RabbitMq.Ticket.Producer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq.Channel.Consumer
{
    public class MessageProcessor
    {
        public string GetTicketCategory(string message)
        {
            var ticket = System.Text.Json.JsonSerializer.Deserialize<TicketModel>(message);
            if(ticket == null)
            {
                Console.WriteLine("Invalid message format");
                return string.Empty;
            }
            Console.WriteLine($"Processing Ticket ID: {ticket.Id}, Issue Type: {ticket.TicketIssueType}");

            string queueName = ticket.TicketIssueType switch
            {
                TicketEnums.TicketIssueType.Product => "product_queue",
                TicketEnums.TicketIssueType.Integration => "integration_queue",
                TicketEnums.TicketIssueType.Billing => "billing_queue",
                TicketEnums.TicketIssueType.Other => "other_queue",
                _ => throw new InvalidOperationException("Unknown ticket issue type")
            };

            return queueName;

        }
    }
}
