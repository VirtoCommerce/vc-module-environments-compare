# Environments Compare Module

## Overview

The Environments Compare Module enables backend administrators to compare platform settings, environment configurations, and system information across multiple Virto Commerce environments (development, staging, production, etc.). This module helps identify configuration discrepancies, troubleshoot environment-specific issues, and ensure consistency across deployments.

The module provides a web-based interface within the Virto Commerce platform where users can select multiple environments, compare their settings side-by-side, and view differences with visual indicators. It supports secure communication with remote environments using API key authentication and automatically masks sensitive data such as passwords and secure strings.

## Key Features

- **Multi-Environment Comparison**: Compare settings across two or more environments simultaneously, including the current environment and remote environments
- **Comprehensive Settings Coverage**:
  - Platform settings (grouped by settings groups)
  - Environment variables (system and process variables)
  - .NET runtime information (framework version, OS description, architecture)
  - Server features and hosting configuration
- **Environment Settings View**: From the environments list, click any environment to open a dedicated blade showing all of its settings in the same structured, filterable layout as the comparison view. This blade focuses on a single environment (without comparison controls), making it easy to inspect, search, and review configuration scopes and groups for one environment at a time.
- **Base Environment Comparison**: Select a base environment and compare all other environments against it to highlight differences
- **Difference Filtering**: Toggle between showing all settings or only differences to focus on what's changed
- **Search**: Quickly filter specific settings using a search bar
- **Settings Export**: Choose an environment and export its settings for future reference
- **Security Features**:
  - API key-based authentication for secure communication with remote environments
  - Automatic masking of secret/sensitive settings (passwords, secure strings) using SHA1 hashes
- **Visual Indicators**: Clear visual feedback showing which settings differ from the base environment and which environments have errors
- **Error Handling**: Displays connection errors and missing settings with descriptive error messages
- **External API Endpoint**: Provides a secure endpoint for remote environments to expose their settings for comparison

## Screenshots

### Admin UI
<img width="1916" height="927" alt="image" src="https://github.com/user-attachments/assets/578612e4-0a81-43c3-90db-5bbe54f9014d" />

### All Environments are Identical
<img width="946" height="686" alt="image" src="https://github.com/user-attachments/assets/94a74430-b284-40e0-b578-274f75da2304" />

### Filter
<img width="957" height="665" alt="image" src="https://github.com/user-attachments/assets/738f65e0-c3f6-411c-a8be-9f8489d14de2" />

### Connection Error 
<img width="956" height="702" alt="image" src="https://github.com/user-attachments/assets/0ee45628-9f2e-43ed-8d45-2bafd38c150b" />

## Setup

### Prerequisites

Before configuring the Environments Compare module, ensure you have:

- At least two running Virto Commerce environments with the Environments Compare module installed on each
- The main environment (from which comparisons are performed) has network access to secondary environments via HTTP/HTTPS protocol
- On each secondary environment:
    - Create **Environments Compare** role with **environments-compare:read** permission
    - Create an account assigned to the EnvironmentsCompare role
    - Create API keys for user
- Valid URLs for all secondary environments that will be compared

### Configuration

#### Main Environment Configuration

> Note: We recommend to keep ApiKey and URL in a secure storage for security reasons.

On the main environment (the environment from which you will perform comparisons), configure the list of secondary environments to compare. Add the following configuration to your `appSettings.json`:

```json
{
  "EnvironmentsCompare": {
    "CurrentEnvironmentName": "Production",
    "ComparableEnvironments": [
      {
        "Name": "QA",
        "Url": "https://qa.mydomaim.com",
        "ApiKey": "a4a86441-cabb-4a60-af90-9c6ebe11a401"
      },
      {
        "Name": "Development",
        "Url": "https://dev.mydomaim.com",
        "ApiKey": "a4a86441-cabb-4a60-af90-9c6ebe11a401"
      }
    ]
  }
}
```

For deployment configuration files (`.yml` format), use the following structure:

```yaml
EnvironmentsCompare__CurrentEnvironmentName=Production
EnvironmentsCompare__ComparableEnvironments__0__Name: QA
EnvironmentsCompare__ComparableEnvironments__0__Url: https://qa.mydomaim.com
EnvironmentsCompare__ComparableEnvironments__0__ApiKey: a4a86441-cabb-4a60-af90-9c6ebe11a401
EnvironmentsCompare__ComparableEnvironments__1__Name: Development
EnvironmentsCompare__ComparableEnvironments__1__Url: https://dev.mydomaim.com
EnvironmentsCompare__ComparableEnvironments__1__ApiKey: a4a86441-cabb-4a60-af90-9c6ebe11a401
```

**Configuration Parameters:**
- `Name`: A descriptive name for the environment (e.g., "Staging", "Production", "Development")
- `Url`: The base URL of the secondary environment (must be accessible from the main environment)
- `ApiKey`: The ApiKey authentication for user in Virto Commerce platform on the secondary environment

#### Secondary Environment Configuration

On each secondary environment that will be compared:
1. Configure role (ex. named: `Environment Compare`) with `environments-compare:read` permission
1. Create a new user with this role
1. Create API key for this user

### Configuration Whitelist

By default, the module has a whitelist of appsettings sections and keys that are compared across environments.

