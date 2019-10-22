# Lab 7 - Connect to the API

In this lab you will use the components you have created and connect them to the .NET Core API. You will learn about generating an OpenAPI client inside your Angular application and using it to make `GET` and `POST` requests.

Goals for this lab:

- [Generating an OpenAPI client](#inspect)
- [Getting high scores from the API](#inspect)
- [Posting scores to the API](#inspect)

## <a name="inspect"></a>Generating an OpenAPI client

### 1. Install swagger-codegen tool
Install `ng-openapi-gen` to generate services and models based on your .NET Core API.
```sh
npm install -g ng-openapi-gen
```

### 2. Generate a client inside your Angular application
The next step is to use the OpenAPI generator to create a client based on our OpenAPI definition. Make sure you are running your Web API. Run the following command from the terminal window:
```sh
#//TODO correct localhost port
ng-openapi-gen --input http://localhost:5000/openapi/v1.json --output src/app/shared/api/leaderboards-api
```
You are using the HTTP endpoint, because the self-signed certificate for the HTTPS endpoint will give errors. The OpenAPI document will stay the same regardless of the protocol you use.

The above command generated your .NET Core API definition to the following directory: `./src/app/shared/api/leaderboards-api/`

The most important files you want to look at are:
- `../leaderboards-api/services/` contain the http calls to get high scores and post scores.
- `../leaderboards-api/models/` contain the definition of the models within the API.
- `../leaderboards-api/api.module.ts` contains the `ApiModule` which you are going to import in the `AppModule`

## 3. Import the ApiModule
Import the `ApiModule` and `HttpClientModule` in the AppModule in file `./src/app/app.module.ts`:
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

- `.forRoot()` is used when a module is "eager". This means it is loaded when the application starts instead of being lazy-loaded when needed.
- `rootUrl` is the URL to the Leaderboards API.

### 4. Remove the old high-score.ts model and fix imports to new api model

In the previous lab you have created a high-score.ts. You can now safely remove it, because the automatically generated model from the OpenAPI client generator provides a similar interface. 

1. Remove file: `./src/app/shared/models/high-score.ts`
2. Fix imports in existing components to switch to the new high-score model (which is located in `./src/app/shared/api/leaderboards-api/models/high-score.ts`).

## Getting high scores from the API
You will get high scores from the OpenAPI client you created in previous exercise.

### 1. Replace code of the high-scores component
Edit the contents of `./src/app/high-scores/high-scores.component.ts` to match:
```ts
export class HighScoresComponent implements OnInit {
  public highScores: HighScore[] = [];

  /* Inject the automatically generated LeaderboardService */
  constructor(private readonly leaderBoardsService: LeaderboardService) {}
  /* #end# */

  /* Subscribe to the Get observable and add the result to the high scores array */
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

`Observable` is a concept of RxJS. It has a reactive approach, where you setup a subscription to the completion of the web request. The handler function representing this subscription is only executed when the call is actually made and completed.

> **Pro tip**
> 
> RxJS is very interesting and deserves its own workshop. If you want to know more about RxJS visit:
> - https://angular.io/guide/rx-library
> - https://www.learnrxjs.io/

## Posting scores to the API
Next, you will post scores to the API and refresh the high scores when a new score is posted.

### 1. Post a score to the API
Using the OpenAPI generated client you can post the form values of the `AddScoreComponent`. Make the following changes in the file `./src/app/add-score.component.ts`.

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

The HTML of the component also needs some tweaking in order to show that the score has been submitted.
```html
<!-- Add *ngIf="!isScorePosted" to <form> -->
<form class="add-score-form" [formGroup]="addScoreForm" *ngIf="!isScorePosted">
<!-- #end -->
</form>

<!-- Add the following <div> below the <form> -->
<div *ngIf="isScorePosted">
  <h1>Thank you for submitting your score!</h1>
  <p>Go back to check if you made it in the list of all time high scores</p> <a mat-button color="primary" [routerLink]="['/']">List of Retro Game high scores</a>
</div>
<!-- #end -->
```

With `*ngIf` you can hide and show content dynamically. This renders and removes components, instead of only the visibility.
After posting the score you should now see a "Thank you" message and the option to go back to the list of high scores.

## Wrap up
In this lab you have created an HTTP client based on the OpenAPI configuration of the .NET Core API. You made a `GET` request to retrieve the list of high scores and a `POST` request to post new scores for your list.

Continue with [Lab 8 - Docker 101](Lab8-Docker101.md).