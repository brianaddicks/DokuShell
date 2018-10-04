
function Set-DokuPage {
  [CmdletBinding()]
	<#
	#>

	Param (
		[Parameter(Mandatory=$True,Position=0)]
		[string]$Page,
    [Parameter(Mandatory=$True,Position=1)]
		[string]$Content
	)

    PROCESS {
      $MethodName = 'wiki.putPage'

      $RpcParams = @()
      $RpcParams += New-RpcParameter $Page
      $RpcParams += New-RpcParameter $Content

      $MethodCall = New-RpcMethodCall $MethodName $RpcParams

      $RestParams  = @{}
      $RestParams += @{'Uri'             = $Global:DokuWiki.ApiUrl }
      $RestParams += @{'Body'            = $MethodCall.PrintPlainXml() }
      $RestParams += @{'ContentType'     = 'application/xml' }
      $RestParams += @{'Method'          = 'post' }
      $RestParams += @{'WebSession'      = $Global:MySession }

      $Request = Invoke-RestMethod @RestParams
  
      if (!$Quiet) {
        return $Request.methodResponse.params.param.value.boolean
      }
    }
}

