# Lab 1 - Getting started

This lab is going to let you prepare your development environment for the rest of the labs in the workshop. Not all steps are required. The more you can prepare, the better the experience during the workshop.

Goals for this lab: 
- [Prepare development laptop](#1)
- [Download required and optional tooling](#2)
- [Clone Git repository for lab code](#3)
- [Prefetch Docker images](#4)
 
## <a name="1"></a>1. Prepare your development laptop
Make sure that your laptop is up-to-date with the latest security patches. This workshop is specific towards Windows as the operating system for your machine. The labs can also be done on Linux, although this can be a bit more challenging.

## <a name="2"></a>2. Download required and optional tooling
First, you will need to have a development IDE installed. The most preferable IDE is [Visual Studio 2019](https://www.visualstudio.com/vs/) if you are running the Windows operating system.

You should also install [Visual Studio Code](https://code.visualstudio.com/) for the Angular labs and as an alternative to Visual Studio 2019 the following cases:
- Your development machine is running OSX or a Linux distribution as your operating system.
- You want to have an light-weight IDE or use an alternative to Visual Studio 2019.

> Download and install either [Visual Studio 2019](https://www.visualstudio.com/downloads/) or [Code](https://www.visualstudio.com/downloads/).
>
> For Visual Studio Code, also install the [Kubernetes](https://marketplace.visualstudio.com/items?itemName=ms-kubernetes-tools.vscode-kubernetes-tools) and [Docker](https://marketplace.visualstudio.com/items?itemName=PeterJausovec.vscode-docker) extensions.

Second, you are going to need the Docker Desktop Community Edition tooling on your development machine. Depending on your operating system you need to choose the correct version of the tooling. Instructions for installing the tooling can be found [here](https://docs.docker.com/install/). You can choose either the stable or edge channel.

> Download and install Docker Community Edition:
> - [Docker Desktop for Windows](https://docs.docker.com/docker-for-windows/install/)
> - [Docker Desktop for Mac](https://docs.docker.com/docker-for-mac/install/)

Download and install [.NET Core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0) if needed.

The following optional tools are recommended, but not required.

- [GitHub Desktop](https://desktop.github.com/) for Git Shell and Git repository utilities

Download and install NodeJS from https://nodejs.org/en/

You need this to be able to run `npm` commands. With this you can also manage the packages of the project.

Install the Angular CLI by running this npm command from a console window:
```sh
npm install -g @angular/cli
```

## <a name="3"></a>3. Clone Git repository for lab code
The workshop uses an example to get you started with Dockerizing a typical ASP.NET Core application. 
The application is themed around high-scores of retro video games. It consists of web front end and a Web API and stores high-scores in a relational database.

Clone the repository to your development machine:
- Create a folder for the source code, e.g. `C:\Sources\Workshop`.
- Open a command prompt from that folder
- Clone the Git repository for the workshop files:

```
git clone https://github.com/XpiritBV/BuildingModernWebApplicationsWorkshop.git
```
- Set an environment variable to the root of the cloned repository from PowerShell:
```
$env:workshop = 'C:\Sources\Workshop'
```
## <a name=""></a> Prefetch Docker images
To avoid downloading large images during the workshop, you can pull the images for the labs ahead of time:
Execute the following commands from a command-line window:

```sh
docker pull mcr.microsoft.com/mssql/server
docker pull mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim
docker pull mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim
```

## Wrapup
You have prepared your laptop and cloud environment to be ready for the next labs. Any issues you may have, can probably be resolved during the labs. Ask your fellow attendees or the proctor to help you, if you cannot solve the issues.

Continue with [Lab 2 - Creating Web API](Lab2-CreatingWebAPI.md).