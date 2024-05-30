# Warehouse Management System
A WarehouseManagementSystem suggested as a solution to E-conomic technical test that requires the implementation of a Publish-Subscribe system

![alt text](https://github.com/TudorBejan/WarehouseManagementSystem/blob/main/WarehouseManagementSystem.png)

The proposed solution is a warehouse product and order management system.
A warehouse receives orders (from an on-line store) and needs to package them and send them to the customer.
In the warehouse there are multiple workers, each worker being resonsible for packaging different products, in different places in the warehouse. 
Each worker has a Terminal that is "connected" to the warehouse system. With this Terminal the worker is notified each time a new order that he needs to pack is created.
Each worker needs to receive orders that are assigned to him and it is not interested in other orders from different locations in the warehouse.

In order to achieve this the Warehouse System is composed of three components:
# 1. Order Processor 
* it has a REST API (with Swagger to make testing easy) that
 - accepts a list of orders that needs to be processed
 - view the list of already processed orders
# 2. Webhook Dispacher
# 3. Terminals

There are two databases:
1. Warehouse
2. Subscription



 When running the WebHookDispatcher service, the Subscription database is pre-poulated with the following data:
    
    
| Id|Topic      |CallbackURL                  
|---|-----------|-----------------------------
|1  |TV         |"http://localhost:5030/webhook/order/new"
|2  |Smartphone |"http://localhost:5031/webhook/order/new"
|3  |Tablet     |"http://localhost:5031/webhook/order/new"


### Code Blocks

    This is a code block.
