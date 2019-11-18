See An [Gurux](http://www.gurux.org/ "Gurux") for an overview.

Join the Gurux Community or follow [@Gurux](http://twitter.com/guruxorg "@Gurux") for project updates.

Gurux.DLMS.AMI is a part of  Gurux Device Framework. 
For more info check out [Gurux](http://www.gurux.org/ "Gurux").

General information
=========================== 

Purpose of Gurux.DLMS.AMI component is given FAST and SIMPLE Advanced Metering Infrastructure for DLMS meters.
This project consists of three different part

* Gurux.AMI
* Gurux.AMI.UI
* GXDLMSDirector

Gurux.AMI offers a meter reading application that you can schedule or order to read wanted objects from DLMS meters. Read values are saved to the database.
Gurux.AMI.UI is simple to use user interface add-in for GXDLMSDirector.
GXDLMSDirector is an application that you can you to generate devices and control for the Gurux.DLMS.AMI.
For more info check out [GXDLMSDirector](http://www.gurux.fi/index.php?q=GXDLMSDirector "GXDLMSDirector").

We are updating documentation on [Gurux web page](http://www.gurux.org/Gurux.DLMS.AMI "Gurux web page").

Starting with Gurux.AMI
=========================== 
Gurux.AMI is using .Net Core 2.2. First you must install it to your pc.
Then you need to [download ](http://www.gurux.org/Downloads/Gurux.DLMS.AMI.zip "download") GuruxAMI or compile it by yourself.
You can get source codes from [GitHub](https://github.org/Gurux/Gurux.DLMS.AMI "GitHub")
If you downloaded the app you need to unzip the file first.

After that, you need to modify appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Reader": {
    "Id": "603C1F03-6EDE-4F4F-B108-3451383F0B0F",
    "Name": "Default",
    "Threads": 10,
    "TaskWaitTime": 60
  },
  "Scheduler": {
    "Disabled": "False"
  },
  "Listener": {
    "Port": 1000
  },
  "Notify": {
    "Port": 4059
  },
  "Server": {
    "Address": "http://*:64881",
  },
  "Client": {
    "Address": "http://localhost:64881",
  },
  "Database": {
    "Type": "SQLite",
    "Settings": "Data Source=:memory:"
  }
}
```

Reader settings
=========================== 

If the amount of the readers is small and you have only one reader, you don't change reader settings.
Otherwise, you need to generate a new Guid and set unique guid as Id for each reader. Update also reader name. You can use any name you want to.
Threads tell the number of concurrent reads. If the value is one, only one meter is read at the time. If value is 10, max 10 meters are read at the same time.
TaskWaitTime tells how long a new task is waited before the reader asks the next one.

Scheduler settings
=========================== 
There can be only one scheduler or scheduler tasks are generated several times. If you have multiple readers, disable schedulers from all the readers, except one.

Listener settings
=========================== 
Listener port tells on what TCP/IP port Gurux.DLMS.AMI service expects incoming connections. In coming connections are used in GPRS network when dynamic connections are used.
 
Notify settings
=========================== 
Notify port tells on what TCP/IP port Gurux.DLMS.AMI service is receiving Push messages from the meters. 

Server settings
=========================== 

The address will tell address of Gurux.DLMS.AMI service is started. There can be only one server. 

Client settings
=========================== 

The address will tell address of Gurux.DLMS.AMI service where clients try to connect.

Database settings
=========================== 
DBType tells what database is used. 
DBAddress tells database settings. In default, we are using in-memory SQLite database. It is for testing purposes. 
You don't need to install any database, just start the app.

At the moment Gurux.AMI is supporting following databases:

* [MySql](http://www.MySql.com/ "MySql")
* [Maria BD](http://www.mariadb.com/ "Maria BD")
* [Microsoft SQL Server](http://www.microsoft.com/ "Microsoft SQL Server")
* [Oracle](http://www.oracle.com/ "Oracle")
* [SQLite](http://www.sqlite.com/ "SQLite")

## Examples for database settings:

MariaDB and MySQL  
Server=HOST;Database=DATABASE;UID=USER_ID;Password=PASSWORD

Microsoft SQL Server (MSSQL)  
Server=HOST;Database=DATABASE;User Id=USER_ID;Password=PASSWORD;

Oracle  
User Id=USER_ID;Password=PASSWORD;Data Source=HOST_NAME:1521/XEPDB1;

Update USER_ID, PASSWORD and HOST_NAME to what you use in your database.

Note! Connection strings might vary for used database version.


If you want to just test this you can start the application without modifications just running:

```csharp
dotnet Gurux.DLMS.AMI.dll
```

Now you can test that is up and running.

http://localhost:64881/api/info

You can see a list of REST interfaces that you can use to control the Gurux.DLMS.AMI. 
This is in-memory database. All your actions are lost when you close the application.

GXDLMSDirector
=========================== 

You don't need GXDLMSDirector at all. It's here because it helps you to get started. You can do all the actions by yourself using JSON interface.

First, you need to set correct parameters and read the association view from the meter with GXDLMSDirector. 
Association view will tell what kind of functionality meter can offer.

After you have read association view, remove COSEM objects you don't want to read, save it and select "Tools" | "Settings" and select "Gurux.DLMS.AMI settings" page.
Set Gurux.DLMS.AMI address: "http://localhost:64881/" and press "Test Connection..." Then accept the changes.
Now select "Data Concentrators" and "Gurux.DLMS.AMI".

As you can see, UI is a little bit different. First you need to create a device template. Device Template describes device settings and what you want to read from the meter. There might be several device templates, even for the same meter.
First you need to import the device template you just saved. "Select "Templates" view and press the right mouse button. Now select "Import" and select device template you just saved.
Select imported device settings and give for the device template name. You can also change default connection settings or set them empty. Then accept the changes.

All device attributes are shown on the right side. You can remove those attributes that you don't want to read. Now you can create a new device.
Select "Add Device" from the "File menu". Change connection settings and give name for the meter. Accept changes.

Now you can see device on the device tree. It's empty. Selecting "Refresh" from "File" menu or pressing F5, device objects are read from the database.
Data values are empty. Selecting object and pressing "Read", read command is sent to the Gurux.DLMS.AMI and it'll read the meter. 
Wait for a while until the value is read and select "Refresh" again. Now you can see read data value.

You can also call actions or write values to the meter in the same way.

Schedules
=========================== 
When you select "Devices" tree you can see list of Schedules. In default there are three schedules, minutely, hourly and daily.
You can add attributes to schedules, selecting COSEM object and "Add to Schedule" from "Tools" menu.
Select Schedule where you want to add it and press OK. 

All attributes of the selected object are added automatically and you can remove attributes that you don't want to read.

You can also watch quick start [video](https://www.youtube.com/embed/KJT2ftPg3cQ "video") how get started.
