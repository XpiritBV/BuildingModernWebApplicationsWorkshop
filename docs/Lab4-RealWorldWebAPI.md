# Lab 4 - Real world web APIs

In this lab you will make your web API more realistic and introduce several pieces of functionality and characteristics typical for a REST API. This includes the ability to do content negotiation, XML support and having an OpenAPI document to describe the service and a Cross-Origin Resource Sharing security policies.

Goals for this lab: 
- [Status code pages](#statuscodepages)
- [Content formatting and negotiation](#contentnegotiation)
- [OpenAPI documentation](#openapi)
- [Versioning](#versioning)
- [CORS Security](#cors)

## <a name="statuscodepages"></a>Status code pages
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

## <a name="contentnegotiation"></a>Content formatting and negotiation
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

TODO: XmlSerializerFormatter
```C#
.AddXmlSerializerFormatters();
```

> #### Question:
> Run your Web API again and notice the returned data format. It is now XML instead of JSON. Can you figure out why this is the case?

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

## <a name="openapi"></a>Adding OpenAPI support 
[OpenAPI](https://www.openapis.org/about) is the REST equivalent for modern web APIs that WSDL was for SOAP web services. It is a convenient way to describe your REST API and is a vendor neutral standard. 

You will add support for OpenAPI and also add a Swagger user interface to interact with your web API based on the metadata in the OpenAPI description. We will use NSwag, but you are free to choose an alternative like Swashbuckle. 

Start by adding a reference to the NuGet package `NSwag.AspNetCore` in the project. 
Next, register the services for NSwag in the `ConfigureServices` method of the `Startup` class.
```c#
services.AddOpenApiDocument(document =>
{
      document.DocumentName = "v1";
      document.PostProcess = d => d.Info.Title = "Retro Gaming Web API v1.0 OpenAPI";
});
```
The OpenAPI is served with custom middleware, so add it to the pipeline configuration in the `Configure` method:
```c#
app.UseOpenApi(config =>
{
      config.DocumentName = "v1";
      config.Path = "/openapi/v1.json";
});
```
and for development purposes add the Swagger user interface as well:
```c#
app.UseSwaggerUi3(config =>
{
   config.SwaggerRoutes.Add(new SwaggerUi3Route("v1.0", "/openapi/v1.json"));

   config.Path = "/openapi";
   config.DocumentPath = "/openapi/v1.json";
});
```
Decorate the `LeaderboardController` and `ScoresController` classes with the `[OpenAPITag]` attribute:
```c#
// LeaderboardController.cs
[OpenApiTag("Leaderboard", Description = "API to retrieve high score leaderboard")]
```
```C#
// ScoresController.cs
[OpenApiTag("Scores", Description = "API to retrieve or post individual high scores")]
```

You can include additional metadata to enrich the OpenAPI documentation. Add XML comments and attributes on top of the individual action methods to indicate the type of data that is accepted and returned. Include this fragment on top of the `GET` action method in the `LeaderboarController` class. 
```c#
// GET api/leaderboard
/// <summary>
/// Retrieve a list of leaderboard scores.
/// </summary>
/// <returns>List of high scores per game.</returns>
/// <response code="200">The list was successfully retrieved.</response>
[ProducesResponseType(typeof(IEnumerable<HighScore>), 200)]
```
The generation of XML documentation can be triggered by checking the `XML Documentation File` checkbox on the Build tab in the project properties dialog of Visual Studio 2019, or by adding the following to the `RetroGamingWebAPI.csproj` file:
```xml
<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
   <DocumentationFile>C:\Sources\Workshop\RetroGamingWebAPI\RetroGamingWebAPI.xml</DocumentationFile>
</PropertyGroup>
```
Run your web API and navigate to `/openapi` to view the Swagger UI 3.0. Use the Leaderboard `GET` action method to retrieve the list of high scores and try the Scores API to `POST` a new high score for the gamer `LX360`.

## <a name="versioning"></a>Versioning
A production level web API needs to be versioned for proper lifecycle management. You will add support for versioning of the API.
Again, you will need a new NuGet package for the versioning support. Add a reference to `Microsoft.AspNetCore.Mvc.Versioning` and create a new method in the `Startup` class.
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
This code will register a default 1.0 API version, read from the URL of the request. Make sure the method is called from  `ConfigureServices`, passing the `IServiceCollection`.
```c#
ConfigureVersioning(services);
```
Each of the two controllers need to have their route format changed and require a API version. Annotate the `LeaderboardController` and `ScoresController` classes with the following attributes:
```c#
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
```
Make sure to replace the existing `[Route]` attribute.
Also, change the startup URL for the web API to be `/api/v1/leaderboard` to reflect the change to route containing a version for the API.

Run your web API and navigate to the new endpoints to check whether they work correctly.

Now that we have a versioned API we can add a newer version. You can add a completely new controller with a `[ApiVersion("2.0")]` attribute on top. Alternatively, you can add a new action method in an existing controller and map it to a specific API version with the `[MapToVersion]` attribute on the method.
Add a new method to the `ScoresController` like so:
```c#
[MapToApiVersion("2.0")]
[HttpGet("{game}")]
public async Task<IEnumerable<Score>> Get(string game)
{
   var scores = context.Scores.Where(s => s.Game == game).Include(s => s.Gamer);
   return await scores.ToListAsync().ConfigureAwait(false);
}
```
Also, put another `[ApiVersion("2.0")]` on the controller.

Try the new controller by running your API and navigating to the `api/v1/scores/Pacman` endpoint. You should get an error like this:
```json
{"error":{"code":"UnsupportedApiVersion","message":"The HTTP resource that matches the request URI 'https://localhost:5001/api/v1/scores/Pacman' does not support the API version '1'.","innerError":null}}
```
This is intended behavior, as the `[MapToApiVersion]` attribute excludes the GET action from the 1.0 version of the API.

Next, try `api/v2/scores/Pacman`. You should get an error, even though your versioning code is completely correct.

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

Should you try to use the OpenAPI documents and Swagger UI you will find that it is broken currently. You need some fixes to take the two versions into account. 
First, add a second OpenAPI document definition in `ConfigureServices`, with an operation processor to filter out the correct controllers and methods based on their version. Replace the existing call to `AddOpenApiDocument`.
```c#
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
```

Then, make the follwoing changes in `Configure`:
```c#
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
``` 
and also 
```c#
app.UseSwaggerUi3(config =>
{
   config.SwaggerRoutes.Add(new SwaggerUi3Route("v1", "/openapi/v1.json"));
   config.SwaggerRoutes.Add(new SwaggerUi3Route("v2", "/openapi/v2.json"));
   
   config.Path = "/openapi";
   config.DocumentPath = "/openapi/v2.json";
});
```
## <a name="cors"></a>CORS Security
There are many aspects of security to be taken care of, but we will address one of these at this point. Our future frontend must be able to call the backend web API we are building now. The browser will not allow HTTP requests from a frontend to a URL that is originating from a different hostname. This is because of Cross-Origin Resource Sharing (CORS) policies. Our web API should indicate that it is allowing incoming web requests from certain origins. For development purposes we will allow all traffic , regardless of its origin, verb or header.

Add the following method to the `Startup` class and call it form the `ConfigureServices` method similar to `ConfigureVersioning`. 
```C#
private void ConfigureSecurity(IServiceCollection services)
{
   services.AddCors(options =>
   {
         options.AddPolicy("CorsPolicy",
            builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
   });
}
```

## Wrapup
You have just enhanced your initially empty web API to include actual real world functionality. This included Entity Framework Core Object relational mapping, XML support, content negotiation, OpenAPI documentation, versioning and CORS security. 

In the next labs you will build an Angular frontend first and connect it to our web API.  

Continue with [Lab 5 - Getting started with Angular](Lab5-GettingStartedAngular.md).
