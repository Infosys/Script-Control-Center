/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infosys.WEM.Resource.DataAccess;
using Infosys.WEM.Resource.Entity;

namespace Infosys.WEM.UnitTests
{
    [TestClass]
    public class CompanyDS
    {
        
        [TestMethod]
        public void AddCompany()
        {
            CompaniesDS company = new CompaniesDS();
            int uid = 1;

            Companies companies = company.Insert(new Companies
            {
                CompanyId = uid,
                CompanyURL = "",
                CreatedOn = System.DateTime.UtcNow,
                DeploymentBaseUrl = "",
                Description = "Dummy Company",
                LastModifiedOn = System.DateTime.UtcNow,
                LogoURL = "",
                Name = "Infosys",
                NormalizedName = "Infosys",
                PartitionKey = "IAP.Work",
                Region = "QLTY",
                RowKey = uid.ToString(),
                StorageBaseUrl = "STRUTL"
            });

            Assert.IsNotNull(companies);
                
        }


        [TestMethod]
        public void UpdateCompany()
        {
            CompaniesDS company = new CompaniesDS();

            int uid = 1;
            Companies companies = company.Insert(new Companies
            {
                CompanyId = uid,
                CompanyURL = "",
                CreatedOn = System.DateTime.UtcNow,
                DeploymentBaseUrl = "",
                Description = "Dummy Company",
                LastModifiedOn = System.DateTime.UtcNow,
                LogoURL = "",
                Name = "Infosys",
                NormalizedName = "Infosys",
                PartitionKey = "IAP.Work",
                Region = "QLTY",
                RowKey = uid.ToString(),
                StorageBaseUrl = "STRUTL"
            });




            companies = company.Update(new Companies
            {
                CompanyId = uid,
                CompanyURL = "",
                CreatedOn = System.DateTime.UtcNow,
                DeploymentBaseUrl = "",
                Description = "Dummy Company two",
                LastModifiedOn = System.DateTime.UtcNow,
                LogoURL = "",
                Name = "Infosys",
                NormalizedName = "Infosys",
                PartitionKey = "IAP.Work",
                Region = "QLTY",
                RowKey = uid.ToString(),
                StorageBaseUrl = "STRUTL"
            });

            Assert.IsNotNull(companies);

        }

        [TestMethod]
        public void RemoveCompany()
        {
            CompaniesDS company = new CompaniesDS();

            int uid = 1;
            Companies companies = company.Insert(new Companies
            {
                CompanyId = uid,
                CompanyURL = "",
                CreatedOn = System.DateTime.UtcNow,
                DeploymentBaseUrl = "",
                Description = "Dummy Company",
                LastModifiedOn = System.DateTime.UtcNow,
                LogoURL = "",
                Name = "Infosys",
                NormalizedName = "Infosys",
                PartitionKey = "IAP.Work",
                Region = "QLTY",
                RowKey = uid.ToString(),
                StorageBaseUrl = "STRUTL"
            });

            bool isDeleted = company.Delete(new Companies
            {
                CompanyId = uid,
                CompanyURL = "",
                CreatedOn = System.DateTime.UtcNow,
                DeploymentBaseUrl = "",
                Description = "Dummy Company",
                LastModifiedOn = System.DateTime.UtcNow,
                LogoURL = "",
                Name = "Infosys",
                NormalizedName = "Infosys",
                PartitionKey = "IAP.Work",
                Region = "QLTY",
                RowKey = uid.ToString(),
                StorageBaseUrl = "STRUTL"
            });

            Assert.IsTrue(isDeleted);

        }
    }
}
