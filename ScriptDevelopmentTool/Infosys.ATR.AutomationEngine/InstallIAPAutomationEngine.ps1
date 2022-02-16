# this agent may support 
#	Work flow execution if the ExecutionEngineSupported in its config file has value = 1
#	Script execution if the the ExecutionEngineSupported in its config file has value = 2
#	Both if the ExecutionEngineSupported in its config file has value = 3


[cmdletbinding()]
param(
    [parameter(mandatory=$True)]
    [string] $executionEngineSupported,
    [parameter(mandatory=$True)]
    [bool] $startService
)

# add the required assemblies
# Add-Type -AssemblyName System.IO

try
{
    #install the windows service
    #the bits would be in the folder 'winservice'
    [string] $currentLocation = Get-Location
    Write-Host "'$currentLocation' is the current location..."
    $runPath = $currentLocation + "\winservice\IAP.AutomationEngine.exe"
    $appConfigPath = $currentLocation + "\winservice\IAP.AutomationEngine.exe.config"
    $winServiceName = "IAP.AutomationEngine"
    $appsettingToReplace = '<add key="ExecutionEngineSupported" value="1"/>'
    $newAppsetting = '<add key="ExecutionEngineSupported" value="' + $executionEngineSupported + '"/>'


    #check if the service is installed, then first un-install it
    $winService = Get-WmiObject -Class Win32_Service -Filter "Name='$winServiceName'"

    if($winService -ne $null)
    {
    	Write-Host "'$winServiceName' exists and hence removing..."
    	Stop-Service $winServiceName
    	#wait for some time for the service to get stopped
    	Start-Sleep -s 5
    	
    	#then un-install the service
    	$winService.Delete()
    	#wait for some time for the service to get un-installed
    	Start-Sleep -s 10
    	Write-Host "'$winServiceName' stopped and un-installed."
    }

    #change the appsetting for ExecutionEngineSupported
    #try reading the config file and update it
    $configData = $null
    try
    {
    	$reader =  [System.IO.StreamReader] $appConfigPath
    	$configData = $reader.ReadToEnd()
    	$reader.close()
    }
    finally
    {
       if($reader -ne $null)
       {
           $reader.dispose()
       }
    }

    if($configData -ne $null)
    {
    	$configData = $configData -replace $appsettingToReplace, $newAppsetting
    	try
    	{
    	   $writer = [System.IO.StreamWriter] $appConfigPath
    	   $writer.write($configData)
    	   $writer.close()
    	}
    	finally
    	{
    	   if($writer -ne $null)
    	   {
    	       $writer.dispose()
    	   }
    	}
    }

    Write-Host "'$winServiceName' being installed..."
    New-Service -BinaryPathName $runPath -Name $winServiceName -DisplayName $winServiceName -StartupType Automatic

    Write-Host "'$winServiceName' installed."

    if($startService)
    {
    	Write-Host "Starting '$winServiceName'..."
    	Start-Service $winServiceName
    }

	#copy Infosys.Lif.IISDoc.dll to C:\IAP\References
	[string] $destLoc = "C:\IAP\References"
	try
	{    
		if( ![System.IO.Directory]::Exists($destLoc))
		{
			[System.IO.Directory]::CreateDirectory($destLoc)
			Copy-Item ".\winservice\Infosys.Lif.IISDoc.dll" $destLoc
		}
		elseif (! [System.IO.File]::Exists($destLoc + "\Infosys.Lif.IISDoc.dll"))
		{
			Copy-Item ".\winservice\Infosys.Lif.IISDoc.dll" $destLoc
		}
	}
	catch
	{
		Write-Host "Failed to copy Infosys.Lif.IISDoc.dll to " + $destLoc 
		Write-Host "Reason- " + $_.Exception.Message
	}

    Write-Host "Installation completed."

}
catch [System.Net.WebException]{
    Write-Host $_.Exception.Message -foregroundcolor red
}