using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvapi.Models
{
    public class EloquaTrackingEntity: TableEntity
    {
        //string instanceid, string customerid, string imageurl, string assetid, string assetname, string assettype
        public EloquaTrackingEntity(string instanceid, string customerid, string imageurl, string assetid, string assetname, string assettype,string sasurl,string renditionheight,string renditionwidth)
        {
            this.PartitionKey = Guid.NewGuid().ToString();
            this.RowKey = Guid.NewGuid().ToString();
            this.InstanceId = instanceid;
            this.CustomerId = customerid;
            this.ImageUrl =imageurl ;
            this.AssetId = assetid;
            this.AssetName = assetname;
            this.AssetType = assettype;
            this.SasUrl = sasurl;
            this.CreatedDate = DateTime.Now.Date;

            this.RenditionHeight = renditionheight;
            this.RenditionWidth = renditionwidth;
            
        }

        public EloquaTrackingEntity()
        {
        }
        public string InstanceId { get; set; }
        public string CustomerId { get; set; }
        public string ImageUrl { get; set; }
        public string  AssetId { get; set; }
        public string AssetName { get; set; }
        public string  AssetType { get; set; }
        public string SasUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RenditionHeight  { get; set; }
        public string  RenditionWidth { get; set; }

    }
}