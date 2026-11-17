# BookStore

An online bookstore built with **ASP.NET Core 9 MVC** and **Entity Framework Core** (code-first, SQL Server).

## Features

**Customers**
- Register / log in (passwords hashed with `PasswordHasher<T>`)
- Browse the book catalog and view book details
- Shopping cart (session-based): add / remove items
- Personal profile with editable details and profile image

**Admin**
- Dashboard
- Book management (create, edit, delete, details)
- User management, including **locking** accounts

## Tech stack

C# · .NET 9 · ASP.NET Core MVC · Entity Framework Core · SQL Server · Razor · Bootstrap

## Data model

`User` · `Role` · `UserProfile` · `Book` · `Category` · `CartItem` · `Order` · `OrderItem`

## Getting started

1. Set `ConnectionStrings:constr` in `appsettings.json` to your SQL Server instance.
2. Apply the migrations:
   ```bash
   dotnet ef database update
   ```
3. Run the app:
   ```bash
   dotnet run
   ```
