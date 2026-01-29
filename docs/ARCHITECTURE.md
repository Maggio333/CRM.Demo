# Przegląd Architektury

## 🏗️ Architektura Systemu

CRM Demo stosuje zasady **Clean Architecture** z wyraźną separacją odpowiedzialności i inwersją zależności. System jest zorganizowany jako **Modular Monolith** z czterema niezależnymi modułami biznesowymi.

---

## 📐 Warstwy Architektoniczne

### 1. Warstwa Domenowa (`CRM.Demo.Domain`)

**Cel:** Zawiera logikę biznesową i modele domenowe. Ta warstwa nie ma zależności od innych warstw.

**Komponenty:**
- **Entities**: Obiekty biznesowe z tożsamością (Customer, Contact, Task, Note)
- **Value Objects**: Niezmienne obiekty bez tożsamości (Email, PhoneNumber, Address)
- **Domain Events**: Zdarzenia reprezentujące zdarzenia biznesowe
- **Domain Exceptions**: Niestandardowe wyjątki dla naruszeń reguł biznesowych

**Kluczowe Zasady:**
- Entities enkapsulują logikę biznesową
- Value Objects zapewniają integralność danych
- Domain Events umożliwiają komunikację opartą na zdarzeniach
- Brak zależności od infrastruktury

**Przykład:**
```csharp
public class Customer : Entity<Guid>
{
    public Email Email { get; private set; }
    
    public void UpdateContactInfo(Email email, PhoneNumber? phoneNumber)
    {
        // Walidacja logiki biznesowej
        if (Status == CustomerStatus.Archived)
            throw new DomainException("Cannot update archived customer");
        
        Email = email;
        // ... logika aktualizacji
    }
}
```

---

### 2. Warstwa Aplikacyjna (`CRM.Demo.Application`)

**Cel:** Orkiestruje przypadki użycia i koordynuje między domeną a infrastrukturą.

**Komponenty:**
- **Commands**: Operacje zapisu (Create, Update, Delete)
- **Queries**: Operacje odczytu (Get, List, Search)
- **Handlers**: Orkiestracja logiki biznesowej
- **DTOs**: Obiekty transferu danych dla komunikacji API
- **Validators**: Walidacja wejścia przy użyciu FluentValidation
- **Mappings**: Profile AutoMapper do transformacji obiektów

**Wzorce:**
- **CQRS**: Commands i Queries rozdzielone
- **MediatR**: Komunikacja in-process dla handlerów
- **Result Pattern**: Explicit error handling

**Przykład:**
```csharp
public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<Guid, string>>
{
    public async Task<Result<Guid, string>> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        // Orkiestracja logiki domenowej
        var email = Email.Create(request.Email);
        var customer = Customer.Create(request.CompanyName, request.TaxId, email);
        
        await _repository.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return Result<Guid, string>.Success(customer.Id);
    }
}
```

---

### 3. Warstwa Infrastruktury (`CRM.Demo.Infrastructure`)

**Cel:** Dostarcza implementacje techniczne dla zewnętrznych zależności.

**Komponenty:**
- **Persistence**: Entity Framework Core, Repositories, UnitOfWork
- **Messaging**: Implementacja Kafka MessageBus
- **Database Migrations**: Migracje EF Core Code First

**Odpowiedzialności:**
- Dostęp do bazy danych
- Integracja z zewnętrznymi serwisami
- Implementacje techniczne

**Przykład:**
```csharp
public class MessageBus : IMessageBus
{
    private readonly IProducer<Null, string> _producer;
    
    public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken ct)
    {
        var topic = GetTopicForEvent(domainEvent);
        var eventJson = JsonSerializer.Serialize(domainEvent);
        await _producer.ProduceAsync(topic, new Message<Null, string> { Value = eventJson }, ct);
    }
}
```

---

### 4. Warstwa Prezentacji (`CRM.Demo.Api`)

**Cel:** Udostępnia aplikację przez HTTP API.

**Komponenty:**
- **Controllers**: Endpointy REST API
- **Middleware**: Obsługa wyjątków, CORS, logowanie żądań
- **Configuration**: Dependency injection, rejestracja serwisów

