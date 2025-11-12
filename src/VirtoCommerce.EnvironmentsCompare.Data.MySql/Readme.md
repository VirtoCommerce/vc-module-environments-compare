## Package manager
```
Add-Migration Initial -Context VirtoCommerce.EnvironmentsCompare.Data.Repositories.EnvironmentsCompareDbContext -Project VirtoCommerce.EnvironmentsCompare.Data.MySql -StartupProject VirtoCommerce.EnvironmentsCompare.Data.MySql -OutputDir Migrations -Verbose -Debug
```

### Entity Framework Core Commands
```
dotnet tool install --global dotnet-ef --version 8.*
```

**Generate Migrations**
```
dotnet ef migrations add Initial -- "{connection string}"
dotnet ef migrations add Update1 -- "{connection string}"
dotnet ef migrations add Update2 -- "{connection string}"
```
etc..

**Apply Migrations**
```
dotnet ef database update -- "{connection string}"
```
