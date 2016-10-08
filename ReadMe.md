#Valant Inventory Coding Excercise

##Overview
The application was developed in Visual Studio 2015, .NET version 4.6.1 and contains 5 projects:

1. KFH.ValantInventory.API: Web API 2.2 project exposing a RESTful web service
2. KFH.ValantInventory.Common: Class library containing Objects and Interfaces shared throughout the application
3. KFH.ValantInventory.Core: Class library containing business logic and repositories for data access
4. KFH.ValantInventory.DataAccess: Class library used for communicating with the data store
5. KFH.ValantInventory.UnitTests: Unit test project. Tests are written using NUnit and Moq

For scalability, the application utilizies the Task-based Asynchronous Pattern (TAP) (aka async/await, more on this below). The API project uses a Unity Container for Inversion of Control (IoC). Using IoC allows us to make the application highly testable. In fact, both the InventoryController and InventoryRepository have 100% code coverage. Having interfaces passed into the constructor instead of creating objects on the fly not only allows the code to be testable, but also highly decoupled. For example the entire data access layer can, and should, be replaced simpy by creating a new factory class that implements the IInventoryDataAccessFactory interface and modifying the KFH.ValantInventory.API.UnityConfig.RegisterComponents() method to resolve IInventoryDataAccessFactory using the new factory.


##Running the application
You should be able to simply clone the repository from Github open up the solution in VS2015, make sure KFH.ValantInventory.API is set as the StartUp project and hit F5, making sure all Nuget packages are restored successfully. By default the application will run under IIS Express at http://localhost:5581/ and will open to a gorgeous (not) Default.htm page with a brief overview of the methods exposed by the API. You can also build the solution and create a site in IIS pointing to the API project, making sure the assigned application pool runs under .NET v4.0. By default the application logs exceptions to c:\tmp\Logs\ValantInventory.log. This can be modified to point to a location of your choosing by modifying the NLog.config file in the KFH.ValantInventory.API project.

##Using the application
Once the application is running, the easiest way to interact with it is via a Rest client such as Postman. I've included a Postman collection named ValantInventory.postman_collection.json under the Tests solution folder (since it's what I used for integration testing). Just import the collection into postman and you should be ready to go. It contains all of the body and headers required for each method to interact with the application. If you modified the IIS Express port, or installed the application in IIS, you will need to modify the file to point to your location.

##Assumptions
The majority of the assumptions made are with regards to the following 2 areas:

* __Notifications__: When I saw the following "Then there is a notification ..." when an item is either expired or deleted, I immediately thought of a queuing  system such as RabbitMQ, Amazon SNS and Amazon SQS depending on what the end goals of the notification are. As such, the IInventoryDataAccessFactory factory contains methods to create both IDeletedInventoryQueue and IExpiredInventoryQueue objects. Both of those interfaces only support Enqueue methods. The current concrete objects implementing those interfaces just use a local ConcurrentQueue as it's backing store, but there is no method to consume the queue as there are many existing products written specifically for this purpose and I shouldn't be writing another one. Instead, there should be an agreed upon queuing mechanism to which the application publishes and clients interested in the notifications subscribe.

* __Expirations__: 
1. Expired items will not be returned from the API.
2. Expirations should be stored in UTC so that an item expires at the same time in all time zones. If an item is added/updated and the Expiration is not in UTC, it will be converted to UTC based on the local time of the server processing the request. 
3. There is currently no mechanism for expiring an item in the inventory as soon as it has passed it's expiration date/time. I've assumed that there is some sort or service or job that is responsible for locating expired items. As such I have exposed an Expire method on the API that will publish the expired item to the ExpiredInventory queue. If the current Expiration is in the future it is set to the current UTC DateTime. 

#Missing/Skimped Areas
* __Documentation__: Aside from the lame Default.html page provided and this document, there is no documentation provided on how to interact with the API. Documentation should be implemented using something like Swagger. Similarly, I only added comments in the code where I thought something may not be completely straightforward or obvious. I didn't spend the time to write comments for all of the Classes and methods.

* __Scalability__: 
1. By using TAP, individual servers hosting the API should be able to able handle a large number of simultaneous requests. However, since the current data stores are in memory, the current DataAccess methods are not asynchronous, they just return Tasks. As previously mentioned, it is anticipated that the current DataAccess layer should be replaced with an asynchronous version to take full advantage of TAP.
2. The application should also scale horizontally provided the current DataAccess layer is replaced with one connected to a common data store and deleted/expired queues. Doing such should be simple as mentioned above in the overview. There are still some possible issues with concurrency and an optimistic locking scheme using either a timestamp or rowversion on the Inventory items should be implemented.

* __Security__: There are too many outstanding questions with regards to how security for this application should be implemented such as:
1. Is this an internal or external facing application?
2. What is the level of granularity, i.e. do we need separate permissions for Read vs. Modify or is it all or nothing?
3. How will autentication be handled, username/password, SSO, IAM Roles, etc.
4. Do we need SSL encryption?
* __Security (cont'd)__: While I don't have the answer to these questions, I have added a custom KFHValantAuthorizeAttribute to the InventoryController class. It currently just allows all requests straight through, but can easily be modified once some or all of the questions above have been answered. In addition, other custom AuthorizeAttributes can be applied to individual methods if finer granularity is required.

* __Logging__: The current logging mechanism just uses NLog to write to a log file. It does not for example format messages as json for ingestion into Splunk or send it to an ELK stack. However, it is easy enough to replace the existing InventoryLogger class with another more robust class that also implements the IInventoryLogger interface. To use the new class instead, just modify the KFH.ValantInventory.API.UnityConfig.RegisterComponents() method to resove IInventoryLogger to the new class.

* __Integration Testing__: My entire integration testing relied on manual tests using Postman. As mentioned above, I have included the ValantInventory.postman_collection.json Postman collection I used in the Solution Items folder. I just did manual testing, but I could have created another collection that excercised the application and included tests to make sure the desired results were achieved. 
