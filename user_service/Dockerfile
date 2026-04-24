FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["ApartmentRentals.sln", "./"]
COPY ["ApartmentRentals.WebAPI/ApartmentRentals.WebAPI.csproj", "ApartmentRentals.WebAPI/"]
COPY ["ApartmentRentals.Data/ApartmentRentals.Data.csproj", "ApartmentRentals.Data/"]
COPY ["ApartmentRentals.Tests/ApartmentRentals.Tests.csproj", "ApartmentRentals.Tests/"]

RUN dotnet restore "ApartmentRentals.sln"

COPY . .

WORKDIR "/src/ApartmentRentals.WebAPI"
RUN dotnet publish "ApartmentRentals.WebAPI.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "ApartmentRentals.WebAPI.dll"]