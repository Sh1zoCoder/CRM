using CRM.Storage;
using Serilog;

Log.Logger = new LoggerConfiguration()
.WriteTo.Console()
.WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<JsonFileLeadStore>();

// Добавляем Swagger
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

Console.Clear();

app.Run();