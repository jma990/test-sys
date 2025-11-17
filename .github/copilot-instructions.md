# Copilot Instructions for Content Management System

## Project Overview
This is an **ASP.NET Core 8 Razor Pages application** - a multi-user content management system with role-based access control (RBAC). Users belong to departments, post announcements, and admins manage accounts and permissions. The application uses **SQLite** for persistence and **Argon2id** for password hashing.

**Key Technologies:**
- Framework: ASP.NET Core 8 Razor Pages (not MVC)
- Database: SQLite with Entity Framework Core 9
- Authentication: Cookie-based with Claims
- Password Security: Argon2id hashing with per-user salts

## Architecture Pattern: Page Model + Controllers Hybrid

The app uses **Razor Pages** for UI workflows BUT also **API Controllers** for specific operations:
- **Razor Pages** (`Pages/*.cshtml.cs`): Handle traditional page navigation, form submissions, role-based filtering
- **API Controllers** (`Controllers/*`): Handle AJAX/JSON endpoints for department/user selection, announcement submissions
  - `AnnouncementsController`: `GetUsers`, `GetDepartments`, `AddAnnouncement` endpoints
  - `DepartmentsController`, `UserManagementController`: Likely handle additional CRUD operations

**Authentication Flow:**
1. Login validates credentials via `CookieService.CreateCookieAsync()` which embeds user ID, email, role, and `MustChangePassword` flag as claims
2. `AuthPageFilter` (applied via `[ServiceFilter]`) enforces:
   - User must be logged in (`user.IsLoggedIn()`)
   - If `MustChangePassword==true`, redirect to `/MandatoryPasswordChange` only
   - Role-based page access (check `IsPageAllowedForRole`)

## Data Model Essentials

**User Roles:** `SuperAdmin` (0), `Admin` (1), `Member` (2)

**Key Entities:**
- `User`: First/Middle/Last name, email, password+salt, role, department, `MustChangePassword` flag, `IsArchived` flag
- `Announcement`: Title, content, author (User), optional department link, attachment bytes, link URL, `IsActive` flag
- `Department`: Name, `IsActive` flag, has many users and announcements
- `ResetPasswordLog`: Audit trail (AdminID → TargetUserID with timestamp)

**Special Constraint:** Department ID 1 is reserved for "Super Admin" and excluded from dropdown filters.

## Routing & Path Management

All page routes are centralized in `Data/PathDirectory.cs` as constants:
- Change page routes ONLY in PathDirectory, never hardcode paths in pages
- Example: `PathDirectory.AnnouncementsPage = "/Announcements"`
- CSS assets also defined there (e.g., `PathDirectory.AnnouncementsCSS = "~/css/announcements.css"`)

**Role-Based Navigation:**
- SuperAdmin: Account Creation, Departments, Account Settings, User Management
- Admin: Announcements, AdminPosts, Account Creation, Departments, Account Settings, User Management
- Member: Announcements, Account Settings only

## Critical Implementation Patterns

### Password Management
```csharp
// Always use HashPassword.This(password, salt) with user's existing salt
var hashedInput = HashPassword.This(Password, user.Salt);
if (hashedInput != user.Password) { /* invalid */ }

// Creating new users: generate salt FIRST
var salt = HashPassword.GenerateSalt();
var hashedPassword = HashPassword.This(newPassword, salt);
```
Argon2id config: 8 parallelism, 64MB memory, 4 iterations (see `HashPassword.cs`).

### Timezone Handling
Use `TimezoneHelper.NowPH()` for all timestamps - app assumes Philippine timezone.

### API Responses
Controllers return JSON with `{ success: bool, message: string, data?: object }` pattern:
```csharp
return Ok(new { success = true, message = "Success", data = result });
return Ok(new { success = false, message = "Error reason" });
```

### Form Binding
- Use `[BindProperty]` for page properties + `[Required]`, `[EmailAddress]` validation
- Controller endpoints using `[FromBody]` for JSON payloads (nested `Data` classes for DTOs)

## Database & Migrations

**Connection string:** `Data Source=ContentManagement.db` (file-based SQLite in app root)

**Add migrations after schema changes:**
```bash
cd Content-Management-System
dotnet ef migrations add MigrationName
dotnet ef database update
```

**Initial setup:** `SuperAdminService.CreateSuperAdmin()` runs at startup to seed default Super Admin if none exists.

## Build & Run

```bash
cd Content-Management-System
dotnet build
dotnet run
# App runs on HTTPS by default, check launchSettings.json for ports
```

**Deployment:** Uses `render.yaml` config - publishes to Release output, runs from `out/` folder.

## Testing Conventions

- **No unit test framework observed** in project structure; focus on manual testing or integration tests if needed
- Page handlers are async-first; always use `async Task` or `async Task<IActionResult>`

## Common Pitfalls to Avoid

1. **Hardcoding paths:** Use `PathDirectory` constants instead
2. **Plain-text passwords:** Every password assignment must use `HashPassword.This()` with a salt
3. **Skipping role checks:** `AuthPageFilter` is applied to protected pages; verify role access in `IsPageAllowedForRole` matches the page's actual requirements
4. **Forgetting `AsNoTracking()`:** Use when retrieving read-only announcement lists to reduce EF Core overhead
5. **Incorrect timezone:** Use `TimezoneHelper.NowPH()` for createdAt/updatedAt, not `DateTime.Now`
6. **Missing claims:** When creating auth cookies, always include `MustChangePassword` claim - it affects redirect logic

## File Organization Reference

- **Data layer:** `Data/AppDbContext.cs` (models + DbContext), `Data/PathDirectory.cs` (routes)
- **Authentication:** `Utilities/CookieService.cs`, `Utilities/UserExtensions.cs`, `PageFilters/AuthPageFilter.cs`
- **Security:** `Utilities/HashPassword.cs`, `Utilities/SuperAdminService.cs`
- **UI:** `Pages/*.cshtml.cs` (page handlers), `Controllers/*.cs` (API endpoints)
