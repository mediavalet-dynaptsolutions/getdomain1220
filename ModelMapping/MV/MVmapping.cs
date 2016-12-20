using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvapi.Models.MV.Folder;
using Mvapi.Models.MV;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Mvapi.Models.Workfront;
namespace Mvapi.ModelMapping.MV
{
    public class MVmapping
    {
        JavaScriptSerializer JSserializer;
     public    MVmapping()
        {
            JSserializer = new JavaScriptSerializer();
        }
        public Folder FolderMapping(string Response)
        {
            var Folder =JsonConvert.DeserializeObject<Folder>(Response);
            //var Folder = JSserializer.Deserialize<Folder>(Response);
            return Folder;
        }

        public MVAssestObj GetMVAssestObj(string Response)
        {
            var MVAssestObj = JsonConvert.DeserializeObject<MVAssestObj>(Response);
            return MVAssestObj;
        }


        public List<WrokFrontFileFolderMetaData> GetWrokFrontMetaData(string BaseUrl,List<Mvapi.Models.MV.Folder.Payload> FolderMetaData, Asset AssestObj)
        {
            List<WrokFrontFileFolderMetaData> ListWrokFrontFileFolderMetaData = new List<WrokFrontFileFolderMetaData>();
            if (AssestObj != null)
            {
                WrokFrontFileFolderMetaData WrokFrontFileFolderMetaData_ = new WrokFrontFileFolderMetaData();
                WrokFrontFileFolderMetaData_.dateModified = AssestObj.record.modifiedAt;
                WrokFrontFileFolderMetaData_.id = AssestObj.id;
                WrokFrontFileFolderMetaData_.kind = "File";
                WrokFrontFileFolderMetaData_.size = AssestObj.file.sizeInBytes.ToString();
                WrokFrontFileFolderMetaData_.mimeType = AssestObj.file.fileType;
                WrokFrontFileFolderMetaData_.title = AssestObj.title;
                WrokFrontFileFolderMetaData_.downloadLink = AssestObj.media.download;
                WrokFrontFileFolderMetaData_.viewLink = AssestObj.media.thumb;
                ListWrokFrontFileFolderMetaData.Add(WrokFrontFileFolderMetaData_);
            }
            else if (FolderMetaData[0] != null)
            {
                foreach(Mvapi.Models.MV.Folder.Payload Folder_ in FolderMetaData)
                {
                    WrokFrontFileFolderMetaData WrokFrontFileFolderMetaData_ = new WrokFrontFileFolderMetaData();
                    WrokFrontFileFolderMetaData_.dateModified = ""; //Folder_.
                    WrokFrontFileFolderMetaData_.id = Folder_.id;
                    WrokFrontFileFolderMetaData_.kind = "Folder";
                    WrokFrontFileFolderMetaData_.size = "";
                    WrokFrontFileFolderMetaData_.mimeType = "";
                    WrokFrontFileFolderMetaData_.title = Folder_.name;
                    WrokFrontFileFolderMetaData_.downloadLink = BaseUrl+"/"+Folder_.tree.path;
                    WrokFrontFileFolderMetaData_.viewLink = BaseUrl + "/" + Folder_.tree.path; ;
                    ListWrokFrontFileFolderMetaData.Add(WrokFrontFileFolderMetaData_);
                    
                }
            }

            return ListWrokFrontFileFolderMetaData;

        }



        public List<WrokFrontFileFolderMetaData> GetItemsInFolder(string BaseUrl, List<Mvapi.Models.MV.Folder.Payload> FolderMetaData, List<Asset> ListAssestObj)
        {
            List<WrokFrontFileFolderMetaData> ListWrokFrontFileFolderMetaData = new List<WrokFrontFileFolderMetaData>();
            if (ListAssestObj.Count > 0)
            {
                foreach (Asset AssestObj in ListAssestObj)
                {
                    if (AssestObj != null)
                    {
                        WrokFrontFileFolderMetaData WrokFrontFileFolderMetaData_ = new WrokFrontFileFolderMetaData();
                        WrokFrontFileFolderMetaData_.dateModified = AssestObj.record.modifiedAt;
                        WrokFrontFileFolderMetaData_.id = AssestObj.id;
                        WrokFrontFileFolderMetaData_.kind = "File";
                        WrokFrontFileFolderMetaData_.size = AssestObj.file.sizeInBytes.ToString();
                        WrokFrontFileFolderMetaData_.mimeType = AssestObj.file.fileType;
                        WrokFrontFileFolderMetaData_.title = AssestObj.title;
                        WrokFrontFileFolderMetaData_.downloadLink = AssestObj.media.download;
                        WrokFrontFileFolderMetaData_.viewLink = AssestObj.media.thumb;
                        ListWrokFrontFileFolderMetaData.Add(WrokFrontFileFolderMetaData_);
                    }
                }
            }
            if (FolderMetaData != null)
            {
                if (FolderMetaData.Count > 0)
                {
                    foreach (Mvapi.Models.MV.Folder.Payload Folder_ in FolderMetaData)
                    {
                        WrokFrontFileFolderMetaData WrokFrontFileFolderMetaData_ = new WrokFrontFileFolderMetaData();
                        WrokFrontFileFolderMetaData_.dateModified = ""; //Folder_.
                        WrokFrontFileFolderMetaData_.id = Folder_.id;
                        WrokFrontFileFolderMetaData_.kind = "Folder";
                        WrokFrontFileFolderMetaData_.size = "";
                        WrokFrontFileFolderMetaData_.mimeType = "";
                        WrokFrontFileFolderMetaData_.title = Folder_.name;
                        WrokFrontFileFolderMetaData_.downloadLink = BaseUrl + "/" + Folder_.tree.path;
                        WrokFrontFileFolderMetaData_.viewLink = BaseUrl + "/" + Folder_.tree.path; ;
                        ListWrokFrontFileFolderMetaData.Add(WrokFrontFileFolderMetaData_);

                    }
                }
            }

            return ListWrokFrontFileFolderMetaData;

        }

    }
}