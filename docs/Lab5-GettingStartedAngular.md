# Lab 5 - Getting started with Angular

This lab will make sure you have set up your laptop correctly to create and run an Angular application.

Goals for this lab:
- [Preparing your development laptop](#preparing)
- [Creating a new Angular application](#createangular)
- [Adding Material Design](#materialdesign)

## <a name="preparing"></a>Preparing your development laptop
Make sure that your laptop is up-to-date with the latest security patches and updates. This workshop is specific towards Windows as the operating system for your machine. The labs can also be done on Linux, although this might be a bit more challenging. 

### Download and install required tooling 
If you already followed the instructions in [Lab 1 - Getting started](Lab1-GettingStarted.md) you can skip this part.

- Download and install Visual Studio Code from https://code.visualstudio.com, 

  You might already have done this in [Lab 1](Lab1-GettingStarted.md). You are going to use this IDE to create the Angular application.
- Download and install NodeJS from https://nodejs.org/en/

  You need this to be able to run `npm` commands. With this you can also manage the packages of the project.
  
  If you've already installed node make sure that you are on version 10.9 or greater. 

- Install the Angular CLI:

  Open the command line tooling in Visual Studio Code
  ```sh
  npm install -g @angular/cli
  ```

You can now initiate Angular CLI commands by typing `ng <command>` 

## <a name="createangular"></a>Creating a new Angular application
 
### 1. Create an Angular App with the Angular CLI

``` sh
ng new RetroGamingSPA
```
This will create an Angular application with the newest version of Angular. When asked choose the following options:

1. Would you like to add Angular routing?: **Yes**
2. Which stylesheet format would you like to use?: **SCSS**

   Sass and Less are also valid options, because they are also CSS Pre-processors. They provide a structured way of writing styles and which compiles to CSS. The Pre-processors do basically the same with some minor differences.

### 2. Switch to application directory

```sh
cd ./RetroGamingSPA
```

### 3. Run the application
The following command runs the Angular application:
```sh
npm start
```
> #### Suggestion
> Running the application like this makes you able to serve the application with a proxy.
> https://github.com/angular/angular-cli/blob/master/docs/documentation/stories/proxy.md

It uses the `package.json` file that contains the npm scripts. Examine the file and find the following section: 
```json
# package.json
{
  "name": "RetroGamingSPA",
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
Alternatively, you can run the application from the command-line using 
```sh
ng serve
```

## <a name="materialdesign"></a>Adding Material Design

Material Design offers a pretty looking interface out of the box, which can be themed. Material Design is a research field project within Google about User Friendly components and UI flows.

If you want to know more about Material Design and its components:
- https://material.angular.io/
- https://material.io/design/

### 1. Add Material Design Components

```sh
ng add @angular/material
```

1. Choose a prebuilt theme name, or "custom" for a custom theme: **Choose the colors you like.**
2. Set up HammerJS for gesture recognition?: **No**
3. Set up browser animations for Angular Material?: **Yes**

### 2. Import Material Design Modules in your AppModule

Create a new Module to include all your Material Design Modules:
```sh
ng g module AppMaterial --flat=true

# Generates a module which includes all Material Modules used in this application
# --flat creates the module file without a directory
```

The above command generates a module named `AppMaterialModule` in the file `./src/app/app-material.module.ts`

The reason for creating a separate module is because the amount of Material Modules can grow fast and pollutes the `app.module.ts` file. Replace the module's content with the following Material Modules which you are going to use throughout the application:

```ts
import { NgModule } from '@angular/core';
import {
  MatListModule,
  MatIconModule,
  MatInputModule,
  MatButtonModule,
  MatSelectModule,
  MatToolbarModule,
  MatRippleModule
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
    MatSelectModule,
    MatRippleModule
  ]
})
export class AppMaterialModule { }
```

Next, import the `AppMaterialModule` in your AppModule file `./src/app/app.module.ts`. In this `AppModule` you declare what components are available for use in your application.


```ts
@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule, 
    AppRoutingModule,
    // Add this module
    AppMaterialModule,
    // #end
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}
```
Fix the import by pressing Ctrl+. in Visual Studio Code. Select the `Import` menu option that appears. You need to repeat this in other lab exercises.

### 3. Add Material theme to styles.scss

Material Design comes with a set of pre-built theme colors:  
- deeppurple-amber.css
- indigo-pink.css
- pink-bluegrey.css
- purple-green.css
Import the material theme you like in the global styles.scss `./styles.scss`

```scss
html, body { height: 100%; }
body { margin: 0; font-family: Roboto, "Helvetica Neue", sans-serif; }

/* You can choose the following pre-built theme colors:  
- deeppurple-amber.css
- indigo-pink.css
- pink-bluegrey.css
- purple-green.css  */

/* Import material theme css */
@import '@angular/material/prebuilt-themes/deeppurple-amber.css';
/* #end */
```

> #### Pro tip
> You can configure your own theming and colors: 
> https://material.angular.io/guide/theming


### 4. Replace boilerplate application content
Replace all of the content in `./src/app/app.component.html` with the following:

```html
<mat-toolbar color="primary">
  <mat-toolbar-row>
    <span>Building Modern Web Apps Workshop - High Scores</span>
    <span class="example-spacer"></span>
    <a mat-icon-button color="white">
      <mat-icon>add</mat-icon>
    </a>
  </mat-toolbar-row>
</mat-toolbar>

<router-outlet></router-outlet>
```
Run your application again with `npm start` and view the result.

A Material toolbar is shown on every page on the top of the page.

`<router-outlet>` makes sure that the right content is displayed when you navigate to a certain url. The next lab will cover `Routing` to a page.

Add some styling to the app-component in file `./src/app/app.component.scss`

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

## Wrapup
You have prepared your laptop and created an Angular application with Material Design which you are going to use in the next labs. Any issues you may have, can probably be resolved during the labs. Ask your fellow attendees or the proctor to help you, if you cannot solve the issues.

Continue with [Lab 5 - Creating Angular components](Lab6-CreatingComponents.md).

## Reference material 
- https://angular.io/docs
