using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.classes;

var builder = WebApplication.CreateBuilder(args);

// ------------------- Configure Services -------------------
builder.Services.AddControllers();

// CORS with credentials
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:63342") // your frontend
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // required for cookies/sessions
    });
});

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add these before builder.Build()
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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

//  Order matters here:
app.UseRouting();       // Needed for session + endpoint routing
app.UseCors("AllowFrontend");

app.UseSession();       // Must be BEFORE MapControllers
app.UseAuthorization();

app.MapControllers();

app.Run();