/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Text;

namespace CodeGenerationEngine.Model
{
	public class Iap_Parameters
	{
		public string Method
		{
			get;
			set;
		}

		public Iap_Parameters(string methodText)
		{
			this.Method = this.GetMethods(methodText);
		}

		private string GetMethods(string _resultParam)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\n\n##############################################################################");
			stringBuilder.Append(string.Format("\n### Method         :  {0}", "Arguments for external use"));
			stringBuilder.Append(string.Format("\n### Description    :  {0}", ""));
			stringBuilder.Append("\n##############################################################################");
			stringBuilder.Append(string.Format("\ndef {0}():", "GetArguments"));
			stringBuilder.Append("\n    pDict = {}");
			stringBuilder.Append(Convert.ToString(_resultParam).Replace("\t", ""));
			stringBuilder.Append(string.Format("\n    return pDict", new object[0]));
			return stringBuilder.ToString();
		}
	}
}
