using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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
