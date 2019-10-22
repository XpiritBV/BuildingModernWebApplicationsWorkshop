# Lab 2 - Creating your Web API

In this lab you will become more familiar with creating a minimalistic Web API in ASP.NET Core. 

Goals for this lab: 
- [Create a solution and empty Web API project](#create)
- [Building a minimal Web API](#webapi)
- [Adding additional HTTP methods](#httpmethods)

## <a name="create"></a>Create a solution and empty Web API project
Make sure you have the .NET Core 3.0 SDK installed. You can check this by running the command 
```sh 
dotnet --list-sdks
```
from the command-line. You should see the versions of all installed .NET Core SDKs similar to this:
```
C:\>dotnet --list-sdks
2.2.402 [C:\Program Files\dotnet\sdk]
3.0.100 [C:\Program Files\dotnet\sdk]  
```
If the 3.0 version of the SDK is not installed, go back to the prerequisites in lab 0 and download and install it.

Next, create a new folder for your entire solution. This can be any folder, e.g. `C:\Sources\Workshop` on a Windows system. Initialize a new Git repository and add a .gitignore file by running:
```sh
git init
dotnet new gitignore
``` 
If you are using Visual Studio Code you can add this to the `.gitignore` file:
```git
.vscode/*
!.vscode/settings.json
!.vscode/tasks.json
!.vscode/launch.json
!.vscode/extensions.json
*.code-workspace
``` 

To get started building the Web API, you are going to use the templating engine of the ASP.NET Core SDK to scaffold a new solution file for Visual Studio 2019:
```sh
dotnet new sln -n BuildingModernWebApplications
```
Also create a new empty Web ASP.NET Core project, that will become the Web API. Add the project to the solution:
```sh
dotnet new web -n RetroGamingWebAPI
dotnet sln add RetroGamingWebAPI/RetroGamingWebAPI.csproj
``` 
This should be enough to create a solution file for Visual Studio 2019 and a project that can run your Web API.
Give it a try by building the solution and starting the project.
```sh
dotnet build
cd RetroGamingWebAPI
dotnet run  
```
The .NET Core (web) application should start and you can view the "Hello World" page at https://localhost:5001.

Stop the application by pressing Ctrl+C.

## <a name="webapi"></a>Building a minimal Web API

Open your new solution `BuildingModernWebApplications.sln` in Visual Studio 2019 or Code and examine the files that are in the project. 

Pay extra attention to the `Program.cs` file which contais the .NET Core 3.0 application bootstrapping using the new `Host` class.

In the `Startup.cs` file add a constructor that has an `IConfiguration` and `IHostEnvironment` parameter and store these in read-only properties. First, import the configuration library.
```c#
using Microsoft.Extensions.Configuration;
```

```c#
public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
{
   Configuration = configuration;
   HostEnvironment = hostEnvironment;
}

public IConfiguration Configuration { get; }
public IHostEnvironment HostEnvironment { get; }
```

The configuration and host environment are created inside the `Program.cs` static `Main` entry point using the `Host.CreateDefaultBuilder` factory method. 

The first step is to add the first Web API controller. Create a new `Controllers` folder inside the `RetroGamingWebAPI` the project and add a new API controller class named `LeaderboardController`. 

You can do this by right-clicking the folder in Visual Studio 2019 and choose to add a new `Controller` item and select the` API Controller Empty`. Alternatively you can add an empty `.cs` file and include following code:

> The folder structure should look like this:
> - RetroGamingWebAPI
>   - Controllers
>     - LeaderboardController.cs

```c#
using Microsoft.AspNetCore.Mvc;

namespace RetroGamingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
    }
}
```

The next step is to add API controllers as services into the dependency injection system. Find the `ConfigureServices` method inside `Startup.cs`  and add the following:

```C#
public void ConfigureServices(IServiceCollection services)
{
   services.AddControllers();
}
```
The `Configure` method builds the request handling pipeline consisting of middleware. Remove the existing endpoint for `"/"` and map the API controllers instead:

```c#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
   /* ... */
   app.UseEndpoints(endpoints =>
   {
      endpoints.MapControllers();
   });
}
```

Notice how ASP.NET Core has no special `ApiController` class anymore and instead uses the `[ApiController]` attribute to indicate the class as an API Controller. 

Next, add a single API method to the `LeaderboardController.cs` for the HTTP `GET` action:
```c#
[HttpGet]
public ActionResult<IEnumerable<int>> Get()
{
   return Ok(new int[] { 42, 1337, 6502, 68000 });
}
```

Fix the imports in order to build the application again. Visual Studio 2019 can help you with this.

Adding a Http Get method is enough to add an endpoint to the routing middleware, and have a receiving controller handle the GET request for the REST API.

Run the Web API in Visual Studio 2019 by switching the host from IIS Express to RetroGamingWebAPI and pressing F5. Or run it from the command-line in the RetroGamingWebAPI folder with:
```sh
C:\Sources\Workshop> dotnet run
```

You can also run it in Visual Studio Code by pressing F5. You will be prompted to configure the launch setting to select the .NET Core debug environment.

Navigate to https://localhost:5001/api/leaderboard using a browser. When hosting in Visual Studio 2019 using IIS Express, the port number will most likely be in the 44000+ range. You should see a JSON array of the hard-coded values displayed in the browser.

## <a name="httpmethods"></a>Adding additional HTTP methods

The Retro Gaming leaderboard Web API must return a list of high scores when making a GET request at `/api/leaderboard`. In order to accomplish this, add a model for the high scores Data Transfer Objects (DTOs). Create a folder called `Models` and add a new class `HighScore`.

> The folder structure should look like this:
> - RetroGamingWebAPI
>   - Models
>     - HighScore.cs

```c#
namespace RetroGamingWebAPI.Models
{
   public class HighScore
   {
      public string Game { get; set; }
      public string Nickname { get; set; }
      public int Points { get; set; }
   }
}
```
Later on the high scores are retrieved from a database. For now, you will use some hardcoded fake high scores. In the `LeaderboardController.cs` class, add a field for this list of high scores and initialize the list in the constructor. Feel free to add your own imaginary scores to the list:
```C#
public class LeaderboardController : ControllerBase
{
   private readonly List<HighScore> scores;
   public LeaderboardController()
   {
      scores = new List<HighScore>()
      {
            new HighScore() { Game = "Pac-man", Nickname = "LX360", Points = 1337 },
      };
   }

   /* ... */
}
```
Also change the signature of the GET action of the `LeaderboardController.cs` to match the new data returned.
```C#
[HttpGet]
public ActionResult<IEnumerable<HighScore>> Get()
{
   return Ok(scores);
}
```
Try the new method by navigating to the leaderboard endpoint https://localhost:5001/api/leaderboard and verify that the data is returned in JSON format. 

## Wrapup
You have just enhanced your initially empty web API with some basic functionality and tested it. The next steps will be to add some real world functionality, including Entity Framework Core Object relational mapping, XML support, content negotiation, OpenAPI documentation, versioning and CORS security.

Continue with [Lab 3 - Entity Framework Core and Dependency Injection](Lab3-EntityFrameworkCore.md).
