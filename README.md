# AEX DEVTEST

## Overview

This is the core backend responsible for processing Vehicle information. It exposes a set of RESTful APIs.

This project is built on .NET 6.0. Downloads for the SDK and runtime are available from the [dotnet website](https://dotnet.microsoft.com/download).

To view the available endpoints, please refer to the Swagger Documentation (https://localhost:7089/swagger/index.html).

## Helpful Development Tips

### appsettings.json

Dotnet uses a JSONC file format (JSON with comments) to store configuration and secrets. This lives inside the `aex.devtest.application` folder, and comes with a [blank version](/src/Application/aex.devtest.application/appsettings.json) which describes the available fields.

src/Application/appsettings.json

When running the application locally, the code expects a filled-out version of this file to be available at `src/Application/aex.devtest/appsettings.[ENVIRONMENT].json`.

Here, `[ENVIRONMENT]` defaults to `local`, and can be overridden by the `ASPNETCORE_ENVIRONMENT` environment variable. In practice, use `appsettings.local.json` for local development. 

### Local SQL Database

The following `docker-compose` file may be useful to you if you'd like to run a local copy of the database

```yaml
version: '3.3'
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2019-latest
    container_name: sqlserver
    restart: always
    environment:
      ACCEPT_EULA: Y
      SA_PASSWORD: 'Password!1'
    ports:
      - '1433:1433'
    expose:
      - '1433'
    volumes:
      - sqlserver-volume:/var/opt/mssql

volumes:
  sqlserver-volume:
```

In order to use this, configure your `appsettings.Local.json` to have the following value for `ConnectionStrings.DefaultConnection`

```
Data Source=127.0.0.1,1433;aex.devtest;Trusted_Connection=False;MultipleActiveResultSets=true;Application Name=aex.devtest;User ID=sa;Password=Password!1;
```
