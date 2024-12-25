using RabbitMq.AssignAgent.Consumer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMq.AssignAgent.Consumer.Services
{
    public class AssignService
    {
        private static int currentAgentIndex = 0;
        private static readonly object agentLock = new object();

        public async Task<bool> AssignTicketToAgent(string categoryQueue, string message)
        {
            try
            {
                var ticket = JsonSerializer.Deserialize<TicketModel>(message);
                var agents = GetAvailableAgents(categoryQueue);

                if (agents == null || !agents.Any())
                {
                    Console.WriteLine($"No available agents for category: {categoryQueue}");
                    return false;
                }

                AgentModel assignedAgent;
                lock (agentLock)
                {
                    assignedAgent = AssignAgent(agents);
                }

                if (assignedAgent == null)
                {
                    Console.WriteLine("Failed to assign ticket to an agent.");
                    return false;
                }

                // Update ticket and notify agent
                ticket.AssignedToAgentId = assignedAgent.Id;
                ticket.AssignedToAgentEmail = assignedAgent.Email;
                await NotifyAgent(assignedAgent, ticket);

                Console.WriteLine($"Ticket {ticket.Id} assigned to agent {assignedAgent.Name}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning ticket: {ex.Message}");
                return false;
            }
        }

        private List<AgentModel> GetAvailableAgents(string categoryQueue)
        {
            // Mocking available agents for each category
            var agents = new Dictionary<string, List<AgentModel>>
            {
                
                {
                    "billing_queue", new List<AgentModel>
                    {
                        new AgentModel { Id = 1, Name = "Agent A", Email = "agentA@example.com" },
                        new AgentModel { Id = 2, Name = "Agent B", Email = "agentB@example.com" }
                    }
                },
                {
                    "other_queue", new List<AgentModel>
                    {
                        new AgentModel { Id = 3, Name = "Agent C", Email = "agentC@example.com" },
                        new AgentModel { Id = 4, Name = "Agent D", Email = "agentD@example.com" }
                    }
                },
                {
                    "product_queue", new List<AgentModel>
                    {
                        new AgentModel { Id = 1, Name = "Agent A", Email = "agentA@example.com" },
                        new AgentModel { Id = 2, Name = "Agent B", Email = "agentB@example.com" }
                    }
                },
                {
                    "integration_queue", new List<AgentModel>
                    {
                        new AgentModel { Id = 1, Name = "Agent A", Email = "agentA@example.com" },
                        new AgentModel { Id = 2, Name = "Agent B", Email = "agentB@example.com" }
                    }
                },

            };

            return agents.ContainsKey(categoryQueue) ? agents[categoryQueue] : null;
        }

        private AgentModel AssignAgent(List<AgentModel> agents)
        {
            if (!agents.Any())
            {
                return null;
            }

            // Thread-safe round-robin logic
            int index = Interlocked.Increment(ref currentAgentIndex) % agents.Count;
            return agents[index];
        }

        private async Task NotifyAgent(AgentModel agent, TicketModel ticket)
        {
            Console.WriteLine($"Notifying agent {agent.Name} at {agent.Email} about ticket {ticket.Id}");
            await Task.CompletedTask;
        }


    }
}
