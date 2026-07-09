FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos la solución y el proyecto de notificaciones
COPY ["API_Reserva_Hotel.sln", "./"]
COPY ["API_Hotel.Notifications/API_Hotel.Notifications.csproj", "API_Hotel.Notifications/"]

# Restauramos dependencias
RUN dotnet restore "API_Hotel.Notifications/API_Hotel.Notifications.csproj"

# Copiamos el resto del código
COPY ["API_Hotel.Notifications/", "API_Hotel.Notifications/"]

# Compilamos y publicamos
WORKDIR "/src/API_Hotel.Notifications"
RUN dotnet publish "API_Hotel.Notifications.csproj" -c Release -o /app/publish

# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "API_Hotel.Notifications.dll"]
