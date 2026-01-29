# Dokumentacja API

## 🌐 Bazowy URL

```
Development: https://localhost:5001/api
Production: <skonfigurowany-url>/api
```

---

## 📋 Endpointy API

### Customers

#### Utwórz Klienta
```http
POST /api/Customers
Content-Type: application/json

{
  "companyName": "Acme Corporation",
  "taxId": "1234567890",
  "email": "contact@acme.com",
  "phoneNumber": "123456789",
  "street": "123 Main St",
  "city": "Warsaw",
  "postalCode": "00-001",
  "country": "Poland"
}
```

**Odpowiedź:**
- `201 Created`: Zwraca ID utworzonego klienta (Guid)
- `400 Bad Request`: Błędy walidacji

---

#### Pobierz Klienta po ID
```http
GET /api/Customers/{id}
```

**Odpowiedź:**
- `200 OK`: Customer DTO
- `404 Not Found`: Klient nie znaleziony

---

#### Pobierz Listę Klientów
```http
GET /api/Customers?pageNumber=1&pageSize=10&searchTerm=acme
```

**Parametry Query:**
- `pageNumber` (int, domyślnie: 1)
- `pageSize` (int, domyślnie: 10)
- `searchTerm` (string, opcjonalnie): Wyszukiwanie w nazwie firmy, emailu

**Odpowiedź:**
- `200 OK`: Tablica Customer DTOs

---

#### Aktualizuj Klienta
```http
PUT /api/Customers/{id}
Content-Type: application/json

{
  "companyName": "Updated Company",
  "email": "new@example.com",
  "phoneNumber": "987654321"
}
```

**Odpowiedź:**
- `204 No Content`: Sukces
- `400 Bad Request`: Błędy walidacji
- `404 Not Found`: Klient nie znaleziony

---

#### Usuń Klienta
```http
DELETE /api/Customers/{id}
```

**Odpowiedź:**
- `204 No Content`: Sukces
- `404 Not Found`: Klient nie znaleziony

**Uwaga:** Powiązane Contacts, Tasks i Notes będą miały swoje klucze obce ustawione na NULL (SET NULL cascade).

---

### Contacts

#### Utwórz Kontakt
```http
POST /api/Contacts
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "type": "Primary",
  "phoneNumber": "123456789",
  "jobTitle": "Manager",
  "department": "Sales",
  "customerId": "guid-here"
}
```

---

#### Pobierz Kontakt po ID
```http
GET /api/Contacts/{id}
```

---

#### Pobierz Listę Kontaktów
```http
GET /api/Contacts?pageNumber=1&pageSize=10&customerId={guid}&searchTerm=john
```

**Parametry Query:**
- `pageNumber`, `pageSize`: Paginacja
- `customerId` (Guid, opcjonalnie): Filtruj po kliencie
- `searchTerm` (string, opcjonalnie): Wyszukiwanie w imieniu, nazwisku, emailu

---

#### Aktualizuj Kontakt
```http
PUT /api/Contacts/{id}
Content-Type: application/json

{
  "firstName": "Jane",
  "email": "jane.doe@example.com"
}
```

---

#### Usuń Kontakt
```http
DELETE /api/Contacts/{id}
```

---

### Tasks

#### Utwórz Zadanie
```http
POST /api/Tasks
Content-Type: application/json

{
  "title": "Follow up with client",
  "description": "Call to discuss proposal",
  "type": "Call",
  "priority": "High",
  "dueDate": "2026-02-15T10:00:00Z",
  "customerId": "guid-here",
  "contactId": "guid-here",
  "createdByUserId": "guid-here"
}
```

---

#### Pobierz Zadanie po ID
```http
GET /api/Tasks/{id}
```

---

#### Pobierz Listę Zadań
```http
GET /api/Tasks?pageNumber=1&pageSize=10&customerId={guid}&status=ToDo
```

**Parametry Query:**
- `pageNumber`, `pageSize`: Paginacja
- `customerId` (Guid, opcjonalnie): Filtruj po kliencie
- `status` (string, opcjonalnie): Filtruj po statusie (ToDo, InProgress, Done, Cancelled)
- `priority` (string, opcjonalnie): Filtruj po priorytecie (Low, Medium, High)

---

#### Aktualizuj Zadanie
```http
PUT /api/Tasks/{id}
Content-Type: application/json

{
  "title": "Updated title",
  "status": "InProgress",
  "priority": "Medium"
}
```

---

#### Usuń Zadanie
```http
DELETE /api/Tasks/{id}
```

---

### Notes

