﻿using static RabbitMq.Ticket.Producer.Enums.TicketEnums;

namespace RabbitMq.Ticket.Producer.Model
{
    public class TicketModel
    {
        public int Id { get; set; }
        public string IssuerName { get; set; }
        public string IssuerEmail { get; set; }
        public string Description { get; set; }
        public TicketIssueType TicketIssueType { get; set; }
        public TicketStage TicketStage { get; set; }
        public int AssignedToAgentId { get; set; }
        public ChannelType ChannelType { get; set; }    
        public string AssignedToAgentEmail { get;set; }
    }
}