**Przykład:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
    {
        var command = new CreateCustomerCommand { /* ... */ };
        var result = await _mediator.Send(command);
        
        return result.IsSuccess 
            ? CreatedAtAction(nameof(Get), new { id = result.Value }, result.Value)
            : BadRequest(new { error = result.Error });
    }
}
```

---

## 🔄 Przepływ Danych

### Operacja Zapis (Command)
```
Client → Controller → MediatR → Command Handler → Repository → UnitOfWork → Database
                                                      ↓
                                              Domain Events → MessageBus → Kafka
```

### Operacja Odczyt (Query)
```
Client → Controller → MediatR → Query Handler → Repository → Database → DTO → Client
```

---

## 🎯 Wzorce Projektowe

### 1. CQRS (Command Query Responsibility Segregation)
- **Commands**: Modyfikują stan, zwracają `Result<TValue, TError>`
- **Queries**: Odczytują dane, zwracają DTOs bezpośrednio

### 2. Repository Pattern
- Abstrakcja w warstwie Application
- Implementacja w warstwie Infrastructure
- Umożliwia testowalność i elastyczność

### 3. Unit of Work Pattern
- Zarządza transakcjami bazy danych
- Zbiera i publikuje Domain Events
- Zapewnia spójność

### 4. Result Pattern
- Explicit error handling
- Brak wyjątków dla logiki biznesowej
- Railway-oriented programming

### 5. Domain Events
- Rozdzielają moduły
- Umożliwiają architekturę opartą na zdarzeniach
- Publikowane przez Kafka

---

## 📦 Struktura Modułu

Każdy moduł biznesowy (Customer, Contact, Task, Note) następuje tej samej strukturze:

```
Module/
├── Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   └── DomainEvents/
├── Application/
│   ├── Commands/
│   ├── Queries/
│   ├── DTOs/
│   └── Mappings/
└── Infrastructure/
    └── Persistence/ (współdzielone)
```

---

## 🔌 Event-Driven Architecture

### Przepływ Domain Events

1. **Entity** tworzy Domain Event
2. **UnitOfWork** zbiera zdarzenia z ChangeTracker
3. **UnitOfWork** publikuje zdarzenia przez MessageBus po udanym zapisie
4. **MessageBus** serializuje i wysyła do Kafka
5. **Kafka** przechowuje zdarzenia w tematach (customers-events, contacts-events, etc.)

### Korzyści
- **Loose Coupling**: Moduły komunikują się przez zdarzenia
- **Skalowalność**: Łatwe dodawanie nowych konsumentów zdarzeń
- **Niezawodność**: Zdarzenia są trwale przechowywane w Kafka
- **Audit Trail**: Kompletna historia zdarzeń

---

## 🧪 Strategia Testowania

### Testy Jednostkowe
- **Warstwa Domenowa**: Test logiki biznesowej, walidacji, Domain Events
- **Warstwa Aplikacyjna**: Test validatorów, handlerów (z mockami)

### Pokrycie Testami
- Value Objects: 100% pokrycia
- Entities: Pokrycie głównej logiki biznesowej
- Validators: Wszystkie reguły walidacji przetestowane

---

## 🔐 Uwagi Bezpieczeństwa

**Uwaga:** To jest projekt demonstracyjny. Dla produkcji:
- Zaimplementuj autentykację (JWT, OAuth2)
- Dodaj polityki autoryzacji
- Użyj bezpiecznego przechowywania haseł
- Zaimplementuj rate limiting
- Dodaj sanitizację wejścia
- Używaj tylko HTTPS
- Zabezpiecz Kafka z SASL/SSL

---

## 📈 Skalowalność

Architektura wspiera:
- **Horizontal Scaling**: Bezstanowa warstwa API
- **Database Scaling**: Read replicas, sharding
- **Event Processing**: Wiele konsumentów Kafka
- **Module Extraction**: Moduły mogą stać się mikrousługami

---

## 🚀 Przyszłe Ulepszenia

- [ ] Autentykacja i Autoryzacja
- [ ] Konsumenci Kafka do przetwarzania zdarzeń
- [ ] Testy integracyjne
- [ ] Monitorowanie wydajności
- [ ] Warstwa cache (Redis)
- [ ] Wersjonowanie API

---

## 👤 Autor

**Arkadiusz Słota**

- 🔗 **LinkedIn**: [www.linkedin.com/in/arkadiusz-słota-229551172](https://www.linkedin.com/in/arkadiusz-słota-229551172)
- 💻 **GitHub**: [https://github.com/Maggio333/CRM.Demo](https://github.com/Maggio333/CRM.Demo)

---

**Ostatnia aktualizacja:** 2026-01-29
