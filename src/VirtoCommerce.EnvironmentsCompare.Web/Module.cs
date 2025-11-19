using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.EnvironmentsCompare.Core;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.EnvironmentsCompare.Data.MySql;
using VirtoCommerce.EnvironmentsCompare.Data.PostgreSql;
using VirtoCommerce.EnvironmentsCompare.Data.Repositories;
using VirtoCommerce.EnvironmentsCompare.Data.Services;
using VirtoCommerce.EnvironmentsCompare.Data.SqlServer;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.MySql.Extensions;
using VirtoCommerce.Platform.Data.PostgreSql.Extensions;
using VirtoCommerce.Platform.Data.SqlServer.Extensions;

namespace VirtoCommerce.EnvironmentsCompare.Web;

public class Module : IModule, IHasConfiguration
{
    public ManifestModuleInfo ModuleInfo { get; set; }
    public IConfiguration Configuration { get; set; }

    public void Initialize(IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<EnvironmentsCompareDbContext>(options =>
        {
            var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
            var connectionString = Configuration.GetConnectionString(ModuleInfo.Id) ?? Configuration.GetConnectionString("VirtoCommerce");

            switch (databaseProvider)
            {
                case "MySql":
                    options.UseMySqlDatabase(connectionString, typeof(MySqlDataAssemblyMarker), Configuration);
                    break;
                case "PostgreSql":
                    options.UsePostgreSqlDatabase(connectionString, typeof(PostgreSqlDataAssemblyMarker), Configuration);
                    break;
                default:
                    options.UseSqlServerDatabase(connectionString, typeof(SqlServerDataAssemblyMarker), Configuration);
                    break;
            }
        });

        serviceCollection.AddTransient<IComparableSettingsMasterProvider, ComparableSettingsMasterProvider>();

        serviceCollection.AddTransient<IComparableSettingsProvider, ComparableEnvironmentVariablesProvider>();
        serviceCollection.AddTransient<IComparableSettingsProvider, ComparableAppSettingsProvider>();
        serviceCollection.AddTransient<IComparableSettingsProvider, ComparablePlatformSettingsProvider>();
        serviceCollection.AddTransient<IComparableSettingsProvider, ComparableModulesProvider>();

        serviceCollection.AddTransient<IEnvironmentsCompareService, EnvironmentsCompareService>();
        serviceCollection.AddTransient<IEnvironmentsCompareClient, EnvironmentsCompareClient>();
        serviceCollection.AddTransient<IEnvironmentsCompareSettingsService, EnvironmentsCompareSettingsService>();
    }

    public void PostInitialize(IApplicationBuilder appBuilder)
    {
        var serviceProvider = appBuilder.ApplicationServices;

        // Register settings
        var settingsRegistrar = serviceProvider.GetRequiredService<ISettingsRegistrar>();
        settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

        // Register permissions
        var permissionsRegistrar = serviceProvider.GetRequiredService<IPermissionsRegistrar>();
        permissionsRegistrar.RegisterPermissions(ModuleInfo.Id, "EnvironmentsCompare", ModuleConstants.Security.Permissions.AllPermissions);

        // Apply migrations
        using var serviceScope = serviceProvider.CreateScope();
        using var dbContext = serviceScope.ServiceProvider.GetRequiredService<EnvironmentsCompareDbContext>();
        dbContext.Database.Migrate();
    }

    public void Uninstall()
    {
        // Nothing to do here
    }
}
