# Lab 2 - Entity Framework Core and Dependency Injection

In this lab you will add more functionality to your web API so it can actually be used. You will use Entity Framnework Core with an in-memory database provider and create a domain model to be serialized using EF Core. In the end you will explicitly register a dependency on a fictitious mail service and inject it using the dependency injection system of .NET Core.

Goals for this lab: 
- [Install Entity Framework Core](#install)
- [Creating a domain model](#efmodel)
- [Using the database context](#dbcontext)
- [Initializing the database](#initialize)
- [Storing data](#storing)
- [Dependency injection](#di)

## <a name="efcore"></a>Installing Entity Framework Core
.NET Core comes with a new version of the Object Relational Mapper Entity Framework, aptly named Entity Framework Core or EF Core for short. It has several providers for datasources and also has an in-memory datastore for testing purposes. We will add support for EF Core to the solution and build some read and write functionality with a object model for our high scores, gamers and video games.

First, add references to the NuGet packages for EF Core 3.0 and the in-memory provider:

- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.InMemory
- Microsoft.EntityFrameworkCore.Tools

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

## <a name="dbcontext"></a>Using the database context
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
and alter the `Configure` method of the Startup class to run the initializer for a development environment.
```C#
if (env.IsDevelopment())
{
      DbInitializer.Initialize(context).Wait();
      app.UseStatusCodePages();
      app.UseDeveloperExceptionPage();
}
```

## <a name="storing"></a>Storing data 
Next, you will add the ability to add more high scores to the database with an additional controller.
Create a new class `ScoresController` inside the Controllers folder. Similar to the `LeaderboardController` inject the `RetroGamingContext` and store it in a field.
```c#
private readonly RetroGamingContext context;

public ScoresController(RetroGamingContext context)
{
   this.context = context;
}
```
The controller will have a single `POST` action method to add a new score.
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
You have just enhanced your initially empty web API to include actual functionality. This included Entity Framework Core Object relational mapping, and the use of .NET Core's dependency injection system. 

In the next labs you will add REST related capabilities such as XML support, content negotiation, OpenAPI documentation, versioning and CORS security policies.

Continue with [Lab 3 - Real world web APIs](Lab3-RealWorldWebAPI.md).