# Lab 3 - Connect to the API

In this lab you will use the components you have created and connect them to the .NET Core API. You will learn about generating an OpenAPI client inside your Angular application and using it to GET and POST.

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

`.forRoot()` is used when a module is "eager," that is, it is not lazy-loaded (loads when the application starts).

`rootUrl` is the url to the Leaderboards API

### 4. Remove the old high-score.ts model and fix imports to new api model

In the previous lab you have created a high-score.ts. You can now safely remove it, because of the automatically generated model from the OpenAPI client generator. 

1. Remove file: `./angular-application/src/app/shared/models/high-score.ts`
2. Fix imports to the new high-score model: `./angular-application/src/app/shared/api/leaderboards-api/models/high-score.ts`


### 5. TODO: CORS ISSUES & OpenAPI V1 and V2 get issue

```cs
// Fix commenting out GetV2 in LeaderboardsController


// Fix in Startup.CS
readonly string MyAllowSpecificOrigins = "_myAllowAllCors";

 services.AddCors(options =>
            {
                options.AddPolicy(MyAllowLocalhostSpecificOrigins,
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

// ....

app.UseCors(MyAllowSpecificOrigins);        
```

## Getting high scores from the API

In this chapter you will get high scores from the OpenAPI client you created in previous chapter.

### 1. Replace code of the high-scores component

Replace the contents of `./angular-application/src/app/high-scores/high-scores.component.ts`

```ts
export class HighScoresComponent implements OnInit {
  public highScores: HighScore[] = [];

  /* Inject the automatically generated LeaderboardService */
  constructor(private readonly leaderBoardsService: LeaderboardService) {}
  /* #end# */

  /* Subscribe to the Get Observable and add the result to the high scores array */
  ngOnInit() {
    this.getHighScores()
      .subscribe(result => {
        this.highScores = result;
      });
  }
  /* #end */

  /*  */
  private getHighScores(): Observable<Array<HighScore>> {
    return this.leaderBoardsService
      .leaderboardGet({
        version: "1.0", 
        format: 'json'
      });
  }
  /* #end */

  onHighScoreSelected(highScore: HighScore) {
    // Log the selected highscore to the developer console.
    console.log(`high score selected`, highScore);
  }
}
```

`Observable` is a concept of RxJS. You first setup a request. Only after calling `.subscribe()` the request is executed.

> **Pro tip**
> 
> RxJS is very interesting and could have its own workshop. If you want to know more about RxJS visit:
> - https://angular.io/guide/rx-library
> - https://www.learnrxjs.io/

## Posting scores to the API

In this chapter you will post scores to the API and refresh the the high scores when a new score is posted.

### 1. Post a score to the API

Post form values of add score component in file: `./angular-application/src/app/add-score.component.ts`

```ts
export class AddScoreComponent implements OnInit {
  addScoreForm: FormGroup;

  /* Add this property */
  isScorePosted: boolean = false;
  /* #end */

  /* Add private readonly scoresService: ScoresService to constructor */
  constructor(private fb: FormBuilder, private readonly scoresService: ScoresService) {}
  /* #end */

  ngOnInit() {
    this.addScoreForm = this.fb.group({
      nickname: [""],
      game: [""],
      points: [0]
    });
  }

  /* Add post score using the scoresService */
  submitScore(): void {
    var formValues = this.addScoreForm.value;

    this.scoresService.scoresPostScore({
      game: formValues.game,
      nickname: formValues.nickname,
      body: formValues.points,
      version: "1.0"
    }).subscribe(x => {
      this.isScorePosted = true;
    });
  }
  /* #end */
}
```

`subscribe()` the request needs to be subscribed to in order to execute the post.


```html
<!-- Add *ngIf="!isScorePosted" to <form> -->
<form class="add-score-form" [formGroup]="addScoreForm" *ngIf="!isScorePosted">
<!-- #end -->
</form>

<!-- Add the following <div> below the <form> -->
<div *ngIf="isScorePosted">
  <h1>Thank you for adding your score!</h1>
  <a [routerLink]="['/']">Go back to view high scores</a>
</div>
<!-- #end -->
```

with `*ngIf` you can hide and show content dynamically. This renders and removes components, instead of only the visibility.

After posting the score you should now see a thank you message and the option to go back.


## Wrap up

In this lab you have created an http client based on the OpenAPI configuration of the .NET Core API. You made a get requests to get high scores and a post request to post new scores.