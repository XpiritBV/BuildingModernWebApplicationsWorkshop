# Lab 3 - Connect to the API

In this lab you will use the components you have created and connect them to the .NET Core API. You will learn about managing state in services and about RXjs Subjects and Observables.

Goals for this lab:

- [Generating an OpenAPI client](#inspect)
- [Getting high scores from the API](#inspect)
- [Posting scores to the API](#inspect)

## Generating an OpenAPI client

### 1. Install swagger-codegen tool

Install `ng-openapi-gen` to generate services and models based on your .NET Core API.

```sh
npm install -g ng-openapi-gen
```

### 2. Generate a client inside your Angular application

```sh
cd ./angular-application

#//TODO correct localhost port
ng-openapi-gen --input http://localhost:5000/openapi/v1.json --output src/app/shared/api/leaderboards-api
```

The above command generated your .Net Core API definition to the following directory: `./angular-application/src/app/shared/api/leaderboards-api/*`

The most important files you want to look at are:
- `../leaderboards-api/services/` contain the http calls to get high scores and post scores.
- `../leaderboards-api/models/` contain the definition of the models within the API.
- `../leaderboards-api/api.module.ts` contains the `ApiModule` which you are going to import in the `AppModule`

## 3. Import the ApiModule

Import the `ApiModule` and `HttpClientModule` in the AppModule in file `./angular-application/src/app/app.module.ts`

```ts
@NgModule({
  declarations: [
    /* ... */
  ],
  imports: [
    /* ... */
    /* Import the following modules */
    HttpClientModule,
    ApiModule.forRoot({ rootUrl: 'https://localhost:5001' }),
    /* #end */
  ],
  providers: [
    /* ... */
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

### 4. TODO: CORS ISSUES & OpenAPI V1 and V2 get issue

```cs
// Fix commenting out GetV2 in LeaderboardsController


// Fix in Startup.CS
readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

services.AddCors(options =>
        {
            options.AddPolicy(MyAllowSpecificOrigins,
            builder =>
            {
                builder.WithOrigins("http://example.com",
                                    "http://www.contoso.com");
            });
        });

// ....

app.UseCors(MyAllowSpecificOrigins);        
```


## Getting high scores from the API

In this chapter you will get high scores from the API you created in the .NET Core labs. 

### 1. Create a service class which will contain your request(s)

Create the service by using the Angular CLI.

```sh
ng generate service shared/services/high-scores

# Short-hand command:
# ng g s shared/services/high-scores
```

The above command generated the following file: `./angular-application/src/app/shared/services/high-score.service.ts`

The script appended `.service.ts` to identify it as a service class.

The service contains the following content: 

``` ts
@Injectable({
  providedIn: 'root'
})
export class HighScoresService {

  constructor() { }
}
```

`@Injectable "root"` marks the service as Singleton. You can save state inside this Service and this state can be used throughout the application.

### 3. Add the service to the AppModule

Add the service as provider to the `AppModule` in file `./angular-application/src/app/app.module.ts`


```ts
@NgModule({
  declarations: [
    /* ... */
  ],
  imports: [
    /* ... */
  ],
  providers: [
    /* Add the service to providers */
    HighScoresService
    /* #end */
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

Adding the `HighScoresService` to the providers makes it (dependency) `Injectable` for Components. You inject this service later on in this lab.

### 4. Add a GET method to the service

Add a get method to the service in file `./angular-application/src/app/shared/services/high-score.service.ts`

```ts
export class HighScoresService {
  constructor(private readonly httpClient: HttpClient) {}

  /* Add this get method */
  getHighScores(): Observable<Array<HighScore>> {
    var uri = "https://localhost:5001/api/v1/leaderboard";
    return this.httpClient.get<Array<HighScore>>(uri);
  }
  /* #end */
}
```