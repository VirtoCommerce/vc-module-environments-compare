using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VirtoCommerce.EnvironmentsCompare.Data.Repositories;

namespace VirtoCommerce.EnvironmentsCompare.Data.PostgreSql;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EnvironmentsCompareDbContext>
{
    public EnvironmentsCompareDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<EnvironmentsCompareDbContext>();
        var connectionString = args.Length != 0 ? args[0] : "Server=localhost;Username=virto;Password=virto;Database=VirtoCommerce3;";

        builder.UseNpgsql(
            connectionString,
            options => options.MigrationsAssembly(typeof(PostgreSqlDataAssemblyMarker).Assembly.GetName().Name));

        return new EnvironmentsCompareDbContext(builder.Options);
    }
}
