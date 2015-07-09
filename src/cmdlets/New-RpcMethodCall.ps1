function New-RpcMethodCall {
    [CmdletBinding()]
	<#
	#>

	Param (
		[Parameter(Mandatory=$True,Position=0)]
		[string]$MethodName,

		[Parameter(Mandatory=$False,Position=1)]
		[array]$RpcParameters
	)

    PROCESS {
        
        $NewRpcMethodCall = New-Object -TypeName DokuShell.RpcMethod

        Write-Verbose "New-RpcMethodCall: $MethodName"
        $NewRpcMethodCall.Name = $MethodName
        
        foreach ($p in $RpcParameters) {
            Write-Verbose "New-RpcMethodCall: $($p.DataType): $($p.Value)"
            Write-Verbose $p.Gettype()
            #$NewRpcMethodCall.Parameters += $p
        }

        $NewRpcMethodCall.Parameters = $RpcParameters

        $Global:TestRpcParameters = $RpcParameters

        $global:TestRpcMethodCall = $NewRpcMethodCall

        return $NewRpcMethodCall
    }
}