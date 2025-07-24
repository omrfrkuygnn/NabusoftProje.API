using Event.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// EF Core DbContext servisi ekleniyor
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddHostedService<EventStatusUpdateService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder =>
        {
            builder.WithOrigins("https://localhost:7256") // Frontend URL'ni yaz
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});


// OpenAPI (Swagger) eklemesi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                // JSON endpoint: /swagger/v1/swagger.json
    app.UseSwaggerUI();             // Görsel arayüz: /swagger
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowLocalhost");

app.MapControllers();

app.Run();
