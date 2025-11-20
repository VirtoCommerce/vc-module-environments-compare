# Virto Commerce Environments Compare Module

## Overview

The Virto Commerce Environments Compare Module enables backend administrators to compare platform settings, environment configurations, and system information across multiple Virto Commerce environments (development, staging, production, etc.). This module helps identify configuration discrepancies, troubleshoot environment-specific issues, and ensure consistency across deployments.

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

## References

- [Deployment](https://docs.virtocommerce.org/platform/developer-guide/Tutorials-and-How-tos/Tutorials/deploy-module-from-source-code/)
- [Installation](https://docs.virtocommerce.org/platform/user-guide/modules-installation/)
- [Home](https://virtocommerce.com)
- [Community](https://www.virtocommerce.org)
- [Download latest release](https://github.com/VirtoCommerce/vc-module-push-messages/releases)

## License

Copyright (c) Virto Solutions LTD. All rights reserved.

This software is licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at http://virtocommerce.com/opensourcelicense.

Unless required by the applicable law or agreed to in written form, the software
distributed under the License is provided on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
