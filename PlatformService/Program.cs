using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

//checking if we are running on production environment or not to determined which db to use
if (builder.Environment.IsProduction())
{

  Console.WriteLine("--> Using SqlServer Db");
  builder.Services.AddDbContext<AppDbContext>(opt =>
  opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));

}
else
{
  Console.WriteLine("--> Using InMem Db");
  builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();

Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}


app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endPoint =>
{

  endPoint.MapControllers();
  endPoint.MapGrpcService<GrpcPlatformService>();

  endPoint.MapGet("/protos/platforms.proto", async context =>
              {
                await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
              });

});

  PrepDataSeeding.PrepPopulation(app, app.Environment.IsProduction());

app.Run();
