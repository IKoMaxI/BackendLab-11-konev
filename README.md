# StoreApiLR11

ASP.NET Core Web API интернет-магазина на Entity Framework Core и MySQL.

## Подготовка базы данных

Укажите данные локального MySQL в `appsettings.json`, затем выполните:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Swagger: `https://localhost:7193/swagger`.
