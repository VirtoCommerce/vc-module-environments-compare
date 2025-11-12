using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VirtoCommerce.EnvironmentsCompare.Data.Repositories;

namespace VirtoCommerce.EnvironmentsCompare.Data.SqlServer;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EnvironmentsCompareDbContext>
{
    public EnvironmentsCompareDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<EnvironmentsCompareDbContext>();
        var connectionString = args.Length != 0 ? args[0] : "Server=(local);User=virto;Password=virto;Database=VirtoCommerce3;";

        builder.UseSqlServer(
            connectionString,
            options => options.MigrationsAssembly(typeof(SqlServerDataAssemblyMarker).Assembly.GetName().Name));

        return new EnvironmentsCompareDbContext(builder.Options);
    }
}
