function New-RpcParameter {
    [CmdletBinding()]
	<#
	#>

	Param (
		[Parameter(Mandatory=$True,Position=0)]
        $Object,

		[Parameter(Mandatory=$False,Position=1)]
		[string]$DataType = $Object.GetType().Name
	)

    PROCESS {
        
        switch ($DataType) {
            { $_ -match 'int' } { $RpcDataType = 'int' }
            { ($_ -match 'double') -or
              ($_ -match 'decimal') -or
              ($_ -match 'float') } { $RpcDataType = 'double' }
            string  { $RpcDataType = 'string' }
            base64  { $RpcDataType = 'base64' }
        }
        
        $NewRpcParam          = New-Object -type DokuShell.RpcParam
        
        $Value = [string]$Object
        Write-Verbose "New-RpcParameter: Writing value `"$Value`""
        $NewRpcParam.Value    = $Value

        Write-Verbose "New-RpcParameter: Write datatype `"$RpcDataType`""
        $NewRpcParam.DataType = $RpcDataType

        return $NewRpcParam
    }
}