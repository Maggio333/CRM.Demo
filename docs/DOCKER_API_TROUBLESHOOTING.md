# Rozwiązywanie Problemów z API w Dockerze

## 🔍 Częste Problemy i Rozwiązania

### Problem 1: "Cannot connect to database"

**Objawy:**
```
System.Net.Sockets.SocketException: Connection refused
```

**Przyczyna:**
API próbuje połączyć się z `localhost:5432`, ale w Dockerze `localhost` to sam kontener, nie host.

**Rozwiązanie:**
Użyj nazwy serwisu Docker Compose zamiast `localhost`:
```yaml
environment:
  - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=CRM_DEMO;Username=postgres;Password=postgres
```

---

### Problem 2: "Kafka connection failed"

**Objawy:**
```
Confluent.Kafka: Failed to connect to broker
```

**Przyczyna:**
Kafka używa `localhost:9092` w advertised listeners, ale w Dockerze kontenery komunikują się przez nazwy serwisów.

**Rozwiązanie:**
Skonfiguruj Kafka z dwoma listenerami:
```yaml
KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka:9092,PLAINTEXT_HOST://localhost:9092
KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,PLAINTEXT_HOST:PLAINTEXT
KAFKA_INTER_BROKER_LISTENER_NAME: PLAINTEXT
```

W API użyj:
```yaml
- Kafka__BootstrapServers=kafka:9092
```

---

### Problem 3: "Port already in use"

**Objawy:**
```
Error: bind: address already in use
```

**Przyczyna:**
Port 5000 lub 5001 jest już zajęty przez lokalnie uruchomione API.

**Rozwiązanie:**
1. Zatrzymaj lokalne API: `Ctrl+C` w terminalu
2. Lub zmień porty w `docker-compose.yml`:
```yaml
ports:
  - "5002:80"  # Zamiast 5000
  - "5003:443" # Zamiast 5001
```

---

### Problem 4: "Migration failed"

**Objawy:**
```
No connection string named 'DefaultConnection' was found
```

**Przyczyna:**
Migracje są uruchamiane lokalnie, ale connection string wskazuje na `localhost`.

**Rozwiązanie:**
Uruchom migracje w kontenerze API:
```bash
docker exec -it crm-demo-api dotnet ef database update --project /app/Infrastructure --startup-project /app --context ApplicationDbContext
```

Lub użyj connection string z nazwą serwisu:
```bash
dotnet ef database update \
  --project ../CRM.Demo.Infrastructure \
  --startup-project . \
  --context ApplicationDbContext \
  --connection "Host=localhost;Port=5432;Database=CRM_DEMO;Username=postgres;Password=postgres"
```

---

### Problem 5: "Build failed - project not found"

**Objawy:**
```
error: The project file 'CRM.Demo.Api.csproj' was not found
```

**Przyczyna:**
Dockerfile używa nieprawidłowego context lub ścieżek.

**Rozwiązanie:**
Upewnij się, że w `docker-compose.yml`:
```yaml
api:
  build:
    context: .  # Główny katalog projektu
    dockerfile: CRM.Demo.Api/Dockerfile
```

---

### Problem 6: "SSL Certificate errors"

**Objawy:**
```
Failed to bind to address https://[::]:443: address already in use
```

**Przyczyna:**
Kestrel próbuje użyć HTTPS, ale nie ma certyfikatu w kontenerze.

**Rozwiązanie:**
W Dockerze użyj tylko HTTP (port 80):
```yaml
environment:
  - ASPNETCORE_URLS=http://+:80
```

Lub wyłącz HTTPS w `Program.cs` dla Docker:
```csharp
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Docker")
{
    builder.WebHost.UseUrls("http://*:80");
}
```

---

## ✅ Sprawdzenie Czy Wszystko Działa

### 1. Sprawdź kontenery
```bash
docker ps
```
Powinieneś zobaczyć:
- `crm-demo-postgres`
- `crm-demo-zookeeper`
- `crm-demo-kafka`
- `crm-demo-api`
- `crm-demo-frontend`

### 2. Sprawdź logi API
```bash
docker logs crm-demo-api
```

Powinieneś zobaczyć:
```
Now listening on: http://[::]:80
Application started.
```

### 3. Sprawdź połączenie z bazą
```bash
docker exec -it crm-demo-api dotnet ef migrations list --project /app/Infrastructure --startup-project /app --context ApplicationDbContext
```

### 4. Sprawdź API
```bash
curl http://localhost:5000/swagger
```

---

## 🎯 Najlepsze Praktyki

1. **Zawsze używaj nazw serwisów** w Dockerze zamiast `localhost`
2. **Sprawdzaj logi** kontenerów: `docker logs <container-name>`
3. **Używaj health checks** w docker-compose.yml
4. **Testuj lokalnie** przed wdrożeniem do Dockera
5. **Używaj zmiennych środowiskowych** zamiast hardcodowanych wartości

---

## 📚 Przydatne Komendy

```bash
# Zobacz logi API
docker logs -f crm-demo-api

# Wejdź do kontenera API
docker exec -it crm-demo-api /bin/bash

# Zrestartuj API
docker-compose restart api

# Zbuduj API od nowa
docker-compose build api
docker-compose up -d api

# Sprawdź sieć Docker
docker network inspect crm-demo_crm-demo-network
```

---

**Ostatnia aktualizacja:** 2026-01-29
