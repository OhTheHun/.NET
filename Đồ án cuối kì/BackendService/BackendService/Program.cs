using BackendService.Data;
using BackendService.Data.DataContext;
using BackendService.Data.Interface;
using BackendService.FluentValidation;
using BackendService.Services;
using BackendService.Services.Interface;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES =================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostgresDbContext>(options =>
                  options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresDb")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200") 
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// DI


var app = builder.Build();

// ================= MIDDLEWARE =================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowWebApp"); 

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
