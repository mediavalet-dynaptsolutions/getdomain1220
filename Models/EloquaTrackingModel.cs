using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvapi.Models
{
    public class EloquaTrackingModel
    {
        public string assetid { get; set; }
        public string customerid { get; set; }
        public string imageurl { get; set; }
        public string assetname { get; set; }
        public string instanceid { get; set; }
        public string assettype { get; set; }
        public string sasurl { get; set; }
        public string  renditionheigt { get; set; }
        public string  renditionwidth { get; set; }
        //public string MyProperty { get; set; }
        //string instanceid, string customerid, string imageurl, string assetid, string assetname, string assettype
    }
}