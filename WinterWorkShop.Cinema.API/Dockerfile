#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WinterWorkShop.Cinema.API/WinterWorkShop.Cinema.API.csproj", "WinterWorkShop.Cinema.API/"]
COPY ["WinterWorkShop.Cinema.Domain/WinterWorkShop.Cinema.Domain.csproj", "WinterWorkShop.Cinema.Domain/"]
COPY ["WinterWorkShop.Cinema.Repositories/WinterWorkShop.Cinema.Repositories.csproj", "WinterWorkShop.Cinema.Repositories/"]
COPY ["WinterWorkShop.Cinema.Data/WinterWorkShop.Cinema.Data.csproj", "WinterWorkShop.Cinema.Data/"]
RUN dotnet restore "WinterWorkShop.Cinema.API/WinterWorkShop.Cinema.API.csproj"
COPY . .
WORKDIR "/src/WinterWorkShop.Cinema.API"
RUN dotnet build "WinterWorkShop.Cinema.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WinterWorkShop.Cinema.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WinterWorkShop.Cinema.API.dll"]