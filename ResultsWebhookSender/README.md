# Results Webhooks

This is a sample project for sending data to a goal webhook endpoint in Results.com. 

## Getting Started

Open this project in Visual Studio and edit the `App.config` file. Modify the SQL connection string to specify an appropriate connection string for your database. For example, after editing the App.config file it might look something like this:
```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="Sql" connectionString="Persist Security Info=False;Integrated Security=true;Initial Catalog=Northwind;server=(local)" providerName="System.Data.SqlClient"/>
  </connectionStrings>
</configuration>
```
See the Microsoft [Connection String](https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection.connectionstring(v=vs.110).aspx) page for more details.

## Running the program

After compiling, you will end up with a `ResultsWebhookSender.exe` program. Calling this program without any command line arguments will display a list of acceptable parameters. Note that you must first provide the 'Sql' connection string in the App.config file. The parameters
```
ResultsWebhookSender [/Proc]  [/Query]  /Url  [/Date]

[/Date]   Date in ISO8601 format yyyy-mm-dd, eg: 2017-05-30. Defaults to
          today's date if not specified
[/Proc]   Name of the stored procedure to execute, eg: sp_get_results -- if
          not specified, the 'Query' parameter must be supplied.
[/Query]  Name of the query to execute enclosed in quotations, eg: "select
          top 1 amount from SalesData order by date desc" -- if not
          specified, the 'Proc' parameter must be supplied.
/Url      Webhook endpoint for your Results.com goal, eg:
          https://api.results.com/v2/goals/1234/webhook/Eyjtvom4...
```

A sample run of this program might look like:
```
ResultsWebhookSender /Query:"select 1000 * rand()" /Url:https://api.results.com/v2/goals/1234/webhook/Eyjtvom4EXAMPLE
```
If you wish to execute the SQL query `select 1000 * rand()`, or:
```
ResultsWebhookSender /Proc:"sp_get_results" /Url:https://api.results.com/v2/goals/1234/webhook/Eyjtvom4EXAMPLE
```
If you wish to execute the SQL stored procedure `sp_get_results`.
