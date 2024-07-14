using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;

var builder = WebApplication.CreateBuilder(args);

// Register the DbContext with the DI container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure middleware
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();