using System.Reflection;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.EnvironmentsCompare.Data.Repositories;

public class EnvironmentsCompareDbContext : DbContextBase
{
    public EnvironmentsCompareDbContext(DbContextOptions<EnvironmentsCompareDbContext> options)
        : base(options)
    {
    }

    protected EnvironmentsCompareDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //modelBuilder.Entity<EnvironmentsCompareEntity>().ToTable("EnvironmentsCompare").HasKey(x => x.Id);
        //modelBuilder.Entity<EnvironmentsCompareEntity>().Property(x => x.Id).HasMaxLength(IdLength).ValueGeneratedOnAdd();

        switch (Database.ProviderName)
        {
            case "Pomelo.EntityFrameworkCore.MySql":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.EnvironmentsCompare.Data.MySql"));
                break;
            case "Npgsql.EntityFrameworkCore.PostgreSQL":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.EnvironmentsCompare.Data.PostgreSql"));
                break;
            case "Microsoft.EntityFrameworkCore.SqlServer":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.EnvironmentsCompare.Data.SqlServer"));
                break;
        }
    }
}
