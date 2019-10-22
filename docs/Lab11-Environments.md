# Lab 5 - Working with environments

In this lab you will learn how to create and run compositions. 

Goals for this lab:
- [Working with compositions and Docker Compose](#work)
- [Create compositions for different environments](#create)
- [Change implementation to work with environment variables](#change)

## <a name="run"></a>Run existing application
We will start with or continue running the existing ASP.NET Core application from Visual Studio. Make sure you have cloned the Git repository, or return to [Lab 1 - Getting Started](Lab1-GettingStarted.md) to clone it now if you do not have the sources. Switch to the `master` branch by using this command.

```
git checkout master
```

> ##### Important
> Make sure you have switched to the `master` branch to use the right .NET solution.
> Make sure you have configured 'Docker Desktop' to run Linux containers.

Open the solution `BuildingModernWebApplications.sln` in Visual Studio. Take your time to navigate the code and familiarize yourself with the various projects in the solution. You should be able to identify these:
- `GamingWebApp`, an ASP.NET MVC Core frontend 
- `RetroGamingWebAPI`, an ASP.NET Core Web API.
- `docker-compose`, a project for running a Docker Compose composition.


There is another way to accomplish the same thing. This uses a multi-stage build in the Dockerfile. Instead of running a new composition that spins up a container to build a container, you can create a

## <a name="create"></a>Create compositions for different environments
One of the useful features of Docker Compose is the layering and cascading of multiple YAML compose files. With it you can introduce concepts such as base compositions, inheritance and overrides.

By default the Docker Compose tooling assumes that your main composition file is called `docker-compose.yml`. Executing `docker-compose <command>` will look for that particular file. Try to run a build that way.

```cmd
docker-compose build
```

It is convenient when your `docker-compose.yml` file is able to build the container images of the composition.  The build of the container images is defined in the `Dockerfile` in the root of the individual projects and uses multiple stages to create a clean final image.

Make sure you understand the `docker-compose.yml` contents.

The Docker support in Visual Studio 2019 makes a similar assumption. It assumes that runtime details for compositions started from Visual Studio are defined in `docker-compose.override.yml`. Open that file which is located underneath the `docker-compose.yml` file in the Solution Explorer tree of Visual Studio.

The combination of the two aforementioned compose files is enough to start a composition. You will need to specify both files in the command in the correct order.

```cmd
docker-compose -f docker-compose.yml -f docker-compose.override.yml up
```

Ideally, your override file for Visual Studio contains the services and settings that are needed when running from the IDE on a development machine.

Take a moment to contemplate whether the `sqlserver` service should be defined in the `docker-compose.yml` file or elsewhere. Remember that running SQL Server from a container is not recommended from a production scenario unless special measures have been taken. For local development purposes it might be useful to have a SQL Server instance that loses its data on each start of the hosting container. You can also choose to use a volume mapping to avoid data loss.

Change the location of the definition to the override compose file. Merge it with the existing service. 

Enhance the override file by adding the dependencies of the web api on the `sqlserver` service. A dependent service indicates this by adding a `depends_on` naming the dependency by service name. In our case this would mean the following at the end of the `retrogamingwebapi` service in the `docker-compose.override.yml` file:

```yaml
depends_on:
  - "sqlserver"
```

## Wrapup
In this lab you have examined the way environments can be used to distinguish various hosting situations for your Docker composition. It is important to know which settings must be changeable for different environemnts, as the Docker images that you build cannot be changed internally.

Continue with [Lab 6 - Registries and clusters](Lab6-RegistriesClusters.md).
