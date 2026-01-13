# Last.fm Albums API

This is an ASP.NET Core Web API that fetches top albums from the Last.fm API and caches the data in Microsoft SQL Server. This API uses for the caching mechanism that retrieves data from the database if available and fresh, otherwise fetches from Last.fm API, saves to the database, and returns the data.

## What This Project Does

This API fetches top albums from Last.fm for a hardcoded artist (The Beatles) and provides RESTful endpoints to retrieve the cached album data. If data exists in the database and is fresh (within 30 minutes), it returns from the database. Otherwise, it fetches fresh data from Last.fm and saves it to the database, then returns it.

https://www.last.fm/api/show/artist.getTopAlbums

## Features

- Fetches top albums from Last.fm API
- Caches data in SQL Server database with 30-minute TTL
- RESTful API endpoints to retrieve album data

## Technologies and Frameworks Used

### Microsoft.AspNetCore.OpenApi (v9.0.10)
Provides OpenAPI/Swagger 

### Microsoft.Data.SqlClient (v6.1.3)
Official Microsoft library for SQL Server database connectivity. 

### DotNetEnv (v3.1.1)
Loads environment variables from `.env` files. 

### ASP.NET Core Built-in Frameworks
- **ASP.NET Core Web API**: Core framework for building REST APIs
- **Dependency Injection**: Built-in DI container for managing service lifetimes
- **HttpClient**: For making HTTP requests to Last.fm API

## Prerequisites

- .NET 9.0 SDK or later
- Docker and Docker Compose (for SQL Server)
- A Last.fm API key

## Environment Variables

Set the following .env variables

- `LASTFM_API_KEY`: Last.fm API key
- `MSSQL_SA_PASSWORD`: SQL Server SA password for Docker container (required for docker-compose)

Create a `.env` file in the project root, and add those variables.

## Setup Instructions

1. **Clone the repository** and navigate to the project directory

2. **Set environment variables**: Create a `.env` file in the project root with your Last.fm API key and SQL Server password

3. **Start the database**: If using Docker Compose, start the SQL Server container using docker-compose up -d

4. **Create the database**: Create a database named `LastFmDb` in SQL Server

5. **Run the schema script**: Execute the `schema.sql` file located in `LiquidLabs.LastFmApi/Database/`. It will create the tables

6. **Navigate to the project directory**: Change to the `LiquidLabs.LastFmApi` directory

7. **Restore dependencies**: Run dotnet restore

8. **Build the project**: Run dotnet build

9. **Run the application**: Run dotnet run

The API will be available at:
- HTTP: `http://localhost:5215`
- HTTPS: `https://localhost:7043`

## API Endpoints

### Get All Albums
**Endpoint:** `GET /albums`

Returns a list of all top albums for The Beatles.

### Get Album by ID
**Endpoint:** `GET /albums/{id}`

Returns a single album by its database ID. Returns 400 Bad Request if ID is not a positive integer, or 404 Not Found if the album does not exist.

### Health Check
**Endpoint:** `GET /health/db`

Checks database connectivity.

## Caching Behavior

The API implements a caching mechanism

1. **First Request**: Fetches data from Last.fm API, saves to database, returns the data
2. **Subsequent Requests (within 30 minutes)**: Returns data from database (cache hit)
3. **After 30 minutes**: Fetches fresh data from Last.fm API, updates database, returns the data

The cache TTL is set to 30 minutes and can be adjusted in `AlbumService.cs`.

## Project Structure

```
LiquidLabs.LastFmApi/
├── Controllers/
│   └── AlbumsController.cs          # API endpoints
├── Data/
│   ├── IAlbumRepository.cs          # Repository interface
│   └── AlbumRepository.cs           # Database access (direct SQL)
├── Database/
│   └── schema.sql                   # Database schema
├── LastFm/
│   ├── ILastFmClient.cs             # Last.fm client interface
│   ├── LastFmClient.cs              # Last.fm API client
│   └── Dtos.cs                       # Data transfer objects
├── Models/
│   └── Album.cs                      # Domain model
├── Services/
│   ├── IAlbumService.cs             # Service interface
│   └── AlbumService.cs              # Business logic
├── Program.cs                        # Application entry point
└── appsettings.json                  # Configuration
```

## Design Decisions

**Hardcoded Artist**: The artist is hardcoded to "The Beatles" as a simplification. This can be easily changed by modifying the `HardcodedArtist` constant in `AlbumService.cs`.

