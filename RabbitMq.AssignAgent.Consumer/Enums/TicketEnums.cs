using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq.AssignAgent.Consumer.Enums
{
    public class TicketEnums
    {
        public enum TicketIssueType
        {
            Billing = 1,
            Product = 2,
            Integration = 3,
            Other = 4
        }
        public enum TicketStage
        {
            Created = 1,
            Pending = 2,
            Resolved = 3,
            Closed = 4,
        }
        public enum ChannelType
        {
            Email = 1,
            Phone = 2,
            Chat = 3,
        }
    }
}
