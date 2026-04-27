# TaskManager

A full-stack .NET Todo application built with a Blazor Server UI, an ASP.NET Core minimal API, and a shared model library.

[![Status](https://img.shields.io/badge/status-Beta-yellow?style=flat)](#)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white&style=flat)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-Server-5C2D91?logo=blazor&logoColor=white&style=flat)](https://learn.microsoft.com/aspnet/core/blazor/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Minimal%20API-0C7CD5?logo=dotnet&logoColor=white&style=flat)](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis)
[![EF Core](https://img.shields.io/badge/EF%20Core-InMemory-6DB33F?style=flat)](https://learn.microsoft.com/ef/core/)

---

## Table of Contents

- [Project Overview](#project-overview)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [API Reference](#api-reference)
- [Project Structure](#project-structure)
- [Docker](#docker)
- [Contributing](#contributing)
- [License](#license)

---

## Project Overview

TaskManager helps you manage daily work with a clean Todo workflow:

- Create, update, complete, and delete tasks.
- Search todos by title/description.
- Filter by all, complete, or incomplete.
- Handle temporary API outages with automatic retry from the UI.

The solution is split into three projects:

- `TaskManager.Client`: Blazor Server frontend.
- `TaskManager.Api`: ASP.NET Core minimal API with EF Core InMemory persistence.
- `TaskManager.Shared`: shared `Todo` model used by both API and client.

---

## Tech Stack

- .NET 10
- ASP.NET Core Minimal API
- Blazor Server (interactive)
- Entity Framework Core InMemory provider
- NSwag / OpenAPI for Swagger docs
- Docker (separate API and Client Dockerfiles)

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- Optional: Docker Desktop

### Installation

```sh
git clone <your-repository-url>
cd TaskManager
dotnet restore TaskManager.slnx
```

### Run Locally

Start the API:

```sh
cd TaskManager.Api
dotnet run
```

Start the client in another terminal:

```sh
cd TaskManager.Client
dotnet run
```

Default local URLs:

- Client: `http://localhost:5237` or `https://localhost:7251`
- API: `http://localhost:5193`
- Swagger UI: `http://localhost:5193/swagger`

### Configuration

- API CORS origins are configured in `TaskManager.Api/appsettings.Development.json` under `Cors:AllowedOrigins`.
- Client API base URL is configured in `TaskManager.Client/appsettings.Development.json` under `TodoApi:BaseUrl`.

---

## API Reference

Base URL: `http://localhost:5193`

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/todoitems` | Get all todos |
| GET | `/todoitems/completed` | Get completed todos |
| GET | `/todoitems/{id}` | Get todo by ID |
| POST | `/todoitems` | Create a new todo |
| PUT | `/todoitems/{id}` | Update an existing todo |
| DELETE | `/todoitems/{id}` | Delete todo by ID |

Todo model fields:

| Field | Type |
| --- | --- |
| `Id` | int |
| `Title` | string |
| `Description` | string? |
| `IsCompleted` | bool |
| `CreatedAt` | DateTime |
| `UpdatedAt` | DateTime? |

---

## Project Structure

```text
TaskManager/
|- TaskManager.Api/        # Minimal API + EF Core InMemory
|- TaskManager.Client/     # Blazor Server UI
|- TaskManager.Shared/     # Shared DTO/model library
|- Dockerfile.Api
|- Dockerfile.Client
|- TaskManager.slnx
```

---

## Docker

Build images:

```sh
docker build -f Dockerfile.Api -t taskmanager-api .
docker build -f Dockerfile.Client -t taskmanager-client .
```

Run containers on a shared network:

```sh
docker network create taskmanager-net

docker run -d --name taskmanager-api --network taskmanager-net -p 5193:8080 taskmanager-api

docker run -d --name taskmanager-client --network taskmanager-net -p 5237:8080 \
	-e TodoApi__BaseUrl=http://taskmanager-api:8080/ taskmanager-client
```

Then open `http://localhost:5237`.

---

## Contributing

Contributions are welcome.

1. Fork the repository.
2. Create a feature branch.
3. Commit your changes.
4. Open a pull request.

---

## License

No license file is currently included in this folder.

If you plan to open-source this project, add a `LICENSE` file (for example MIT) and update this section.
