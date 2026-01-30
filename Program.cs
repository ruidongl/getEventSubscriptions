using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.EventGrid;
using Azure.ResourceManager.Resources;
using Microsoft.Extensions.Configuration;

var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
    .Build();

var azureSection = configuration.GetSection("Azure");

if (!azureSection.Exists())
{
    throw new InvalidOperationException("Azure configuration section is missing in appsettings.json.");
}

string subscriptionId = azureSection["SubscriptionId"]
    ?? throw new InvalidOperationException("Azure:SubscriptionId is required in appsettings.json.");
string resourceGroupName = azureSection["ResourceGroup"]
    ?? throw new InvalidOperationException("Azure:ResourceGroup is required in appsettings.json.");
string storageAccountName = azureSection["StorageAccount"]
    ?? throw new InvalidOperationException("Azure:StorageAccount is required in appsettings.json.");

Console.WriteLine($"Searching for Event Grid system topics for storage account: {storageAccountName}");
Console.WriteLine();

// Authenticate using DefaultAzureCredential
var credential = new DefaultAzureCredential();
var armClient = new ArmClient(credential);

// Get the subscription
var subscription = await armClient.GetSubscriptionResource(
    new Azure.Core.ResourceIdentifier($"/subscriptions/{subscriptionId}")).GetAsync();

// Get system topics in the resource group that are associated with the storage account
var resourceGroup = await subscription.Value.GetResourceGroupAsync(resourceGroupName);
var systemTopics = resourceGroup.Value.GetSystemTopics();

int topicsFound = 0;

await foreach (var systemTopic in systemTopics.GetAllAsync())
{
    // Filter for storage account system topics
    var sourceString = systemTopic.Data.Source?.ToString() ?? string.Empty;
    if (!string.IsNullOrEmpty(sourceString) && 
        sourceString.Contains(storageAccountName, StringComparison.OrdinalIgnoreCase))
    {
        topicsFound++;
        Console.WriteLine($"System Topic: {systemTopic.Data.Name}");
        Console.WriteLine($"  Topic Type: {systemTopic.Data.TopicType}");
        Console.WriteLine($"  Source: {systemTopic.Data.Source}");
        Console.WriteLine($"  Location: {systemTopic.Data.Location}");
        Console.WriteLine($"  Provisioning State: {systemTopic.Data.ProvisioningState}");

        // Get event subscriptions for this system topic
        var eventSubscriptions = systemTopic.GetSystemTopicEventSubscriptions();
        int subscriptionCount = 0;

        Console.WriteLine("  Event Subscriptions:");
        await foreach (var eventSubscription in eventSubscriptions.GetAllAsync())
        {
            subscriptionCount++;
            Console.WriteLine($"    - {eventSubscription.Data.Name}");
            Console.WriteLine($"      Destination Type: {eventSubscription.Data.Destination?.GetType().Name ?? "N/A"}");
            Console.WriteLine($"      Provisioning State: {eventSubscription.Data.ProvisioningState}");
        }

        Console.WriteLine($"  Total Event Subscriptions: {subscriptionCount}");
        Console.WriteLine();
    }
}

if (topicsFound == 0)
{
    Console.WriteLine($"No system topics found for storage account '{storageAccountName}' in resource group '{resourceGroupName}'.");
}
else
{
    Console.WriteLine($"Found {topicsFound} system topic(s) for storage account '{storageAccountName}'.");
}
