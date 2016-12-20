using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvapi.Models
{
    public class EloquaOauthClientEntity : TableEntity
    {
        //string instanceid, string customerid, string imageurl, string assetid, string assetname, string assettype
        public EloquaOauthClientEntity(string clientid, string clientsecretcode, string clientencodedcode)
        {
            this.PartitionKey = Guid.NewGuid().ToString();
            this.RowKey = Guid.NewGuid().ToString();
            this.ClientId = clientid;
            this.ClientSecret = clientsecretcode;
            this.ClientEncodedCode = clientencodedcode;
         
            this.CreatedDate = DateTime.Now.Date;

        }

        public EloquaOauthClientEntity()
        {
        }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ClientEncodedCode { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}