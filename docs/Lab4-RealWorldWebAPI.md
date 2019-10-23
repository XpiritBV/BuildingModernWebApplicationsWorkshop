# Lab 4 - Real world Web APIs

In this lab you will make your web API more realistic and introduce several pieces of functionality and characteristics typical for a REST API. This includes the ability to do content negotiation, XML support and having an OpenAPI document to describe the service and a Cross-Origin Resource Sharing security policies.

Goals for this lab: 
- [Status code pages](#statuscodepages)
- [Content formatting and negotiation](#contentnegotiation)
- [OpenAPI documentation](#openapi)
- [Versioning](#versioning)
- [CORS Security](#cors)

## <a name="statuscodepages"></a>Status code pages

Start the project and go to the following non-existing url. https://localhost:5001/non-existing-url

You may notice, the Web API does not return any readable information for pages that return a status code other than `200 OK`. You can switch this on with some middleware from the `Microsoft.AspNetCore.Diagnostics` NuGet package. You do not have to add the NuGet package as it is included in to `Microsoft.AspNetCore.App` metapackage already.

Inside the `Configure` method of `Startup.cs` add the following:
```c#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RetroGamingContext context)
{
   if (env.IsDevelopment())
   {
      app.UseStatusCodePages();
      /* ... */
   }
}
```
Start the project again and navigate to the non-existent endpoint. You should now see some plain text message indicating `Status Code: 404; Not Found`.

Change the launch URL of the project in the properties of the Visual Studio 2019 project. So everytime the project starts it starts with the `api/leaderboards` endpoint.
In Visual Studio Code this is accomplished by adding the following fragment to your `launchSettings.json` file inside the `Properties` folder:
```json
{
   "launchBrowser" : {
      "enabled": true, 
      "args": "${auto-detect-url}",
      "windows": {
         "command": "cmd.exe",
         "args": "/C start ${auto-detect-url}/api/leaderboard"
      }
   },
   /* ... */ 
}
```

## <a name="contentnegotiation"></a>Content formatting and negotiation

The Web API must figure out which encoding or data format it has to return. By default the JSON format is used. Typically a client can specify a supported format in its request using headers or in the URL. To add support for both JSON and XML, some changes have to be made.

First of all add some options to the configuration of the controllers inside the `ConfigureServices` method of the `Startup.cs` class. 
```c#
public void ConfigureServices(IServiceCollection services)
{
   // Replace existing services.AddControllers();
   services
      .AddControllers(options => {
         options.RespectBrowserAcceptHeader = true;
         options.ReturnHttpNotAcceptable = true;
      })
      .AddXmlSerializerFormatters();

   /* ... */
}
```
The options will enable the use of `ACCEPT` headers in browser requests.
Returning XML requires one of the two available XML formatters `XmlSerializerFormatter` and `XmlDataContractSerializerFormatter`, which follow the W3C and Microsoft's DataContract rules for serialization respectively.

> #### Question:
> Run your Web API again and notice the returned data format. It is now XML instead of JSON. Can you figure out why this is the case?

Use the Postman tool to send a GET request to https://localhost:5001/api/leaderboard with an ACCEPT header of `application/json`. Also try the following ACCEPT headers:
```cs
'application/xml', 'application/json;q=0.9', '*/*;q=0.8'
```

A browser friendly alternative is the use of a format filter combined with a mapping for convenient abbreviations to the actual ACCEPT encoding types. Add the following options to the `AddControllers` method:
```c#
public void ConfigureServices(IServiceCollection services)
{
   services
      .AddControllers(options => {
         /* ... */
         options.FormatterMappings.SetMediaTypeMappingForFormat("xml", new MediaTypeHeaderValue("application/xml"));
         options.FormatterMappings.SetMediaTypeMappingForFormat("json", new MediaTypeHeaderValue("application/json"));
      })
   /* ... */
}
```
It defines mapping for the abbriviations of `xml` and `json`. 
Next, go to the `GET` action of the `LeaderboardController.cs` class and add a `[FormatFilter]` attribute and change the route in the `[HttpGet]` attribute to accept a format to use.
```c#
[HttpGet("{format?}")]
[FormatFilter]
public async Task<ActionResult<IEnumerable<HighScore>>> Get()
{
   /* ... */
}
```
Try your Web API again by navigating to the endpoints:
```
/api/leaderboard/xml
/api/leaderboard/json
```
and observe the behavior of the returned data format.

Per action you can specify the allowed return data formats with the `[Produces]` attribute. Add this to the top of the `GET` action method.
```c#
[Produces("application/json", "application/xml")]
```
Retry your previous URLs, You can try removing one of the two string arguments in the `[Produces]` attribute and notice that `json` or `xml` stopped working.

## <a name="openapi"></a>Adding OpenAPI support 
[OpenAPI](https://www.openapis.org/about) is the REST equivalent for modern web APIs that WSDL was for SOAP web services. It is a convenient way to describe your REST API and is a vendor neutral standard. 

You will add support for OpenAPI to your API. It will also provide you with a Swagger user interface to interact with your web API based on the metadata in the OpenAPI description. You will use NSwag, but you are free to choose an alternative like Swashbuckle. 

Start by adding a reference to the NuGet package `NSwag.AspNetCore` in the project.
```sh
cd RetroGamingWebAPI
dotnet add package NSwag.AspNetCore
```
Next, register the services for NSwag in the `ConfigureServices` method of the `Startup.cs` class.
```c#
public void ConfigureServices(IServiceCollection services)
{
   services.AddOpenApiDocument(document =>
   {
         document.DocumentName = "v1";
         document.PostProcess = d => d.Info.Title = "Retro Gaming Web API v1.0 OpenAPI";
   });
   /* ... */
}
```
The OpenAPI is served with custom middleware. Include OpenAPI inside your project. For development purposes also add the Swagger user interface in the `ConfigureServices` method of the `Startup.cs` class.
```c#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RetroGamingContext context)
{
   app.UseOpenApi(config =>
   {
         config.DocumentName = "v1";
         config.Path = "/openapi/v1.json";
   });

   app.UseSwaggerUi3(config =>
   {
      config.SwaggerRoutes.Add(new SwaggerUi3Route("v1.0", "/openapi/v1.json"));

      config.Path = "/openapi";
      config.DocumentPath = "/openapi/v1.json";
   });

   /* ... */
}
```
Decorate the `LeaderboardController.cs` and `ScoresController.cs` classes with the `[OpenAPITag]` attribute:
```c#

[OpenApiTag("Leaderboard", Description = "API to retrieve high score leaderboard")]
/* ... */
public class LeaderboardController : ControllerBase
{
   /* ... */
}
```
```C#
[OpenApiTag("Scores", Description = "API to retrieve or post individual high scores")]
/* ... */
public class ScoresController : ControllerBase
{
   /* ... */
}
```
You can include additional metadata to enrich the OpenAPI documentation. Add XML comments and attributes above the individual action methods to indicate the type of data that is accepted and returned. Include this fragment to the `GET` action method in the `LeaderboarController.cs` class. 
```c#
// GET api/leaderboard
/// <summary>
/// Retrieve a list of leaderboard scores.
/// </summary>
/// <returns>List of high scores per game.</returns>
/// <response code="200">The list was successfully retrieved.</response>
[ProducesResponseType(typeof(IEnumerable<HighScore>), 200)]
/* ... */
public async Task<ActionResult<IEnumerable<HighScore>>> Get()
{
   /* ... */
}
```
The generation of XML documentation can be enabled by checking the `XML Documentation File` checkbox on the Build tab in the project properties dialog of Visual Studio 2019, or by adding the following to the `RetroGamingWebAPI.csproj` file:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
   <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
      <DocumentationFile>C:\Sources\Workshop\RetroGamingWebAPI\RetroGamingWebAPI.xml</DocumentationFile>
   </PropertyGroup>
   <!-- ... -->
</Project>
```
Run your web API and navigate to `/openapi` to view the Swagger UI 3.0. In the Swagger UI, use the Leaderboard `GET` action method to retrieve the list of high scores and try the Scores API to `POST` a new high score for the gamer `LX360`.

## <a name="versioning"></a>Versioning

A production level Web API needs to be versioned for proper lifecycle management. You will add support for versioning of the API.
Again, you will need the `Microsoft.AspNetCore.Mvc.Versioning` NuGet package for the versioning support. 

```sh
cd RetroGamingWebAPI
dotnet add package Microsoft.AspNetCore.Mvc.Versioning -v '4.0.0-preview8.19405.7'
```
Notice that the package is a preview version. This will soon be made final, so you can use that package when it does.

Next, create a new method in the `Startup.cs` class.
```c#
private void ConfigureVersioning(IServiceCollection services)
{
   services.AddApiVersioning(options => {
      options.AssumeDefaultVersionWhenUnspecified = true;
      options.DefaultApiVersion = new ApiVersion(1, 0);
      options.ReportApiVersions = true;
      options.ApiVersionReader = new UrlSegmentApiVersionReader();
   });
}
```
This code will register a default 1.0 API version, read from the URL of the request. Make sure the method is called from  `ConfigureServices` inside the `Startup.cs` class, passing the `IServiceCollection`.
```c#
public void ConfigureServices(IServiceCollection services)
{
   /* ... */
   ConfigureVersioning(services);
}
```
Both controllers need to have their route format changed and require an API version. Annotate the `LeaderboardController.cs` and `ScoresController.cs` classes with the following attributes:
```c#
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
```
Make sure to replace the existing `[Route]` attribute.

Also, change the startup URL for the web API to be `/api/v1/leaderboard` inside the `Properties\launchSettings.json` to reflect the change to route containing a version for the API.

Run your web API and navigate to the new endpoints to check whether they still work correctly.

Now that you have a versioned API you can add a newer version. One possibility is to add a completely new controller with an `[ApiVersion("2.0")]` attribute on top. Alternatively, you can add a new action method in an existing controller and map it to a specific API version with the `[MapToVersion]` attribute on the method.
Add a new method to the `ScoresController.cs` class like so:
```c#
[MapToApiVersion("2.0")]
[HttpGet("{game}")]
public async Task<IEnumerable<Score>> Get(string game)
{
   var scores = context.Scores.Where(s => s.Game == game).Include(s => s.Gamer);
   return await scores.ToListAsync().ConfigureAwait(false);
}
```
This could be an incremental change to an existing API, where this Get method is not available in version 1.0.

Also, put `[ApiVersion("2.0")]` on top of the `ScoresController.cs` class, together with the already existing `[ApiVersion("1.0")]`.

```c#
[ApiVersion("1.0")]
[ApiVersion("2.0")]
/* ... */
public class ScoresController : ControllerBase
{
   /* ... */
}
```
Using both version, you declare that the controller is providing both API versions. Having the `[MapToApiVersion("2.0")]` on a method means that it is only available in version 2.0 of the API.

Run the API and try the new controller by running your API and navigating to the `api/v1/scores/Pacman` endpoint. You should get an error like this:
```json
{"error":{"code":"UnsupportedApiVersion","message":"The HTTP resource that matches the request URI 'https://localhost:5001/api/v1/scores/Pacman' does not support the API version '1'.","innerError":null}}
```
This is intended behavior, as the `[MapToApiVersion]` attribute excludes the GET action from the 1.0 version of the API.

Next, try `api/v2/scores/Pacman` and notice you get an error, even though your versioning code is completely correct.

The reason is that our object model contains a circular reference. The serialization will fail because of this. You can fix this by introducing the Newtonsoft.Json serializer and configuring it to ignore reference loops. 
Add a NuGet packages for `Microsoft.AspNetCore.Mvc.NewtonsoftJson`
```sh
cd RetroGamingWebAPI
dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson
```
Change the configuration of the `AddControllers()` in `ConfigureServices` in the `Startup.cs` class and add `AddNewtonsoftJson` with ReferenceLoopHandling.Ignore.
```C#
public void ConfigureServices(IServiceCollection services)
{
   /* ... */

   services
      .AddControllers(options => { /* ... */ })
      .AddXmlSerializerFormatters()  
      .AddNewtonsoftJson(setup => {
         setup.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
      });

   /* ... */
}
```
Try running the API again and see if it works correctly now.

Try to use the OpenAPI documentation `/openapi` and Swagger UI you will find that it is currently showing the API methods of both versions. You need some fixes to take the two versions into account. 

First, add a second OpenAPI document definition in `ConfigureServices` in `Startup.cs` class, with an operation processor to filter out the correct controllers and methods based on their version. Replace the existing  `AddOpenApiDocument` with the following.
```c#
 public void ConfigureServices(IServiceCollection services)
{
   // Remove existing services.AddOpenApiDocument and replace with these:
   services.AddOpenApiDocument(document =>
   {
         document.OperationProcessors.Add(new ApiVersionProcessor() { IncludedVersions = new string[] { "1.0" } });
         document.DocumentName = "v1";
         document.PostProcess = d => d.Info.Title = "Retro Gaming Web API v1.0 OpenAPI";
   });
   services.AddOpenApiDocument(document =>
   {
         document.OperationProcessors.Add(new ApiVersionProcessor() { IncludedVersions = new string[] { "2.0" } });
         document.DocumentName = "v2";
         document.PostProcess = d => d.Info.Title = "Retro Gaming Web API v2.0 OpenAPI";
   });

   /* ... */
}
```

After, replace the existing `app.UseOpenApi` and `app.UseSwaggerUi3` in `Configure` in `Startup.cs` to support both version of the API. Replace the following:
```c#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RetroGamingContext context)
{
   // Remove existing app.UseOpenApi and replace with these:
   app.UseOpenApi(config =>
   {
         config.DocumentName = "v1";
         config.Path = "/openapi/v1.json";
   });
   app.UseOpenApi(config =>
   {
         config.DocumentName = "v2";
         config.Path = "/openapi/v2.json";
   });

   // Remove existing app.UseSwaggerUi3 and replace with this:
   app.UseSwaggerUi3(config =>
   {
      config.SwaggerRoutes.Add(new SwaggerUi3Route("v1", "/openapi/v1.json"));
      config.SwaggerRoutes.Add(new SwaggerUi3Route("v2", "/openapi/v2.json"));
      
      config.Path = "/openapi";
      config.DocumentPath = "/openapi/v2.json";
   });

   /* ... */
}
```

Run the Web API and go to `/openapi`. Notice that both versions now work correctly in your OpenAPI.

## <a name="cors"></a>CORS Security
There are many aspects of security to be taken care of. For now let's address only one of these at this point. Your future frontend must be able do requests to the Web API you are building now. The browser will not allow HTTP requests from a frontend to a URL that is originating from a different hostname. This is because of Cross-Origin Resource Sharing (CORS) policies. Our web API should indicate that it is allowing incoming web requests from certain origins. For development purposes you will allow all traffic, regardless of its origin, verb or header.

Add a new method to the `Startup.cs` class:
```C#
private void ConfigureSecurity(IServiceCollection services)
{
   services.AddCors(options =>
   {
         options.AddPolicy("CorsPolicy",
            builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
         );
   });
}
```

Make sure the method is called from  `ConfigureServices` inside the `Startup.cs` class, passing the `services` variable.
```c#
public void ConfigureServices(IServiceCollection services)
{
   /* ... */
   ConfigureSecurity(services);
}
```
Use the CORS policy you configured inside the `Configure` of the `Startup.cs` class:

```c#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RetroGamingContext context)
{
   app.UseCors("CorsPolicy");

   /* ... */
}
```

## Wrapup
You have just enhanced your initially empty web API to include actual real world functionality. This included Entity Framework Core Object relational mapping, XML support, content negotiation, OpenAPI documentation, versioning and CORS security. 

In the next labs you will build an Angular SPA frontend and connect it to your web API.  

Continue with [Lab 5 - Getting started with Angular](Lab5-GettingStartedAngular.md).
