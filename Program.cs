using Serilog;
using Microsoft.EntityFrameworkCore;
using CRM.Data;

Log.Logger = new LoggerConfiguration()
.WriteTo.Console()
.WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Добавляем Swagger
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<TokenMiddleware>();

app.MapControllers();

Console.Clear();

app.Run();