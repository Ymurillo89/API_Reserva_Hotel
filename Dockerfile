FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["API_Reserva_Hotel.sln", "./"]
COPY ["API_Reserva_Hotel/API_Reserva_Hotel.csproj", "API_Reserva_Hotel/"]
COPY ["API_Hotel.Application/API_Hotel.Application.csproj", "API_Hotel.Application/"]
COPY ["API_Hotel.Domain/API_Hotel.Domain.csproj", "API_Hotel.Domain/"]
COPY ["API_Hotel.Infrastructure/API_Hotel.Infrastructure.csproj", "API_Hotel.Infrastructure/"]
COPY ["API_Hotel.Tests/API_Hotel.Tests.csproj", "API_Hotel.Tests/"]
COPY ["API_Hotel.Notifications/API_Hotel.Notifications.csproj", "API_Hotel.Notifications/"]

RUN dotnet restore "API_Reserva_Hotel.sln"

COPY . .

WORKDIR "/src/API_Reserva_Hotel"
RUN dotnet build "API_Reserva_Hotel.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API_Reserva_Hotel.csproj" -c Release -o /app/publish

FROM build AS publish-notifications
WORKDIR "/src/API_Hotel.Notifications"
RUN dotnet publish "API_Hotel.Notifications.csproj" -c Release -o /app/publish

FROM base AS api-final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API_Reserva_Hotel.dll"]

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS notifications-final
WORKDIR /app
COPY --from=publish-notifications /app/publish .
ENTRYPOINT ["dotnet", "API_Hotel.Notifications.dll"]