# Lab 3 - Entity Framework Core and Dependency Injection

In this lab you will add more functionality to your web API so it can actually be used. You will use Entity Framework Core with an in-memory database provider and create a domain model to be serialized using EF Core. In the end you will explicitly register a dependency on a fictitious mail service and inject it using the dependency injection system of .NET Core.

Goals for this lab: 
- [Install Entity Framework Core](#install)
- [Creating a domain model](#efmodel)
- [Creating and registering an in-memory RetroGaming Database Context](#createdbcontext)
- [Using the database context](#dbcontext)
- [Initializing the database](#initialize)
- [Storing data](#storing)
- [Dependency injection](#di)

## <a name="efcore"></a>Installing Entity Framework Core

.NET Core comes with a new version of the Object Relational Mapper Entity Framework, aptly named Entity Framework Core or EF Core for short. It has several providers for datasources and also has an in-memory datastore for testing purposes. We will add support for EF Core to the solution and build some read and write functionality with a object model for our high scores, gamers and video games.

First, add references to the NuGet packages for EF Core 3.0 and the in-memory provider:

```sh
cd RetroGamingWebAPI
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.InMemory
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Relational

```

## <a name="efmodel"></a>Creating a domain model

The next step is to create domain entities that represent the data being stored using EF Core. You will add two classes using a code-first approach. As opposed to database-first, code-first starts with the C# object model that implies and generates a database schema for storage. 

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
using System.ComponentModel.DataAnnotations.Schema;

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
`System.ComponentModel.DataAnnotations.Schema` is needed for the `[ForeignKey]` attribute.

These classes represent our simplistic domain model for retro video gaming. The `Score` class holds a high score for a certain game, with a reference to the `Gamer` that holds the record. Conversely, the `Gamer` has a list of all his high scores. 

## <a name="createdbcontext"></a>Creating and registering an in-memory RetroGaming Database Context

//TODO: Some extra explanation about DbContext

The first step is to create a new `Infrastructure` folder inside the `RetroGamingWebAPI` project and add a class named `RetroGamingContext`. 

> The folder structure should look like this:
> - RetroGamingWebAPI
>   - Infrastructure
>     - RetroGamingContext.cs

Replace the `RetroGamingContext.cs` class with the following code:

```cs
namespace RetroGamingWebAPI.Infrastructure
{
    public class RetroGamingContext : DbContext
    {
        public RetroGamingContext(DbContextOptions<RetroGamingContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gamer>().ToTable("Gamers");
            modelBuilder.Entity<Score>().ToTable("Scores");
        }

        public DbSet<Gamer> Gamers { get; set; }
        public DbSet<Score> Scores { get; set; }
    }
}
```

//TODO some explanation about the Context File...

Configure the RetroGamingContext as an in-memory database. Open the `Startup.cs` file and locate the `ConfigureServices` method. Add the bootstrapping of EF Core and the in-memory provider:
```c#

public void ConfigureServices(IServiceCollection services)
{
   /* ... */
   services.AddDbContext<RetroGamingContext>(options => {
      options.UseInMemoryDatabase(databaseName: "RetroGaming");
   });
}
```

## <a name="dbcontext"></a>Using the database context

To use the new DbContext class in your `LeaderboardController.cs` it needs to be injected by the dependency injection system. Change the constructor to accept an argument of type `RetroGamingContext` and store it in a readonly field.
```c#
public class LeaderboardController : ControllerBase
{
   private readonly RetroGamingContext context;
   /* ... */

   public LeaderboardController(RetroGamingContext context)
   {
      this.context = context;
      /* ... */
   }
}
```
With the context available inside the `LeaderboardController.cs`, it can be used in each of the action methods. Alter the `Get` action to return data retrieved from the in-memory provider and projecting it to `HighScore`  objects. Additionally, make the signature `async`.

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

## <a name="initialize"></a>Initializing the database 

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
and alter the `Configure` method of the `Startup.cs` class to run the initializer for only the development environment.
```C#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
   if (env.IsDevelopment())
   {
      DbInitializer.Initialize(context).Wait();
      //TODO: UseStatusCodePages Copy paste foutje?
      app.UseStatusCodePages();
      /* ... */
   }

   /* ... */
}
```

## <a name="storing"></a>Storing data 

Next, you will add the ability to add more high scores to the database with an additional controller.
Create a new class `ScoresController.cs` inside the Controllers folder. Similar to the `LeaderboardController.cs` inject the `RetroGamingContext` and store it in a field.
```c#
[Route("api/[controller]")]
[ApiController]
public class ScoresController : ControllerBase
{
   private readonly RetroGamingContext context;

   public ScoresController(RetroGamingContext context)
   {
      this.context = context;
   }
}
```
The Controller will be available from an endpoint, because of the `[ApiController]` Attribute.

Add a single `POST` action method to the `ScoresController.cs` controller to be able to add new scores.
```c#
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
Use a tool like Postman to test the `POST` action. Make sure you set the `Content-Type` to `application/json`.

//TODO Example url to post to with parameters and body...

## Using Dependency Injection
.NET Core has a built-in dependency injection system and ASP.NET Core makes extensive use of this itself.  

To get familiar with injecting dependencies in our web API you will add a fictious mail service as an abstraction and an implementation. Create interface `IMailService.cs` and class `MailService` inside the `Infrastructure` folder.

Add the following to interface `IMailService.cs` 
```C#
namespace RetroGamingWebAPI.Infrastructure
{
    public interface IMailService
    {
        void SendMail(string message);
    }
}
```

Add the following to class `MailService.cs` 
```c#
namespace RetroGamingWebAPI.Infrastructure
{
    public class MailService : IMailService
    {
        public void SendMail(string message)
        {
            Debug.WriteLine($"Sending mail from mail service: {message}");
        }
    }
}
```
Register the mail service as a dependency in the `ConfigureServices` method of the `Startup.cs` class as a transient object. This means a new object will be constructed each time the dependency is requested.
```C#
public void ConfigureServices(IServiceCollection services)
{
   /* ... */
   services.AddTransient<IMailService, MailService>();
}
```

Inject the mail service in the `ScoresController.cs` class and store it in a readonly field, like the RetroGamingContext.
```c#
public class ScoresController : ControllerBase
{
   /* ... */
   private readonly IMailService mailService;

   public ScoresController(RetroGamingContext context, IMailService mailService)
   {
      /* ... */
      this.mailService = mailService;
   }
}
```
Use the mail service at the end of the `POST` action method in class `ScoresController.cs` to report that a new high score has been registered.
```c#
public async Task PostScore(string nickname, string game, [FromBody] int points)
{
   /* ... */
   mailService.SendMail($"New high score of {score.Points} for game '{score.Game}'");
}
```

Verify that the mail service object is resolved and injected correctly and that the API still works.

## Wrapup
You have just enhanced your initially empty web API to include actual functionality. This included Entity Framework Core Object relational mapping, and the use of .NET Core's dependency injection system. 

In the next labs you will add REST related capabilities such as XML support, content negotiation, OpenAPI documentation, versioning and CORS security policies.

Continue with [Lab 4 - Real world web APIs](Lab4-RealWorldWebAPI.md).