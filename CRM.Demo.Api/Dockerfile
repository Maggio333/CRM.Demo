# Multi-stage build dla .NET API
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiuj pliki .csproj i przywróć zależności
COPY ["CRM.Demo.Api/CRM.Demo.Api.csproj", "CRM.Demo.Api/"]
COPY ["CRM.Demo.Application/CRM.Demo.Application.csproj", "CRM.Demo.Application/"]
COPY ["CRM.Demo.Domain/CRM.Demo.Domain.csproj", "CRM.Demo.Domain/"]
COPY ["CRM.Demo.Infrastructure/CRM.Demo.Infrastructure.csproj", "CRM.Demo.Infrastructure/"]

RUN dotnet restore "CRM.Demo.Api/CRM.Demo.Api.csproj"

# Kopiuj resztę kodu
COPY . .

# Zbuduj aplikację
WORKDIR "/src/CRM.Demo.Api"
RUN dotnet build "CRM.Demo.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
WORKDIR "/src/CRM.Demo.Api"
RUN dotnet publish "CRM.Demo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Skopiuj opublikowane pliki
COPY --from=publish /app/publish .

# Expose porty
EXPOSE 80
EXPOSE 443

# Uruchom aplikację
ENTRYPOINT ["dotnet", "CRM.Demo.Api.dll"]
