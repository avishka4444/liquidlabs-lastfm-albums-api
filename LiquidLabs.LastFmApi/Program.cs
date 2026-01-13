using LiquidLabs.LastFmApi.Data;
using LiquidLabs.LastFmApi.LastFm;
using LiquidLabs.LastFmApi.Services;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;

DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IAlbumService, AlbumService>();

builder.Services.AddHttpClient<ILastFmClient, LastFmClient>(c =>
{
    c.BaseAddress = new Uri("https://ws.audioscrobbler.com");
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Last.fm Albums API",
        Version = "v1",
        Description = "API for fetching and caching top albums from Last.fm"
    });
});

var app = builder.Build();

// Configure the HTTP requests
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Last.fm Albums API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.MapGet("/health/db", async (IConfiguration config) =>
{
    var cs = config.GetConnectionString("DefaultConnection");
    await using var conn = new SqlConnection(cs);
    await conn.OpenAsync();

    await using var cmd = new SqlCommand("SELECT 1", conn);
    var resultObj = await cmd.ExecuteScalarAsync();
    var result = Convert.ToInt32(resultObj);

    return Results.Ok(new { db = "ok", result });
});

app.MapControllers();

app.Run();
