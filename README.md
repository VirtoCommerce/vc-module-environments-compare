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
- **Base Environment Comparison**: Select a base environment and compare all other environments against it to highlight differences
- **Difference Filtering**: Toggle between showing all settings or only differences to focus on what's changed
- **Settings Export**: Choose an environment and export its settings for future reference
- **Security Features**:
  - API key-based authentication for secure communication with remote environments
  - Automatic masking of secret/sensitive settings (passwords, secure strings) using SHA1 hashes
- **Visual Indicators**: Clear visual feedback showing which settings differ from the base environment and which environments have errors
- **Error Handling**: Displays connection errors and missing settings with descriptive error messages
- **External API Endpoint**: Provides a secure endpoint for remote environments to expose their settings for comparison

## Screenshots

<img width="1701" height="871" alt="image" src="https://github.com/user-attachments/assets/ebb73931-3197-4587-adfe-2e60dad18114" />

## Setup

### Prerequisites

Before configuring the Environments Compare module, ensure you have:

- At least two running Virto Commerce environments with the Environments Compare module installed on each
- The main environment (from which comparisons are performed) has network access to secondary environments via HTTP/HTTPS protocol
- On each secondary environment:
    - Create **Environments Compare** role with **environments-compare:access** permission
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
    "ComparableEnvironments": [
      {
        "Name": "ENV-1",
        "Url": "http://localhost:5002/",
        "ApiKey": "a4a86441-cabb-4a60-af90-9c6ebe11a401"
      },
      {
        "Name": "ENV-2",
        "Url": "http://localhost:5003/",
        "ApiKey": "a4a86441-cabb-4a60-af90-9c6ebe11a401"
      }
    ]
  }
}
```

For deployment configuration files (`.yml` format), use the following structure:

```yaml
EnvironmentsCompare__ComparableEnvironments__0__Name=ENV-1
EnvironmentsCompare__ComparableEnvironments__0__Url=http://localhost:5002/
EnvironmentsCompare__ComparableEnvironments__0__ApiKey=a4a86441-cabb-4a60-af90-9c6ebe11a401
EnvironmentsCompare__ComparableEnvironments__1__Name=ENV-2
EnvironmentsCompare__ComparableEnvironments__1__Url=http://localhost:5003/
EnvironmentsCompare__ComparableEnvironments__1__ApiKey=a4a86441-cabb-4a60-af90-9c6ebe11a401
```

**Configuration Parameters:**
- `Name`: A descriptive name for the environment (e.g., "Staging", "Production", "Development")
- `Url`: The base URL of the secondary environment (must be accessible from the main environment)
- `ApiKey`: The ApiKey authentication for user in Virto Commerce platform on the secondary environment

#### Secondary Environment Configuration

On each secondary environment that will be compared:
1. Configure role (ex. named: `Environment Compare`) with `environments-compare:access` permission
1. Create a new user with this role
1. Create API key for this user

### Configuration Whitelist

By default, module has white list of appsettings sections and keys that are compared across environments.

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
  - Settings with differences are highlighted in red
  - Setting scopes (e.g., AppSettings, PlatformSettings, StoreSettings) have colored left borders to help identify them when scrolling
  - Color coding helps distinguish between different setting groups

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
