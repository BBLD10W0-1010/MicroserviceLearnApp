using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataService.Http;
using PlatformService.AsyncDataServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SQLServer db");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else
{
    Console.WriteLine("--> Using InMem DB");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>(); //регистрация зависимости - Реализацией интерфейса IPlatformRepo теперь является PlatformRepo

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

Console.WriteLine(
    $"--> CommandService endpoint: {builder.Configuration["CommandService"]}"
);

var app = builder.Build();

PrepDb.PrerPopulation(app, builder.Environment.IsProduction());

await app.Services.GetRequiredService<IMessageBusClient>().ConnectAsync();

app.MapOpenApi();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
