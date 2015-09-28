function Connect-Dokuwiki {
    [CmdletBinding()]
	<#
	#>

	Param (
		[Parameter(Mandatory=$True,Position=0)]
		[ValidatePattern("\d+\.\d+\.\d+\.\d+|(\w\.)+\w")]
		[string]$Host,

    [Parameter(ParameterSetName="credential",Mandatory=$True,Position=1)]
    [pscredential]$Credential,

		[Parameter(Mandatory=$False,Position=2)]
		[int]$Port = $null,

		[Parameter(Mandatory=$False,Position=2)]
		[string]$WebRoot = "",
    
		[Parameter(Mandatory=$False)]
		[alias('http')]
		[switch]$HttpOnly,
		
		[Parameter(Mandatory=$False)]
		[alias('q')]
		[switch]$Quiet
	)

  BEGIN {

		if ($HttpOnly) {
			$Protocol = "http"
			if (!$Port) { $Port = 80 }
		} else {
			$Protocol = "https"
			if (!$Port) { $Port = 443 }
    }
			
			$global:Dokuwiki = New-Object DokuShell.Server
			
      $global:Dokuwiki.Protocol = $Protocol
			$global:Dokuwiki.Host     = $Host
			$global:Dokuwiki.Port     = $Port
      $global:Dokuwiki.WebRoot  = $WebRoot.trim('/')

      $UserName = $Credential.UserName
      $Password = $Credential.getnetworkcredential().password
			
			$global:Dokuwiki.OverrideValidation()
      
  }

  PROCESS {
        
    $Params = @()
    $Params += New-RpcParameter $UserName
    $Params += New-RpcParameter $Password
    $global:params = $Params

    $MethodCall = New-RpcMethodCall "dokuwiki.login" $Params

    $RestParams  = @{}
    $RestParams += @{'Uri'             = $Global:DokuWiki.ApiUrl }
    $RestParams += @{'Body'            = $MethodCall.PrintPlainXml() }
    $RestParams += @{'ContentType'     = 'application/xml' }
    $RestParams += @{'Method'          = 'post' }
    $RestParams += @{'SessionVariable' = "Global:MySession" }

    $Login = Invoke-RestMethod @RestParams

		if (!$Quiet) {
			return $Login
		}
  }
}