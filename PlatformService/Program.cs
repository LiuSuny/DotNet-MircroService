using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

//checking if we are running on production environment or not to determined which db to use
if(builder.Environment.IsProduction()){

  Console.WriteLine("--> Using SqlServer Db");
   builder.Services.AddDbContext<AppDbContext>(opt => 
   opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));

}else{
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
Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope(); 
  var services = scope.ServiceProvider;    
  try
     {
           
     //Returns a service object of type DataContext.
      var context = services.GetRequiredService<AppDbContext>();

      //if app is in production we run migrations         
        if(app.Environment.IsProduction())
          Console.WriteLine("--> Attempting to apply migrations...");
         context.Database.Migrate();            
        PrepDataSeeding.SeedData(context);
    }
      catch (Exception ex)
    {               
      var logger  = app.Services.GetRequiredService<ILogger<Program>>();
      logger.LogError(ex, "An error occured during migration");
    }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

 

app.Run();
