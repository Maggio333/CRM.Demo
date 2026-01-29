# Dokumentacja Testów

## 📊 Przegląd

Projekt zawiera kompleksowe testy jednostkowe dla warstw **Domain** i **Application**, zapewniając jakość kodu i utrzymywalność.

### Statystyki Testów

- **Warstwa Domenowa**: 54 testy ✅
- **Warstwa Aplikacyjna**: 26 testów ✅
- **Łącznie**: 80 testów ✅ (100% przechodzi)

---

## 🏗️ Struktura Projektów Testowych

```
CRM.Demo/
├── CRM.Demo.Domain.Tests/          # Testy dla Domain layer
│   ├── ValueObjects/
│   │   ├── EmailTests.cs
│   │   └── PhoneNumberTests.cs
│   └── Entities/
│       └── CustomerTests.cs
│
└── CRM.Demo.Application.Tests/     # Testy dla Application layer
    └── Validators/
        └── CreateCustomerCommandValidatorTests.cs
```

---

## 🧪 Testy Warstwy Domenowej

### Value Objects

#### `EmailTests.cs`
Testuje walidację i zachowanie Value Object `Email`:
- ✅ Tworzenie z poprawnym emailem
- ✅ Normalizacja do lowercase
- ✅ Walidacja pustych/whitespace wartości
- ✅ Walidacja niepoprawnego formatu
- ✅ Równość Value Objects
- ✅ Implicit conversion do string
- ✅ Metoda `ToString()`

#### `PhoneNumberTests.cs`
Testuje walidację i zachowanie Value Object `PhoneNumber`:
- ✅ Tworzenie z poprawnym numerem (9 cyfr)
- ✅ Walidacja pustego country code
- ✅ Walidacja pustego numeru
- ✅ Walidacja niepoprawnego formatu (8, 10 cyfr, litery, etc.)
- ✅ Formatowanie `FullNumber` (+48...)
- ✅ Równość Value Objects
- ✅ Metoda `ToString()`

### Entities

#### `CustomerTests.cs`
Testuje logikę biznesową encji `Customer`:
- ✅ Tworzenie Customer z poprawnymi danymi
- ✅ Publikowanie Domain Event (`CustomerCreatedEvent`)
- ✅ Walidacja pustego company name
- ✅ Walidacja pustego tax ID
- ✅ Tworzenie z PhoneNumber i Address
- ✅ Aktualizacja informacji kontaktowych
- ✅ Publikowanie Domain Event przy zmianie emaila
- ✅ Brak eventu gdy email się nie zmienił
- ✅ Blokada aktualizacji archived customer
- ✅ Zmiana statusu
- ✅ Publikowanie Domain Event przy zmianie statusu
- ✅ Brak eventu gdy status się nie zmienił
- ✅ Przypisanie sales rep
- ✅ Czyszczenie Domain Events

---

## 🧪 Testy Warstwy Aplikacyjnej

### Validators

#### `CreateCustomerCommandValidatorTests.cs`
Testuje walidację FluentValidation dla `CreateCustomerCommand`:
- ✅ Walidacja poprawnego command
- ✅ Walidacja pustego company name
- ✅ Walidacja company name > 200 znaków
- ✅ Walidacja pustego tax ID
- ✅ Walidacja tax ID != 10 znaków
- ✅ Walidacja pustego emaila
- ✅ Walidacja niepoprawnego formatu emaila
- ✅ Walidacja emaila > 255 znaków
- ✅ Walidacja poprawnego numeru telefonu (9 cyfr, +48, etc.)
- ✅ Walidacja niepoprawnego numeru telefonu
- ✅ Opcjonalność numeru telefonu

---

## 🚀 Uruchamianie Testów

### Wszystkie Testy
```bash
dotnet test
```

### Tylko Testy Domenowe
```bash
dotnet test CRM.Demo.Domain.Tests/CRM.Demo.Domain.Tests.csproj
```

### Tylko Testy Aplikacyjne
```bash
dotnet test CRM.Demo.Application.Tests/CRM.Demo.Application.Tests.csproj
```

### Z Pokryciem Kodu
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Verbose Output
```bash
dotnet test --verbosity normal
```

---

## 📦 Biblioteki Testowe

### Testy Domenowe
- **xUnit** - Framework testowy
- **FluentAssertions** - Fluent assertions dla czytelnych testów

### Testy Aplikacyjne
- **xUnit** - Framework testowy
- **FluentAssertions** - Fluent assertions
- **Moq** - Framework do mockowania (dla przyszłych testów Handlers)

---

## 🎯 Pokrycie Testami

### Warstwa Domenowa
- ✅ **Value Objects**: Email, PhoneNumber (kompleksowe pokrycie)
- ✅ **Entities**: Customer (pełne pokrycie logiki biznesowej)
- ✅ **Domain Events**: Testowane pośrednio przez Entities

### Warstwa Aplikacyjna
- ✅ **Validators**: CreateCustomerCommandValidator (pełne pokrycie)
- ⚠️ **Handlers**: Do dodania (wymaga mockowania Repository, UnitOfWork, etc.)
- ⚠️ **Queries**: Do dodania

---

## 📝 Najlepsze Praktyki

### 1. **Wzorzec Arrange-Act-Assert**
Wszystkie testy następują wzorcu AAA:
```csharp
[Fact]
public void TestName()
{
    // Arrange - przygotowanie danych
    var email = "test@example.com";
    
    // Act - wykonanie akcji
    var result = Email.Create(email);
    
    // Assert - weryfikacja wyniku
    result.Value.Should().Be("test@example.com");
}
```

### 2. **Theory vs Fact**
- `[Fact]` - Pojedynczy przypadek testowy
- `[Theory]` - Test parametryzowany (`[InlineData]`)

### 3. **FluentAssertions**
Używamy FluentAssertions dla czytelnych asercji:
```csharp
result.Should().NotBeNull();
result.Value.Should().Be("expected");
act.Should().Throw<DomainException>();
```

### 4. **Nazewnictwo Testów**
Nazwy testów jasno opisują co testują:
- `Create_ValidEmail_ShouldCreateEmail`
- `Create_EmptyEmail_ShouldThrowDomainException`
- `UpdateContactInfo_ArchivedCustomer_ShouldThrowDomainException`

---

## 🔮 Przyszłe Ulepszenia

### Do Dodania:
1. **Testy Handlers** - Testy dla Command/Query Handlers używając Moq
2. **Testy Integracyjne** - Testy integracyjne z bazą danych InMemory
3. **Testy React** - Testy komponentów używając Vitest/Jest
4. **Pokrycie Kodu** - Konfiguracja i raportowanie pokrycia kodu
5. **Integracja CI/CD** - Automatyczne uruchamianie testów w pipeline

---

## 📚 Przydatne Linki

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Moq Documentation](https://github.com/moq/moq4)

---

## 👤 Autor

**Arkadiusz Słota**

- 🔗 **LinkedIn**: [www.linkedin.com/in/arkadiusz-słota-229551172](https://www.linkedin.com/in/arkadiusz-słota-229551172)
- 💻 **GitHub**: [https://github.com/Maggio333/CRM.Demo](https://github.com/Maggio333/CRM.Demo)

---

**Ostatnia aktualizacja:** 2026-01-29
