steps:


o	To create the installation env:

1. Create folder and add the script- InstallIAPAutomationEngine.ps1
2. In the same folder create another folder- winservice
3. In this folder add the bits for the windows service.
4. In the IAP.AutomationEngine.exe.config make sure to have an appsetting:

<add key="ExecutionEngineSupported" value="1"/>

o	To start the installation:

1. Start power shell console in admin mode 
2. And run the powershell script from its location e.g.:

PS D:\Assignments\powershell_winservice_installer> .\InstallIAPAutomationEngine.ps1 3 $True


Here:
> First parameter is the new value for ExecutionEngineSupported appsetting
> Second value is to tell if the windows service is to be started once installed


this agent may support 
o	Work flow execution if the appsetting: ExecutionEngineSupported in its config file has value = 1
o	Script execution if the the appsetting: ExecutionEngineSupported in its config file has value = 2
o	Both if the appsetting: ExecutionEngineSupported in its config file has value = 3


