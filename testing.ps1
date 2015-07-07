[CmdletBinding()]
Param (
    [Parameter(Mandatory=$False,Position=0)]
	[switch]$PushToStrap
)

$VerbosePreference = "Continue"
if ($PushToStrap) {
    & ".\buildmodule.ps1" -PushToStrap
} else {
    & ".\buildmodule.ps1"
}

ipmo .\*.psd1

$PaPass  = $PaPass | ConvertTo-SecureString -AsPlainText -force
$PaCred  = New-Object System.Management.Automation.PSCredential $PaUser,$PaPass