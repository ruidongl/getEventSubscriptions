# Azure Event Grid Subscriptions Viewer

## Project Overview
This is a .NET console application that queries Azure Event Grid system topics for a specific Azure Storage account and returns the count of event subscriptions for each system topic.

## Technologies
- .NET 8.0
- Azure.ResourceManager.EventGrid
- Azure.Identity

## Development Guidelines
- Use Azure DefaultAzureCredential for authentication
- Follow async/await patterns for Azure SDK calls
- Use proper error handling for Azure operations
