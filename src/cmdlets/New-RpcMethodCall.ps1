function New-RpcMethodCall {
    [CmdletBinding()]
	<#
	#>

	Param (
		[Parameter(Mandatory=$True,Position=0)]
		[string]$MethodName,

		[Parameter(Mandatory=$False,Position=1)]
		[array]$Parameters
	)

    PROCESS {
        
        $NewRpcMethodCall = New-Object -TypeName DokuShell.RpcMethod
        $NewRpcMethodCall.Name = $MethodName
        $NewRpcMethodCall.Parameters = $Parameters

        return $NewRpcMethodCall
    }
}