
# ONFarm

# ON Farm — Application Desktop WPF

> Le pilotage intelligent des patients en pharmacie.

## Prérequis

- **Windows 10/11** (obligatoire pour WPF)
- **Visual Studio 2022** (avec le workload "Développement .NET Desktop")
- **.NET 8 SDK**
- **PostgreSQL 14+** installé localement

## Installation

### 1. Configurer PostgreSQL

```sql
-- Dans pgAdmin ou psql :
CREATE DATABASE onfarm_db;
```

### 2. Configurer la chaîne de connexion

Dans `ONFarm.View/App.xaml.cs`, modifier :

```csharp
options.UseNpgsql("Host=localhost;Port=5432;Database=onfarm_db;Username=postgres;Password=VOTRE_MOT_DE_PASSE")
```

### 3. Ouvrir la solution

```
ONFarm.sln → Visual Studio 2022
```

### 4. Restaurer les packages NuGet

```
Build → Restore NuGet Packages
```

### 5. Appliquer les migrations

Dans la console Package Manager (projet **ONFarm.Infrastructure** comme Default Project) :

```
PM> Add-Migration InitialCreate
PM> Update-Database
```

Ou en ligne de commande :

```bash
dotnet ef migrations add InitialCreate --project ONFarm.Infrastructure --startup-project ONFarm.View
dotnet ef database update --project ONFarm.Infrastructure --startup-project ONFarm.View
```

### 6. Lancer l'application

Définir `ONFarm.View` comme projet de démarrage → **F5**

---

## Architecture

```
ONFarm.sln
├── ONFarm.View          → WPF (XAML, code-behind, converters, styles)
├── ONFarm.ViewModel     → MVVM (ViewModels, commands)
├── ONFarm.Application   → Services applicatifs, interfaces
├── ONFarm.Domain        → Entités, interfaces, enums
├── ONFarm.Infrastructure→ EF Core, repositories, DbContext
└── ONFarm.Common        → RelayCommand, utilitaires
```

## Fonctionnalités

- ✅ Tableau de bord avec statistiques
- ✅ Liste patients avec recherche et filtres
- ✅ Ajout / modification / suppression de patients
- ✅ Suivi des ordonnances et factures manquées
- ✅ Système de rappels avec notifications
- ✅ Calendrier hebdomadaire
- ✅ Export Excel

## Packages NuGet utilisés

| Package | Usage |
|---------|-------|
| `Npgsql.EntityFrameworkCore.PostgreSQL` | ORM PostgreSQL |
| `Microsoft.EntityFrameworkCore` | ORM |
| `ClosedXML` | Export/Import Excel |
| `Microsoft.Extensions.DependencyInjection` | IoC Container |

