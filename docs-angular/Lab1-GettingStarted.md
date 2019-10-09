# Lab 1 - Getting started

This lab will setup your laptop to create and run an Angular application.

Goals for this lab:

- Prepare development laptop
- Create new Angular application


# Prepare development laptop

Make sure that your laptop is up-to-date with the latest security patches. This workshop is specific towards Windows as the operating system for your machine. The labs can also be done on Linux, although this can be a bit more challenging.

## Download required tooling 

- Install Visual Studio code https://code.visualstudio.com/

You are going to use this IDE to create the Angular application

- Install NodeJS https://nodejs.org/en/

You need this to be able to run `npm` commands. With this you can also manage the packagess of the project.

- Install the Angular CLI

Open the command line tooling in Visual Studio code

```sh
npm install -g @angular/cli
```

You can now initiate Angular CLI commands by typing `ng <command>` 

# Create new Angular application

1. Create an Angular App with the Angular CLI

``` sh
ng new angular-application
```

This will create an Angular application with the newest version of Angular.

Do the following setup:

**Would you like to add Angular routing?** : (y)es

**Which stylesheet format would you like to use?**: Sass

SCSS and Less are also valid options, because they are also CSS Pre-processors. They provide a structured way of writing styles and which compiles to CSS. The Pre-processors do basically the same with some differences.

3. Switch to angular-workshop application directory

```sh
cd ./angular-workshop
```

4. Add Material Design Components

```sh
ng add @angular/material
```

**Choose a prebuilt theme name, or "custom" for a custom theme:** Choose the colors you like :)

Material Design offers an out of the box pretty looking interface, which can be themed respectively. Material Design is a research field project within Google about User Friendly components and UI flows.

If you want to know more about Material Design and its components:

- https://material.angular.io/
- https://material.io/design/

## Replace boilerplate application content

Replace the content of `angular-workshop/src/app/app.component` with the following:

``` html
<router-outlet></router-outlet>
```

`<router-outlet>` makes sure that the right content is displayed when you navigate to a certain url. The next lab will cover `routing` to a page.


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
You have prepared your laptop and created an Angular application which you are going to use in the next labs. Any issues you may have, can probably be resolved during the labs. Ask your fellow attendees or the proctor to help you, if you cannot solve the issues.

Continue with [Lab 2](Lab2-Docker101.md).