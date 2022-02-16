/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

Setup httpmodule on IIS Server
1. Copy the bin directory to the target web application configured on IIS (refer document)
2. To enable logging manually copy the following dll's to the target bin folder
	a. Microsoft.Practices.EnterpriseLibrary.Data.dll
	b. Microsoft.Practices.EnterpriseLibrary.Logging.Database.dll
	c. Microsoft.Practices.ServiceLocation.dll
	d. Microsoft.Practices.Unity.dll
	e. Microsoft.Practices.Unit.Interception.dll
3. Ensure that you have copied the loggiing configuration for the project App.configuration file
4. Credentials used to run the apppool for the application in which the module is going to be hosted would need to be provided access in the Log db
