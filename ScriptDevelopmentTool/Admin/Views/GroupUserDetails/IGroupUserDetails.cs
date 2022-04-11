﻿/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infosys.ATR.Admin.Entities;

namespace Infosys.ATR.Admin.Views
{
   public interface IGroupUserDetails
    {
       List<UserRoles> Roles { get; set; }
    }
}