using API_Hotel.Application;
using API_Hotel.Infrastructure;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TuProyecto.API.Middlewares;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//(AUTHORIZE)
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa tu token JWT en el formato: {tu_token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var secret = builder.Configuration["Jwt:SecretKey"] ?? "SUPER_SECRET_KEY_FOR_JWT_API_HOTEL_2026_LEAD_DEV_LONG_KEY!!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "API_Hotel",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "API_Hotel_Clients",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {              
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var resultado = "{\n  \"Mensaje\": \"401 No Autorizado. Debes enviar un Token .\"\n}";

                return context.Response.WriteAsync(resultado);
            },

            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var resultado = "{\n  \"Mensaje\": \"403 Prohibido. Tu rol actual no tiene permisos suficientes para ejecutar esta acción.\"\n}";
                return context.Response.WriteAsync(resultado);
            }
        };
    });

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration); 

var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();