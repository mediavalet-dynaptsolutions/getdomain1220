using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Web.Http;





using System.Web.Script.Serialization;
using Mvapi.Lib;
using System.Web.Http.Cors;
using Mvapi.DataAccessLayer;
using Mvapi.ModelMapping.MV;
using Mvapi.Models.MV.Folder;
using Mvapi.Models.MV;
using Mvapi.Models.Workfront;
using System.Net;
using System.IO;

namespace Mvapi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class WorkfrontController : ApiController
    {

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        readonly static log4net.ILog loggers = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        WrokFrontFileFolderMetaData WrokFrontFileFolderMetaData_;

        string clientId = "JkOnJ9zIQ1vWvP3FvsJVx-3iOnSd-6a-";
        string clientSecret = "U_ZHCQackGJHW4-Jn4qfGce6JLV9qAKhJEGahyRHVpeYVWf_r8iSaSt4z6AZn8kC";
        string baseUrl = "https://api.codeproject.com/";

        // The server base address

        string MVToken = "";
        string MVBaseURL = "";
        public WorkfrontController()
        {
            DataAccess.InitializeStorageAccount();
            WrokFrontFileFolderMetaData_ = new WrokFrontFileFolderMetaData();
            MVToken = GetMVToken();
        }







        #region WorkfontStart




        [AcceptVerbs("POST")]
        public bool Authentication(TokenClass jsonobject)
        {

            return true;
        }



        [AcceptVerbs("POST")]
        public IHttpActionResult AuthenticationTest(TokenClass jsonobject)
        {

            var s = jsonobject;

            string url = "?state = ";
             System.Uri uri = new System.Uri(url);

            return Redirect(uri);
        }



        [AcceptVerbs("Get")]
        public async Task<string> WorkfrontToken(string Token)
        {
            string OauthToken = await GetAccessToken();
            return OauthToken;

        }

        [AcceptVerbs("Get")]
        public async Task<string> WorkfrontTokenRedirect(string Token)
        {
            string OauthToken = await GetAccessToken();
            return OauthToken;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetMVToken()
        {
            IWebReqRes WebReqRes_ = new WebReqRes();

            #region GetLogin URl

            string URL = "https://mediavaletappsapi-dev.azurewebsites.net/api/mediavalet/getdomaindata?email=philippeadmin@mediavalet.net";

            string ContentType = "application/json; charset=utf-8";//  "application /json";
                                                                   // string Token = WebReqRes_.GetToken(organizationId, signedInUserUniqueName, settingId);
            string Response = WebReqRes_.GetRequest(null, ContentType, URL);
            JavaScriptSerializer JSserializer = new JavaScriptSerializer();
            MediaValetLogin MediaValetLogin_ = JsonConvert.DeserializeObject<MediaValetLogin>(Response);
            MVBaseURL = MediaValetLogin_.ApiUrl;
            #endregion

            #region Get Token 

            var PostBody = @"grant_type=password&password=1234test!&username=philippeadmin@mediavalet.net";
            Response = WebReqRes_.postdata("", MVBaseURL + "/authorization/token", PostBody);
            TokenClass TokenClass_ = JsonConvert.DeserializeObject<TokenClass>(Response);
            #endregion
            return TokenClass_.access_token;


        }


        [AcceptVerbs("Get")]
        public WrokFrontFileFolderMetaData GetWorkfrontMetaData(string id)
        {
           // string WrokFrontFileFolderMetaDataID = "996344cc-21a7-413e-90a9-4ba6df3dd40c";
            string ContentType = "application/json; charset=utf-8";
            string Endpoint = "/folders";
            MVToken=GetMVToken();
            var MetaData = getMetadata(id, MVToken, ContentType, MVBaseURL, ref Endpoint);
            return WrokFrontFileFolderMetaData_;
        }



        [AcceptVerbs("Get")]
        public List<WrokFrontFileFolderMetaData> GetListOfItemInFolder(string parentId)
        {
            // string WrokFrontFileFolderMetaDataID = "996344cc-21a7-413e-90a9-4ba6df3dd40c";
            string ContentType = "application/json; charset=utf-8";
            string Endpoint = "/folders";
            MVToken = GetMVToken();

            List<WrokFrontFileFolderMetaData> listWrokFrontFileFolderMetaData = GetItemMetadataInFolder(parentId, MVToken, ContentType, MVBaseURL, Endpoint);
            if (listWrokFrontFileFolderMetaData != null)
            {

            }
            else
            {
                Endpoint = "/folders/" + parentId + "/subfolders";
                var MetaData = getMetadata(parentId, MVToken, ContentType, MVBaseURL, ref Endpoint);
                listWrokFrontFileFolderMetaData = GetItemMetadataInFolder(parentId, MVToken, ContentType, MVBaseURL, Endpoint);
            }

            return listWrokFrontFileFolderMetaData;
        }


        [AcceptVerbs("Get")]
        public List<WrokFrontFileFolderMetaData> WorkfrontSearch(string Searchtext)
        {
            string ContentType = "application/json; charset=utf-8";
           
            MVToken = GetMVToken();
            List<WrokFrontFileFolderMetaData> listWrokFrontFileFolderMetaData = WorkFrontSearch(Searchtext, MVToken, ContentType,MVBaseURL);
          
                return listWrokFrontFileFolderMetaData;
        }


        [AcceptVerbs("Get")]
        public async Task<byte[]> download(string id )
        {



            // //var client = new HttpClient();
            // //var queryString = HttpUtility.ParseQueryString(string.Empty);

            // //// Request headers
            // //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{subscription key}");

            // var uri = MVBaseURL + "/assets/" + id + "/download?";

            //// var response = await client.GetAsync(uri);
            //// return response;


            string ContentType = "application/json; charset=utf-8";
            string Endpoint = "/folders";
            MVToken = GetMVToken();
            var MetaData = getMetadata(id, MVToken, ContentType, MVBaseURL, ref Endpoint);
            byte[] data;
            using (WebClient client = new WebClient())
            {
                data = client.DownloadData(WrokFrontFileFolderMetaData_.viewLink);
            }
            return data;

            // return ReadImageFile(WrokFrontFileFolderMetaData_.viewLink);
        }


        [AcceptVerbs("Get")]
        public byte[] thumbnail(string id)
        {
            string ContentType = "application/json; charset=utf-8";
            string Endpoint = "/folders";
            MVToken = GetMVToken();
            var MetaData = getMetadata(id, MVToken, ContentType, MVBaseURL, ref Endpoint);
            byte[] data;
            using (WebClient client = new WebClient())
            {
                data = client.DownloadData(WrokFrontFileFolderMetaData_.viewLink);
            }
            return data;
         
        }













        #endregion


        #region Common function 


        #region SearchAsses
        public List<WrokFrontFileFolderMetaData> WorkFrontSearch(string Search, string Token, string ContentType, string URL)
        {
            MVAssestObj MVAssestObj_ = new MVAssestObj();
            MVmapping MVmapping = new MVmapping();
            IWebReqRes WebReqRes_ = new WebReqRes();
            //string SearhURl = MVBaseURL + "/assets?count=25&offset=0&search=girl&sort=title+A";
            string assesturl = MVBaseURL + "/assets?count=25&offset=0&search=" + Search + "&sort=title+A";
           var  Response = WebReqRes_.GetRequest(Token, ContentType,  assesturl);
            if (!string.IsNullOrEmpty(Response))
            {
                MVAssestObj_ = MVmapping.GetMVAssestObj(Response);
            }
            List<WrokFrontFileFolderMetaData> listWrokFrontFileFolderMetaData = MVmapping.GetItemsInFolder(URL, null, MVAssestObj_.payload.assets);
           
            return listWrokFrontFileFolderMetaData;
        }

        #endregion
        /// <summary>
        /// Get All List in In Folder
        /// </summary>
        /// <param name="SearchID"></param>
        /// <param name="Token"></param>
        /// <param name="ContentType"></param>
        /// <param name="URL"></param>
        /// <param name="Endpoint"></param>
        /// <returns></returns>
        public List<WrokFrontFileFolderMetaData> GetItemMetadataInFolder(string SearchID, string Token, string ContentType, string URL, string Endpoint)
        {
            MVAssestObj MVAssestObj_ = new MVAssestObj();
            List<Models.MV.Folder.Payload> ListFolderpayload = new List<Models.MV.Folder.Payload>();
            List<WrokFrontFileFolderMetaData> listWrokFrontFileFolderMetaData = new List<WrokFrontFileFolderMetaData>();
            List<Models.MV.Folder.Payload> FolderPayload_ = new List<Models.MV.Folder.Payload>();
            MVmapping MVmapping = new MVmapping();
            IWebReqRes WebReqRes_ = new WebReqRes();
            var Response = WebReqRes_.GetRequest(Token, ContentType, URL + Endpoint);
            Folder Folder_ = MVmapping.FolderMapping(Response);
            var FolderPayload = Folder_.payload.Where(i => i.id == SearchID).FirstOrDefault();
            if (FolderPayload != null)
            {
                string assesturl = "/categories/{0}/assets";
                assesturl = string.Format(assesturl, SearchID);
                Response = WebReqRes_.GetRequest(Token, ContentType, URL + assesturl);
                if (!string.IsNullOrEmpty(Response))
                {
                    MVAssestObj_ = MVmapping.GetMVAssestObj(Response);
                }
                Endpoint = "/folders/" + SearchID + "/subfolders";
                Response = WebReqRes_.GetRequest(Token, ContentType, URL + Endpoint);
                Folder_ = MVmapping.FolderMapping(Response);
            }
            else
            {
             
                {
                   
                    string assesturl = "/categories/{0}/assets";
                    assesturl = string.Format(assesturl, SearchID);
                    Response = WebReqRes_.GetRequest(Token, ContentType, URL + assesturl);
                    if (!string.IsNullOrEmpty(Response))
                    {
                        MVAssestObj_ = MVmapping.GetMVAssestObj(Response);

                    }
                    Endpoint = "/folders/" + SearchID + "/subfolders";
                    Response = WebReqRes_.GetRequest(Token, ContentType, URL + Endpoint);
                   Folder_ = MVmapping.FolderMapping(Response);
                }

            }

            MVmapping MVmapping_ = new MVmapping();
            listWrokFrontFileFolderMetaData = MVmapping_.GetItemsInFolder(URL, Folder_.payload, MVAssestObj_.payload.assets);

            return listWrokFrontFileFolderMetaData;
        }


        public Mvapi.Models.MV.Folder.Payload getMetadata(string SearchID, string Token, string ContentType, string URL, ref string Endpoint)
        {
            List<Models.MV.Folder.Payload> FolderPayload_ = new List<Models.MV.Folder.Payload>();
            MVmapping MVmapping = new MVmapping();
            // #region GetFoldedr

            IWebReqRes WebReqRes_ = new WebReqRes();
            var Response = WebReqRes_.GetRequest(Token, ContentType, URL + Endpoint);
            Folder Folder_ = MVmapping.FolderMapping(Response);
            var FolderPayload = Folder_.payload.Where(i => i.id == SearchID).FirstOrDefault();
            if (FolderPayload != null)
            {
                //  return FolderPayload;

                FolderPayload_.Add(FolderPayload);
                WrokFrontFileFolderMetaData_ = MVmapping.GetWrokFrontMetaData(URL, FolderPayload_, null)[0];
                return FolderPayload;
            }
            else if (FolderPayload == null)
            {
                var GetAllFolderID = Folder_.payload.Select(i => i.id).ToList();

                if (GetAllFolderID.Count > 0)
                {
                    foreach (string FolderID in GetAllFolderID)
                    {
                        if (!string.IsNullOrEmpty(FolderID))
                        {
                            string assesturl = "/categories/{0}/assets";
                            var Subfolder = Folder_.payload.Where(i => i.id == FolderID).FirstOrDefault();
                            if (Subfolder.subfolderCount > 0)
                            {
                                Endpoint = "/folders/" + FolderID + "/subfolders?count=1000";
                                FolderPayload = getMetadata(SearchID, Token, ContentType, URL, ref Endpoint);
                                if (FolderPayload == null)
                                {
                                    assesturl = string.Format(assesturl, FolderID);
                                    Response = WebReqRes_.GetRequest(Token, ContentType, URL + assesturl);
                                    if (!string.IsNullOrEmpty(Response))
                                    {
                                        MVAssestObj MVAssestObj_ = MVmapping.GetMVAssestObj(Response);
                                        var AssestObj = MVAssestObj_.payload.assets.Where(oAssest => oAssest.id == SearchID).FirstOrDefault();
                                        if (AssestObj != null)
                                        {
                                            WrokFrontFileFolderMetaData_ = MVmapping.GetWrokFrontMetaData(URL, null, AssestObj)[0];
                                            return null;
                                        }
                                    }
                                }
                            }
                            if (Subfolder.subfolderCount == 0)
                            {
                                assesturl = string.Format(assesturl, FolderID);
                                Response = WebReqRes_.GetRequest(Token, ContentType, URL + assesturl);
                                if (!string.IsNullOrEmpty(Response))
                                {
                                    MVAssestObj MVAssestObj_ = MVmapping.GetMVAssestObj(Response);
                                    var AssestObj = MVAssestObj_.payload.assets.Where(oAssest => oAssest.id == SearchID).FirstOrDefault();
                                    if (AssestObj != null)
                                    {
                                        WrokFrontFileFolderMetaData_ = MVmapping.GetWrokFrontMetaData(URL, null, AssestObj)[0];
                                        return null;
                                    }
                                }
                            }



                        }
                    }
                }

            }
            if (FolderPayload != null)
            {
                FolderPayload_.Add(FolderPayload);
                WrokFrontFileFolderMetaData_ = MVmapping.GetWrokFrontMetaData(URL, FolderPayload_, null)[0];
                return FolderPayload;
            }
            return null;
        }

        #endregion





        #region WorkFrontGetToken
        /// <summary>
        /// Get  the OAuth2 Token
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetAccessToken()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Build up the data to POST.
                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                postData.Add(new KeyValuePair<string, string>("client_id", clientId));
                postData.Add(new KeyValuePair<string, string>("client_secret", clientSecret));

                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);
                // Post to the Server and parse the response.
                HttpResponseMessage response = await client.PostAsync("Token", content);
                string jsonString = await response.Content.ReadAsStringAsync();
                object responseData = JsonConvert.DeserializeObject(jsonString);

                // return the Access Token.
                return ((dynamic)responseData).access_token;
            }
        }

        #endregion




        public static byte[] ReadImageFile(string imageLocation)
        {
            byte[] imageData = null;
            FileInfo fileInfo = new FileInfo(imageLocation);
            long imageFileLength = fileInfo.Length;
            FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            imageData = br.ReadBytes((int)imageFileLength);
            return imageData;
        }

        public string ImageToBase64(string imageurl)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(imageurl);
            string decodedString = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            Uri uri = new Uri(decodedString);
            WebClient client = new WebClient();
            byte[] imageBytes = client.DownloadData(uri);
            string base64String = Convert.ToBase64String(imageBytes);
            client.Dispose();
            return "data:image/png;base64," + base64String;
        }





        public void GetLoginObj()
        {

            #region AllAPIList

            //var apiEndpoints = {
            //    authenticate: "/authorization/token",
            //    assets: "/assets",
            //    recentupload: "/recentlyUploaded",
            //    mostviewed: "/mostViewed",
            //    categories: "/categories",
            //    getclientid: "/OrganizationalUnits/current",
            //    getfolders: "/folders",
            //    getuserauthtype: "/users/current",
            //    attributes: "/attributes",
            //    renditionconfig: "/Config/renditionkinds",
            //    renditions: "/renditions"
            //};

            #endregion



            IWebReqRes WebReqRes_ = new WebReqRes();

            #region GetLogin URl

            string URL = "https://mediavaletappsapi-dev.azurewebsites.net/api/mediavalet/getdomaindata?email=philippeadmin@mediavalet.net";

            string ContentType = "application/json; charset=utf-8";//  "application /json";
                                                                   // string Token = WebReqRes_.GetToken(organizationId, signedInUserUniqueName, settingId);
            string Response = WebReqRes_.GetRequest(null, ContentType, URL);
            JavaScriptSerializer JSserializer = new JavaScriptSerializer();
            MediaValetLogin MediaValetLogin_ = JSserializer.Deserialize<MediaValetLogin>(Response);
            URL = MediaValetLogin_.ApiUrl;
            #endregion

            #region Get Token 

            var PostBody = @"grant_type=password&password=1234test!&username=philippeadmin@mediavalet.net";
            Response = WebReqRes_.postdata("", URL + "/authorization/token", PostBody);
            TokenClass TokenClass_ = JSserializer.Deserialize<TokenClass>(Response);
            #endregion


            //URL = URL + "/folders";
            string Endpoint = "/folders";
            var MetaData = getMetadata("996344cc-21a7-413e-90a9-4ba6df3dd40c", TokenClass_.access_token, ContentType, URL, ref Endpoint);

            var s1 = WrokFrontFileFolderMetaData_;
            WrokFrontFileFolderMetaData_ = null;
            Endpoint = "/folders";
            MetaData = getMetadata("f64e2706-37c7-4d85-808e-ab640a5a5944", TokenClass_.access_token, ContentType, URL, ref Endpoint);
            var s = WrokFrontFileFolderMetaData_;
            WrokFrontFileFolderMetaData_ = null;
            //  MetaData = getMetadata("7e2cefb5-8761-40fe-a1ff-e59fd0ef81ec", TokenClass_.access_token, ContentType, URL, ref Endpoint);
            //MetaData = getMetadata("6c16d5a9-642f-4f7f-a6e2-f0ab199fbc98", TokenClass_.access_token, ContentType, URL, ref Endpoint);
            Endpoint = "/folders";
            MetaData = getMetadata("3a961b67-9b28-4010-95de-25630280c10b", TokenClass_.access_token, ContentType, URL, ref Endpoint);
            var s3 = WrokFrontFileFolderMetaData_;
            WrokFrontFileFolderMetaData_ = null;
            MetaData = getMetadata("f64e2706-37c7-4d85-808e-ab640a5a5944", TokenClass_.access_token, ContentType, URL, ref Endpoint);
            Endpoint = "/folders";
            
            MetaData = getMetadata("82b66d1c-67cb-45c2-9909-d77da2785470", TokenClass_.access_token, ContentType, URL, ref Endpoint);
            var s2 = WrokFrontFileFolderMetaData_;
            WrokFrontFileFolderMetaData_ = null;

            //Recursive("111", TokenClass_.access_token, ContentType, ref URL);

            //  return;


            #region DownloadSASUrl
            var sResponse = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL + "/assets/9c16f2e9-6aeb-4b2d-be83-35001d195b9a/download");

            #endregion



            #region GetFoldedr
            //https://api-test.mediavalet.net/folders
            Response = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL);
            Response = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL + "/folders");

            #endregion

            //#region Search
            ////https://api-test.mediavalet.net/assets?count=25&offset=25&search=Love&sort=title+A;
            Response = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL + "/assets?count=25&offset=25&search=Love&sort=title+A");
            //#endregion

            //#region assets
            ////https://api-test.mediavalet.net/assets?count=25&offset=25&search=Love&sort=title+A;
            Response = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL + "/assets");
            //#endregion


            //#region recentlyUploaded
            ////https://api-test.mediavalet.net/recentlyUploaded;
            Response = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL + "/recentlyUploaded");
            //#endregion


            //#region mostViewed
            ////https://api-test.mediavalet.net/mostViewed;
            //Response = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL + "/mostViewed");
            //#endregion


            #region categories
            //https://api-test.mediavalet.net/categories;
            Response = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL + "/categories");
            #endregion



            #region /OrganizationalUnits/current
            //https://api-test.mediavalet.net/assets?count=25&offset=25&search=Love&sort=title+A;
            Response = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL + "/OrganizationalUnits/current");
            #endregion


            #region /folders
            //https://api-test.mediavalet.net/assets?count=25&offset=25&search=Love&sort=title+A;
            Response = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL + "/folders");
            #endregion

            #region /attributes
            //https://api-test.mediavalet.net//users/current;
            Response = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL + "/attributes");
            #endregion

            #region /folders
            //https://api-test.mediavalet.net/assets?count=25&offset=25&search=Love&sort=title+A;
            Response = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL + "/folders");
            #endregion

            #region /attributes
            //https://api-test.mediavalet.net/attributes;
            Response = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL + "/attributes");
            #endregion


            #region /Config/renditionkinds
            //https://api-test.mediavalet.net/assets?count=25&offset=25&search=Love&sort=title+A;
            Response = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL + "/Config/renditionkinds");
            #endregion

            #region //renditions
            //https://api-test.mediavalet.net/attributes;
            Response = WebReqRes_.GetRequest(TokenClass_.access_token, ContentType, URL + "/renditions");
            #endregion



        }


       





        public Mvapi.Models.MV.Folder.Payload getMetadata11(string SearchID, string Token, string ContentType, string URL, ref string Endpoint)
        {
            List<Models.MV.Folder.Payload> FolderPayload_ = new List<Models.MV.Folder.Payload>();
            MVmapping MVmapping = new MVmapping();
            // #region GetFoldedr

            IWebReqRes WebReqRes_ = new WebReqRes();
            var Response = WebReqRes_.GetRequest(Token, ContentType, URL + Endpoint);
            Folder Folder_ = MVmapping.FolderMapping(Response);
            var FolderPayload = Folder_.payload.Where(i => i.id == SearchID).FirstOrDefault();
            if (FolderPayload != null)
            {
                //  return FolderPayload;

                FolderPayload_.Add(FolderPayload);
                WrokFrontFileFolderMetaData_ = MVmapping.GetWrokFrontMetaData(URL, FolderPayload_, null)[0];
                return FolderPayload;
            }
            else if (FolderPayload == null)
            {
                var GetAllFolderID = Folder_.payload.Select(i => i.id).ToList();

                if (GetAllFolderID.Count > 0)
                {
                    foreach (string FolderID in GetAllFolderID)
                    {
                        if (!string.IsNullOrEmpty(FolderID))
                        {
                            string assesturl = "/categories/{0}/assets";
                            var Subfolder = Folder_.payload.Where(i => i.id == FolderID).FirstOrDefault();
                            if (Subfolder.subfolderCount > 0)
                            {
                                Endpoint = "/folders/" + FolderID + "/subfolders?count=1000";
                                FolderPayload = getMetadata(SearchID, Token, ContentType, URL, ref Endpoint);
                                if (FolderPayload == null)
                                {
                                    assesturl = string.Format(assesturl, FolderID);
                                    Response = WebReqRes_.GetRequest(Token, ContentType, URL + assesturl);
                                    if (!string.IsNullOrEmpty(Response))
                                    {
                                        MVAssestObj MVAssestObj_ = MVmapping.GetMVAssestObj(Response);
                                        var AssestObj = MVAssestObj_.payload.assets.Where(oAssest => oAssest.id == SearchID).FirstOrDefault();
                                        if (AssestObj != null)
                                        {
                                            WrokFrontFileFolderMetaData_ = MVmapping.GetWrokFrontMetaData(URL, null, AssestObj)[0];
                                            return null;
                                        }
                                    }
                                }
                            }
                            if (Subfolder.subfolderCount == 0)
                            {
                                assesturl = string.Format(assesturl, FolderID);
                                Response = WebReqRes_.GetRequest(Token, ContentType, URL + assesturl);
                                if (!string.IsNullOrEmpty(Response))
                                {
                                    MVAssestObj MVAssestObj_ = MVmapping.GetMVAssestObj(Response);
                                    var AssestObj = MVAssestObj_.payload.assets.Where(oAssest => oAssest.id == SearchID).FirstOrDefault();
                                    if (AssestObj != null)
                                    {
                                        WrokFrontFileFolderMetaData_ = MVmapping.GetWrokFrontMetaData(URL, null, AssestObj)[0];
                                        return null;
                                    }
                                }
                            }



                        }
                    }
                }

            }
            if (FolderPayload != null)
            {
                FolderPayload_.Add(FolderPayload);
                WrokFrontFileFolderMetaData_ = MVmapping.GetWrokFrontMetaData(URL, FolderPayload_, null)[0];
                return FolderPayload;
            }
            return null;
        }

        //public Mvapi.Models.MV.Folder.Payload getMetadata3(string ID, string Token, string ContentType, string URL, ref string Endpoint)
        //{
        //    List<Models.MV.Folder.Payload> FolderPayload_ = new List<Models.MV.Folder.Payload>();
        //    MVmapping MVmapping = new MVmapping();
        //    // #region GetFoldedr

        //    IWebReqRes WebReqRes_ = new WebReqRes();
        //    var Response = WebReqRes_.GetRequest(Token, ContentType, URL + Endpoint);
        //    Folder Folder_ = MVmapping.FolderMapping(Response);
        //    var FolderPayload = Folder_.payload.Where(i => i.id == ID).FirstOrDefault();
        //    if (FolderPayload != null)
        //    {
        //        return FolderPayload;
        //    }
        //    else if (FolderPayload == null)
        //    {
        //        var GetAllFolderID = Folder_.payload.Select(i => i.id).ToList();

        //        if (GetAllFolderID.Count > 0)
        //        {
        //            foreach (string FolderID in GetAllFolderID)
        //            {
        //                if (!string.IsNullOrEmpty(FolderID))
        //                {
        //                    string assesturl = "/categories/{0}/assets";
        //                    var Subfolder = Folder_.payload.Where(i => i.id == FolderID).FirstOrDefault();
        //                    if (Subfolder.subfolderCount > 0)
        //                    {
        //                        Endpoint = "/folders/" + FolderID + "/subfolders?count=1000";
        //                        FolderPayload = getMetadata(ID, Token, ContentType, URL, ref Endpoint);
        //                        if (FolderPayload == null)
        //                        {
        //                            assesturl = string.Format(assesturl, FolderID);
        //                            Response = WebReqRes_.GetRequest(Token, ContentType, URL + assesturl);
        //                            if (!string.IsNullOrEmpty(Response))
        //                            {
        //                                MVAssestObj MVAssestObj_ = MVmapping.GetMVAssestObj(Response);
        //                                var AssestObj = MVAssestObj_.payload.assets.Where(oAssest => oAssest.id == FolderID).FirstOrDefault();
        //                                if (AssestObj != null)
        //                                {
        //                                    var Assestobj = MVmapping.GetWrokFrontMetaData(URL, null, AssestObj);
        //                                    return null;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    if (Subfolder.subfolderCount == 0)
        //                    {
        //                        assesturl = string.Format(assesturl, FolderID);
        //                        Response = WebReqRes_.GetRequest(Token, ContentType, URL + assesturl);
        //                        if (!string.IsNullOrEmpty(Response))
        //                        {
        //                            MVAssestObj MVAssestObj_ = MVmapping.GetMVAssestObj(Response);
        //                            var AssestObj = MVAssestObj_.payload.assets.Where(oAssest => oAssest.id == FolderID).FirstOrDefault();
        //                            if (AssestObj != null)
        //                            {
        //                                var Assestobj = MVmapping.GetWrokFrontMetaData(URL, null, AssestObj);
        //                                return null;
        //                            }
        //                        }
        //                    }



        //                }
        //            }
        //        }

        //    }
        //    FolderPayload_.Add(FolderPayload);
        //    var Assest = MVmapping.GetWrokFrontMetaData(URL, FolderPayload_, null);
        //    return FolderPayload;
        //}





        //public void getMetadata2(string ID, string Token, string ContentType, string URL, string Endpoint)
        //{
        //    MVmapping MVmapping = new MVmapping();
        //    // #region GetFoldedr

        //    IWebReqRes WebReqRes_ = new WebReqRes();
        //    var Response = WebReqRes_.GetRequest(Token, ContentType, URL + "/folders");
        //    Folder Folder_ = MVmapping.FolderMapping(Response);
        //    var Folder = Folder_.payload.Where(i => i.id == ID).FirstOrDefault();
        //    if (Folder == null)
        //    {
        //        var GetAllFolderID = Folder_.payload.Select(i => i.id).ToList();

        //        foreach (string id in GetAllFolderID)
        //        {
        //            if (!string.IsNullOrEmpty(id))
        //            {
        //                Response = WebReqRes_.GetRequest(Token, ContentType, URL + "/folders/" + id + "/subfolders?count=1000");
        //                Folder_ = MVmapping.FolderMapping(Response);


        //            }
        //        }

        //    }
        //}


        //    #endregion
        //}









      


    }
}



