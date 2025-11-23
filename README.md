# POC Project Documentation

## Overview

This project is a Proof of Concept (POC) using Duende Identity Server and AdminUI. The project structure includes multiple components such as IdentityServer, AdminUI, WeatherApi, and WeatherClient.

## Project Structure

```
IdentityAdminUI.sln
README.md
src/
    AdminUI/
        AdminUI.csproj
        appsettings.Development.json
        appsettings.json
        Program.cs
        wwwroot/
    IdentityServer/
        appsettings.Development.json
        appsettings.json
        Config.cs
        Database/
        IdentityServer.csproj
        keys/
    WeatherApi/
        WeatherApi.csproj
        Program.cs
        appsettings.Development.json
        appsettings.json
    WeatherClient/
        WeatherClient.csproj
        Program.cs
        appsettings.Development.json
        appsettings.json
        wwwroot/
        Models/
        Services/
        Components/
```

## Prerequisites

- .NET SDK
- Node.js (for AdminUI)
- Duende Identity Server
- AdminUI
- Local PostgreSQL instance

## Setup Instructions

### Install EF Core Tools

```sh
dotnet tool install --global dotnet-ef
```

### Migrations for IdentityServer

Navigate to the `IdentityServer` project directory and run the following commands:

```sh
dotnet ef migrations add InitialIdentityServerMigration -c ApplicationDbContext
dotnet ef migrations add InitialIdentityServerMigration -c ConfigurationDbContext
dotnet ef migrations add InitialIdentityServerMigration -c PersistedGrantDbContext

dotnet ef database update -c ApplicationDbContext
dotnet ef database update -c ConfigurationDbContext
dotnet ef database update -c PersistedGrantDbContext
```

### Running the Projects

You can run all projects by opening the solution file IdentityAdminUI.sln in Visual Studio and starting the solution.

#### IdentityServer

Navigate to the `IdentityServer` project directory and run:

```sh
dotnet run
```

#### AdminUI

Navigate to the `AdminUI` project directory and run:

```sh
dotnet run
```

### Configuration

#### IdentityServer

Configuration files for IdentityServer are located in the 

IdentityServer

 directory:

- `appsettings.Development.json`
- `appsettings.json`

#### AdminUI

Configuration files for AdminUI are located in the 

AdminUI

 directory:

- `appsettings.Development.json`
- `appsettings.json`

### WeatherApi

The `WeatherApi` project provides a simple API to fetch weather data. It includes controllers that handle HTTP requests and return weather information.

#### Running WeatherApi

Navigate to the `WeatherApi` project directory and run:

```sh
dotnet run
```

### WeatherClient

The `WeatherClient` project is a client application that consumes the `WeatherApi` to display weather data. It includes a simple console application that makes HTTP requests to the `WeatherApi` and displays the results.

#### Running WeatherClient

Navigate to the `WeatherClient` project directory and run:

```sh
dotnet run
```

### Documentation

For detailed documentation on IdentityServer and AdminUI, refer to the following resources:

- What is IdentityServer
- Integrating with IdentityServer
- Integrating with IdentityServer Custom Schema
- AdminUI Overview
- AdminUI Partial Implementation
- AdminUI Full Implementation

### Additional Resources

- [Duende IdentityServer Documentation](https://docs.duendesoftware.com/identityserver/v7/)
- [AdminUI Documentation](https://docs.identityserver.com/adminui/)