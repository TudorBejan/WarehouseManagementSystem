# Warehouse Management System
This is the suggested solution to E-conomic technical test that requires the implementation of a Publish-Subscribe system.

![alt text](https://github.com/TudorBejan/WarehouseManagementSystem/blob/main/WarehouseManagementSystem.png)

The proposed solution is a warehouse product and order management system.
A warehouse receives orders (for example an on-line store) and needs to pack and send them to the customer.
In the warehouse there are multiple workers, each of them being responsible for packaging different products, in different places of the warehouse. 
Each worker has a Terminal that is "connected" to the warehouse system. On this Terminal the worker is notified each time a new order is assigned to him.
Each worker needs to receive orders that are assigned to him and he is not interested in other orders from different locations in the warehouse.

In order to achieve this, the Warehouse System is composed of three components:
## 1. Order Processor 
####  It consists of an REST API and business logic for processing the orders.
* the REST API (configured with Swagger to make testing easy):
 	- accepts a list of orders that needs to be processed
 	- displays the list of already processed orders
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
* it has a consumer that listens to the WebhookEvent queue
* for each webhook event that is received:
	- it searches in the SubscriptionDB to find the Terminals that are "interested" in receiving that event
	- it pushes the webhook event to the Terminals that are subscribed
	
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
<pre><code>docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management</code></pre>
3. Open the solution in Visual Studio and run each project:
   * TV_Terminal
   * SmallElectronics_Terminal
   * WebhookDispatcher
   * OrderProcessor
4. Go to http://localhost:5089/swagger/index.html
5. Try the POST /orders resource (it is already prepolutated with orders data) and also can see the response with the process status of each order
6. Observe in the WebhookDispatcher Console, TV_Terminal Console and SmallElectronics_Terminal Console how the orders "flows" into the system
7. Try the GET /ordersHistory resource to see the orders in the system

Unit Tests are located in OrderProcessor.UnitTests project. The tests are written for the most complex method in the project (that constains the business logic of the Order Processor)
## Demo:
https://drive.google.com/file/d/1y9Op8StGeg2CppYTa642lia9J2G_RpdJ/view?usp=sharing
