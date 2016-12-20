using Mvapi.DataAccessLayer;
using Mvapi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;
//using System.Web.Mvc;

namespace Mvapi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MediaValetController : ApiController
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(MediaValetController));
        //initialize storage account
        public MediaValetController()
        {
            DataAccess.InitializeStorageAccount();
        }

        //get categories data
        public CategoriesModel GetCategories()
        {
            string orgunitid = "";
            string application = "";
            string email = "";
            var param = Request.GetQueryNameValuePairs();

            if (param != null)
            {
                IDictionary<string, string> qs = param.ToDictionary(k => k.Key.ToLower(), v => v.Value.ToLower());
                if (qs.Count == 3) //3 //2
                {
                    orgunitid = qs["orgunitid"];
                    application = qs["application"];
                    email = qs["email"];
                }
            }

            CategoriesModel categoriesModel = new CategoriesModel();
            try
            {
                if (!string.IsNullOrEmpty(orgunitid))
                {
                    if (!string.IsNullOrEmpty(application))
                    {
                        var data = DataAccess.GetCategoryData(orgunitid, application, email);

                        if (data != null)
                        {
                            categoriesModel.Domain = data.DomainName;
                            categoriesModel.Email = data.Email;
                            categoriesModel.Categories = data.Categories;
                            categoriesModel.OrgUnitId = data.OrgUnitId;
                            categoriesModel.Application = data.Application.ToLower();
                            return categoriesModel;
                        }
                        categoriesModel.Domain = data.ToString();
                        categoriesModel.Email = "";
                        categoriesModel.Categories = "";
                        categoriesModel.OrgUnitId = "";
                        categoriesModel.Application = "";
                        return categoriesModel;
                    }
                    categoriesModel.Application = "Application name missing";
                    return categoriesModel;
                }
                categoriesModel.OrgUnitId = "Please provide Organization Id.";
                return categoriesModel;
            }
            catch
            {
                categoriesModel.OrgUnitId = "Not Found Data";
                return categoriesModel;
            }
        }

        //update categories data
        [AcceptVerbs("Get", "Post")]
        public string AddNewCategories()
        {
            string domain = "";
            string email = "";
            string orgunitid = "";
            string categories = "";
            string application = "";
            var param = Request.GetQueryNameValuePairs();

            if (param != null)
            {
                IDictionary<string, string> qs = param.ToDictionary(k => k.Key.ToLower(), v => v.Value.ToLower());
                if (qs.Count == 5) //4 //3
                {
                    domain = qs["domain"];
                    email = qs["email"];
                    orgunitid = qs["orgunitid"];
                    categories = qs["categories"];
                    application = qs["application"];
                }
                else
                {
                    return "Parameters missing.";
                }
            }
            CategoriesModel categoriesModel = new CategoriesModel();
            if (!string.IsNullOrEmpty(domain))
            {
                if (!string.IsNullOrEmpty(email))
                {
                    if (!string.IsNullOrEmpty(orgunitid))
                    {
                        if (!string.IsNullOrEmpty(application))
                        {
                            categoriesModel.Domain = domain;
                            categoriesModel.Email = email;
                            categoriesModel.OrgUnitId = orgunitid;
                            categoriesModel.Categories = categories;
                            categoriesModel.Application = application.ToLower();
                            string result = DataAccess.InsertNewCategory(categoriesModel);
                            return result;
                        }
                        return "Application name can't be empty.";
                    }
                    return "Organization id can't be empty.";
                }
                return "Empty email.";
            }
            return "Domain name not found.";
        }

        //update categories
        [AcceptVerbs("Get", "Put")]
        public string UpdateExistingCategories()
        {
            string email = "";
            string orgunitid = "";
            string categories = "";
            string application = "";
            var param = Request.GetQueryNameValuePairs();

            if (param != null)
            {
                IDictionary<string, string> qs = param.ToDictionary(k => k.Key.ToLower(), v => v.Value);

                if (qs.Count == 4)    

                {
                    email = qs["email"];
                    orgunitid = qs["orgunitid"];
                    categories = qs["categories"];
                    application = qs["application"];
                }
                else
                {
                    return "Parameters missing.";
                }
            }
            CategoriesModel categoriesModel = new CategoriesModel();

            if (!string.IsNullOrEmpty(orgunitid)) 

            {
                if (!string.IsNullOrEmpty(email))
                {
                    if (!string.IsNullOrEmpty(application))
                    {
                        categoriesModel.Email = email;
                        categoriesModel.OrgUnitId = orgunitid;
                        categoriesModel.Categories = categories;
                        categoriesModel.Application = application.ToLower();
                        string result = DataAccess.RefreshExistingCategory(categoriesModel);
                        return result;
                    }
                    return "Application name can't be empty.";
                }
                return "Empty email.";
            }
            return "Organization id can't be empty.";
        }


        [AcceptVerbs("Get", "Post")]
        public DomainDataModel GetDomainData()
        {
            logger.Info("Get Domain Called");
            string emailDomain = "";
            //var data=String.Empty;
            var param = Request.GetQueryNameValuePairs();
            DomainDataModel domainModel = new DomainDataModel();
            try
            {
                if (param != null)
                {
                    IDictionary<string, string> qs = param.ToDictionary(k => k.Key.ToLower(), v => v.Value.ToLower());

                    if (qs.Count >= 1)
                    {
                        var getdomain = qs["email"];
                        var appsname = qs["appsname"];

                        var index = qs["email"].IndexOf('@');
                        char character = (char)92;
                        var index2 = qs["email"].IndexOf(character);
                        if (index >= 0 && index2 < 0)
                        {

                            var data = DataAccess.GetDomainData(getdomain, "email", appsname);
                            if (data == null)
                            {
                                var domain = getdomain.Split('@');
                                data = DataAccess.GetDomainData(domain[1], "domain", appsname);

                                if (data != null)
                                {
                                    domainModel.DomainName = data.DomainName;
                                    domainModel.ApiUrl = data.ApiUrl;
                                    domainModel.EmailDomain = data.EmailDomain;
                                    return domainModel;
                                }
                                else
                                {
                                    domainModel.ApiUrl = "Domain name could not be resolved";
                                }

                                return domainModel;
                            }
                            else
                            {
                                domainModel.DomainName = data.DomainName;
                                domainModel.ApiUrl = data.ApiUrl;
                                domainModel.EmailDomain = data.EmailDomain;
                                return domainModel;

                            }
                        }

                        else if (index2 >= 0 && index >= 0)
                        {
                            var domainsplit = qs["email"].Split(character);
                            var data = DataAccess.GetDomainData(domainsplit[0], "domain", appsname);

                            var ind = 0;
                            if (domainsplit[1] != null)
                            {
                                ind = domainsplit[1].IndexOf('@');
                            }
                            if (data == null && ind >= 0)
                            {

                                data = DataAccess.GetDomainData(domainsplit[1], "email", appsname);
                                if (data == null)
                                {
                                    var domainnamesplit = domainsplit[1].Split('@');
                                    data = DataAccess.GetDomainData(domainnamesplit[1], "domain", appsname);

                                    if (data != null)
                                    {
                                        domainModel.DomainName = data.DomainName;
                                        domainModel.ApiUrl = data.ApiUrl;
                                        domainModel.EmailDomain = data.EmailDomain;
                                        return domainModel;
                                    }
                                    else
                                    {
                                        domainModel.ApiUrl = "Domain name could not be resolved";
                                        return domainModel;
                                    }
                                }
                                else
                                {
                                    domainModel.DomainName = data.DomainName;
                                    domainModel.ApiUrl = data.ApiUrl;
                                    domainModel.EmailDomain = data.EmailDomain;
                                    return domainModel;
                                }
                            }
                            else
                            {
                                domainModel.DomainName = data.DomainName;
                                domainModel.ApiUrl = data.ApiUrl;
                                domainModel.EmailDomain = data.EmailDomain;
                                return domainModel;
                            }

                        } else if (index < 0 && index2 < 0)
                        {
                            var data = DataAccess.GetDomainData(qs["email"], "email", appsname);

                            if (data != null)
                            {
                                domainModel.DomainName = data.DomainName;
                                domainModel.ApiUrl = data.ApiUrl;
                                domainModel.EmailDomain = data.EmailDomain;
                                return domainModel;
                            }
                            else
                            {
                                domainModel.ApiUrl = "Domain name could not be resolved";
                                return domainModel;
                            }

                        } else if (index2 > 0)
                        {
                            var splits = qs["email"].Split(character);
                            var data = DataAccess.GetDomainData(splits[0], "domain", appsname);

                            if (data != null)
                            {
                                domainModel.DomainName = data.DomainName;
                                domainModel.ApiUrl = data.ApiUrl;
                                domainModel.EmailDomain = data.EmailDomain;
                                return domainModel;
                            }
                            else
                            {

                                data = DataAccess.GetDomainData(splits[1], "email", appsname);

                                if (data != null)
                                {
                                    domainModel.DomainName = data.DomainName;
                                    domainModel.ApiUrl = data.ApiUrl;
                                    domainModel.EmailDomain = data.EmailDomain;
                                    return domainModel;
                                }
                                else
                                {
                                    domainModel.ApiUrl = "Domain name could not be resolved";
                                    return domainModel;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                domainModel.ApiUrl = "Empty/Invalid domain name";
                return domainModel;
            }
            domainModel.ApiUrl = "Empty/Invalid domain name";
            return domainModel;
        }

        //set domain data
        [AcceptVerbs("Get", "Post")]
        public DomainDataModel SaveDomainData()
        {
            DomainDataModel domainModel = new DomainDataModel();

            try {
                string domain = "";
                string apiUrl = "";
                string email = "";
                string orgunitid = "",application="";
                var param = Request.GetQueryNameValuePairs();
                if (param != null)
                {
                    IDictionary<string, string> qs = param.ToDictionary(k => k.Key.ToLower(), v => v.Value.ToLower());
                    if (qs.Count >= 3)
                    {

                        domain = qs["domain"]; 
                        apiUrl = qs["apiurl"];
                        email = qs["email"];
                        orgunitid = qs["orgunitid"];
                        application=qs["application"];

                    }
                }



                if (!string.IsNullOrEmpty(domain) && domain.Contains("."))
                {
                    if (!string.IsNullOrEmpty(email) && email.Contains("@") && email.Contains("."))
                    {
                        if (!string.IsNullOrEmpty(apiUrl) && apiUrl.Contains("https://") && apiUrl.Contains("."))
                        {
                            string result = DataAccess.SaveDomainData(domain, apiUrl, email, orgunitid, application);
                            domainModel.ApiUrl = result;
                            return domainModel;
                        }
                        domainModel.ApiUrl = "Empty/Invalid ApiUrl.";
                        return domainModel;
                    }

                    domainModel.EmailDomain = "Empty/Invalid email.";
                    return domainModel;
                }

                domainModel.ApiUrl = "Empty/Invalid domain name";
                return domainModel;
            }
            catch (Exception) {
                domainModel.ApiUrl = "Empty/Invalid domain name";
                return domainModel;
            }

        }
        [AcceptVerbs("Get", "Post")]
        public void SavetrackingData()
        {
            string OrgUnitId;
            string AssetId;
            string Username;
            string TimeStamp;
            string Events;
            string Application;
            string Assetname;
            
            var param = Request.GetQueryNameValuePairs();
            if (param != null)
            {
                IDictionary<string, string> qs = param.ToDictionary(k => k.Key.ToLower(), v => v.Value.ToLower());
                if (qs.Count == 6)
                {
                    try
                    {
                        OrgUnitId = qs["orgunitid"];
                        AssetId = qs["assetid"];
                        Username = qs["username"];
                        Events = qs["events"];
                        Application = qs["application"];
                        Assetname = qs["assetname"];
                        TrackingDataAcessLayer.SaveTrackingData(Guid.NewGuid().ToString(), AssetId, Username, OrgUnitId, Application, Events, Assetname);
                    }
                    catch (Exception ex)
                    {
                        // return ex.Message.ToString();
                    }

                }
            }
            // return "";
        }
        [AcceptVerbs("Get", "Post")]
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
        [AcceptVerbs("Get", "Post")]
        public string AddMetaData(string domainname, string email, string orgunitid, string metalist, string application)
        {
            string orgid;
            string domain;
            string mail;
            string metadatalist;
            string apps;

            var param = Request.GetQueryNameValuePairs();
            if (param != null)
            {
                IDictionary<string, string> qs = param.ToDictionary(k => k.Key.ToLower(), v => v.Value.ToLower());
                if (qs.Count == 5)
                {
                    orgid = qs["orgunitid"];
                    domain = qs["domainname"];
                    mail = qs["email"];
                    metadatalist = qs["metalist"];
                    apps = qs["application"];
                    MetaDataModel meta = new MetaDataModel();
                    meta.Email = mail;
                    meta.DomainName = domain;
                    meta.OrgUnitId = orgid;
                    meta.MetaDataList = metadatalist;
                    meta.Application = application;
                    var data = DataAccess.AddMetaData(meta);

                }
            }
            return "success";
        }

        [AcceptVerbs("Get", "Post")]
        public string UpdateMetaData()
        {
            string partitionid;
            string metadatalist;
            var param = Request.GetQueryNameValuePairs();
            try
            {
                if (param != null)
                {
                    IDictionary<string, string> qs = param.ToDictionary(k => k.Key.ToLower(), v => v.Value.ToLower());
                    if (qs.Count == 2)
                    {
                        metadatalist = qs["metadatlist"];
                        partitionid = qs["partitionid"];
                        var data = DataAccess.UpdateMetaData(partitionid, metadatalist);

                    }
                }
                return "success";

            } catch

            {
                return "fail";
            }
        }

        [AcceptVerbs("Get", "Post")]
        public MetaDataModel GetMetaData(string domainname, string email, string orgunitid, string application)

        {

            /*Get MetaData*/
            string orgid;
            string domain;
            string mail;
            string apps;
            MetaDataModel mv = new MetaDataModel();// = null;
            var param = Request.GetQueryNameValuePairs();

            try
            {
                if (param != null)
                {
                    IDictionary<string, string> qs = param.ToDictionary(k => k.Key.ToLower(), v => v.Value.ToLower());
                    if (qs.Count == 4)
                    {
                        orgid = qs["orgunitid"];
                        domain = qs["domainname"];
                        mail = qs["email"];
                        apps = qs["application"];
                        var data = DataAccess.GetMetaData(mail, domain, orgid, apps);
                        if (data != null)
                        {

                            mv.DomainName = data.DomainName;
                            mv.Email = data.Email;
                            mv.OrgUnitId = data.OrgUnitId;
                            mv.Application = data.Application;
                            mv.MetaDataList = data.MetaDataList;
                            mv.partitionkey = data.PartitionKey;
                        }
                    }
                }
                return mv;
            }
            catch
            {
                return mv;

            }
        }
        [AcceptVerbs("Get")]
        public string CreateAccount(string companyname, string companyusername, string companyemail, string companyphonenumber, string appname, string ipaddress)
        {
            try
            {
                MailMessage objeto_mail = new MailMessage();
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.sendgrid.net";
                objeto_mail.IsBodyHtml = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("mvo365", "@mvo365!123");
                objeto_mail.From = new MailAddress("365registration@mediavalet.com");
                objeto_mail.To.Add(new MailAddress("365registration@mediavalet.com"));
                var strdate = DateTime.Now.ToString("dd-MM-yyyy");
                objeto_mail.Subject = "New registration from Office 365";
                string mediavaletmailbody = "<div style=\"color: rgb(34, 34, 34);font-family:arial,sans-serif;font-size:12.8px;line-height: normal; \">Hi,</div> <div style=\"color:rgb(34, 34, 34);font-family: arial,sans-serif;font-size:12.8px;line-height: normal;\" > &nbsp;</div><div style=\"color: rgb(34, 34, 34); font-family: arial,sans-serif; font-size:12.8px; line-height: normal;\" > Please process my request for access to MediaValet Library.My details are given below:</div><div style=\"color: rgb(34, 34, 34);font-family: arial, sans-serif; font-size: 12.8px; line-height: normal;\"> &nbsp;</div>  <div style=\"color: rgb(34, 34, 34); font-family: arial, sans-serif; font-size: 12.8px; line-height: normal;\" ><ol> <li style=\"margin-left: 15px;\">Date: " + strdate + " &nbsp;&nbsp; &nbsp; &nbsp; &nbsp;</li><li style=\"margin-left: 15px;\"> Source IP: " + ipaddress + " </li><li style=\"margin-left: 15px;\"> App Name: " + appname + " </li><li style=\"margin-left: 15px;\"> User Name: " + companyusername + " </li><li style=\"margin-left: 15px;\"> Organization Name: " + companyname + "</li> <li style =\"margin-left: 15px;\"> Organization &nbsp; Email: &nbsp; " + companyemail + " </li> <li style=\"margin-left: 15px;\"> Organization &nbsp; Phone Number: " + companyphonenumber + " </li> </ol><p style=\"margin-left:15px;\">&nbsp;</p></div>";
                objeto_mail.Body = mediavaletmailbody;
                client.Send(objeto_mail);
                return "sent";
            } catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
      
        [AcceptVerbs("Get", "Post")]
        public string UploadFileFinalPatchRequest()
        {
            var param = Request.GetQueryNameValuePairs();
            IDictionary<string, string> qs = param.ToDictionary(k => k.Key.ToLower(), v => v.Value.ToLower());
            string baseaddress = qs["baseaddress"];
            string base64encodetoken = qs["base64encodetoken"];
            string uploadid = qs["uploadid"];
            string postdata = "[{\r \"op\":\"replace\",\"path\":\"/status\",\"value\":1}]";
            postdata = "[{\"op\":\"replace\",\"path\":\" / status\",\"value\":1}]";
            try
            {
                var http = (HttpWebRequest)WebRequest.Create(new Uri(baseaddress));
                http.Method = "PATCH";
                http.Headers.Add("Authorization", base64encodetoken);
                http.ContentType = "application/json";
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(postdata);

                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                var response = http.GetResponse();

                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();

                var elouqaresponse = (JsonConvert.DeserializeObject(content)).ToString();
                return elouqaresponse;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        [HttpPost]
        public KeyValuePair<bool, string> UploadFile()
        {
            try
            {
                if (HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var httpPostedFile = HttpContext.Current.Request.Files["uploadedfile"];
                    var filename = httpPostedFile.FileName;
                    var filesize = httpPostedFile.ContentLength;
                    if (httpPostedFile != null)
                    {
                        var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadedFiles"), httpPostedFile.FileName);
                        httpPostedFile.SaveAs(fileSavePath);
                        return new KeyValuePair<bool, string>(true, "File uploaded successfully.");
                    }
                    return new KeyValuePair<bool, string>(true, "Could not get the uploaded file.");
                }

                return new KeyValuePair<bool, string>(true, "No file found to upload.");
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, "An error occurred while uploading the file. Error Message: " + ex.Message);
            }
        }

        [AcceptVerbs("Get", "Post")]
        public string FileUplaodMediaValet(string sasurls, string filename)
        {
            var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadedFiles"), filename);
            var base64EncodedBytes = System.Convert.FromBase64String(sasurls);
            string decodedStringurl = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            byte[] fileToSends = File.ReadAllBytes(@fileSavePath);
            var http = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(new Uri(decodedStringurl));
            http.ContentType = "application/json";
            http.Method = "PUT";
            http.Headers.Add("x-ms-blob-type", "BlockBlob");
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            try
            {
                Stream newStream = http.GetRequestStream();
                newStream.Write(fileToSends, 0, fileToSends.Length);
                newStream.Close();
                var response = http.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                string content = sr.ReadToEnd();
                return "done";
            }
            catch (Exception)
            {
                return "fail";
            }
        }


        [AcceptVerbs("Get", "Post")]
        public DomainDataModel GetDomainData(string email, string appsname = null)
        {
            logger.Info("Get Domain Called");
            string emailDomain = "";
            DomainDataModel domainModel = new DomainDataModel();
            try
            {
                if (email != null)
                {

                    if (email != null && appsname == null)
                    {
                        var getdomain = email;
                        var index = email.IndexOf('@');

                        char character = (char)92;
                        var index2 = email.IndexOf(character);
                        if (index >= 0 && index2 < 0)
                        {

                            var data = DataAccess.GetDomainData(getdomain, "email", appsname);
                            if (data == null)
                            {
                                var domain = getdomain.Split('@');
                                data = DataAccess.GetDomainData(domain[1], "domain", appsname);

                                if (data != null)
                                {
                                    domainModel.DomainName = data.DomainName;
                                    domainModel.ApiUrl = data.ApiUrl;
                                    domainModel.EmailDomain = data.EmailDomain;
                                    return domainModel;
                                }
                                else
                                {
                                    domainModel.ApiUrl = "Domain name could not be resolved";
                                }

                                return domainModel;
                            }
                            else
                            {
                                domainModel.DomainName = data.DomainName;
                                domainModel.ApiUrl = data.ApiUrl;
                                domainModel.EmailDomain = data.EmailDomain;
                                return domainModel;
                            }
                        }

                        else if (index2 >= 0 && index >= 0)
                        {

                            var domainsplit = email.Split(character);
                            var data = DataAccess.GetDomainData(domainsplit[0], "domain", appsname);

                            var ind = 0;
                            if (domainsplit[1] != null)
                            {
                                ind = domainsplit[1].IndexOf('@');
                            }
                            if (data == null && ind >= 0)
                            {

                                data = DataAccess.GetDomainData(domainsplit[1], "email", appsname);
                                if (data == null)
                                {
                                    var domainnamesplit = domainsplit[1].Split('@');
                                    data = DataAccess.GetDomainData(domainnamesplit[1], "domain", appsname);

                                    if (data != null)
                                    {
                                        domainModel.DomainName = data.DomainName;
                                        domainModel.ApiUrl = data.ApiUrl;
                                        domainModel.EmailDomain = data.EmailDomain;
                                        return domainModel;
                                    }
                                    else
                                    {
                                        domainModel.ApiUrl = "Domain name could not be resolved";
                                        return domainModel;
                                    }
                                }
                                else
                                {
                                    domainModel.DomainName = data.DomainName;
                                    domainModel.ApiUrl = data.ApiUrl;
                                    domainModel.EmailDomain = data.EmailDomain;
                                    return domainModel;
                                }
                            }
                            else
                            {
                                domainModel.DomainName = data.DomainName;
                                domainModel.ApiUrl = data.ApiUrl;
                                domainModel.EmailDomain = data.EmailDomain;
                                return domainModel;
                            }

                        }
                        else if (index < 0 && index2 < 0)
                        {

                            var data = DataAccess.GetDomainData(email, "email", appsname);

                            if (data != null)
                            {
                                domainModel.DomainName = data.DomainName;
                                domainModel.ApiUrl = data.ApiUrl;
                                domainModel.EmailDomain = data.EmailDomain;
                                return domainModel;
                            }
                            else
                            {
                                domainModel.ApiUrl = "Domain name could not be resolved";
                                return domainModel;
                            }

                        }
                        else if (index2 > 0)
                        {

                            var splits = email.Split(character);
                            var data = DataAccess.GetDomainData(splits[0], "domain", appsname);

                            if (data != null)
                            {
                                domainModel.DomainName = data.DomainName;
                                domainModel.ApiUrl = data.ApiUrl;
                                domainModel.EmailDomain = data.EmailDomain;
                                return domainModel;
                            }
                            else
                            {

                                data = DataAccess.GetDomainData(splits[1], "email", appsname);

                                if (data != null)
                                {
                                    domainModel.DomainName = data.DomainName;
                                    domainModel.ApiUrl = data.ApiUrl;
                                    domainModel.EmailDomain = data.EmailDomain;
                                    return domainModel;
                                }
                                else
                                {
                                    domainModel.ApiUrl = "Domain name could not be resolved";
                                    return domainModel;
                                }
                            }
                        }
                    }
                    else if (email != null && appsname != null)
                    {
                        //start
                        var index = email.IndexOf('@');
                        char character = (char)92;
                        var index2 = email.IndexOf(character);
                        if (index >= 0 && index2 < 0)
                        {

                            var data = DataAccess.GetDomainData(email, "email", appsname);
                            if (data == null)
                            {
                                var domain = email.Split('@');
                                data = DataAccess.GetDomainData(domain[1], "domain", appsname);

                                if (data != null)
                                {
                                    domainModel.DomainName = data.DomainName;
                                    domainModel.ApiUrl = data.ApiUrl;
                                    domainModel.EmailDomain = data.EmailDomain;
                                    return domainModel;
                                }
                                else
                                {
                                    domainModel.ApiUrl = "Domain name could not be resolved";
                                }

                                return domainModel;
                            }
                            else
                            {
                                domainModel.DomainName = data.DomainName;
                                domainModel.ApiUrl = data.ApiUrl;
                                domainModel.EmailDomain = data.EmailDomain;
                                return domainModel;

                            }
                        }

                        else if (index2 >= 0 && index >= 0)
                        {
                            var domainsplit = email.Split(character);
                            var data = DataAccess.GetDomainData(domainsplit[0], "domain", appsname);

                            var ind = 0;
                            if (domainsplit[1] != null)
                            {
                                ind = domainsplit[1].IndexOf('@');
                            }
                            if (data == null && ind >= 0)
                            {

                                data = DataAccess.GetDomainData(domainsplit[1], "email", appsname);
                                if (data == null)
                                {
                                    var domainnamesplit = domainsplit[1].Split('@');
                                    data = DataAccess.GetDomainData(domainnamesplit[1], "domain", appsname);

                                    if (data != null)
                                    {
                                        domainModel.DomainName = data.DomainName;
                                        domainModel.ApiUrl = data.ApiUrl;
                                        domainModel.EmailDomain = data.EmailDomain;
                                        return domainModel;
                                    }
                                    else
                                    {
                                        domainModel.ApiUrl = "Domain name could not be resolved";
                                        return domainModel;
                                    }
                                }
                                else
                                {
                                    domainModel.DomainName = data.DomainName;
                                    domainModel.ApiUrl = data.ApiUrl;
                                    domainModel.EmailDomain = data.EmailDomain;
                                    return domainModel;
                                }
                            }
                            else
                            {
                                domainModel.DomainName = data.DomainName;
                                domainModel.ApiUrl = data.ApiUrl;
                                domainModel.EmailDomain = data.EmailDomain;
                                return domainModel;
                            }

                        }
                        else if (index < 0 && index2 < 0)
                        {
                            var data = DataAccess.GetDomainData(email, "email", appsname);

                            if (data != null)
                            {
                                domainModel.DomainName = data.DomainName;
                                domainModel.ApiUrl = data.ApiUrl;
                                domainModel.EmailDomain = data.EmailDomain;
                                return domainModel;
                            }
                            else
                            {
                                domainModel.ApiUrl = "Domain name could not be resolved";
                                return domainModel;
                            }

                        }
                        else if (index2 > 0)
                        {
                            var splits = email.Split(character);
                            var data = DataAccess.GetDomainData(splits[0], "domain", appsname);

                            if (data != null)
                            {
                                domainModel.DomainName = data.DomainName;
                                domainModel.ApiUrl = data.ApiUrl;
                                domainModel.EmailDomain = data.EmailDomain;
                                return domainModel;
                            }
                            else
                            {

                                data = DataAccess.GetDomainData(splits[1], "email", appsname);

                                if (data != null)
                                {
                                    domainModel.DomainName = data.DomainName;
                                    domainModel.ApiUrl = data.ApiUrl;
                                    domainModel.EmailDomain = data.EmailDomain;
                                    return domainModel;
                                }
                                else
                                {
                                    domainModel.ApiUrl = "Domain name could not be resolved";
                                    return domainModel;
                                }
                            }
                        }

                        //end
                    }
                }
            }
            catch (Exception)
            {
                domainModel.ApiUrl = "Empty/Invalid domain name";
                return domainModel;
            }
            domainModel.ApiUrl = "Empty/Invalid domain name";
            return domainModel;
        }

    }
}

/* The */