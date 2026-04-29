FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["ApartmentRentals.sln", "./"]
COPY ["ApartmentRentals.WebAPI/ApartmentRentals.WebAPI.csproj", "ApartmentRentals.WebAPI/"]
COPY ["ApartmentRentals.Data/ApartmentRentals.Data.csproj", "ApartmentRentals.Data/"]
COPY ["ApartmentRentals.Tests/ApartmentRentals.Tests.csproj", "ApartmentRentals.Tests/"]

COPY . .

RUN dotnet restore "ApartmentRentals.sln"
RUN dotnet build "ApartmentRentals.sln" -c $BUILD_CONFIGURATION

WORKDIR "/src/ApartmentRentals.WebAPI"
RUN dotnet publish "ApartmentRentals.WebAPI.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# FROM base AS final
# WORKDIR /app
# COPY --from=build /app/publish .

WORKDIR /app/publish

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "ApartmentRentals.WebAPI.dll"]