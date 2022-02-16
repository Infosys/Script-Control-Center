/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Resource.Entity;

namespace Infosys.WEM.Resource.DataAccess
{
    public static class UserExtension
    {
        public static User GeneratePartitionKey(this User entity)
        {
            //TODO
            if (entity.CompanyId != 0)
            {
                entity.PartitionKey = entity.CompanyId.ToString("00000") + ";" + entity.Alias;
            }
            return entity;

        }
        public static User GenerateRowKey(this User entity)
        {
            if (entity.Id != 0)
            {
                string rowKey = entity.Id.ToString("00000");
                entity.RowKey = rowKey;
            }
            return entity;

        }
        public static User GeneratePartitionKeyAndRowKey(this User entity)
        {
            return entity.GeneratePartitionKey().GenerateRowKey();
        }
    }
}
