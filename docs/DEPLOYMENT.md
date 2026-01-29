# Przewodnik Wdrożenia

## 🚀 Przegląd Wdrożenia

Ten przewodnik obejmuje opcje wdrożenia i konfigurację dla aplikacji CRM Demo.

---

## 📋 Wymagania

### Środowisko Deweloperskie
- .NET 8.0 SDK
- Docker Desktop
- Node.js 18+
- PostgreSQL 16 (lub Docker)
- Apache Kafka (lub Docker)

### Środowisko Produkcyjne
- .NET 8.0 Runtime
- Baza danych PostgreSQL 16
- Klaster Apache Kafka
- Reverse proxy (Nginx, IIS, etc.)
- Certyfikaty SSL

---

## 🐳 Wdrożenie Docker

### Usługi Infrastrukturalne

Uruchom wymagane usługi używając Docker Compose:

```bash
docker-compose up -d
```

To uruchamia:
- **PostgreSQL** na porcie 5432
- **Zookeeper** na porcie 2181
- **Kafka** na portach 9092 (wewnętrzny) i 9093 (zewnętrzny)

### Zmienne Środowiskowe

Utwórz plik `.env` lub ustaw zmienne środowiskowe:

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=CRM_DEMO
KAFKA_BOOTSTRAP_SERVERS=localhost:9093
```

---

## 🔧 Konfiguracja

### Ustawienia Aplikacji

#### Development (`appsettings.Development.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CRM_DEMO;Username=postgres;Password=postgres"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9093"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### Production (`appsettings.Production.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=prod-db-host;Port=5432;Database=CRM_DEMO;Username=prod_user;Password=secure_password"
  },
  "Kafka": {
    "BootstrapServers": "kafka-cluster:9092"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Error"
    }
  },
  "AllowedHosts": "yourdomain.com"
}
```

---

## 🗄️ Konfiguracja Bazy Danych

### 1. Uruchom Migracje

```bash
cd CRM.Demo.Api
dotnet ef database update \
  --project ..\CRM.Demo.Infrastructure \
  --startup-project . \
  --context ApplicationDbContext
