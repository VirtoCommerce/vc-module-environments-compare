using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class ComparableEnvironmentVariablesProvider() : IComparableSettingsProvider
{
    protected virtual IList<string> VisibleVariables
    {
        get
        {
            return [
                "COMPUTERNAME",
                "HOME",
                "HOMEDRIVE",
                "HOMEPATH",
                "NUMBER_OF_PROCESSORS",
                "OS",
                "PATH",
                "PROCESSOR_ARCHITECTURE",
                "PROCESSOR_IDENTIFIER",
                "PROCESSOR_LEVEL",
                "PROCESSOR_REVISION",
                "SESSIONNAME",
                "SystemDrive",
                "SystemRoot",
                "TEMP",
                "TMP",
                "USERDOMAIN",
                "USERNAME",
                "USERPROFILE",
                ];
        }
    }

    public Task<IList<ComparableSettingScope>> GetComparableSettingsAsync()
    {
        var result = AbstractTypeFactory<ComparableSettingScope>.TryCreateInstance();
        result.ScopeName = "Environment";

        var resultGroup = AbstractTypeFactory<ComparableSettingGroup>.TryCreateInstance();
        resultGroup.GroupName = "Variables";
        result.SettingGroups.Add(resultGroup);

        foreach (var keyValue in Environment.GetEnvironmentVariables())
        {
            if (!(keyValue is DictionaryEntry keyValueEntry) || !VisibleVariables.Contains(keyValueEntry.Key?.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var resultSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
            resultSetting.Name = keyValueEntry.Key.ToString();
            resultSetting.Value = keyValueEntry.Value;
            resultGroup.Settings.Add(resultSetting);
        }

        return Task.FromResult((IList<ComparableSettingScope>)[result]);
    }
}
