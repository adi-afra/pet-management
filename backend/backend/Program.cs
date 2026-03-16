using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.classes;

var builder = WebApplication.CreateBuilder(args);

// ------------------- Configure Services -------------------

// Add controllers
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:63343") // match your frontend port
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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


// ------------------- Middleware -------------------
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

// Apply the CORS policy here
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();