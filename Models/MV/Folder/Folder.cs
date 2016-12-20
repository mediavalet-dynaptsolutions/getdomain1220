using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvapi.Models.MV.Folder
{
    public class MetaInformation
    {
        public int count { get; set; }
        public int offset { get; set; }
        public string filters { get; set; }
        public string sortPath { get; set; }
        public int elapsedTimeInMS { get; set; }
    }

    public class Meta
    {
        public MetaInformation metaInformation { get; set; }
        public string createdAt { get; set; }
    }

    public class RecordCount
    {
        public int totalRecordsFound { get; set; }
        public int startingRecord { get; set; }
        public int recordsReturned { get; set; }
    }

    public class Tree
    {
        public string path { get; set; }
        public string name { get; set; }
    }

    public class Links
    {
        public List<string> functions { get; set; }
    }

    public class Subfolder
    {
        public string name { get; set; }
        public string description { get; set; }
        public int assetCount { get; set; }
        public int subfolderCount { get; set; }
        public Tree tree { get; set; }
        public Links _links { get; set; }
    }

    public class Tree2
    {
        public string path { get; set; }
        public string name { get; set; }
    }

    public class Links2
    {
        public List<object> functions { get; set; }
        public string self { get; set; }
    }

    public class Payload
    {

        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int? assetCount { get; set; }
        public int? subfolderCount { get; set; }
        public Tree tree { get; set; }
        public Links _links { get; set; }
        public List<Subfolder> subfolders { get; set; }
      
       
    }

    public class Folder
    {
        public string apiVersion { get; set; }
        public Meta meta { get; set; }
        public RecordCount recordCount { get; set; }
        public List<Payload> payload { get; set; }
    }

    public class MediaValetLogin
    {
        public string DomainName { get; set; }
        public string ApiUrl { get; set; }
        public string EmailDomain { get; set; }
    }

    public class TokenClass
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string state { get; set; }
        string redirectUrl { get; set; }
        
    }
}