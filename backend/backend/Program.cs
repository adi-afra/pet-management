using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.classes;

var builder = WebApplication.CreateBuilder(args);

// ------------------- Configure Services -------------------

// Add controllers
builder.Services.AddControllers();

// Configure EF Core with SQL Server (AWS RDS)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Swagger for API testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ------------------- Configure Middleware -------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();   
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();