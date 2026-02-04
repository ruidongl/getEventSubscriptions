# PowerShell Utilities

## Get-SystemTopicSubscriptionCount.ps1

Counts Event Grid event subscriptions for a system topic and optionally enforces a threshold.

### Requirements
- PowerShell 5.1 or later (or PowerShell 7)
- Az PowerShell modules with Az.EventGrid 2.2.0 or newer (`Install-Module Az.EventGrid -Scope CurrentUser`)
- Logged-in Azure session (`Connect-AzAccount`)

### Usage
```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Get-SystemTopicSubscriptionCount.ps1 \
    -SystemTopicName "<system-topic-name>" \
    -ResourceGroupName "<resource-group-name>" \
    [-SubscriptionId "<subscription-id>"] \
    [-Threshold 400]
```

#### Parameters
- `SystemTopicName` (required): Name of the Event Grid system topic.
- `ResourceGroupName` (required): Resource group containing the system topic.
- `SubscriptionId` (optional): Azure subscription to target. Defaults to the active context.
- `Threshold` (optional): Maximum allowed subscriptions (default `400`). Setting `0` disables the alert.

### Output
- Lists each discovered event subscription name.
- Displays the total count.
- Throws an error if the count exceeds the threshold.

### Example
```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\Get-SystemTopicSubscriptionCount.ps1 \
    -SystemTopicName "mystorage-edf6" \
    -ResourceGroupName "rg-eventgrid" \
    -Threshold 100
```

### Notes
- The script validates that the Az.EventGrid module is available before running.
- If `SubscriptionId` is omitted, the script uses the subscription from the current Azure context.
