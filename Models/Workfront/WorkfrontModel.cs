using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvapi.Models.Workfront
{
    public class WorkfrontModel
    {
    }




    public class WrokFrontFileFolderMetaData
    {
        public string title { get; set; }
        public string kind { get; set; }
        public string viewLink { get; set; }
        public string downloadLink { get; set; }
        public string mimeType { get; set; }
        public string dateModified { get; set; }
        public string size { get; set; }
        public string id { get; set; }
    }
}