/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region Using Directives

using System;

#endregion Using Directives


namespace ScintillaNET.Configuration
{
    public enum ConfigurationLoadOrder
    {
        BuiltInCustomUser,
        BuiltInUserCustom,
        CustomBuiltInUser,
        CustomUserBuiltIn,
        UserBuiltInCustom,
        UserCustomBuiltIn
    }
}
