using Microsoft.EntityFrameworkCore;
using PlatformService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>(); //регистрация зависимости - Реализацией интерфейса IPlatformRepo теперь является PlatformRepo

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();

var app = builder.Build();

PrepDb.PrerPopulation(app);
// Configure the HTTP request pipeline.

app.MapOpenApi();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