You can extend or narrow down which appsettings sections and keys are compared by configuring `EnvironmentsCompare:WhiteList`.
It supports `Include` and `Exclude` lists for both `SectionKeys` and `SettingKeys`.

- `SectionKeys` controls which top-level configuration sections are considered.
- `SettingKeys` controls which specific keys are treated as public (non-secret) during comparison.

Example `appsettings.json`:

```json
{
  "EnvironmentsCompare": {
    "WhiteList": {
      "SectionKeys": {
        "Include": [ "Logging", "ConnectionStrings" ],
        "Exclude": [ "Notifications" ]
      },
      "SettingKeys": {
        "Include": [ "LoginPageUI:BackgroundUrl" ],
        "Exclude": [ "Assets:AzureBlobStorage:CdnUrl" ]
      }
    }
  }
}
```

## Scenarios

### Accessing the Environments List

1. Open the main environment in your browser
2. Navigate to the Environments Compare module
3. The environments list displays:
   - **Current** environment (the main environment, always available)
   - All secondary environments configured in the `ComparableEnvironments` setting

### Exporting Environment Settings

You can export settings from any environment for documentation or backup purposes:

1. From the environments list, select the environment you want to export
2. Click the export action for that environment
3. The system generates a JSON file containing all settings for the selected environment

**Export Behavior:**
- **Current Environment**: Exports a comprehensive JSON file with all available settings
- **Secondary Environments**: Exports settings retrieved from the remote environment. If the connection fails, the exported file will contain an error description instead of settings

### Comparing Environments

1. From the environments list, select two or more environments to compare (including the current environment if desired)
2. Click the **"Compare"** button
3. The comparison blade opens, displaying settings side-by-side in columns

**Comparison Interface Features:**

- **Environment Status Indicators:**
  - Environments that failed to connect are displayed in gray
  - The current environment (if selected) is always visible and available
  - Error messages are displayed for environments with connection issues

- **Base Environment Selection:**
  - The leftmost environment column is used as the base for comparison by default
  - To change the base environment, click the column header with the microscope icon (🔬)
  - Selecting an unavailable environment as the base will result in all settings being marked as different

- **View Modes:**
  - **Show Differences Only** (default): Displays only settings that differ from the base environment
  - **Show All**: Toggle to view all settings, including those that match the base environment

- **Visual Indicators:**
  - Settings with differences are highlighted
  - Setting scopes (e.g., AppSettings, PlatformSettings, StoreSettings) have colored left borders to help identify them when scrolling
  - Color coding helps distinguish between different setting groups

### Extending with a custom IComparableSettingsProvider

Use a custom provider when you need to:
- Compare additional configuration sources not covered by the built-in providers (e.g., external services, secrets vaults, tenant-specific configs).
- Add domain/module-specific settings scopes and grouping rules.
- Normalize or transform values before comparison (e.g., redact parts of secrets, map legacy keys, or compute derived values).

Providers are responsible for returning one or more `ComparableSettingScope` objects containing groups and settings ready for the comparison UI.

Minimal implementation:

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.EnvironmentsCompare.Core.Models;
using VirtoCommerce.EnvironmentsCompare.Core.Services;
using VirtoCommerce.Platform.Core.Common;

public class MyCustomSettingsProvider : IComparableSettingsProvider
{
  public Task<IList<ComparableSettingScope>> GetComparableSettingsAsync() { var scope = AbstractTypeFactory<ComparableSettingScope>.TryCreateInstance(); scope.ScopeName = "MyCustomScope";
      var group = AbstractTypeFactory<ComparableSettingGroup>.TryCreateInstance();
      group.GroupName = "MyCustomGroup";
      scope.SettingGroups.Add(group);

      // Example settings (IsSecret = false marks it public for direct comparison)
      var s1 = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
      s1.Name = "MySection:MyKey";
      s1.Value = "SomeValue";
      s1.IsSecret = false;
      group.Settings.Add(s1);

      var s2 = AbstractTypeFactory<ComparableSetting>.TryCreateInstance();
      s2.Name = "MySection:SecretKey";
      s2.Value = "redacted"; // or hashed
      s2.IsSecret = true;    // will be compared as secure
      group.Settings.Add(s2);

      return Task.FromResult<IList<ComparableSettingScope>>(new List<ComparableSettingScope> { scope });
  }
}
```

Register your provider in `Module.cs` (Web project), register the provider in DI so it participates in comparison.

```csharp
serviceCollection.AddTransient<IComparableSettingsProvider, MyCustomSettingsProvider>();
```


## References

- [Deployment](https://docs.virtocommerce.org/platform/developer-guide/Tutorials-and-How-tos/Tutorials/deploy-module-from-source-code/)
- [Installation](https://docs.virtocommerce.org/platform/user-guide/modules-installation/)
- [Home](https://virtocommerce.com)
- [Community](https://www.virtocommerce.org)
- [Download latest release](https://github.com/VirtoCommerce/vc-module-environments-compare/releases)

## License

Copyright (c) Virto Solutions LTD. All rights reserved.

This software is licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at http://virtocommerce.com/opensourcelicense.

Unless required by the applicable law or agreed to in written form, the software
distributed under the License is provided on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