#### Utwórz Notatkę
```http
POST /api/Notes
Content-Type: application/json

{
  "content": "Customer expressed interest in premium package",
  "title": "Sales Call Notes",
  "type": "Meeting",
  "category": "Sales",
  "customerId": "guid-here",
  "contactId": "guid-here",
  "taskId": "guid-here",
  "createdByUserId": "guid-here"
}
```

---

#### Pobierz Notatkę po ID
```http
GET /api/Notes/{id}
```

---

#### Pobierz Listę Notatek
```http
GET /api/Notes?pageNumber=1&pageSize=10&customerId={guid}&type=Meeting
```

**Parametry Query:**
- `pageNumber`, `pageSize`: Paginacja
- `customerId` (Guid, opcjonalnie): Filtruj po kliencie
- `type` (string, opcjonalnie): Filtruj po typie notatki

---

#### Aktualizuj Notatkę
```http
PUT /api/Notes/{id}
Content-Type: application/json

{
  "content": "Updated content",
  "title": "Updated title"
}
```

---

#### Usuń Notatkę
```http
DELETE /api/Notes/{id}
```

---

## 📝 Modele Danych

### CustomerDto
```typescript
{
  id: string;              // Guid
  companyName: string;
  taxId: string;           // 10 znaków
  email: string;
  phoneNumber?: string;    // Opcjonalne
  street?: string;
  city?: string;
  postalCode?: string;
  country?: string;
  status: string;          // Prospect, Active, Inactive, Archived
  createdAt: string;       // ISO 8601
  updatedAt?: string;
}
```

### ContactDto
```typescript
{
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  type: string;            // Primary, Secondary, Billing, etc.
  phoneNumber?: string;
  jobTitle?: string;
  department?: string;
  customerId?: string;
  status: string;
  role?: string;
  createdAt: string;
  updatedAt?: string;
}
```

### TaskDto
```typescript
{
  id: string;
  title: string;
  description?: string;
  type: string;            // Call, Email, Meeting, Task
  priority: string;       // Low, Medium, High
  status: string;         // ToDo, InProgress, Done, Cancelled
  dueDate?: string;
  startDate?: string;
  completedDate?: string;
  customerId?: string;
  contactId?: string;
  assignedToUserId?: string;
  createdAt: string;
  createdByUserId: string;
  updatedAt?: string;
}
```

### NoteDto
```typescript
{
  id: string;
  content: string;
  title?: string;
  type: string;            // Meeting, Call, Email, Note
  category?: string;
  customerId?: string;
  contactId?: string;
  taskId?: string;
  createdAt: string;
  createdByUserId: string;
  updatedAt?: string;
  updatedByUserId?: string;
}
```

---

## ⚠️ Odpowiedzi Błędów

### Błąd Walidacji (400 Bad Request)
```json
{
  "error": "Validation failed",
  "errors": [
    {
      "property": "Email",
      "message": "Email is required"
    },
    {
      "property": "TaxId",
      "message": "Tax ID must be 10 characters"
    }
  ]
}
```

### Błąd Logiki Biznesowej (400 Bad Request)
```json
{
  "error": "Cannot update archived customer"
}
```

### Nie Znaleziono (404)
```json
{
  "error": "Customer with ID {id} not found"
}
```

### Błąd Serwera (500)
```json
{
  "error": "An error occurred while processing your request"
}
```

---

## 🔐 Autentykacja

**Uwaga:** Autentykacja nie jest zaimplementowana w tym projekcie demonstracyjnym. Dla produkcji:
- Użyj tokenów JWT
- Zaimplementuj OAuth2
- Dodaj autoryzację opartą na rolach

---

## 📊 Rate Limiting

**Uwaga:** Rate limiting nie jest zaimplementowany. Dla produkcji, rozważ:
- Limity per-IP
- Limity per-user
- Limity oparte na kluczach API

---

## 📖 Interaktywna Dokumentacja

Pełna interaktywna dokumentacja API dostępna przez **Swagger UI**:
- Development: `https://localhost:5001/swagger`
- Zawiera schematy request/response
- Funkcjonalność try-it-out
- Specyfikacja OpenAPI 3.0

---

## 👤 Autor

**Arkadiusz Słota**

- 🔗 **LinkedIn**: [www.linkedin.com/in/arkadiusz-słota-229551172](https://www.linkedin.com/in/arkadiusz-słota-229551172)
- 💻 **GitHub**: [https://github.com/Maggio333/CRM.Demo](https://github.com/Maggio333/CRM.Demo)

---

**Ostatnia aktualizacja:** 2026-01-29
