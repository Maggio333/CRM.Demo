# CRM Demo - Enterprise-Grade Modular Monolith

> **Projekt demonstracyjny** prezentujący nowoczesne wzorce architektury oprogramowania i najlepsze praktyki w rozwoju aplikacji .NET i React.

[![CI](https://github.com/Maggio333/CRM.Demo/actions/workflows/ci.yml/badge.svg)](https://github.com/Maggio333/CRM.Demo/actions/workflows/ci.yml)
[![Tests](https://github.com/Maggio333/CRM.Demo/actions/workflows/test.yml/badge.svg)](https://github.com/Maggio333/CRM.Demo/actions/workflows/test.yml)

## 🎯 Przegląd Projektu

CRM Demo to pełnoprawna aplikacja Customer Relationship Management zbudowana z wykorzystaniem **Domain-Driven Design (DDD)**, **CQRS** i **Event-Driven Architecture**. Projekt demonstruje wzorce architektury na poziomie enterprise, zasady czystego kodu i nowoczesne praktyki programistyczne.

### Kluczowe Cechy

- ✅ **Modular Monolith** - architektura z 4 niezależnymi modułami
- ✅ **Domain-Driven Design** - Entities, Value Objects i Domain Events
- ✅ **CQRS** - implementacja wzorca z użyciem MediatR
- ✅ **Event-Driven Architecture** - Apache Kafka
- ✅ **Clean Architecture** - wyraźna separacja warstw
- ✅ **80+ testów jednostkowych** - kompleksowe pokrycie
- ✅ **OpenAPI/Swagger** - dokumentacja API
- ✅ **Full-stack** - implementacja (.NET 8.0 + React 18)

---

## 🚀 Szybki Start

### Wymagania

- **.NET 8.0 SDK**
- **Docker Desktop** (dla PostgreSQL i Kafka)
- **Node.js 18+** (dla frontendu React)

### Instrukcje Instalacji

**📖 Szczegółowe instrukcje uruchomienia:** Zobacz [Przewodnik Szybkiego Startu](./docs/QUICK_START.md)

**Krótkie podsumowanie:**

1. **Uruchom infrastrukturę (Docker):**
   ```bash
   docker-compose up -d
   ```

2. **Zastosuj migracje bazy danych:**
   ```bash
   cd CRM.Demo.Api
   dotnet ef database update --project ..\CRM.Demo.Infrastructure --startup-project . --context ApplicationDbContext
   ```

3. **Uruchom API:**
   ```bash
   cd CRM.Demo.Api
   dotnet run
   ```

4. **Uruchom Frontend** (w nowym terminalu):
   ```bash
   cd CRM.Demo.Web
   npm install
   npm run dev
   ```

**Dostępne adresy:**
- API: `https://localhost:5001` (uruchom ręcznie: `cd CRM.Demo.Api && dotnet run`)
- Swagger: `https://localhost:5001/swagger`
- Frontend: `http://localhost:5173` (uruchomiony w Dockerze)

1. **Sklonuj repozytorium**
   ```bash
   git clone https://github.com/Maggio333/CRM.Demo.git
   cd CRM.Demo
   ```

2. **Uruchom wszystkie usługi w Dockerze**
   ```bash
   docker-compose up -d
   ```
   To uruchamia:
   - Bazę danych PostgreSQL (port 5432)
   - Apache Kafka z Zookeeper (porty 9092, 2181)
   - **API .NET** (porty 5000, 5001)
   - **Frontend React** (port 5173, serwowany przez Nginx)

3. **Migracje** (automatyczne!)
   
   🎉 **Migracje są automatycznie stosowane przy starcie API!**
   
   API automatycznie czeka na bazę danych i stosuje wszystkie migracje. Sprawdź logi:
   ```bash
   docker logs crm-demo-api
   ```

4. **Sprawdź czy wszystko działa**
   - API: `http://localhost:5000` lub `https://localhost:5001`
   - Swagger: `http://localhost:5000/swagger`
   - Frontend: `http://localhost:5173`

**Wszystko działa w Dockerze - nie musisz uruchamiać niczego lokalnie!**

---

## 📁 Struktura Projektu

```
CRM.Demo/
├── CRM.Demo.Domain/              # Warstwa Domenowa
│   ├── Customers/               # Agregat Customer
│   ├── Contacts/                # Agregat Contact
│   ├── Tasks/                   # Agregat Task
│   ├── Notes/                    # Agregat Note
│   └── Common/                   # Wspólne abstrakcje domenowe
│
├── CRM.Demo.Application/         # Warstwa Aplikacyjna
│   ├── Customers/               # Przypadki użycia Customer (Commands/Queries)
│   ├── Contacts/                # Przypadki użycia Contact
│   ├── Tasks/                   # Przypadki użycia Task
│   ├── Notes/                   # Przypadki użycia Note
│   └── Common/                  # Wspólna logika aplikacyjna
│
├── CRM.Demo.Infrastructure/     # Warstwa Infrastruktury
│   ├── Persistence/             # EF Core, Repositories, UnitOfWork
│   └── Messaging/               # Implementacja Kafka MessageBus
│
├── CRM.Demo.Api/                # Warstwa Prezentacji
│   ├── Controllers/             # Endpointy REST API
│   └── Middleware/              # Obsługa wyjątków, CORS
│
├── CRM.Demo.Web/                # Frontend (React + TypeScript)
│   ├── src/
│   │   ├── pages/              # Komponenty stron
│   │   ├── services/           # Klienci API
│   │   └── types/              # Definicje TypeScript
│
├── CRM.Demo.Domain.Tests/       # Testy jednostkowe warstwy domenowej
├── CRM.Demo.Application.Tests/  # Testy jednostkowe warstwy aplikacyjnej
│
└── docs/                        # Dokumentacja projektu
```

---

## 🏗️ Architektura

### Wzorce Architektoniczne

- **Modular Monolith**: Niezależne moduły (Customers, Contacts, Tasks, Notes) mogą ewoluować niezależnie
- **Clean Architecture**: Wyraźna separacja odpowiedzialności z inwersją zależności
- **Domain-Driven Design**: Bogate modele domenowe z logiką biznesową enkapsulowaną w encjach
- **CQRS**: Separacja operacji odczytu (Queries) i zapisu (Commands)
- **Event-Driven Architecture**: Domain Events publikowane przez Kafka dla luźnego sprzężenia

### Stack Technologiczny

**Backend:**
- .NET 8.0
- ASP.NET Core Web API
- Entity Framework Core 8.0 (PostgreSQL)
- MediatR (CQRS)
- FluentValidation
- AutoMapper
- Confluent.Kafka

**Frontend:**
- React 18
- TypeScript
- Vite
- React Router
- Axios

**Infrastruktura:**
- PostgreSQL 16
- Apache Kafka
- Docker & Docker Compose

**Testowanie:**
- xUnit
- FluentAssertions
- Moq

---

## ✨ Funkcjonalności

### Funkcjonalności Backend
- ✅ Pełne operacje CRUD dla wszystkich modułów
- ✅ Paginacja i filtrowanie
- ✅ Walidacja wejścia (FluentValidation)
- ✅ Zarządzanie transakcjami
- ✅ Domain Events z integracją Kafka
- ✅ Obsługa błędów (Result Pattern)
- ✅ Dokumentacja OpenAPI/Swagger

### Funkcjonalności Frontend
- ✅ Responsywny interfejs z nowoczesnym designem
- ✅ Pełne operacje CRUD
- ✅ Wyszukiwanie i filtrowanie
- ✅ Paginacja
- ✅ Obsługa błędów i feedback użytkownika
- ✅ Type-safe integracja z API

### Zapewnienie Jakości
- ✅ 80+ testów jednostkowych
- ✅ Pokrycie logiki domenowej
- ✅ Pokrycie reguł walidacji
- ✅ Zasady czystego kodu
- ✅ Zasady SOLID

---

## 📚 Dokumentacja

Kompleksowa dokumentacja dostępna w katalogu `docs/`:

- **[Szybki Start](./docs/QUICK_START.md)** - Szczegółowa instrukcja uruchomienia aplikacji
- **[Architektura](./docs/ARCHITECTURE.md)** - Szczegółowy przegląd architektury
- **[Dokumentacja API](./docs/API.md)** - Endpointy API i użycie
- **[Testowanie](./docs/TESTING.md)** - Strategia testowania i wytyczne
- **[Wdrożenie](./docs/DEPLOYMENT.md)** - Przewodnik wdrożenia i konfiguracji
- **[CI/CD](./docs/CI_CD.md)** - Konfiguracja Continuous Integration i Deployment

---

## 🧪 Uruchamianie Testów

```bash
# Uruchom wszystkie testy
dotnet test

# Uruchom konkretny projekt testowy
dotnet test CRM.Demo.Domain.Tests
dotnet test CRM.Demo.Application.Tests

# Z pokryciem kodu
dotnet test /p:CollectCoverage=true
```

**Statystyki Testów:**
- Warstwa Domenowa: 54 testy
- Warstwa Aplikacyjna: 26 testów
- **Łącznie: 80 testów** (wszystkie przechodzą ✅)

---

## 🔧 Konfiguracja

### Połączenie z Bazą Danych

Domyślny connection string (development):
```
Host=localhost;Port=5432;Database=CRM_DEMO;Username=postgres;Password=postgres
```

### Konfiguracja Kafka

Broker Kafka skonfigurowany do działania na `localhost:9093` (port zewnętrzny).

Konfiguracja w `appsettings.json`:
```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9093"
  }
}
```

---

## 🚦 Wytyczne Programistyczne

### Styl Kodu
- Przestrzegaj konwencji C#
- Używaj znaczących nazw
- Utrzymuj metody małe i skupione
- Stosuj zasady SOLID

### Testowanie
- Pisz testy jednostkowe dla logiki domenowej
- Testuj przypadki brzegowe i reguły walidacji
- Utrzymuj wysokie pokrycie testami

### Commity
- Używaj jasnych, opisowych komunikatów commitów
- Stosuj format conventional commits gdy możliwe

### CI/CD
- Wszystkie commity uruchamiają automatyczne buildy i testy
- Pull requests wymagają przechodzących testów
- Formatowanie kodu jest automatycznie sprawdzane
- Zobacz [Dokumentację CI/CD](./docs/CI_CD.md) po szczegóły

---

## 📝 Licencja

MIT License - Projekt demonstracyjny do celów edukacyjnych i portfolio.

---

## 👤 Autor

**Arkadiusz Słota**

- 🔗 **LinkedIn**: [www.linkedin.com/in/arkadiusz-słota-229551172](https://www.linkedin.com/in/arkadiusz-słota-229551172)
- 💻 **GitHub**: [https://github.com/Maggio333/CRM.Demo](https://github.com/Maggio333/CRM.Demo)

Projekt stworzony jako demonstracja techniczna prezentująca praktyki programistyczne na poziomie enterprise.

---

**Uwaga:** Projekt używa prostych poświadczeń do celów deweloperskich. **Nigdy nie używaj tych poświadczeń w środowiskach produkcyjnych.**
