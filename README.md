# KNUST Hall Maintenance Reporting System

Replaces the paper "complaint book" at the hall assistant's desk with a simple
digital system: students report an issue, staff/maintenance triage and update
it, and everyone can see the status instead of waiting weeks with no answer.

Built as a lean 6-screen ASP.NET Core MVC app, styled with KNUST's brand
colors (Limeade green, Yellow Green, Witch Haze).

## Screens
1. **Login** — shared entry point, redirects by role
2. **Student Dashboard** — a student's own complaints + status
3. **Create Complaint** — report an issue (category, location, description, photo)
4. **Complaint Details** — student view of one complaint + history
5. **Staff Dashboard** — all complaints, filterable by status/category (shared by Staff and Maintenance roles)
6. **Complaint Details / Assignment** — staff view: assign to maintenance, update status, leave notes

## Tech stack
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core + SQL Server (your local SSMS instance)
- Simple cookie authentication with role claims (Student / Staff / Maintenance) — no full Identity UI scaffolding, kept intentionally minimal
- Bootstrap 5 (via CDN) + a small custom stylesheet for the KNUST theme

## Running it

1. Install the [.NET 8 SDK](https://dotnet.microsoft.com/download) if you don't have it.
2. Check the connection string in `appsettings.json`:
   ```
   Server=(localdb)\MSSQLLocalDB;Database=KnustHallMaintenance;Trusted_Connection=True;TrustServerCertificate=True
   ```
   This targets **LocalDB** (the lightweight SQL Server instance that ships
   with Visual Studio) — it's the default server name SSMS shows on a typical
   dev machine. If your SSMS "Server name" field shows something different
   (e.g. `.\SQLEXPRESS` or a named server), change the `Server=` value to match
   that exactly.
   - Uses Windows/trusted authentication (no username/password needed) —
     matches the default in SSMS's connect dialog.
3. Generate and apply the initial migration (creates the database + tables from your models). In Visual Studio, open **Tools → NuGet Package Manager → Package Manager Console** (or the "Package Manager Console" tab at the bottom, which you already have open) and run:
   ```powershell
   Add-Migration InitialCreate
   Update-Database
   ```
   Or if you'd rather use the CLI (a terminal in the project folder):
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
   Either way, this creates a `Migrations/` folder in the project with a
   snapshot of your schema, and applies it to `KnustHallMaintenance` on your
   LocalDB instance.
4. Press **F5**. The app also calls `Database.Migrate()` on startup, so any
   pending migrations get applied automatically from then on — you won't need
   to repeat step 3 unless you change the models again (see below).
5. Open the URL shown in the console (e.g. `https://localhost:5001`) if it
   doesn't launch automatically. Demo accounts are seeded on first run.

### Making a model change later
Whenever you add/rename/remove a field on `ApplicationUser`, `Complaint`, or
`ComplaintUpdate`:
```powershell
Add-Migration DescribeYourChange
Update-Database
```
Then just F5 — `Database.Migrate()` applies it automatically, and your
existing data is preserved (unlike the old `EnsureCreated()` approach, which
would have required deleting the database).

## Demo accounts (seeded automatically)

| Role | Email | Password |
|---|---|---|
| Student | student@knust.edu.gh | Password123! |
| Staff | staff@knust.edu.gh | Password123! |
| Maintenance | maintenance@knust.edu.gh | Password123! |

## Project structure

```
Controllers/    AccountController, ComplaintsController (student), StaffController, HomeController
Models/         ApplicationUser, Complaint, ComplaintUpdate, enums, ViewModels
Data/           ApplicationDbContext, DbInitializer (seeding)
Services/       IComplaintService / ComplaintService — complaint business logic
                IUserService / UserService — user lookups, password verification
                Controllers depend on these interfaces only — none of them touch ApplicationDbContext directly
Helpers/        StatusBadgeHelper — maps status to CSS class / display text
Views/          Account, Complaints, Staff, Shared/_Layout
wwwroot/css/    site.css — KNUST color theme
```

## Design notes (for your defense)

- **Status is a plain enum state machine** (`Submitted → Assigned → InProgress → Resolved`),
  not a generic workflow engine — simple to reason about, easy to explain.
- **`ComplaintUpdate` is an audit log** — every assignment and status change writes
  a timestamped entry, so both students and staff can see full history. This
  directly answers the "nobody tells you anything" problem with the old book system.
- **One shared Staff Dashboard/Details for both Staff and Maintenance roles** —
  `[Authorize(Roles = "Staff,Maintenance")]` gates the controller; the "Assign"
  section only shows when a complaint has no assignee yet, keeping the UI simple.
- **Business logic sits in services, not controllers.** `ComplaintService` owns
  the `Complaints`/`ComplaintUpdates` tables (create, assign, status transitions,
  history logging). `UserService` owns the `Users` table (lookup by email/id,
  password verification, listing maintenance staff). No controller injects
  `ApplicationDbContext` — each one only depends on `IComplaintService` and/or
  `IUserService`, which is what makes the controllers unit-testable without a
  real database (mock the interfaces).
- **Real EF Core migrations**, not `EnsureCreated()` — schema changes are
  tracked as files in `Migrations/`, applied via `Database.Migrate()` on
  startup, and preserve existing data when you evolve the models. This is the
  standard approach for anything beyond a throwaway prototype, and gives you
  a version history of every schema change to point to in a defense.

## Extending it later
- Add SLA badges (e.g. flag complaints unresolved after N days)
- Add an analytics screen (avg. resolution time by category/hall)
- Swap SQLite for SQL Server for a multi-hall deployment
- Add email/SMS notifications on status change
