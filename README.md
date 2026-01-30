# Azure Event Grid Subscriptions Viewer

A .NET console application that queries Azure Event Grid system topics for a specific Azure Storage account and returns the count of event subscriptions for each system topic.

## Prerequisites

- .NET 8.0 SDK
- Azure subscription with Event Grid system topics configured
- Azure CLI logged in or appropriate Azure credentials configured

## Configuration

Update the Azure settings in `appsettings.json` before running. Keep placeholders in source control and move real values to a local-only file:

```json
{
  "Azure": {
    "SubscriptionId": "00000000-0000-0000-0000-000000000000",
    "ResourceGroup": "your-resource-group-name",
    "StorageAccount": "your-storage-account-name"
  }
}
```

Optionally create an environment-specific file (for example `appsettings.Development.json`) and set the `DOTNET_ENVIRONMENT` variable to load it.

To avoid checking secrets into source control, use .NET user secrets while developing locally:

```bash
dotnet user-secrets init
dotnet user-secrets set "Azure:SubscriptionId" "<your-subscription-id>"
dotnet user-secrets set "Azure:ResourceGroup" "<your-resource-group>"
dotnet user-secrets set "Azure:StorageAccount" "<your-storage-account>"
```

## Authentication

This application uses `DefaultAzureCredential` which supports multiple authentication methods:

1. **Environment Variables** - Service principal credentials
2. **Azure CLI** - Logged in via `az login`
3. **Visual Studio** - Logged in through VS
4. **Managed Identity** - When running in Azure

For local development, the easiest method is to log in via Azure CLI:
```bash
az login
```

## Usage

```bash
dotnet run
```

## Output

The application will display:
- System topic name and properties
- List of event subscriptions for each topic
- Total count of event subscriptions per topic

Example output:
```
Searching for Event Grid system topics for storage account: mystorageaccount

System Topic: mystorageaccount-topic
  Topic Type: Microsoft.Storage.StorageAccounts
  Source: /subscriptions/.../storageAccounts/mystorageaccount
  Location: eastus
  Provisioning State: Succeeded
  Event Subscriptions:
    - blob-created-subscription
      Destination Type: AzureFunctionEventSubscriptionDestination
      Provisioning State: Succeeded
    - blob-deleted-subscription
      Destination Type: WebHookEventSubscriptionDestination
      Provisioning State: Succeeded
  Total Event Subscriptions: 2

Found 1 system topic(s) for storage account 'mystorageaccount'.
```

## Dependencies

- Azure.ResourceManager.EventGrid
- Azure.Identity
