using Microsoft.EntityFrameworkCore;
using PlatformService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

 using var scope = app.Services.CreateScope(); 
  var services = scope.ServiceProvider;    
  try
     {
           
     //Returns a service object of type DataContext.
      var context = services.GetRequiredService<AppDbContext>();
               
     // context.Database.Migrate(); 
      PrepDataSeeding.SeedData(context);
    }
      catch (Exception ex)
    {
                
      var logger  = app.Services.GetRequiredService<ILogger<Program>>();
      logger.LogError(ex, "An error occured during migration");
    }

app.Run();
