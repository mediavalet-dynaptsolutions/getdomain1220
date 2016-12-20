using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvapi.Models
{
    public class MediaValetCategoryDataEntity : TableEntity
    {
        public MediaValetCategoryDataEntity(string domainName, string email, string categories, string application, string orgunitid)
        {
            this.PartitionKey = Guid.NewGuid().ToString();
            this.RowKey = Guid.NewGuid().ToString();
            this.DomainName = domainName;
            this.Email = email;
            this.Categories = categories;
            this.Application = application;
            this.OrgUnitId = orgunitid;
            
        }

        public MediaValetCategoryDataEntity()
        {
        }

        public string DomainName { get; set; }
        public string Email { get; set; }
        public string Categories { get; set; }
        public string Application { get; set; }
        public string OrgUnitId { get; set; }


    }
}