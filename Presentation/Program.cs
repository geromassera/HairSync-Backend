using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Presentation.Middleware;
using Application.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Application.Services;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Interfaces.External;
using Infrastructure.Services.External;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography.Xml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<ITreatmentService, TreatmentService>();
builder.Services.AddScoped<ITreatmentRepository, TreatmentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICustomAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IJokeService, JokeService>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretForKey"]!))
        };

    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    setupAction.AddSecurityDefinition("ApiBearerAuth", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Pegá acá el token JWT generado al loguearte."
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiBearerAuth" // Debe coincidir con el Id de AddSecurityDefinition
                }
            }, new List<string>() }
    });
});
// En Program.cs, en la sección de registro de servicios

builder.Services.AddSwaggerGen(setupAction =>
{
    setupAction.AddSecurityDefinition("ApiBearerAuth", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http, // El tipo de seguridad es HTTP
        Scheme = "Bearer", // El esquema es "Bearer" (como en "Bearer <token>") [cite: 396]
        Description = "Pegá acá el token JWT generado al loguearte." // [cite: 397]
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiBearerAuth" // Debe coincidir con el Id de AddSecurityDefinition
                }
            }, new List<string>() }
    });
});

const string JokeApiClientName = "JokeApiClient";

builder.Services.AddHttpClient(JokeApiClientName, client =>
{
    client.BaseAddress = new Uri("https://official-joke-api.appspot.com/");

    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .AllowAnyOrigin()    
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();