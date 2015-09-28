###############################################################################
## Start Powershell Cmdlets
###############################################################################

###############################################################################
# Connect-Dokuwiki

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

###############################################################################
# Get-DokuPage

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
    $RestParams += @{'ContentType'     = 'application/xml' }
    $RestParams += @{'Method'          = 'post' }
    $RestParams += @{'WebSession'      = $Global:MySession }

    $Request = Invoke-RestMethod @RestParams    

		if (!$Quiet) {
			return $Request
		}
  }
}

###############################################################################
# New-RpcMethodCall

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
            $NewRpcMethodCall.Parameters.Add($p)
        }

        #$NewRpcMethodCall.Parameters = $RpcParameters

        #$Global:TestRpcParameters = $RpcParameters

        #$global:TestRpcMethodCall = $NewRpcMethodCall

        return $NewRpcMethodCall
    }
}

###############################################################################
# New-RPCParameter

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

###############################################################################
# Set-DokuPage


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


###############################################################################
## Start Helper Functions
###############################################################################

###############################################################################
## Export Cmdlets
###############################################################################

Export-ModuleMember *-*
