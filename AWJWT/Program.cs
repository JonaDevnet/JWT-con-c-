//// formas de cors
////builder.Services.AddCors(options =>
////{
////    options.AddPolicy("NewPolicy", app =>
////    {
////        app.AllowAnyHeader().AllowAnyMethod().AllowCredentials();
////    });
////});
//builder.Services.AddCors(policyBuilder => policyBuilder.AddDefaultPolicy(policy => policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod()));

using AWJWT.Customs;
using AWJWT.Models;
using AWJWT.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de la conexión a la base de datos
builder.Services.AddDbContext<BdjwtContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL"));
});

// Servicios personalizados
builder.Services.AddSingleton<Utilities>();
builder.Services.AddScoped<JwtService>();

// Configuración de autenticación JWT
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]!))
    };
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Permite solicitudes desde el cliente React
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Permite cookies y credenciales
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aplica la política de CORS antes de la autenticación
app.UseCors("AllowFrontendLocalhost");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
