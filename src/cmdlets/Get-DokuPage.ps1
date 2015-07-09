function Get-DokuPage {
    [CmdletBinding()]
	<#
	#>

	Param (
		[Parameter(Mandatory=$True,Position=0)]
		[string]$Page
	)

    PROCESS {
        $MethodName = 'wiki.getPage'

        $RpcParams = @()
        $RpcParams += New-RpcParameter $Page
        $Global:TestRpcParams = $RpcParams

        $MethodCall = New-RpcMethodCall $MethodName $RpcParams

        $RestParams  = @{}
        $RestParams += @{'Uri'             = $Global:DokuWiki.ApiUrl }
        $RestParams += @{'Body'            = $MethodCall.PrintPlainXml() }
        $RestParams += @{'ContentType'     = 'xml' }
        $RestParams += @{'Method'          = 'post' }
        $RestParams += @{'WebSession'      = $Global:MySession }

        $Request = Invoke-RestMethod @RestParams

        

		if (!$Quiet) {
			return $Request
		}
    }
}