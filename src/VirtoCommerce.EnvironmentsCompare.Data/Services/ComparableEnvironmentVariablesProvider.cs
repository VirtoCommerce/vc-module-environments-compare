using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Newtonsoft.Json;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.EnvironmentsCompare.Data.Services;

public class ComparableEnvironmentVariablesProvider(IWebHostEnvironment webHostEnvironment, IServer server) : IComparableSettingsProvider
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

        AddEnvironmentVariables(result);

        AddNetRuntime(result);

        AddServerFeatures(result);

        AddHosting(result);

        return Task.FromResult((IList<ComparableSettingScope>)[result]);
    }

    protected virtual void AddEnvironmentVariables(ComparableSettingScope result)
    {
        var variablesResultGroup = AbstractTypeFactory<ComparableSettingGroup>.TryCreateInstance();
        variablesResultGroup.GroupName = "Variables";
        result.SettingGroups.Add(variablesResultGroup);

        foreach (var keyValue in Environment.GetEnvironmentVariables())
        {
            if (!(keyValue is DictionaryEntry keyValueEntry) || !VisibleVariables.Contains(keyValueEntry.Key?.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var resultSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
            resultSetting.Name = keyValueEntry.Key.ToString();
            resultSetting.Value = keyValueEntry.Value?.ToString();
            variablesResultGroup.Settings.Add(resultSetting);
        }
    }

    protected virtual void AddNetRuntime(ComparableSettingScope result)
    {
        var netResultGroup = AbstractTypeFactory<ComparableSettingGroup>.TryCreateInstance();
        netResultGroup.GroupName = ".NET runtime";
        result.SettingGroups.Add(netResultGroup);

        var frameworkDescriptionResultSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
        frameworkDescriptionResultSetting.Name = nameof(RuntimeInformation.FrameworkDescription);
        frameworkDescriptionResultSetting.Value = RuntimeInformation.FrameworkDescription;
        netResultGroup.Settings.Add(frameworkDescriptionResultSetting);

        var osDescriptionResultSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
        osDescriptionResultSetting.Name = nameof(RuntimeInformation.OSDescription);
        osDescriptionResultSetting.Value = RuntimeInformation.OSDescription;
        netResultGroup.Settings.Add(osDescriptionResultSetting);

        var processArchitectureResultSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
        processArchitectureResultSetting.Name = nameof(RuntimeInformation.ProcessArchitecture);
        processArchitectureResultSetting.Value = RuntimeInformation.ProcessArchitecture;
        netResultGroup.Settings.Add(processArchitectureResultSetting);

        var runtimeIdentifierResultSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
        runtimeIdentifierResultSetting.Name = nameof(RuntimeInformation.RuntimeIdentifier);
        runtimeIdentifierResultSetting.Value = RuntimeInformation.RuntimeIdentifier;
        netResultGroup.Settings.Add(runtimeIdentifierResultSetting);
    }

    protected virtual void AddServerFeatures(ComparableSettingScope result)
    {
        var serverResultGroup = AbstractTypeFactory<ComparableSettingGroup>.TryCreateInstance();
        serverResultGroup.GroupName = "Server";
        result.SettingGroups.Add(serverResultGroup);

        foreach (var feature in server.Features)
        {
            var featureResultSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
            featureResultSetting.Name = feature.Key.Name;
            featureResultSetting.Value = JsonConvert.SerializeObject(feature.Value);
            serverResultGroup.Settings.Add(featureResultSetting);
        }
    }

    protected virtual void AddHosting(ComparableSettingScope result)
    {
        var hostingResultGroup = AbstractTypeFactory<ComparableSettingGroup>.TryCreateInstance();
        hostingResultGroup.GroupName = "Hosting";
        result.SettingGroups.Add(hostingResultGroup);

        var applicationNameResultSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
        applicationNameResultSetting.Name = nameof(webHostEnvironment.ApplicationName);
        applicationNameResultSetting.Value = webHostEnvironment.ApplicationName;
        hostingResultGroup.Settings.Add(applicationNameResultSetting);

        var contentRootPathResultSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
        contentRootPathResultSetting.Name = nameof(webHostEnvironment.ContentRootPath);
        contentRootPathResultSetting.Value = webHostEnvironment.ContentRootPath;
        hostingResultGroup.Settings.Add(contentRootPathResultSetting);

        var environmentNameResultSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
        environmentNameResultSetting.Name = nameof(webHostEnvironment.EnvironmentName);
        environmentNameResultSetting.Value = webHostEnvironment.EnvironmentName;
        hostingResultGroup.Settings.Add(environmentNameResultSetting);

        var webRootPathResultSetting = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
        webRootPathResultSetting.Name = nameof(webHostEnvironment.WebRootPath);
        webRootPathResultSetting.Value = webHostEnvironment.WebRootPath;
        hostingResultGroup.Settings.Add(webRootPathResultSetting);
    }
}
