param(
    [Parameter(Mandatory = $true)]
    [string]$SystemTopicName,

    [Parameter(Mandatory = $true)]
    [string]$ResourceGroupName,

    [Parameter(Mandatory = $false)]
    [string]$SubscriptionId,

    [Parameter(Mandatory = $false)]
    [int]$Threshold = 400
)

# Ensure Az.EventGrid module is available
if (-not (Get-Module -ListAvailable -Name Az.EventGrid)) {
    throw "Az.EventGrid module is not installed. Install it with 'Install-Module Az.EventGrid'."
}

try {
    $context = Get-AzContext -ErrorAction Stop
} catch {
    throw "No Azure context found. Run Connect-AzAccount before executing this script. $_"
}

$effectiveSubscriptionId = $SubscriptionId

if ($SubscriptionId) {
    # Switch context only when a subscription ID is provided
    Set-AzContext -SubscriptionId $SubscriptionId | Out-Null
} else {
    $effectiveSubscriptionId = $context.Subscription.Id
}

if (-not $effectiveSubscriptionId) {
    throw "Unable to determine subscription id. Provide -SubscriptionId or set an Azure context with Connect-AzAccount."
}

try {
    $subscriptions = Get-AzEventGridSystemTopicEventSubscription -ResourceGroupName $ResourceGroupName -SystemTopicName $SystemTopicName -SubscriptionId $effectiveSubscriptionId -ErrorAction Stop
} catch {
    throw "Failed to retrieve event subscriptions for system topic '$SystemTopicName' in resource group '$ResourceGroupName'. $_"
}

$count = ($subscriptions | Measure-Object).Count

if ($count -gt 0) {
    Write-Host "Event subscriptions for system topic '$SystemTopicName':"
    foreach ($subscription in $subscriptions) {
        Write-Host " - $($subscription.name)" -ForegroundColor Cyan
    }
}

Write-Host "System topic '$SystemTopicName' has $count event subscription(s)."

if ($count -gt $Threshold) {
    Write-Error "Subscription count exceeded threshold: $count" -ErrorAction Stop
}

# Return the count for pipeline usage
return $count
