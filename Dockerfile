# Etapa 1: Base para correr la API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Etapa 2: Construcción (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos la solución y los proyectos primero (para aprovechar el caché de Docker)
COPY ["API_Reserva_Hotel.sln", "./"]
COPY ["API_Reserva_Hotel/API_Reserva_Hotel.csproj", "API_Reserva_Hotel/"]
COPY ["API_Hotel.Application/API_Hotel.Application.csproj", "API_Hotel.Application/"]
COPY ["API_Hotel.Domain/API_Hotel.Domain.csproj", "API_Hotel.Domain/"]
COPY ["API_Hotel.Infrastructure/API_Hotel.Infrastructure.csproj", "API_Hotel.Infrastructure/"]
COPY ["API_Hotel.Tests/API_Hotel.Tests.csproj", "API_Hotel.Tests/"]
COPY ["API_Hotel.Notifications/API_Hotel.Notifications.csproj", "API_Hotel.Notifications/"]

# Restauramos los paquetes de Nuget
RUN dotnet restore "API_Reserva_Hotel.sln"

# Copiamos todo el resto del código
COPY . .

# Compilamos el proyecto principal de la API
WORKDIR "/src/API_Reserva_Hotel"
RUN dotnet build "API_Reserva_Hotel.csproj" -c Release -o /app/build

# Publicamos el ejecutable final
FROM build AS publish
RUN dotnet publish "API_Reserva_Hotel.csproj" -c Release -o /app/publish

# Etapa 3: Imagen Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API_Reserva_Hotel.dll"]