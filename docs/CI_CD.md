# Konfiguracja CI/CD

Ten projekt używa **GitHub Actions** do Continuous Integration i Continuous Deployment.

---

## 🔄 Workflows

### 1. CI Workflow (`.github/workflows/ci.yml`)

**Wyzwalacze:**
- Push do gałęzi `main` lub `develop`
- Pull requests do gałęzi `main` lub `develop`

**Zadania:**
- **Build and Test**: Buduje rozwiązanie .NET, uruchamia wszystkie testy jednostkowe
- **Frontend Build**: Buduje aplikację React frontend
- **Code Quality**: Uruchamia `dotnet format` do sprawdzania formatowania kodu

**Czas trwania:** ~3-5 minut

---

### 2. Test Workflow (`.github/workflows/test.yml`)

**Wyzwalacze:**
- Push do gałęzi `main` lub `develop`
- Pull requests do gałęzi `main` lub `develop`
- Ręczne uruchomienie (`workflow_dispatch`)

**Zadania:**
- **Unit Tests**: Uruchamia testy dla każdego projektu testowego równolegle
  - Domain Tests
  - Application Tests
- **Test Summary**: Agreguje i publikuje wyniki testów

**Funkcje:**
- ✅ Równoległe wykonywanie testów
- ✅ Zbieranie pokrycia kodu
- ✅ Wyniki testów publikowane jako komentarze w PR
- ✅ Przechowywanie artefaktów (30 dni)

**Czas trwania:** ~2-4 minuty

---

### 3. Release Workflow (`.github/workflows/release.yml`)

**Wyzwalacze:**
- Utworzenie GitHub Release
- Ręczne uruchomienie z inputem wersji

**Zadania:**
- **Build and Publish**: 
  - Buduje rozwiązanie .NET
  - Uruchamia wszystkie testy
  - Publikuje aplikację API
  - Buduje React frontend
  - Tworzy archiwum release
  - Wgrywa do GitHub Releases

**Wynik:**
- Archiwum release: `crm-demo-{version}.tar.gz`
- Zawiera: Binarne API + Build frontendu

**Czas trwania:** ~5-8 minut

---

## 📊 Status Badges

Dodaj te badge'y do swojego README.md:

```markdown
![CI](https://github.com/Maggio333/CRM.Demo/workflows/CI/badge.svg)
![Tests](https://github.com/Maggio333/CRM.Demo/workflows/Tests/badge.svg)
```

---

## 🧪 Testowanie Lokalne

### Używając Act (GitHub Actions lokalnie)

