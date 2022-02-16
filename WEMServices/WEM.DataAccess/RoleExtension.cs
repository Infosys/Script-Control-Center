/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Resource.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infosys.WEM.Resource.DataAccess
{
    public static class RoleExtension
    {
        public static Role GeneratePartitionKey(this Role entity)
        {
            //TODO
            entity.PartitionKey = "IAP";
            return entity;

        }
        public static Role GenerateRowKey(this Role entity)
        {

            if (entity.Id != null || entity.Id != 0)
            {
                string rowKey = entity.Id.ToString("0000");
                entity.RowKey = rowKey;
            }
            return entity;

        }
        public static Role GeneratePartitionKeyAndRowKey(this Role entity)
        {
            return entity.GeneratePartitionKey().GenerateRowKey();
        }
    }
}
