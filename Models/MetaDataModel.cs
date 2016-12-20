using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvapi.Models
{
    public class MetaDataModel
    {
        public string partitionkey { get; set; }
        public string DomainName { get; set; }
        public string Email { get; set; }
        public string Application { get; set; }
        public string OrgUnitId { get; set; }
        public string MetaDataList { get; set; }
    }
}