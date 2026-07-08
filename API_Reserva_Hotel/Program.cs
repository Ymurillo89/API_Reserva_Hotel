using API_Hotel.Application;
using API_Hotel.Infrastructure; // Ensure this namespace is correct and contains the AddInfrastructure extension method

var builder = WebApplication.CreateBuilder(args);

// 1. Añadimos controladores y Swagger para probar visualmente
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. CONECTAMOS NUESTRAS CAPAS (Clean Architecture)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration); // Ensure AddInfrastructure is implemented in the Infrastructure namespace

var app = builder.Build();

// 3. Configuración de Middleware en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();