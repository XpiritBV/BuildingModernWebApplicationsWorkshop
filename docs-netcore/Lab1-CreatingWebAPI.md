# Lab 2 - Creating your Web API

In this lab you will become more familiar with creating a minimalistic Web API in ASP.NET Core. 

Goals for this lab: 
- [Create a solution and empty Web API project](#create)
- [Create a solution and empty Web API project](#create)

## <a name="create"></a>Create a solution and empty Web API project
Make sure you have the .NET Core 3.0 SDK installed. You can check this by running the command 
```cmd 
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
```cmd
git init
dotnet new gitignore
``` 
from the command-line inside the new folder.
For Visual Studio Code you can add this to the `.gitignore` file:
```git
.vscode/*
!.vscode/settings.json
!.vscode/tasks.json
!.vscode/launch.json
!.vscode/extensions.json
*.code-workspace
``` 

To get started building our Web API, you are going to use the templating engine of the .NET Core SDK to scaffold a new solution file:
```cmd
dotnet new sln -n BuildingModernWebApplications
```
Also create a new empty ASP.NET Core project, that will become the Web API. Add the project to the solution:
```cmd
dotnet new web -n RetroGamingWebAPI
dotnet sln add RetroGamingWebAPI\RetroGamingWebAPI.csproj
``` 
This should be enough to create a solution for Visual Studio 2019 and a project that can run your Web API.
Give it a try by building the solution and starting the project.
```cmd
dotnet build
cd RetroGamingWebAPI
dotnet run  
```
The ASP.NET Core application should start and you can view the "Hello World" page at https://localhost:5001.
Stop the running application by pressing Ctrl+C.

## <a name="webapi"></a>Building a minimal Web API
Open your new solution `BuildingModernWebApplications.sln` in Visual Studio 2019 or Code and examine the files that are in the project. Pay extra attention to the `Program.cs` file and the .NET Core 3.0 application bootstrapping using the new `Host` class.

In the `Startup.cs` file add a constructor that has an `IConfiguration` and `IHostEnvironment` parameter and store these in read-only properties. The configuration and host environment are created by the `Host.CreateDefaultBuilder` factory method in the Program static `Main` entry point.

```c#
public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
{
   Configuration = configuration;
   HostEnvironment = hostEnvironment;
}

public IConfiguration Configuration { get; }
public IHostEnvironment HostEnvironment { get; }
```

The next step is to add the controllers as services into the dependency injection system. Find the `ConfigureServices` method and add the controllers to the services registered on the `IServiceCollection`:

```C#
public void ConfigureServices(IServiceCollection services)
{
   services.AddControllers();
}
```
The `Configure` method builds the request handling pipeline consisting of middleware. For now you will only change the endpoints of the application. Remove the existing endpoint for `"/"` and map the controllers instead:
```C#
app.UseEndpoints(endpoints =>
{
      endpoints.MapControllers();
});
```
The final step is to add the first Web API controller. Create a new `Controllers` folder in the project and add a new API controller named `LeaderboardController`. You can do this by right-clicking the folder in Visual Studio 2019 and choose to add a new `Controller` item and select the` API Controller Empty`. Alternatively you can add a text file and include following code:

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

Notice how ASP.NET Core has no special `ApiController` class anymore and instead uses the `[ApiController]` attribute to indicate the class as an API. 

Next, add a single API method for the HTTP `GET` action:
```c#
[HttpGet]
public ActionResult<IEnumerable<int>> Get()
{
   return Ok(new int[] { 42, 1337, 6502, 68000 });
}
```

These preparations are enough to add an endpoint to the routing middleware, and have a receiving controller handle the GET request for the REST API.
Give it another try. Start the project in Visual Studio 2019 by switching the host form IIS Express to RetroGamingWebAPI and pressing F5. Or run it from the command-line in the RetroGamingWebAPI folder with:
```cmd
C:\Sources\Workshop> dotnet run
```
In Visual Studio Code you will also press F5 and configure the .

Navigate to https://localhost:5001/api/leaderboard using a browser. When hosting in Visual Studio 2019 using IIS Express, the port number will most likely be in the 44000+ range. You should see a JSON array of the hard-coded values displayed in the browser.

## Adding additional HTTP methods
The Retro Gaming leaderboard Web API must return a list of high scores when making a GET request at `/api/leaderboard`. In order to accomplish this, add a model for the high scores Data Transfer Objects (DTOs). Create a folder called `Models` and add a new class `HighScore`.
```c#
public class HighScore
{
   public string Game { get; set; }
   public string Nickname { get; set; }
   public int Points { get; set; }
}
```  
Later on the high scores are retrieved from a database. For now, you will use some hardcoded fake high scores.
In the `LeaderboardController` class, add a field for this list of fake scores and initialize the list in the constructor. Feel free to add your own imaginary scores to the list:
```C#
private readonly List<HighScore> scores;
public LeaderboardController()
{
   scores = new List<HighScore>()
   {
         new HighScore() { Game = "Pac-man", Nickname = "LX360", Points = 1337 },
   };
}
```
Also change the signature of the GET action of the controller to match the new data returned.
```C#
[HttpGet]
public ActionResult<IEnumerable<HighScore>> Get()
{
   return Ok(scores);
}
```
Try the new method by navigating to the leaderboard endpoint and verify that the data is returned in JSON format. 

## Status codes
As you may have noticed, the Web API does not return any readable information for pages that return a status code other than `200 OK`. You can switch this on with some middleware from the `Microsoft.AspNetCore.Diagnostics` NuGet package. You do not have to add the NuGet package as it is included in to Microsoft.AspNetCore.App metapackage already.

Inside the `Startup.Configure` method change the existing fragment inside the `if` statement to be:
```c#
if (env.IsDevelopment())
{
   app.UseStatusCodePages();
   app.UseDeveloperExceptionPage();
}
```
Start the project again and navigate to a non-existent endpoint, such as the root of the Web API. You should now see some plain text message indicating `Status Code: 404; Not Found`.

You might want to change the launch URL of the project in the properties of the Visual Studio 2019 project.
In Visual Studio Code this is accomplished by adding the following fragment to your launch.json file:
```json
"launchBrowser" : {
   "enabled": true, 
   "args": "${auto-detect-url}",
   "windows": {
      "command": "cmd.exe",
      "args": "/C start ${auto-detect-url}/api/leaderboard"
   }
},
```

## Content formatting and negotiation
The Web API must figure out which encoding or data format it has to return. By default the JSON format is used. Typically a client can specify a supported format in its request using headers or in the URL. To add support for both JSON and XML some changes have to be made.

First of all add some options to the configuration of the controllers inside the `ConfigureServices` method of the `Startup` class. 
```c#
services
   .AddControllers(options => {
      options.RespectBrowserAcceptHeader = true;
      options.ReturnHttpNotAcceptable = true;
   })
```
These options will enable the use of `ACCEPT` headers in a browser request.
Returning XML requires one of the two available XML formatters `XmlSerializerFormatter` and `XmlDataContractSerializerFormatter`, which follow the W3C and Micrsosoft's DataContract rules for serialization respectively.

Run your Web API again and notice the returned data format. It is now XML instead of JSON. Can you figure out why this is the case?

Use Postman to send a request with an ACCEPT header of `application/json`. Also try:
```
application/xml, application/json;q=0.9, */*;q=0.8
```

A browser friendly alternative is the use of a format filter combined with a mapping for convenient abbreviations to the actual ACCEPT encoding types. Change the options in the `AddControllers` method to this:
```c#
.AddControllers(options => {
   options.RespectBrowserAcceptHeader = true;
   options.ReturnHttpNotAcceptable = true;
   options.FormatterMappings.SetMediaTypeMappingForFormat("xml", new MediaTypeHeaderValue("application/xml"));
   options.FormatterMappings.SetMediaTypeMappingForFormat("json", new MediaTypeHeaderValue("application/json"));
})
```
It defines mapping for the abbriviations of `xml` and `json`. 
Next, go to the `GET` action of the `LeaderboardController` and add a format filter attribute and change the route in the `HttpGet` attribute to accept a format to use.
```c#
[HttpGet("{format?}")]
[FormatFilter]
public ActionResult<IEnumerable<HighScore>> Get()
{
   return Ok(scores);
}
```
Try your Web API again by navigating to the endpoints:
```
/api/leaderboard/xml
/api/leaderboard/json
```
and observe the behavior of the returned data format.

Per action you can filter the allowed return data formats with the `Produces` attribute. Add this to the top of the `GET` action method.
```c#
[Produces("application/json")]
```
and retry your previous URLs.

## EF Core
.NET Core comes with a new version of the Object Relational Mapper Entity Framework, aptly named Entity Framework Core or EF Core for short. It has several providers for datasources and also has an in-memory datastore for testing purposes. We will add support for EF Core to the solution and build some read and write functionality with a object model for our high scores, gamers and video games.

First, add references to the NuGet packages for EF Core 3.0 and the in-memory provider:

- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.InMemory
- Microsoft.EntityFrameworkCore.Tools

Create a new class in the `Models` folder called `Gamer`:
```c#
public class Gamer
{
   public int Id { get; set; }
   public Guid GamerGuid { get; set; }
   public string Nickname { get; set; }
   public virtual ICollection<Score> Scores { get; set; }
}
```
and another class `Score`:
```C#
public class Score
{
   public int Id { get; set; }
   public int Points { get; set; }
   public string Game { get; set; }
   public int GamerId { get; set; }
   [ForeignKey("GamerId")]
   public Gamer Gamer { get; set; }
}
```
Add a `using` statement to `System.ComponentModel.DataAnnotations.Schema` for the `[ForeignKey]` attribute.

These classes represent our simplistic domain model for retro video gaming. The `Score` class holds a high score for a certain game, with a reference to the `Gamer` that holds the record. Conversely, the `Gamer` has a list of all his high scores. 

Open the `Startup.cs` file and locate the `ConfigureServices` method. Add the bootstrapping of EF Core and the in-memory provider:
```c#
services.AddDbContext<RetroGamingContext>(options => {
      options.UseInMemoryDatabase(databaseName: "RetroGaming");
});
```

To use the new DbContext class in our `LeaderboardController` it needs to be injected by the dependency injection system. Change the constructor to accept an argument of type `RetroGamingContext` and store it in a readonly field.
```c#
private readonly RetroGamingContext context;

public LeaderboardController(RetroGamingContext context)
{
   this.context = context;
}
```
With the context available inside the controller, it can be used in each of the action methods. Alter the `Get` action to return data retrieved from the in-memory provider and projecting it to `HighScore` objects. Additionally, make the signature `async`.
```c#
public async Task<ActionResult<IEnumerable<HighScore>>> Get()
{
   var scores = context.Scores
      .Select(score => new HighScore()
      {
         Game = score.Game,
         Points = score.Points,
         Nickname = score.Gamer.Nickname
      });

   return Ok(await scores.ToListAsync().ConfigureAwait(false));
}
```

During development of our web API it is convenient to have some test data available. We will use a database initializer to create some dummy data. It can also be used to populate some static master data. Add a new `DbInitializer` class inside the `Infrastructure` folder. 
```c#
public class DbInitializer
{
   public async static Task Initialize(RetroGamingContext context)
   {
      context.Database.EnsureCreated();
      if (context.Gamers.Any())
      {
            return;
      }
      var gamer = context.Gamers.Add(new Gamer() { GamerGuid = Guid.NewGuid(), Nickname = "LX360", Scores = new List<Score>() { new Score() { Points = 1337, Game = "Pacman" } } });
      context.Gamers.Add(new Gamer() { GamerGuid = Guid.NewGuid(), Nickname = "LeekGeek", Scores = new List<Score>() { new Score() { Points = 6510, Game = "Space Invaders" } } });
      await context.SaveChangesAsync();
   }
}
```
and alter the `Configure` method of the Startup class to run the initializer for a development environment.
```C#
if (env.IsDevelopment())
{
      DbInitializer.Initialize(context).Wait();
      app.UseStatusCodePages();
      app.UseDeveloperExceptionPage();
}
```

Next, you will add the ability to add more high scores to the database with an additional controller.
Create a new class `ScoresController` inside the Controllers folder. Similar to the `LeaderboardController` inject the `RetroGamingContext` and store it in a field.
```c#
private readonly RetroGamingContext context;

public ScoresController(RetroGamingContext context)
{
   this.context = context;
}
```
The controller will have two actions: a `GET` and a `POST` method to retrieve a particular score for a game, and to add a new score respectively.
```c#
[HttpGet("{game}")]
public async Task<IEnumerable<Score>> Get(string game)
{
   var scores = context.Scores.Where(s => s.Game == game).Include(s => s.Gamer);
   return await scores.ToListAsync().ConfigureAwait(false);
}

[HttpPost("{nickname}/{game}")]
public async Task PostScore(string nickname, string game, [FromBody] int points)
{
   // Lookup gamer based on nickname
   Gamer gamer = await context.Gamers
         .FirstOrDefaultAsync(g => g.Nickname.ToLower() == nickname.ToLower())
         .ConfigureAwait(false);

   if (gamer == null) return;

   // Find highest score for game
   var score = await context.Scores
         .Where(s => s.Game == game && s.Gamer == gamer)
         .OrderByDescending(s => s.Points)
         .FirstOrDefaultAsync()
         .ConfigureAwait(false);

   if (score == null)
   {
         score = new Score() { Gamer = gamer, Points = points, Game = game };
         await context.Scores.AddAsync(score);
   }
   else
   {
         if (score.Points > points) return;
         score.Points = points;
   }
   await context.SaveChangesAsync().ConfigureAwait(false);
}
```
Try the new controller by running your API and navigating to the `scores/Pacman` endpoint. You should get an error, even if all your code is correct.

The reason is that our object model contains a circular reference. The serialization will fail because of this. You can fix this by introducing the Newtonsoft.Json serializer and configuring it to ignore reference loops. 
Add a NuGet packages for `Microsoft.AspNetCore.Mvc.NewtonsoftJson` and change the configuration of the controllers in `ConfigureServices`.
```C#
options.FormatterMappings.SetMediaTypeMappingForFormat("json", new MediaTypeHeaderValue("application/json"));
})
.AddNewtonsoftJson(setup => {
   setup.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
})
```
Try running the API again and see if it works correctly now.

## Using Dependency Injection
.NET Core has a built-in dependency injection system and ASP.NET Core makes extensive use of this itself.  

To get familiar with injecting dependencies in our web API you will add a fictious mail service as an abstraction and an implementation. Create a folder named `Infrastructure` and add an interface `IMailService` and class `MailService`.
```C#
public interface IMailService
{
   void SendMail(string message);
}
public class MailService : IMailService
{
   public void SendMail(string message)
   {
      Debug.WriteLine($"Sending mail from mail service: {message}");
   }
}
```
Register the mail service as a dependency in the `ConfigureServices` method of the `Startup` class as a transient object. This means a new object will be constructed  each time the dependency is requested.
```C#
services.AddTransient<IMailService, MailService>();
```

Inject the mail service in the `ScoresController` and store it in a readonly field, like the RetroGamingContext.
```c#
private readonly IMailService mailService;

public ScoresController(RetroGamingContext context, IMailService mailService)
{
   this.context = context;
   this.mailService = mailService;
}
```
Use the mail service at the end of the `POST` action method to report that a new high score has been registered.
```c#
mailService.SendMail($"New high score of {score.Points} for game '{score.Game}'");
```

Verify that the mail service object is resolved and injected correctly and that the API still works.
 
## Wrapup
You have just learned some of the basic ...

Continue with [Lab 3 - Dockerizing .NET Core](Lab3-BuildingWebAPI.md).