```

### 2. Zweryfikuj Bazę Danych

Połącz się z PostgreSQL i zweryfikuj tabele:
```sql
\c CRM_DEMO
\dt
```

Oczekiwane tabele:
- `Customers`
- `Contacts`
- `Tasks`
- `Notes`
- `__EFMigrationsHistory`

---

## 🌐 Wdrożenie API

### Opcja 1: Self-Hosted (Kestrel)

```bash
cd CRM.Demo.Api
dotnet publish -c Release -o ./publish
cd publish
dotnet CRM.Demo.Api.dll
```

### Opcja 2: IIS

1. Zainstaluj .NET 8.0 Hosting Bundle
2. Utwórz IIS Application Pool (No Managed Code)
3. Wdróż do strony IIS
4. Skonfiguruj bindings i SSL

### Opcja 3: Kontener Docker

Utwórz `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CRM.Demo.Api/CRM.Demo.Api.csproj", "CRM.Demo.Api/"]
COPY ["CRM.Demo.Application/CRM.Demo.Application.csproj", "CRM.Demo.Application/"]
COPY ["CRM.Demo.Domain/CRM.Demo.Domain.csproj", "CRM.Demo.Domain/"]
COPY ["CRM.Demo.Infrastructure/CRM.Demo.Infrastructure.csproj", "CRM.Demo.Infrastructure/"]
RUN dotnet restore "CRM.Demo.Api/CRM.Demo.Api.csproj"
COPY . .
WORKDIR "/src/CRM.Demo.Api"
RUN dotnet build "CRM.Demo.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CRM.Demo.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRM.Demo.Api.dll"]
```

Build i uruchomienie:
```bash
docker build -t crm-demo-api .
docker run -p 5000:80 -e ConnectionStrings__DefaultConnection="..." crm-demo-api
```

---

## ⚛️ Wdrożenie Frontendu

### Development Build

```bash
cd CRM.Demo.Web
npm install
npm run dev
```

### Production Build

```bash
cd CRM.Demo.Web
npm install
npm run build
```

Wynik w katalogu `dist/`.

### Wdrożenie do Static Hosting

#### Opcja 1: Nginx
```nginx
server {
    listen 80;
    server_name yourdomain.com;
    
    root /var/www/crm-demo/dist;
    index index.html;
    
    location / {
        try_files $uri $uri/ /index.html;
    }
    
    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

#### Opcja 2: Azure Static Web Apps
```bash
npm install -g @azure/static-web-apps-cli
swa deploy ./dist --deployment-token <token>
```

#### Opcja 3: Vercel/Netlify
- Połącz repozytorium GitHub
- Skonfiguruj build command: `npm run build`
- Ustaw output directory: `dist`

---

## 🔐 Konfiguracja Bezpieczeństwa

### SSL/TLS

1. Uzyskaj certyfikat SSL (Let's Encrypt, komercyjny CA)
2. Skonfiguruj Kestrel:
```json
{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://*:443",
        "Certificate": {
          "Path": "/path/to/certificate.pfx",
          "Password": "certificate-password"
        }
      }
    }
  }
}
```

### Konfiguracja CORS

Zaktualizuj `Program.cs` dla produkcji:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

---

## 📊 Monitoring & Logging

### Application Insights (Azure)

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### Serilog (File/Console)

```csharp
builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.Console()
          .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day);
});
```

---

## 🔄 Pipeline CI/CD

### Przykład GitHub Actions

```yaml
name: Build and Deploy

on:
  push:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
      - run: dotnet build
      - run: dotnet test
      - run: dotnet publish -c Release -o ./publish
      - uses: actions/upload-artifact@v3
        with:
          name: publish
          path: ./publish
```

---

## 🧪 Health Checks

Dodaj endpoint health checks:

```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString)
    .AddKafka(kafkaConfig);

app.MapHealthChecks("/health");
```

---

## 📈 Uwagi o Wydajności

### Baza Danych
- Użyj connection pooling
- Skonfiguruj query timeouts
- Włącz query caching
- Użyj read replicas dla zapytań

### API
- Włącz response compression
- Skonfiguruj caching headers
- Używaj async/await wszędzie
- Zaimplementuj rate limiting

### Frontend
- Włącz gzip compression
- Użyj CDN dla statycznych assetów
- Zaimplementuj lazy loading
- Optymalizuj bundle size

---

## 🚨 Rozwiązywanie Problemów

### Problemy z Połączeniem do Bazy Danych
- Zweryfikuj connection string
- Sprawdź reguły firewall
- Upewnij się że PostgreSQL działa
- Zweryfikuj poświadczenia

### Problemy z Połączeniem do Kafka
- Zweryfikuj że Kafka działa: `docker ps`
- Sprawdź konfigurację bootstrap servers
- Zweryfikuj łączność sieciową
- Sprawdź logi Kafka: `docker logs crm-demo-kafka`

### Problemy z Migracjami
- Upewnij się że baza danych istnieje
- Sprawdź historię migracji
- Zweryfikuj że narzędzia EF Core są zainstalowane
- Przejrzyj pliki migracji

---

## 📝 Uwagi Specyficzne dla Środowiska

### Development
- Użyj prostych poświadczeń (tylko dla demo)
- Włącz szczegółowe logowanie
- Użyj HTTP (nie HTTPS)
- Zezwól na wszystkie źródła CORS

### Production
- Użyj silnych haseł
- Włącz tylko HTTPS
- Ogranicz CORS do konkretnych źródeł
- Użyj zmiennych środowiskowych dla sekretów
- Włącz security headers
- Zaimplementuj rate limiting
- Skonfiguruj monitoring i alerty

---

## 👤 Autor

**Arkadiusz Słota**

- 🔗 **LinkedIn**: [www.linkedin.com/in/arkadiusz-słota-229551172](https://www.linkedin.com/in/arkadiusz-słota-229551172)
- 💻 **GitHub**: [https://github.com/Maggio333/CRM.Demo](https://github.com/Maggio333/CRM.Demo)

---

**Ostatnia aktualizacja:** 2026-01-29
