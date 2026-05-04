# Library of Things — Peer-to-Peer Rental Marketplace

A .NET MAUI mobile application that allows community members to list items for rent and request rentals from other users.

## Student Information
- **Name:** Anita Mwangi
- **Student ID:** 40807544
- **Module:** SET09102, Edinburgh Napier University

## GitHub Repository
https://github.com/anitawanjiku/RentalApp

---

## Features
- User authentication (login and registration)
- Browse available items
- List new items for rent
- Request rentals
- View incoming and outgoing rental requests

---

## Tech Stack
- .NET 9.0 / .NET MAUI
- PostgreSQL 16 (via Docker)
- Entity Framework Core 9
- xUnit + Moq (testing)
- GitHub Actions (CI/CD)

---

## Setup Instructions

### Prerequisites
- .NET 9 SDK
- Docker Desktop
- Android Emulator with Android 34+
- Java JDK 21
- Android SDK Build Tools 35

### Environment Variables
Set these before building:
```bash
export JAVA_HOME=/Library/Java/JavaVirtualMachines/jdk-21.jdk/Contents/Home
export ANDROID_HOME=~/Android
export PATH=$PATH:~/Android/platform-tools:~/Android/cmdline-tools/latest/bin:~/Android/emulator
```

### Database Setup
1. Start the PostgreSQL container:
```bash
docker compose up -d db
```

2. Run migrations:
```bash
dotnet ef database update --project StarterApp.Database --startup-project StarterApp.Database --context StarterApp.Database.Data.AppDbContext --connection "Host=localhost;Port=5432;Database=appdb;Username=app_user;Password=app_password"
```

### Build and Run
```bash
dotnet build -c Debug
adb install -r ./StarterApp/bin/Debug/net9.0-android/android-arm64/com.companyname.starterapp-Signed.apk
```

---

## Running Tests
```bash
dotnet test StarterApp.Test/StarterApp.Test.csproj
```

Expected output: 14 tests, 0 failures.

---

## CI/CD
GitHub Actions workflow runs on every push to `main`. It builds the Database and Test projects and runs all unit tests automatically.

Workflow file: `.github/workflows/build.yml`

---

## Architecture
The app follows MVVM architecture with a Repository Pattern and Service Layer:
- **Models:** Item, Rental, User (in StarterApp.Database/Models)
- **Repositories:** IItemRepository, IRentalRepository (in StarterApp.Database/Data/Repositories)
- **ViewModels:** ItemsListViewModel, RentalsViewModel, etc (in StarterApp/ViewModels)
- **Views:** XAML pages with no logic (in StarterApp/Views)

---

## Known Issues
- Items list loads twice on page appear (minor display bug)
- User ID is hardcoded to 1 in rental requests — requires logged-in user integration