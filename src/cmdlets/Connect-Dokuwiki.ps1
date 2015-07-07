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
			
			$global:Dokuwiki = New-Object DokuShell.Server
			
            $global:Dokuwiki.Protocol = $Protocol
			$global:Dokuwiki.Host     = $Host
			$global:Dokuwiki.Port     = $Port

            $UserName = $Credential.UserName
            $Password = $Credential.getnetworkcredential().password
			
			$global:Dokuwiki.OverrideValidation()
		}
    }

    PROCESS {
        
        $Params = @()
        $Params += New-RpcParameter $UserName
        $Params += New-RpcParameter $Password

        $MethodCall = New-RpcMethodCall "dokuwiki.login" $Params

        $RestParams  = @{}
        $RestParams += @{'Uri'             = $Global:DokuWiki.ApiUrl }
        $RestParams += @{'Body'            = $MethodCall.PrintPlainXml() }
        $RestParams += @{'ContentType'     = 'xml' }
        $RestParams += @{'Method'          = 'post' }
        $RestParams += @{'SessionVariable' = "Global:MySession" }

        $Login = Invoke-RestMethod @RestParams

        return $Login

        <#
        
        $QueryStringTable = @{ type = "op"
                               cmd  = "<show><system><info></info></system></show>" }

        $QueryString = HelperCreateQueryString $QueryStringTable
        Write-Debug "QueryString: $QueryString"
		$url         = $global:PaDeviceObject.UrlBuilder($QueryString)
        Write-Debug "URL: $Url"

		try   { $QueryObject = $global:PaDeviceObject.HttpQuery($url) } `
        catch {	throw $_.Exception.Message       	           }

        $Data = HelperCheckPaError $QueryObject
		$Data = $Data.system

        $global:PaDeviceObject.Name            = $Data.hostname
        $global:PaDeviceObject.Model           = $Data.model
        $global:PaDeviceObject.Serial          = $Data.serial
        $global:PaDeviceObject.OsVersion       = $Data.'sw-version'
        $global:PaDeviceObject.GpAgent         = $Data.'global-protect-client-package-version'
        $global:PaDeviceObject.AppVersion      = $Data.'app-version'
        $global:PaDeviceObject.ThreatVersion   = $Data.'threat-version'
        $global:PaDeviceObject.WildFireVersion = $Data.'wildfire-version'
        $global:PaDeviceObject.UrlVersion      = $Data.'url-filtering-version'

        #$global:PaDeviceObject = $PaDeviceObject

		
		if (!$Quiet) {
			return $global:Dokuwiki
		}
        #>
    }
}