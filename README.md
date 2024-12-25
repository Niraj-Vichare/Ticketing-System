# RabbitMQ Ticketing System

A ticketing system that integrates with RabbitMQ for ticket generation, consumption, categorization, and assignment to agents. It processes messages from multiple queues, categorizes tickets based on issue type, and assigns them to available agents.

---

## Table of Contents

- [Project Overview](#project-overview)
- [Services](#services)
  - [1) Ticket Generator Service](#1-ticket-generator-service)
  - [2) Queue Consumer Service](#2-queue-consumer-service)
  - [3) Ticket Assigning Service](#3-ticket-assigning-service)
- [Installation and Setup](#installation-and-setup)

---

## Project Overview

This project consists of three services that interact with RabbitMQ for managing tickets:

1. **Ticket Generator Service** - Generates tickets based on incoming requests and pushes them to RabbitMQ queues based on the ticket's `ChannelType`.
2. **Queue Consumer Service** - Listens to queues, processes the tickets, categorizes them based on their `TicketIssueType`, and routes them to appropriate queues.
3. **Ticket Assigning Service** - Consumes tickets, assigns them to available agents based on a round-robin algorithm, and notifies agents about their assignments.

---

## Services

### 1) Ticket Generator Service

#### Overview

The **Ticket Generator Service** listens for HTTP POST requests at `/Ticket/GeneratorTicket`, receives ticket data, serializes it, and sends it to a RabbitMQ queue based on the ticket's `ChannelType`. This setup allows the system to segregate tickets by communication medium (e.g., email, phone).

#### Service Flow:
1. A user sends a POST request with a `TicketModel` to `/Ticket/GeneratorTicket`.
2. The service validates the incoming request and initializes the RabbitMQ queue connection.
3. The ticket is serialized into JSON and sent to a specific RabbitMQ queue based on the `ChannelType`.

#### RabbitMQ Setup:
- Each ticket is published to a queue based on its `ChannelType`. The queue name is dynamically created using the format: `{ChannelType}_queue` (e.g., `1_queue` for Email, `2_queue` for Phone).
- The service ensures that the queue exists and creates it if necessary.

---

### 2) Queue Consumer Service

#### Overview

The **Queue Consumer Service** listens to multiple queues (e.g., `Email_queue`, `Chat_queue`, `Phone_queue`), processes the ticket data, and categorizes the tickets based on `TicketIssueType`. The service then publishes the message to the appropriate queue using RabbitMQ's Exchange and Routing Key system.

#### Service Flow:

- **Queue Consumption**:
    - The service consumes messages from multiple queues:
      - `Email_queue`
      - `Chat_queue`
      - `Phone_queue`

- **Message Processing**:
    - The service processes messages from these queues, deserializes the ticket information, and checks the `TicketIssueType`.

- **Ticket Categorization**:
    - Based on the `TicketIssueType`, the service categorizes tickets into different queues:
      - `product_queue` (for Product-related issues)
      - `integration_queue` (for Integration-related issues)
      - `billing_queue` (for Billing-related issues)
      - `other_queue` (for all other issues)

- **Publishing the Message**:
    - After categorizing the ticket, the service publishes the ticket to the correct queue:
      - **Exchange**: `categorized_exchanges`
      - **Routing Key**: Based on the `TicketIssueType` (e.g., `product_queue`, `billing_queue`).

- **Graceful Shutdown**:
    - The service can gracefully shut down, ensuring all connections and channels to RabbitMQ are closed properly.

---

### 3) Ticket Assigning Service

#### Overview

The **Ticket Assigning Service** integrates with RabbitMQ to consume ticket messages, assigns tickets to available agents using a round-robin algorithm, and notifies agents about their newly assigned tickets. It handles multiple categories of tickets such as `integration_queue`, `product_queue`, `billing_queue`, and `other_queue`.

#### Service Flow:

- **Queue Initialization**:
    - The service initializes the RabbitMQ connection and sets up necessary queues (e.g., `integration_queue`, `product_queue`).

- **Message Consumption**:
    - The service listens for messages from:
      - `integration_queue`
      - `product_queue`
      - `billing_queue`
      - `other_queue`

- **Ticket Assignment**:
    - Tickets are assigned to agents based on the queue category (e.g., tickets from `product_queue` will be assigned to product specialists).
    - A **round-robin algorithm** is used for even distribution of tickets among agents.

- **Agent Notification**:
    - After assigning a ticket, the service notifies the agent with the ticket details such as:
      - Ticket ID
      - Issue Type
      - Description

---

## Installation and Setup

### Clone the Repository:

```bash
git clone https://github.com/your-username/your-repository.git
cd your-repository
```

### Docker Setup
``` bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management
```


