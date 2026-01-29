# Szybki Start - Instrukcja Uruchomienia

## 🚀 Szybkie Uruchomienie Aplikacji

Ten dokument zawiera szczegółowe instrukcje uruchomienia całej aplikacji CRM Demo.

---

## 📋 Wymagania Wstępne

Przed uruchomieniem upewnij się, że masz zainstalowane:

- ✅ **.NET 8.0 SDK** - [Pobierz tutaj](https://dotnet.microsoft.com/download/dotnet/8.0)
- ✅ **Docker Desktop** - [Pobierz tutaj](https://www.docker.com/products/docker-desktop)
- ✅ **Node.js 18+** - [Pobierz tutaj](https://nodejs.org/)
- ✅ **Git** (opcjonalnie, jeśli klonujesz repozytorium)

---

## 🔧 Ręczne Uruchomienie (Krok po Kroku)

### Krok 1: Uruchom Wszystkie Usługi w Dockerze

Otwórz terminal w głównym katalogu projektu i uruchom:

```bash
docker-compose up -d
```

To uruchomi:
- **PostgreSQL** na porcie 5432
- **Zookeeper** na porcie 2181
- **Kafka** na porcie 9092
- **API .NET** na portach 5000 (HTTP) i 5001 (HTTPS)
- **Frontend (React)** na porcie 5173 (serwowany przez Nginx)

**Wszystko działa w Dockerze - nie musisz uruchamiać niczego lokalnie!**

**Sprawdź czy kontenery działają:**
```bash
docker ps
```

Powinieneś zobaczyć 3 kontenery:
- `crm-demo-postgres`
- `crm-demo-zookeeper`
- `crm-demo-kafka`

---

### Krok 2: Migracje (Automatyczne!)

**🎉 Migracje są automatycznie stosowane przy starcie API!**

API automatycznie:
- ✅ Czeka na gotowość bazy danych (retry logic - 10 prób)
- ✅ Stosuje wszystkie oczekujące migracje
- ✅ Loguje postęp w konsoli

**Nie musisz ręcznie uruchamiać migracji!**

Jeśli chcesz sprawdzić logi migracji:
```bash
docker logs crm-demo-api
```

Powinieneś zobaczyć:
```
✅ Połączenie z bazą danych nawiązane
🔄 Stosowanie migracji bazy danych...
✅ Migracje zastosowane pomyślnie
```

**Uwaga:** W produkcji migracje powinny być uruchamiane ręcznie lub przez CI/CD pipeline.

---

### Krok 3: Sprawdź Czy Wszystko Działa

**API:**
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger: `http://localhost:5000/swagger`

**Frontend:**
- `http://localhost:5173`

**Wszystko działa w Dockerze - nie musisz uruchamiać niczego lokalnie!**

---

### Krok 4: Development (Opcjonalnie - jeśli chcesz uruchomić lokalnie)

**Uwaga:** Frontend jest już uruchomiony w Dockerze! Jeśli chcesz uruchomić go lokalnie do developmentu:

Otwórz **nowy terminal** i przejdź do katalogu frontendu:

```bash
cd CRM.Demo.Web
```

Zainstaluj zależności (tylko przy pierwszym uruchomieniu):

```bash
npm install
```

Uruchom serwer deweloperski:

```bash
npm run dev
```

**Oczekiwany wynik:**
```
  VITE v7.x.x  ready in xxx ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
```

**Frontend będzie dostępny pod adresem:**
- Docker: `http://localhost:5173` (już uruchomiony w Dockerze)
- Development: `http://localhost:5173` (jeśli uruchomisz lokalnie)

---

## ✅ Weryfikacja Uruchomienia

### 1. Sprawdź Docker

```bash
docker ps
```

Powinieneś zobaczyć 5 działających kontenerów:
- `crm-demo-postgres`
- `crm-demo-zookeeper`
- `crm-demo-kafka`
- `crm-demo-api`
- `crm-demo-frontend`

### 2. Sprawdź API

Otwórz w przeglądarce:
- Swagger UI: `http://localhost:5000/swagger` (lub `https://localhost:5001/swagger`)
- Health check (jeśli zaimplementowany): `http://localhost:5000/health`

### 3. Sprawdź Frontend

Otwórz w przeglądarce:
- `http://localhost:5173`

Powinieneś zobaczyć interfejs aplikacji CRM z zakładkami:
- Customers
- Contacts
- Tasks
- Notes

---

## 🛑 Zatrzymywanie Aplikacji

### Zatrzymaj Frontend
W terminalu z frontendem naciśnij: `Ctrl+C`

### Zatrzymaj API (jeśli uruchomione lokalnie)
W terminalu z API naciśnij: `Ctrl+C`

**Uwaga:** Jeśli API działa w Dockerze, nie musisz go zatrzymywać osobno - `docker-compose down` zatrzyma wszystko.

### Zatrzymaj Docker
```bash
docker-compose down
```

**Aby usunąć również dane (volumes):**
```bash
docker-compose down -v
```

---

## 🔍 Rozwiązywanie Problemów

### Problem: Docker nie działa

**Objawy:**
- `docker ps` zwraca błąd
- `docker-compose up -d` kończy się błędem

**Rozwiązanie:**
1. Uruchom Docker Desktop
2. Poczekaj aż Docker się w pełni uruchomi
3. Spróbuj ponownie

---

### Problem: Port już zajęty

**Objawy:**
- `Address already in use`
- `Port 5432 is already allocated`

**Rozwiązanie:**

**Dla PostgreSQL (5432):**
```bash
# Windows (PowerShell)
netstat -ano | findstr :5432
taskkill /PID <PID> /F

# Linux/macOS
lsof -ti:5432 | xargs kill -9
```

**Dla API (5000/5001):**
Zmień porty w `appsettings.json` lub `launchSettings.json`

**Dla Frontendu (5173):**
```bash
# Uruchom z innym portem
npm run dev -- --port 5174
```

---

### Problem: Migracje nie działają

**Objawy:**
- `dotnet ef` nie jest rozpoznawane jako komenda
- Błędy podczas `database update`

**Rozwiązanie:**

1. **Zainstaluj EF Core Tools:**
```bash
dotnet tool install --global dotnet-ef
```

2. **Zweryfikuj connection string** w `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CRM_DEMO;Username=postgres;Password=postgres"
  }
}
```

3. **Sprawdź czy PostgreSQL działa:**
```bash
docker ps | grep postgres
```

---

### Problem: Frontend nie łączy się z API

**Objawy:**
- Błędy CORS w konsoli przeglądarki
- `Network Error` w aplikacji

**Rozwiązanie:**

1. **Sprawdź czy API działa w Dockerze:**
   ```bash
   docker ps | grep api
   docker logs crm-demo-api
   ```

2. **Sprawdź czy API odpowiada:**
   - Otwórz `http://localhost:5000/swagger` w przeglądarce

3. **Sprawdź logi frontendu:**
   ```bash
   docker logs crm-demo-frontend
   ```

4. **Sprawdź konfigurację CORS** w `Program.cs` (powinna pozwalać na `http://localhost:5173`)

5. **Sprawdź czy nginx proxy działa:**
   - Sprawdź logi: `docker logs crm-demo-frontend`
   - Sprawdź konfigurację nginx w `CRM.Demo.Web/nginx.conf`

---

### Problem: Kafka nie działa

**Objawy:**
- Błędy w logach API dotyczące Kafka
- `Connection refused` dla Kafka

**Rozwiązanie:**

1. **Sprawdź czy Kafka działa:**
```bash
docker ps | grep kafka
docker logs crm-demo-kafka
```

2. **Sprawdź konfigurację** w `appsettings.json`:
```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9093"
  }
}
```

3. **Zrestartuj Kafka:**
```bash
docker-compose restart kafka
```

---

## 📝 Uwagi

### Certyfikaty SSL (Development)

Podczas pierwszego uruchomienia API na HTTPS, przeglądarka może pokazać ostrzeżenie o niebezpiecznym certyfikacie. To normalne dla certyfikatów deweloperskich - zaakceptuj je.

### Dane Testowe

Po uruchomieniu aplikacji, baza danych jest pusta. Możesz:
- Dodać dane przez Swagger UI (`https://localhost:5001/swagger`)
- Dodać dane przez interfejs frontendu (`http://localhost:5173`)

### Porty

Domyślne porty:
- **PostgreSQL**: 5432
- **Kafka (zewnętrzny)**: 9093
- **API (HTTPS)**: 5001
- **API (HTTP)**: 5000
- **Frontend**: 5173

Jeśli któreś z nich są zajęte, zmień je w odpowiednich plikach konfiguracyjnych.

---

## 🎓 Następne Kroki

Po pomyślnym uruchomieniu aplikacji:

1. **Przejrzyj dokumentację:**
   - [Architektura](./ARCHITECTURE.md)
   - [API Documentation](./API.md)
   - [Testing](./TESTING.md)

2. **Przetestuj funkcjonalności:**
   - Utwórz klienta (Customer)
   - Dodaj kontakt (Contact)
   - Utwórz zadanie (Task)
   - Dodaj notatkę (Note)

3. **Uruchom testy:**
   ```bash
   dotnet test
   ```

---

## 👤 Autor

**Arkadiusz Słota**

- 🔗 **LinkedIn**: [www.linkedin.com/in/arkadiusz-słota-229551172](https://www.linkedin.com/in/arkadiusz-słota-229551172)
- 💻 **GitHub**: [https://github.com/Maggio333/CRM.Demo](https://github.com/Maggio333/CRM.Demo)

---

**Ostatnia aktualizacja:** 2026-01-29
