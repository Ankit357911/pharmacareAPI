# PharmaCare API

`PharmaCare API` is a `.NET 10` backend for the PharmaCare desktop frontend.

This project provides secure, JWT-protected endpoints for:
- authentication and account management,
- medicine inventory management,
- transactions and dashboard analytics,
- event management,
- earnings summary.

---

## Tech Stack

- `.NET 10` / `ASP.NET Core Web API`
- `Entity Framework Core`
- `SQL Server`
- `JWT Bearer Authentication`
- `BCrypt` password hashing

---

## Run Locally

1. Configure `appsettings.json` (or environment variables) for:
   - `ConnectionStrings:DefaultConnection`
   - `JwtSettings` (`Key`, `Issuer`, `Audience`, `DurationInMinutes`)
2. Apply migrations:
   - `dotnet ef database update`
3. Run API:
   - `dotnet run`

Swagger is enabled in Development mode.

Use `appsettings.Example.json` as a safe template for local or deployment configuration.

---

## Frontend-required Endpoints (Implemented)

> Only the necessary endpoints used by the PharmaCare frontend are listed below.

### Auth & Users
- `POST /api/Users/register`
- `POST /api/Users/login`
- `GET /api/Users/staff` *(Admin)*
- `POST /api/Users/staff` *(Admin)*
- `PUT /api/Users/staff/{accountId}` *(Admin)*
- `DELETE /api/Users/staff/{accountId}` *(Admin)*

### Medicines
- `GET /api/Medicines/names`
- `GET /api/Medicines/details/{name}`
- `GET /api/Medicines/categories`
- `PUT /api/Medicines/update-rate-stock`
- `GET /api/Medicines/stock/{name}`
- `POST /api/Medicines/remove-stock`
- `POST /api/Medicines/upsert`
- `GET /api/Medicines/search?query={query}&take={take}`

### Transactions
- `POST /api/Transactions`
- `GET /api/Transactions/{code}`
- `GET /api/Transactions/my-transactions?page={page}&pageSize={pageSize}`
- `GET /api/Transactions/recent?take={take}`
- `GET /api/Transactions/search?customerName={name}&take={take}`
- `GET /api/Transactions/most-sold`
- `GET /api/Transactions/weekly-earnings`

### Events
- `GET /api/Events/ongoing`
- `GET /api/Events/options`
- `POST /api/Events` *(Admin)*
- `DELETE /api/Events/ongoing` *(Admin)*

### Earnings
- `GET /api/Earnings/summary/current`

---

## Security Notes

- Passwords are hashed with `BCrypt`.
- JWT is required for protected routes.
- Admin-only operations use role-based authorization.
- User responses are sanitized through DTOs (no password hash exposure).

---

## Status

This API was refactored to replace direct SQL from frontend with secure, service-based HTTP endpoints suitable for internship/portfolio-level backend architecture.

---

## Additional Docs

- `docs/API_EXAMPLES.md` for quick request samples.
