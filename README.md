# Warehouse Management System
A WarehouseManagementSystem suggested as a solution to E-conomic technical test that requires the implementation of a Publish-Subscribe system

![alt text](https://github.com/TudorBejan/WarehouseManagementSystem/blob/main/WarehouseManagementSystem.png)

The proposed solution is a warehouse product and order management system.
A warehouse receives orders (from an on-line store) and needs to package them and send them to the customer.
In the warehouse there are multiple workers, each worker being responsible for packaging different products, in different places in the warehouse. 
Each worker has a Terminal that is "connected" to the warehouse system. With this Terminal the worker is notified each time a new order that he needs to pack is created.
Each worker needs to receive orders that are assigned to him and it is not interested in other orders from different locations in the warehouse.

In order to achieve this, the Warehouse System is composed of three components:
## 1. Order Processor 
* it has a REST API (configured with Swagger to make testing easy) that:
 	- accepts a list of orders that needs to be processed
 	- view the list of already processed orders
* for each received order, the Order Processor:
	- searches in the WarehouseDB to find the product location in the warehouse
	- attaches the product location information to the order
	- sends a webhook event to the WebhookEvent queue containing the order and product location information
	- stores the order in the database
	
When running the OrderProcessor service, the Warehouse database is pre-populated with the following data:

|Id|ProductId |WarehouseArea  |WarehouseFloor|WarehouseSection|
|-|-----------|---------------|--------------|-------
|1|1001       |"A"            |0             |1
|2|2001       |"B"            |0             |20
|3|2002       |"B"            |0             |21

## 2. Webhook Dispatcher
* it has a consumer that listen to the WebhookEvent queue
* for each webhook event it receives:
	- searches in the SubscriptionDB to find the Terminals that are "interested" in receiving that event
	- pushes the webhook event to the terminals that are subscribed
	
When running the WebHookDispatcher service, the SubscriptionDB is pre-populated with the following data:    

| Id|Topic      |CallbackURL                  
|---|-----------|-----------------------------
|1  |TV         |"http://localhost:5030/webhook/order/new"
|2  |Smartphone |"http://localhost:5031/webhook/order/new"
|3  |Tablet     |"http://localhost:5031/webhook/order/new"

## 3. Terminals
There are two terminals in the solution:
#### TV_Terminal
- should only receives orders that contain TVs
#### SmallElectornics_Terminal
- should receive orders that contain both smartphones and/or tables

## How to run it:
1. Install Docker (if you do not have it): https://www.docker.com/products/docker-desktop/
2. Start the RabbitMQ broker
    docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
3. Open the solution in Visual Studio and run each project:
   * TV_Terminal
   * SmallElectronics_Terminal
   * WebhookDispatcher
   * OrderProcessor
4. Go to http://localhost:5089/swagger/index.html
5. Try the POST /orders resource (it is already prepolutated with orders data)
6. Observe in the WebhookDispatcher Console, TV_Terminal Console and SmallElectronics_Terminal Console how the orders "flows" into the system
