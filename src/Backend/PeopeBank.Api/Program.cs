using PeopleBank.Api.Middleware;
using PeopleBank.Application;
using PeopleBank.Infrastructure;
using PeopleBank.Infrastructure.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (!builder.Configuration.GetValue<bool>("SkipMigrations"))
{
    await ExecuteMigrations();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//await ExecuteMigrations();

if (!builder.Configuration.GetValue<bool>("SkipMigrations"))
{
    await ExecuteMigrations();
}

app.Run();

async Task ExecuteMigrations()
{
    await using var scope = app.Services.CreateAsyncScope();

    DatabaseMigration.ExecuteMigrations(scope.ServiceProvider);
}

public partial class Program { } 