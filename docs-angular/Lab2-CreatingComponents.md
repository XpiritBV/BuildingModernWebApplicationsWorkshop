# Lab 2 - Creating Components

During lab you will create components with the Angular CLI to display highscores and add items using the Angular `FormBuilder`. You will learn how to navigate to these components using Angular `Routing`. The lab will also teach you how to use typesafe models with `Typescript`.

Goals for this lab:

- [Create a highscore component and display it](#inspect)
- [Display highscores in a list](#manage)
- [Create a form using FormBuilder](#working)

## Create a highscore component and display it

### 1. Create the component with the Angular CLI

The following will create a component where you are going to show highscores

```sh
ng generate component highscores/highscores-list

# You can also write the command short-handed:
# ng g c highscores/highscores-list

```

The script created a component folder `./angular-application/src/app/highscores/highscores-list` containing 4 files:

- **\*.component.ts**

  - Here you will put the logic for the component and connects to the .html and .sass file.

- **\*.component.spec.ts**

  - With the spec class you are able to unit test your component

- **\*.component.html**

  - This will show your content.

- **\*.component.sass**
  - The sass file gives you the oppurtunity to style the component. See 1. GettingStarted to learn more about Sass.

The Angular CLI command also added your component to the `AppModule`. You can find this file in `./angular-application/src/app/app.module.ts`

```ts
@NgModule({
  declarations: [
    AppComponent,
    // Added component
    HighscoresListComponent
    // #end
  ],
  imports: [BrowserModule, AppRoutingModule],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}
```

In the AppModule you can declare what components are available for use in your application.

### 2. Display the component

You are going to make the component visible in the app root of your application.

Add the following to `./angular-application/app/src/app.compomnent.html`

```html
<!-- If you havn't done already. Remove all the boilerplate code from the default created Angular application. You will end up with following: -->

<!-- The name of the component -->
<app-highscores-list></app-highscores-list>
<!-- #end -->

<router-outlet></router-outlet>
```

The name of the component is specified in `./angular-application/src/app/highscores/highscores-list/highscores-list.component.ts`

### 3. Run the application

```
npm start
```

You should now see the contents of the component: "highscores-list works!"

## Display highscores in a list

For this chapter you will need the component you created in the previous chapter: `./angular-application/src/app/highscores/highscores-list/*`

### 1. Create a model to hold highscores

Create a the following file `./angular-application/src/shared/models/highscore.ts`

// TODO: 
```ts
interface Highscore {
    // Properties...
  name: string;

  score: number;
    // #end
}
```

The `highscore.ts` is in a shared folder, so it can easily be used throughout the whole application when the application grows larger.

**interface vs class**

An interface is used because you are only interested in the properties (signature) of a highscore. Later you will see that these properties directly map onto the results from the API.

### 2. Create highscores

Create a highscores in the following file `./angular-application/src/app/highscores/highscores-list/highscores-list.component.ts`

// TODO: 
```ts
@Component({
  selector: 'app-highscores-list',
  templateUrl: './highscores-list.component.html',
  styleUrls: ['./highscores-list.component.sass']
})
export class HighscoresListComponent implements OnInit {

  public highscores: Highscore[];

  constructor() { }

  ngOnInit() {
    this.highscores = [
      {
        name: "highscore1",
        score: 99
      },
      {
        name: "highscore2",
        score: 1337
      }
    ];
  }
}
```

### 3. Display highscores

Display the highscores in the following file `./angular-application/src/app/highscores/highscores-list/highscores-list.component.html`

``` html


```