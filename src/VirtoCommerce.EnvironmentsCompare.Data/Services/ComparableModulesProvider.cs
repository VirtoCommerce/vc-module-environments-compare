using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Modules;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class ComparableModulesProvider : IComparableSettingsProvider
{
    public Task<IList<ComparableSettingScope>> GetComparableSettingsAsync()
    {
        var result = AbstractTypeFactory<ComparableSettingScope>.TryCreateInstance();
        result.ScopeName = "VirtoCommerce";

        var modulesResultGroup = AbstractTypeFactory<ComparableSettingGroup>.TryCreateInstance();
        modulesResultGroup.GroupName = "Installed modules";
        result.SettingGroups.Add(modulesResultGroup);

        foreach (var module in ModuleBootstrapper.Instance.GetInstalledModules())
        {
            var moduleSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
            moduleSetting.Name = module.ModuleName;
            moduleSetting.Value = $"{module.Version} (state: {module.State})";
            modulesResultGroup.Settings.Add(moduleSetting);
        }

        var platformResultGroup = AbstractTypeFactory<ComparableSettingGroup>.TryCreateInstance();
        platformResultGroup.GroupName = "Platform";
        result.SettingGroups.Add(platformResultGroup);

        var platformVersionSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
        platformVersionSetting.Name = "Version";
        platformVersionSetting.Value = PlatformVersion.CurrentVersion?.ToString();
        platformResultGroup.Settings.Add(platformVersionSetting);

        return Task.FromResult((IList<ComparableSettingScope>)[result]);
    }
}
