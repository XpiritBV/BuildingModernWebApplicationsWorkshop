# Lab 1 - Getting started

This lab will setup your laptop to create and run an Angular application.

Goals for this lab:

- Prepare development laptop
- Create new Angular application


## Prepare development laptop

Make sure that your laptop is up-to-date with the latest security patches. This workshop is specific towards Windows as the operating system for your machine. The labs can also be done on Linux, although this can be a bit more challenging.

### 1. Download required tooling 

- Install Visual Studio code https://code.visualstudio.com/

You are going to use this IDE to create the Angular application

- Install NodeJS https://nodejs.org/en/

You need this to be able to run `npm` commands. With this you can also manage the packagess of the project.

- **Install the Angular CLI:**

  Open the command line tooling in Visual Studio code
  ```sh
  npm install -g @angular/cli
  ```

You can now initiate Angular CLI commands by typing `ng <command>` 

### 2. Reference material 

- https://angular.io/docs

## Creating a new Angular application
 
### 1. Create an Angular App with the Angular CLI

``` sh
ng new angular-application
```

This will create an Angular application with the newest version of Angular.

Do the following setup:

**Would you like to add Angular routing?** : (y)es

**Which stylesheet format would you like to use?**: SCSS

Sass and Less are also valid options, because they are also CSS Pre-processors. They provide a structured way of writing styles and which compiles to CSS. The Pre-processors do basically the same with some differences.

### 2. Switch to angular-application application directory

```sh
cd ./angular-application
```

### 3. Run the application

The following command runs the Angular application using the package.json: `./angular-application/package.json `. 

> #### Suggestion
> Running the application like this makes you able to serve the application with for example a proxy.
> https://github.com/angular/angular-cli/blob/master/docs/documentation/stories/proxy.md

``` sh
npm start

# package.json
{
  "name": "angular-application",
  "version": "0.0.0",
  "scripts": {
    "ng": "ng",
    "start": "ng serve",
    "build": "ng build",
    "test": "ng test",
    "lint": "ng lint",
    "e2e": "ng e2e"
  },
```

Ofcourse you can also run the application `ng serve` from the command line.

### 4. Add Material Design Components

```sh
ng add @angular/material
```

**Choose a prebuilt theme name, or "custom" for a custom theme:** Choose the colors you like :)

**Set up HammerJS for gesture recognition?:** no

**Set up browser animations for Angular Material?:** Yes

Material Design offers an out of the box pretty looking interface, which can be themed respectively. Material Design is a research field project within Google about User Friendly components and UI flows.

If you want to know more about Material Design and its components:

- https://material.angular.io/
- https://material.io/design/

### 5. Import Material Design Modules in your AppModule

Create a new Module to include all your Material Design Modules:

```sh
ng g module AppMaterial --flat=true

# Generates a module which includes all Material Modules used in this application
# --flat creates the module file without a directory
```

The above command generates the following file `./angular-application/src/app/app-material-components.module.ts`

Replace the module's content with the following Material Modules which you are going to use throughout the application:

```ts
import {
  MatListModule,
  MatIconModule,
  MatInputModule,
  MatButtonModule,
  MatSelectModule,
  MatToolbarModule
} from '@angular/material';

@NgModule({
  declarations: [],
  imports: [],
  exports: [
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule
  ]
})
export class AppMaterialModule { }
```

Import the `AppMaterialModule` in your AppModule file `./angular-application/src/app/app.module.ts` 

```ts
@NgModule({
  declarations: [
    AppComponent,
    HighscoresListComponent
  ],
  imports: [
    BrowserModule, 
    AppRoutingModule,
    // Add this module
    AppMaterialModule
    // #end
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}
```

The reason for creating a separate Module is because the amount of Material Modules can grow fast and pollutes the `app.module.ts` file

### 5. Add Material theme to styles.scss

Import a material theme you like in the global styles.scss `./angular-application/src/styles.scss`

```scss
html, body { /* ... */}
body { /* ... */ }

/* Import material theme css */
@import '@angular/material/prebuilt-themes/indigo-pink.css';
/* #end */

/* You can choose the following pre-built theme colors:  
- deeppurple-amber.css
- indigo-pink.css
- pink-bluegrey.css
- purple-green.css  */
```

> #### Pro tip
> You can configure your own theming and colors: 
> https://material.angular.io/guide/theming


### 6. Replace boilerplate application content

Replace the content of `./angular-application/src/app/app.component.html` with the following:

```html
<mat-toolbar color="primary">
  <mat-toolbar-row>
    <span>Angular Workshop</span>
    <span class="example-spacer"></span>
    <mat-icon class="example-icon">add</mat-icon>
  </mat-toolbar-row>
</mat-toolbar>

<div class="app-container">
  <router-outlet></router-outlet>
</div>
```

A Material toolbar is shown on every page on the top of the page.

`<router-outlet>` makes sure that the right content is displayed when you navigate to a certain url. The next lab will cover `Routing` to a page.

Add some styling to the app-component in file `./angular-application/src/app/app.component.scss`

```scss
.example-icon {
  padding: 0 14px;
}

.example-spacer {
  flex: 1 1 auto;
}

.app-container {
  margin: 20px;
}
```

> #### Suggestion
>
> Run the application and check if you see the Material Toolbar with your configured theme color.


What can I tell:

- Routing
- Sass
- Proxy back-end
- Material Design Components
- FormBuilder
- Angular CLI commands
    - ng new
    - ng component
- Services 
- Http
- Interceptors
- AuthGuards
- Lazy loading modules
- Pipes
- RXJS
   - Subjects
   - Subscriptions
- Blackbox (Jest / Snapshots) testing / whitebox testing (Jasmine)
- Protractor
- Storybook
- Deployment Build
- Run SPA in a docker container

## Wrapup
You have prepared your laptop and created an Angular application with Material Design which you are going to use in the next labs. Any issues you may have, can probably be resolved during the labs. Ask your fellow attendees or the proctor to help you, if you cannot solve the issues.

Continue with [Lab 2](Lab2-CreatingComponents.md).