Zainstaluj [act](https://github.com/nektos/act):

```bash
# Windows (Chocolatey)
choco install act-cli

# macOS (Homebrew)
brew install act

# Linux
curl https://raw.githubusercontent.com/nektos/act/master/install.sh | sudo bash
```

Uruchom workflows lokalnie:

```bash
# Uruchom CI workflow
act push

# Uruchom konkretny workflow
act workflow_dispatch -W .github/workflows/release.yml

# Uruchom z konkretnym eventem
act pull_request
```

---

## ⚙️ Konfiguracja

### Zmienne Środowiskowe

Workflows używają tych zmiennych środowiskowych:

- `DOTNET_VERSION`: Wersja .NET SDK (domyślnie: `8.0.x`)
- `NODE_VERSION`: Wersja Node.js (domyślnie: `18.x`)

### Wymagane Sekrety (dla produkcji)

Jeśli potrzebujesz wdrożyć do produkcji, dodaj te sekrety w GitHub:

1. Przejdź do: **Settings → Secrets and variables → Actions**
2. Dodaj sekrety:
   - `DEPLOY_KEY`: Klucz SSH do wdrożenia na serwer
   - `DOCKER_HUB_USERNAME`: Nazwa użytkownika Docker Hub (jeśli używasz Dockera)
   - `DOCKER_HUB_TOKEN`: Token dostępu Docker Hub

---

## 📈 Szczegóły Workflow

### Proces Build

1. **Checkout code** z repozytorium
2. **Setup .NET** SDK (wersja 8.0.x)
3. **Restore** pakiety NuGet
4. **Build** rozwiązanie w konfiguracji Release
5. **Run tests** z pokryciem kodu
6. **Build frontend** (aplikacja React)
7. **Upload artifacts** (wyniki testów, outputy build)

### Wykonywanie Testów

Testy uruchamiają się równolegle dla szybszego wykonania:
- Domain Tests: ~1-2 minuty
- Application Tests: ~1-2 minuty

Wyniki testów są:
- ✅ Wyświetlane w podsumowaniu workflow
- ✅ Wgrywane jako artefakty
- ✅ Publikowane jako komentarze w PR (jeśli workflow PR)

### Pokrycie Kodu

Pokrycie kodu jest zbierane używając:
- `coverlet.collector` (już w projektach testowych)
- Format wyjściowy: Cobertura XML
- Wgrywane jako artefakty

Aby zobaczyć pokrycie lokalnie:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

---

## 🚀 Wdrożenie

### Automatyczne Wdrożenie (Przyszłość)

Aby włączyć automatyczne wdrożenie, dodaj zadanie deployment:

```yaml
deploy:
  name: Deploy to Production
  runs-on: ubuntu-latest
  needs: build-and-publish
  if: github.ref == 'refs/heads/main'
  steps:
    - name: Deploy
      uses: appleboy/ssh-action@master
      with:
        host: ${{ secrets.HOST }}
        username: ${{ secrets.USERNAME }}
        key: ${{ secrets.DEPLOY_KEY }}
        script: |
          # Komendy wdrożenia
```

---

## 🔍 Rozwiązywanie Problemów

### Błędy Build

**Problem:** Build kończy się błędami zależności
**Rozwiązanie:** 
- Sprawdź output `dotnet restore`
- Zweryfikuj że wszystkie referencje projektów są poprawne
- Upewnij się że pakiety NuGet są dostępne

**Problem:** Build frontendu kończy się błędem
**Rozwiązanie:**
- Sprawdź kompatybilność wersji Node.js
- Zweryfikuj że `package-lock.json` jest commitowany
- Uruchom `npm ci` lokalnie aby przetestować

### Błędy Testów

**Problem:** Testy kończą się błędem w CI ale przechodzą lokalnie
**Rozwiązanie:**
- Sprawdź kod specyficzny dla środowiska
- Zweryfikuj zależności danych testowych
- Przejrzyj izolację testów

### Workflow Nie Uruchamia Się

**Problem:** Workflow nie uruchamia się przy push
**Rozwiązanie:**
- Zweryfikuj że plik workflow jest w `.github/workflows/`
- Sprawdź że nazwy gałęzi pasują do wyzwalaczy workflow
- Upewnij się że składnia YAML jest poprawna

---

## 📝 Najlepsze Praktyki

1. **Utrzymuj workflows szybkie**: Używaj równoległych zadań gdzie możliwe
2. **Cache dependencies**: Używaj akcji `cache` dla npm/dotnet
3. **Fail fast**: Uruchamiaj szybkie sprawdzenia najpierw (format, lint)
4. **Artifact retention**: Ustaw odpowiednie dni przechowywania
5. **Bezpieczeństwo**: Nigdy nie commituj sekretów, używaj GitHub Secrets
6. **Dokumentacja**: Utrzymuj pliki workflow dobrze skomentowane

---

## 🔗 Powiązana Dokumentacja

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET GitHub Actions](https://github.com/actions/setup-dotnet)
- [Node.js GitHub Actions](https://github.com/actions/setup-node)
- [Act - Local GitHub Actions](https://github.com/nektos/act)

---

## 👤 Autor

**Arkadiusz Słota**

- 🔗 **LinkedIn**: [www.linkedin.com/in/arkadiusz-słota-229551172](https://www.linkedin.com/in/arkadiusz-słota-229551172)
- 💻 **GitHub**: [https://github.com/Maggio333/CRM.Demo](https://github.com/Maggio333/CRM.Demo)

---

**Ostatnia aktualizacja:** 2026-01-29
