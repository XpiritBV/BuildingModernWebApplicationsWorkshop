# Lab 9 - Dockerizing a .NET Core application

During this lab you will take an existing application and port it to use Docker containers.

Goals for this lab:
- [Run existing application](#run)
- [Add Docker support to .NET Core application](#add)
- [Run and debug applications from containers](#debug)
- [Build container images](#build)
- [Running SQL Server in a Docker container](#sql)

## <a name="run"></a>Run existing application
We will start with running the existing ASP.NET Core application from Visual Studio. Make sure you have cloned the Git repository, or return to [Lab 1 - Getting Started](Lab1-GettingStarted.md) to clone it now if you do not have the sources. Switch to the `StartDocker` branch by using this command:

```cmd
git checkout start
```

> ##### Important

> Make sure you have switched to the `start` branch to use the right .NET solution. If you are still on the `master` branch, you will use the completed solution. 

> Make sure you have configured 'Docker Desktop' to run Linux containers.
> If your VS2019 debugger won't start and attach, reset 'Docker Desktop' to its factory defaults and recreate network shares by using the settings screen.

Open the solution `BuildingModernWebApplications.sln` in Visual Studio 2019. Take your time to navigate the code and familiarize yourself with the web API project and Angular SPA in the solution if you haven't built that yourself in the previous labs. You should be able to identify:
- ASP.NET Core Web API `RetroGamingWebAPI` 
- Angular frontend website project `RetroGamingSPA`

## Switching to SQL Server 

Up until now, the web API has used an in-memory database provider to serve as the backing store. You will switch to SQL Server for Linux container instance to provide the backend for data storage on your development machine. Later your are going to change this to a hosted Azure SQL Server instance. Before you continue, make sure you are running the SQL Server as described in [Lab 8](Lab8-Docker101.md#lab-2---docker-101).

You will need to make a small adjustment to the original code and replace the in-memory provider with the SQL Server provider.

In `ConfigureServices` of the `Startup` class in the Web API project, find and replace the part that registers the in-memory provider with:
```c#
services.AddDbContext<RetroGamingContext>(options =>
{
    string connectionString =
        Configuration.GetConnectionString("RetroGamingContext");
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null);
    });
});
```
Notice how it uses the same RetroGamingContext and reads a connection string from the configuration that is stored inside `appsettings.json`.

Add the connection string in the application settings file `appsettings.json`:
```json
"ConnectionStrings": {
    "RetroGamingContext": "Server=tcp:127.0.0.1,5433;Database=RetroGaming2019;User Id=sa;Password=Pass@word;Trusted_Connection=False;"
  },
```

Finally, add the following method to the `RetroGamingContext` class to support the mapping to specific tables in SQL Server.
```c#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Gamer>().ToTable("Gamers");
    modelBuilder.Entity<Score>().ToTable("Scores");
}
```

> ##### Important
> Update the connectionstring in the appsettings.json file to use the computername instead of localhost or 127.0.0.1. We will need this later. 

```json
{
  "ConnectionStrings": {
    "RetroGamingContext": "Server=tcp:machinename,5433;Database=Leaderboard;User Id=sa;Password=Pass@word;Trusted_Connection=False;"
  }
```

Then start the container, if you did not already do this.
```
docker run -e ACCEPT_EULA=Y -e MSSQL_PID=Developer -e SA_PASSWORD="Pass@word" --name sqldocker -p 5433:1433 -d mcr.microsoft.com/mssql/server
```

Right-click the RetroGamingWebAPI and start to debug a new instance. Navigate to the Web API endpoint at https://localhost:5001/openapi. Experiment with the GET and POST operations that are offered from the Open API user interface. Try to retrieve the list of high scores, and add a new high score for one of the registered player names.

Make sure you know how this application is implemented. Set breakpoints if necessary and navigate the flow of the application for the home page.

## <a name="add"></a>Add Docker support

Visual Studio 2019 offers tooling for adding support to run your application in a Docker container. You will first add container support to the Web API project.

To get started you can right-click the RetroGamingWebApi project and select Add, Container Orchestrator Support from the context menu. Choose `Docker Compose` as the local orchestrator from the dropdown.

<img src="images/AddContainerOrchestratorSupport.PNG" width="400" />

In the next dialog, choose `Linux` as the target operating system.

<img src="images/AddDockerSupportTargetOS.png" width="400" />

Observe the changes that Visual Studio 2019 makes to your solution.  

Most noticeably you will see that a new Docker Compose project has been added with the name `docker-compose`. It is now your start project for the solution. *(If it's not, make sure to configure it as the startup project.)*

Inspect the contents of the `docker-compose.yml` and `docker-compose.override.yml` files if you haven't already. The compose file specifies which services (containers), volumes (data) and networks (connectivity) need to be created and run. The `override` file is used for local hosting and debugging purposes. Ensure that you understand the meaning of the various entries in the YAML files.

Notice how the `docker-compose.override.yml` file contains some port mappings, defining the ports inside the container and the composition and outside of it. Your actual port numbers are most likely different:
```
ports:
  - "14069:80"
  - "44325:443"
```
This implies that the URLs for your Web API will not be 5000 and 5001 anymore, but the ones listed in the override file instead.

Run your solution and navigate to the `api/v1/leaderboard` endpoint. 

> If you encounter the error 'The DOCKER_REGISTRY variable is not set. Defaulting to a blank string.', make sure you started Visual Studio as an administrator

> ### Question
> 
> Does the application still work?

Now that the Web API project is running from a Docker container, it is not working anymore. You can try to find what is causing the issue, but do not spend too much time to try to fix it. We will do that next.

> Some things to try if you feel like finding the cause:
> - Inspect the running and stopped container
> - Try to reach the Web API from http://localhost:44325/openapi. Remember to replace the port number with your specific one.
> - Verify application settings for the web API project. 

## <a name="sql"></a>Running SQL Server in a Docker container

Now that your application is running the Web API project in a Docker container, you can also run SQL Server in the same composition. This is convenient for isolated development and testing purposes. It eliminates the need to install SQL Server locally and to start the container for SQL Server manually.

Go ahead and add the definition for a container service in the `docker-compose.override.yml` file.

```yaml
sqlserver:
  image: mcr.microsoft.com/mssql/server
  environment:
    - SA_PASSWORD=Pass@word
    - MSSQL_PID=Developer
    - ACCEPT_EULA=Y
  ports:
    - "5433:1433"
```

Remember that from the Docker CLI you used many environment variables to bootstrap the container instance. Go back to the previous lab to check what these are.

> ##### Which additional changes are needed?
> Stop and think about any other changes that might be required to take into account that the database server is now also running in a container.

Start the solution by pressing `F5`. See if it works correctly. Timebox your efforts to try to fix any errors.

As it turns out your connection string to the database is no longer valid. When running inside the composition, services such as the Web API and SQL Server instance are resolved by their internal name. Special addresses like 127.0.0.1, localhost or your machine name have a different meaning and do not resolve correctly. 

> ##### Hint
> Changing the setting in the `appsettings.json` file will work and you could choose to do so for now. It does mean that the setting for running without container will not work anymore. So, what other place can you think of that might work?

You will need to change the connection string for the Web API to reflect the new way of hosting of the database. Add a new environment variable for the connection string of the leaderboard.webapi service in the `docker-compose.override.yml` file:

```yaml
- ConnectionStrings__RetroGamingContext=Server=tcp:sqlserver;Database=RetroGaming2048;User Id=sa;Password=Pass@word;Trusted_Connection=False;
```

> ##### Strange connection string or not? 
> There are at least two remarkable things in this connection string. Can you spot them and explain why? Don't worry if not, as we will look at this in the [Networking lab](Lab10-Networking.md).
 
With this change, you should be able to run your Web API completely from containers. Make sure you have stopped any containers related to the application. Give it a try and fix any issues that occur. 

## <a name="debug"></a>Debugging with Docker container instances
One of the nicest features of the Docker support in Visual Studio 2019 is the debugging support while running container instances. Check out how easy debugging is by stepping through the application like before.

Put a breakpoint at the first statement of the `Get` method in the `LeaderboardController` class in the `RetroGamingWebAPI` project. Run the application by pressing F5. You should be hitting the breakpoint.

## <a name="build"></a>Building container images
Start a command prompt and use the Docker CLI to check which container instances are running at the moment. 
```cmd
docker ps 
```
There should be two containers related to the application:
- SQL Server in `dockercompose<id>_sqlserver_1`.
- Web API in `dockercompose<id>_retrogamingwebapi_1`.

where `<id>` is a random unique integer value.

> ##### New container images
> Which new container images are on your system at the moment? Check your images list with the Docker CLI.

Stop your application if necessary. Verify that any container instances are actually stopped. If not, stop them by executing the following command for each of the container instances:

```
docker kill <container-id>
```

> Remember that you can use the first unique part of the container ID or its name

*Note that you can get the same result by performing running `Clean solution` from the `Build` menu in Visual Studio.*

Now, try and run the Web application image yourself. Start a container instance.

```cmd
docker run -p 8080:80 -it --name webapi retrogamingwebapi:dev
```

Check whether the web application is working. It shouldn't work and you'll find that it brings you in a bash shell on Linux.

```cmd
root@65e40486ab0f:/app#
```

Your container image does not contain any of the binaries that make your ASP.NET Core Web API run. Visual Studio 2019 uses volume mapping to map the files on your file system into the running container, so it can detect any changes thereby allowing small edits during debug sessions.

> ##### Debug images from Visual Studio 2019
> Remember that Visual Studio 2019 creates Debug images that do not work when run from the Docker CLI.

> ##### Asking for help
> Remember that you can ask your proctor for help. Also, working with fellow attendees is highly recommended, as it can be fun and might be faster. Of course, you are free to offer help when asked.

## Wrapup
In this lab you have added Docker support to run both of your projects from Docker containers as well as the SQL Server instance. You enhanced the Docker Compose file that describes the composition of your complete application. In the next lab you will improve the networking part of the composition.

Continue with [Lab 10 - Networking](Lab10-Networking.md).