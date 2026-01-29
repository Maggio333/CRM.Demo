# CRM Demo Web - Frontend (React/TypeScript)

Frontend aplikacji CRM Demo zbudowany z React, TypeScript i Vite.

## 🚀 Szybki Start

### Wymagania

- Node.js 18+ i npm
- Backend API uruchomiony na `http://localhost:5292`

### Instalacja i uruchomienie

```bash
# Zainstaluj zależności
npm install

# Uruchom w trybie development
npm run dev
```

Aplikacja będzie dostępna na `http://localhost:5173` (domyślny port Vite).

### Build

```bash
# Zbuduj aplikację do produkcji
npm run build

# Podgląd production build
npm run preview
```

## 📁 Struktura Projektu

```
src/
├── components/          # Komponenty wielokrotnego użytku
├── pages/              # Strony (routy)
│   ├── Customers/
│   ├── Contacts/
│   ├── Tasks/
│   └── Notes/
├── services/           # API clients
│   ├── api.ts         # Axios instance
│   ├── customersApi.ts
│   ├── contactsApi.ts
│   ├── tasksApi.ts
│   └── notesApi.ts
├── types/              # TypeScript types
│   └── models.ts      # DTOs z API
├── App.tsx            # Główny komponent + routing
└── main.tsx           # Entry point
```

## 🔌 Konfiguracja API

Base URL API można skonfigurować w pliku `.env`:

```env
VITE_API_BASE_URL=http://localhost:5292/api
```

Domyślnie używa `http://localhost:5292/api` (jeśli `.env` nie istnieje).

## 🛠️ Technologie

- **React 19** - Biblioteka UI
- **TypeScript** - Typowanie statyczne
- **Vite** - Build tool
- **React Router** - Routing
- **Axios** - HTTP client

## 📝 Status Implementacji

- ✅ Struktura projektu
- ✅ Routing
- ✅ API clients (Customers, Contacts, Tasks, Notes)
- ✅ Customers - Lista (podstawowa)
- ⏳ Customers - Formularz tworzenia/edycji
- ⏳ Contacts - Lista i formularze
- ⏳ Tasks - Lista i formularze
- ⏳ Notes - Lista i formularze

## 🎨 Styling

Obecnie używamy zwykłego CSS. W przyszłości można dodać Tailwind CSS dla szybszego stylowania.

## 📚 Dokumentacja

Więcej informacji w `docs/FRONTEND_PLAN.md`.
