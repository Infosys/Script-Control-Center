# Script-Control-Center
Script Control Center is a centralized script repository and script execution engine. The Script Control Center enables large teams to manage and execute scripts written in different technologies from a single interface. It allows Enterprises to build a repository of scripts which can be reused and standardized across teams.


# Build Instructions
1.Download source code <br />
2.Unblock the file by right click on the Zip file > Properties > Under General tab select Unblock > Apply > Ok  <br />
3.Extract the files to any path <br />
4.Open WorkflowExecutionManager solution in visual studio from the path ../WEMServices/WorkflowExecutionManager.sln <br />
5.Open visual studio cmd Prompt and execute<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MSBuild ~/WEMServices/WorkflowExecutionManager.sln <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OR <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Right click on WorkflowExecutionManager.sln > Build Solution <br />
6.Replace all the respective (list of all WEM dlls) dlls after the latest build of WEMServices projects to ../ScriptDevelopmentTool/References <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.AuotmationTracker.Contracts.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.BusinessComponent.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.BusinessEntity.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Client.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Common.contracts.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Contracts.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.DataAccess.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.DataEntity.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Export.Contracts.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Export.DataAccess.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Host.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.IDataAccess.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Infrastructure.Common.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Infrastructure.SecurityCore.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Nia.Contracts.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Node.Contracts.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Observer.Contracts.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.ScriptExecution.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.ScriptExecutionLibrary.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Scripts.Contracts.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Scripts.DataAccess.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Search.Contracts.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.Search.DataAccess.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.SecurityAccess.Contracts.dll <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WEM.ServiceImplementation.dll <br />
7.Open ATR solution in visual studio from the path ../ScriptDevelopmentTool/ATR.sln <br />
8.In the visual studio command prompt execute the following command <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MSBuild ~/ScriptDevelopmentTool/ATR.sln <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OR <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Right click on ATR.sln > Build Solution <br />

# Release Instructions
1.Follow the Build Instructions for ATR.sln <br/>
2.Go to ..\ScriptDevelopmentTool\bin\Debug and copy all the runtime files <br/>
3.Copy them to the ScriptControlCenter release directory ..\iapwemServices\bin
