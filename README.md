# Script-Control-Center
Script Control Center is a centralized script repository and script execution engine. The Script Control Center enables large teams to manage and execute scripts written in different technologies from a single interface. It allows Enterprises to build a repository of scripts which can be reused and standardized across teams.


# Build Instructions
1.Download source code <br />
2.Right click the Zip file and remove/unblock the security note <br />
3.Navigate to the Script-Control-Center/ScriptDevelopmentTool/ path <br />
4.Open visual studio cmd Prompt <br />
5.Open WorkflowExecutionManager solution <br />
6.Execute following command <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MSBuild ~/Script-Control-Center-main/WEMServices/WorkflowExecutionManager.sln <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OR <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Right click on WorkflowExecutionManager.sln > Build Solution <br />
7.Replace all the respective (list of all dlls) dlls after the latest build of WEMServices projects to Script-Control-Center/ScriptDevelopmentTool/References <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Script-Control-Center-main\Script-Control-Center-main\WEMServices\WEM.Host <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Businesscomponent.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.BusinessEntity.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Host.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.ServiceImplementation.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Script-Control-Center-main\Script-Control-Center-main\WEMServices\WEM.Host.ConsoleApp\bin\Debug <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Client.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Export.DataAccess.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Scripts.Contracts.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Search.Contracts.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.ScriptExecution.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Scripts.DataAccess.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.ServiceImplementation.dll <br />
8.Open the solution ATR from Script-Control-Center/ScriptDevelopmentTool <br />
9.Execute following command <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MSBuild ~/Script-Control-Center-main/ScriptDevelopmentTool/ATR.sln <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OR <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Right click on ATR.sln > Build Solution <br />

# Release Instructions
1.Follow the Build Instructions for ATR.sln <br/>
2.Go to Script-Control-Center-main\ScriptDevelopmentTool\References and copy all the runtime files <br/>
3.Copy them to the ScriptControlCenter directory ..\iapwemServices\bin
