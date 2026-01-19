# TimeManager

Aplikacja desktopowa do zarządzania czasem, nawykami i zasobami domowymi. System wspiera użytkownika w planowaniu zadań z wykorzystaniem algorytmu automatycznego harmonogramowania opartego na priorytetach i krzywej energetycznej REFA.

## Spis treści

- [Wymagania systemowe](#wymagania-systemowe)
- [Instalacja](#instalacja)
- [Struktura projektu](#struktura-projektu)
- [Uruchomienie aplikacji](#uruchomienie-aplikacji)
- [Ładowanie przykładowych danych](#ładowanie-przykładowych-danych)
- [Konta testowe](#konta-testowe)

---

## Wymagania systemowe

### Wymagania sprzętowe

| Komponent | Wymaganie minimalne |
|-----------|---------------------|
| Procesor | Intel Core i3 / AMD Ryzen 3 (2 GHz) |
| Pamięć RAM | 4 GB |
| Przestrzeń dyskowa | 500 MB |
| Rozdzielczość ekranu | 1280 × 720 px |

### Wymagania programowe

| Oprogramowanie | Wersja |
|----------------|--------|
| System operacyjny | Windows 10 (64-bit) lub nowszy |
| .NET Runtime | .NET 8.0 Desktop Runtime |
| SQL Server | SQL Server 2019 Express lub nowszy |

---

## Instalacja

### 1. Instalacja .NET 8.0 Desktop Runtime

1. Pobierz instalator ze strony Microsoft:  
   https://dotnet.microsoft.com/download/dotnet/8.0
2. Uruchom instalator i postępuj zgodnie z instrukcjami.
3. Zweryfikuj instalację:
   ```powershell
   dotnet --version
   ```

### 2. Instalacja SQL Server Express

1. Pobierz SQL Server 2022 Express:  
   https://www.microsoft.com/sql-server/sql-server-downloads
2. Uruchom instalator i wybierz opcję **Basic**.
3. Zapamiętaj nazwę instancji (domyślnie: `.\SQLEXPRESS`).
4. (Opcjonalnie) Zainstaluj SQL Server Management Studio (SSMS).

### 3. Utworzenie bazy danych

#### Sposób 1: SQL Server Management Studio

1. Otwórz SSMS i połącz się z instancją SQL Server.
2. Otwórz plik `Database\DatabaseSchema.sql`.
3. Wykonaj skrypt (F5).


### 4. Konfiguracja połączenia (opcjonalnie)

Domyślny ciąg połączenia znajduje się w pliku `Database\DatabaseHelper.cs`:

```csharp
private static readonly string _connectionString = 
    @"Server=.\SQLEXPRESS;Database=TimeManagerDB;
      Trusted_Connection=True;
      TrustServerCertificate=True;";
```

W przypadku innej konfiguracji serwera SQL należy zmodyfikować powyższy ciąg.

---

## Struktura projektu

```
TimeManager5/
├── bin/                          # Pliki wykonywalne (po kompilacji)
├── Controls/                     # Kontrolki użytkownika
│   └── SleepCircleControl.cs
├── Database/                     # Skrypty bazy danych
│   ├── DatabaseHelper.cs         # Klasa pomocnicza połączenia z bazą
│   ├── DatabaseSchema.sql        # Schemat bazy danych
│   └── SampleData.sql            # Przykładowe dane testowe
├── Forms/                        # Formularze Windows Forms
│   ├── AccountDialog/            # Dialog konta użytkownika
│   ├── AddContainerForm/         # Dodawanie kontenerów
│   ├── AddEventForm/             # Dodawanie wydarzeń
│   ├── AddMedicineScheduleForm/  # Harmonogram leków
│   ├── AddRecipeForm/            # Dodawanie przepisów
│   ├── AddRewardForm/            # Dodawanie nagród
│   ├── AddTrackingItemForm/      # Dodawanie elementów śledzenia
│   ├── AdminPanel/               # Panel administratora
│   ├── CalendarView/             # Widok kalendarza
│   ├── EditRewardForm/           # Edycja nagród
│   ├── EditTrackingItemForm/     # Edycja elementów śledzenia
│   ├── EventDetailsForm/         # Szczegóły wydarzenia
│   ├── FocusModeActiveForm/      # Tryb skupienia (Focus Mode)
│   ├── HabitStepForm/                   # Zarządzanie nawykami
│   ├── HomeBrowserForm/          # Przeglądarka układu domu
│   ├── LoginForm/                # Logowanie
│   ├── MainForm/                 # Główne okno aplikacji
│   ├── MoveTrackingItemDialog/   # Przenoszenie produktów
│   ├── Notifications/            # Panel powiadomień
│   ├── PlantEditorForm/          # Edycja roślin
│   ├── QuickTasksForm/           # Szybkie zadania
│   ├── RecipeDetailsForm/        # Szczegóły przepisu
│   ├── RedeemedRewardsForm/      # Historia nagród
│   ├── ShoppingListForm/         # Lista zakupów
│   ├── StatisticsView/           # Statystyki i nagrody
│   └── TrackingView/             # Moduł śledzenia
├── Models/                       # Modele danych
│   ├── Event.cs                  # Model wydarzenia
│   ├── HabitModels.cs            # Modele nawyków
│   ├── HomeLayoutModels.cs       # Modele układu domu
│   ├── PointsModels.cs           # Modele punktów i nagród
│   ├── TrackingModels.cs         # Modele śledzenia zasobów
│   ├── User.cs                   # Model użytkownika
│   └── UserSession.cs            # Sesja użytkownika
├── Services/                     # Serwisy (logika biznesowa)
│   ├── EventSchedulingAlgorithm.cs  # Algorytm planowania
│   ├── EventService.cs           # Obsługa wydarzeń
│   ├── FirstAidService.cs        # Obsługa apteczki
│   ├── FocusModeService.cs       # Tryb skupienia
│   ├── HomeLayoutService.cs      # Układ domu
│   ├── NotificationService.cs    # Powiadomienia
│   ├── PointsService.cs          # System punktów
│   ├── ShoppingListService.cs    # Lista zakupów
│   ├── TrackingService.cs        # Śledzenie zasobów
│   └── UserService.cs            # Zarządzanie użytkownikami
├── Program.cs                    # Punkt wejścia aplikacji
├── TimeManager.csproj            # Plik projektu
└── TimeManager.sln               # Plik rozwiązania Visual Studio
```

---

## Uruchomienie aplikacji

### Uruchomienie z Visual Studio

1. Otwórz plik `TimeManager.sln` w Visual Studio 2022.
2. Upewnij się, że wybrana jest konfiguracja **Debug** lub **Release**.
3. Naciśnij **F5** lub wybierz **Debug > Start Debugging**.

### Uruchomienie z wiersza poleceń

```powershell
cd TimeManager5
dotnet build
dotnet run
```

### Uruchomienie skompilowanej aplikacji

1. Przejdź do folderu `bin\Debug\net8.0-windows\` lub `bin\Release\net8.0-windows\`.
2. Uruchom plik `TimeManager.exe`.

---

## Ładowanie przykładowych danych

Plik `Database\SampleData.sql` zawiera kompletny zestaw danych testowych:

- **Użytkownicy:** admin, Parent, Kid
- **Układ domu:** 2 piętra, kontenery z przedmiotami
- **Żywność:** produkty w lodówce, zamrażarce i spiżarni
- **Przepisy:** 2 przykładowe przepisy ze składnikami
- **Nawyki:** kategorie i kroki nawyków
- **Leki:** apteczka z harmonogramami przyjmowania
- **Rośliny:** 2 rośliny z harmonogramem podlewania
- **Nagrody:** system motywacyjny z punktami
- **Wydarzenia:** przykładowe zadania w kalendarzu

### Sposób 1: SQL Server Management Studio

1. Otwórz SSMS i połącz się z instancją SQL Server.
2. Otwórz plik `Database\SampleData.sql`.
3. Wykonaj skrypt (F5).
4. Sprawdź komunikaty — poprawne wykonanie wyświetli serię `Seeding...`.

---

## Konta testowe

Po załadowaniu danych przykładowych (`SampleData.sql`) dostępne są następujące konta:

| Użytkownik | Hasło | Rola | Opis |
|------------|-------|------|------|
| `admin` | `admin123` | Administrator | Pełny dostęp do systemu |
| `Parent` | BRAK | User | Konto rodzica z przykładowymi danymi |
| `Kid` | BRAK | Kid | Konto dziecka przypisane do Parent |

> **Uwaga:** Przy pierwszym uruchomieniu z pustą bazą danych, domyślne konto administratora (`admin` / `admin123`) jest tworzone automatycznie. Należy ustawić hasła użytkownikom z poziomu administratora, aby móc się zalogować na ich konta.

---

## Funkcjonalności aplikacji

### Moduły główne

| Moduł | Opis |
|-------|------|
| **Kalendarz** | Planowanie wydarzeń z automatycznym harmonogramowaniem |
| **Tracking** | Śledzenie żywności, nawyków, leków i roślin |
| **Home Browser** | Wirtualna mapa mieszkania z lokalizacją przedmiotów |
| **Statistics** | Statystyki punktów i system nagród |

### Kluczowe funkcje

- **Automatyczne planowanie zadań** — algorytm uwzględniający priorytety, krzywą energetyczną aktywności mózgu
- **Focus Mode** — sesje skupienia
- **Śledzenie terminów ważności** — powiadomienia o przeterminowanych produktach
- **System punktów i nagród** — motywacja do wykonywania zadań
- **Zarządzanie kontami rodzinnymi** — role Administrator, User, Kid

---

## Licencja

Projekt akademicki — Politechnika Śląska, Wydział Automatyki, Elektroniki i Informatyki.

---

## Autor

Projekt zrealizowany w ramach pracy inżynierskiej.



