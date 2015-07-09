function New-RpcMethodCall {
    [CmdletBinding()]
	<#
	#>

	Param (
		[Parameter(Mandatory=$True,Position=0)]
		[string]$MethodName,

		[Parameter(Mandatory=$False,Position=1)]
		$RpcParameters
	)

    PROCESS {
        
        $NewRpcMethodCall = New-Object -TypeName DokuShell.RpcMethod

        Write-Verbose "New-RpcMethodCall: $MethodName"
        $NewRpcMethodCall.Name = $MethodName
        
        foreach ($p in $RpcParameters) {
            Write-Verbose "New-RpcMethodCall: $($p.DataType): $($p.Value)"
            
        }

        $NewRpcMethodCall.Parameters = $RpcParameters

        $Global:TestRpcParameters = $RpcParameters

        $global:TestRpcMethodCall = $NewRpcMethodCall

        return $NewRpcMethodCall
    }
}