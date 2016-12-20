using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Mvapi.Models
{
    public class MediaValetMetaData : TableEntity
    {
        public MediaValetMetaData(string domainName, string email, string application, string orgunitid, string metadatalist)
        {
            this.PartitionKey = Guid.NewGuid().ToString();
            this.RowKey = Guid.NewGuid().ToString();
            this.DomainName = domainName;
            this.Email = email;
            this.Application = application;
            this.OrgUnitId = orgunitid;
            this.MetaDataList = metadatalist;
        }

        public MediaValetMetaData()
        {
        }
        public string DomainName { get; set; }
        public string Email { get; set; }

        public string Application { get; set; }
        public string OrgUnitId { get; set; }
        public string MetaDataList { get; set; }
    }
}