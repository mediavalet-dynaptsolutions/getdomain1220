using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Mvapi.Models;
using System.Configuration;

namespace Mvapi.DataAccessLayer
{
    public class DataAccess
    {
        static CloudStorageAccount account;

        public static void InitializeStorageAccount()
        {
            var storageAccount = ConfigurationManager.AppSettings["StorageAccountName"];
            var key = ConfigurationManager.AppSettings["StorageAccountKey"];
            account = new CloudStorageAccount(new StorageCredentials(storageAccount, key), true);
            CloudTableClient tableClient = account.CreateCloudTableClient();
            var tableName = ConfigurationManager.AppSettings["CategoryTableName"];

            CloudTable table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExistsAsync();
        }

        public static MediaValetCategoryDataEntity GetCategoryData(string orgunitid, string application,string email) 
        {
            CloudTableClient tableClient = account.CreateCloudTableClient();
            var tableName = ConfigurationManager.AppSettings["CategoryTableName"];
            CloudTable table = tableClient.GetTableReference(tableName);
            var tableQuery = new TableQuery<MediaValetCategoryDataEntity>();

            var query = from mvdata in table.CreateQuery<MediaValetCategoryDataEntity>()
                        where  mvdata.Email == email && mvdata.OrgUnitId == orgunitid && mvdata.Application == application        select mvdata;
            return query.FirstOrDefault();

        }
        public static string InsertNewCategory(CategoriesModel categoriesModel)
        {
            try
            {
                CloudTableClient tableClient = account.CreateCloudTableClient();
                var tableName = ConfigurationManager.AppSettings["CategoryTableName"];

                CloudTable table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExistsAsync();

                TableBatchOperation batchOperation = new TableBatchOperation();

                var tableQuery = new TableQuery<MediaValetCategoryDataEntity>();
                string domainFilter = TableQuery.GenerateFilterCondition("DomainName", QueryComparisons.Equal, categoriesModel.Domain);
                string emailFilter = TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, categoriesModel.Email);
                string orgidFilter = TableQuery.GenerateFilterCondition("OrgUnitId", QueryComparisons.Equal, categoriesModel.OrgUnitId);
                string appFilter = TableQuery.GenerateFilterCondition("Application", QueryComparisons.Equal, categoriesModel.Application);

                string domainemailFilter = TableQuery.CombineFilters(domainFilter, TableOperators.And, emailFilter);
                string domainemailorgFilter = TableQuery.CombineFilters(domainemailFilter, TableOperators.And, orgidFilter);
                tableQuery.FilterString = TableQuery.CombineFilters(domainemailorgFilter, TableOperators.And, appFilter);
                
                var data = table.ExecuteQuery(tableQuery);

                if (data.FirstOrDefault() == null)
                {
                    MediaValetCategoryDataEntity entity = new MediaValetCategoryDataEntity(categoriesModel.Domain, categoriesModel.Email, categoriesModel.Categories, categoriesModel.Application, categoriesModel.OrgUnitId);

                    batchOperation.Insert(entity);

                    table.ExecuteBatch(batchOperation);

                    return "Data inserted successfully";
                }
                return "Similar data already exists.";
            }
            catch (Exception)
            {
                return "An error occured while performing your operation";
            }
        }
        public static string RefreshExistingCategory(CategoriesModel categoriesModel)
        {
            try
            {
                CloudTableClient tableClient = account.CreateCloudTableClient();
                var tableName = ConfigurationManager.AppSettings["CategoryTableName"];
                CloudTable table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExistsAsync();
                var tableQuery = new TableQuery<MediaValetCategoryDataEntity>();

                string emailFilter = TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, categoriesModel.Email);
                string orgidFilter = TableQuery.GenerateFilterCondition("OrgUnitId", QueryComparisons.Equal, categoriesModel.OrgUnitId);
                string appFilter = TableQuery.GenerateFilterCondition("Application", QueryComparisons.Equal, categoriesModel.Application);

                string emailorgFilter = TableQuery.CombineFilters(emailFilter, TableOperators.And, orgidFilter);
                tableQuery.FilterString = TableQuery.CombineFilters(emailorgFilter, TableOperators.And, appFilter);

                var data = table.ExecuteQuery(tableQuery);
                if (data.FirstOrDefault() != null)
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();
                    MediaValetCategoryDataEntity entity = data.FirstOrDefault();
                    //entity.DomainName = categoriesModel.Domain;
                    entity.Email = categoriesModel.Email;
                    //entity.OrgUnitId = categoriesModel.OrgUnitId;
                    entity.Categories = categoriesModel.Categories;
                    //entity.Application = categoriesModel.Application;
                    batchOperation.Replace(entity);
                    table.ExecuteBatch(batchOperation);
                    return "Data updated successfully";
                }
                return "No data found for these details.";
            }
            catch (Exception e)
            {
                return "An error occured while performing your operation";
            }
        }

        
        public static MediaValetDomainDataEntity GetDomainData(string SearchVal,string SearchField,string appsname)
        {

            CloudTableClient tableClient = account.CreateCloudTableClient();
            var tableName = ConfigurationManager.AppSettings["DomainTableName"];
            CloudTable table = tableClient.GetTableReference(tableName);

             MediaValetDomainDataEntity obj=new MediaValetDomainDataEntity();
            if (SearchField == "email")
            {
                 var query = from mvdata in table.CreateQuery<MediaValetDomainDataEntity>()
                            where mvdata.EmailDomain == SearchVal && mvdata.Application == appsname
                            select mvdata;
                obj = query.FirstOrDefault();
            }

            else if (SearchField == "domain")
            {
                var query = from mvdata in table.CreateQuery<MediaValetDomainDataEntity>()
                            where mvdata.DomainName == SearchVal && mvdata.Application == appsname
                            select mvdata;
                obj = query.FirstOrDefault();
               

            }
            return obj;
        }
       
        public static string GetApiByDomainName(string domainName)
        {
            CloudTableClient tableClient = account.CreateCloudTableClient();
            var tableName = ConfigurationManager.AppSettings["DomainTableName"];
            CloudTable table = tableClient.GetTableReference(tableName);
            TableQuery<MediaValetDomainDataEntity> query = new TableQuery<MediaValetDomainDataEntity>().Where(TableQuery.GenerateFilterCondition("DomainName",
                                                              QueryComparisons.Equal, domainName));
            var data = table.ExecuteQuery(query);
            return data.FirstOrDefault().ApiUrl;
        }

        public static string SaveDomainData(string domainName, string apiUrl, string email,string orgunitid,string application)
        {
            try
            {
                CloudTableClient tableClient = account.CreateCloudTableClient();
                var tableName = ConfigurationManager.AppSettings["DomainTableName"];
                CloudTable table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExistsAsync();
                TableBatchOperation batchOperation = new TableBatchOperation();
                var tableQuery = new TableQuery<MediaValetDomainDataEntity>();
                var query = from mvdata in table.CreateQuery<MediaValetDomainDataEntity>()
                            where mvdata.DomainName == domainName && mvdata.EmailDomain == email && mvdata.Application == application 
                            select mvdata;
                var data = query.FirstOrDefault();
                if (data == null)
                {
                    MediaValetDomainDataEntity entity = new MediaValetDomainDataEntity(domainName, apiUrl, email, orgunitid, application);
                    batchOperation.Insert(entity);
                    table.ExecuteBatch(batchOperation);
                    return "Data saved successfully";
                }
                return "Similar data already exists.";
            }
            catch (Exception ex)
            {
                return "An error occured while performing your operation";
            }
        }

        public static MediaValetMetaData GetMetaData(string email, string domaniname, string orgunitid, string appsname)
        {
            CloudTableClient tableClient = account.CreateCloudTableClient();
            var tableName = ConfigurationManager.AppSettings["MetaData"];
            CloudTable table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExistsAsync();
            TableBatchOperation batchOperation = new TableBatchOperation();
            CloudTable tableQuery = tableClient.GetTableReference(tableName);
            var query = from mvdata in tableQuery.CreateQuery<MediaValetMetaData>()
                        where  mvdata.OrgUnitId == orgunitid && mvdata.Application == appsname
                        select mvdata;
            return query.FirstOrDefault();
        }
        public static string AddMetaData(MetaDataModel model)
        {
            var message = "";
            try
            {
                CloudTableClient tableClient = account.CreateCloudTableClient();
                var tableName = ConfigurationManager.AppSettings["MetaData"];
                CloudTable table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExistsAsync();
                TableBatchOperation batchOperation = new TableBatchOperation();
                CloudTable tableQuery = tableClient.GetTableReference(tableName);
                var querys = from mvdata in tableQuery.CreateQuery<MediaValetMetaData>()
                            where  mvdata.OrgUnitId == model.OrgUnitId && mvdata.Application == model.Application
                            select mvdata;
                if (querys.FirstOrDefault() == null)
                {
                    MediaValetMetaData mediavaletmdData = new MediaValetMetaData(model.DomainName, model.Email,
                    model.Application, model.OrgUnitId, model.MetaDataList);
                    batchOperation.Insert(mediavaletmdData);
                    table.ExecuteBatch(batchOperation);
                    message = "saved successfully";

                }
                else
                {
                    MediaValetMetaData entity = querys.FirstOrDefault();
                    entity.MetaDataList = model.MetaDataList;
                    batchOperation.Replace(entity);
                    table.ExecuteBatch(batchOperation);
                    return "updated successfully";
                }
                return message;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public static string UpdateMetaData(string partionid,string metadatalist)
        {
            var msg = "";
            try
            {
                CloudTableClient tableClient = account.CreateCloudTableClient();
                var tableName = ConfigurationManager.AppSettings["MetaData"];
                CloudTable table = tableClient.GetTableReference(tableName);
                string partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partionid);
                //string domainemailFilter = TableQuery.CombineFilters(domainFilter, TableOperators.And, emailFilter);
                TableQuery<MediaValetMetaData> query = new TableQuery<MediaValetMetaData>().Where(partitionFilter);//.Take(5);
               // tableQuery.FilterString = TableQuery.GenerateFilterConditionForGuid (orgidFilter);//, TableOperators.And, appFilter);
                var data = table.ExecuteQuery(query);
                if (data.FirstOrDefault() != null)
                {
                    TableBatchOperation batchOperation = new TableBatchOperation();
                    MediaValetMetaData entity = data.FirstOrDefault();
                    entity.MetaDataList = metadatalist;
                    batchOperation.Replace(entity);
                    table.ExecuteBatch(batchOperation);
                    msg= "updated successfully";
                }
                return msg;
            }
            catch (Exception e)
            {
                return "error_";
            }
        }


        public static string EloquaAssetTracking(EloquaTrackingModel eloquaTrackingModel)
        {
            try
            {
                CloudTableClient tableClient = account.CreateCloudTableClient();
                var tableName = ConfigurationManager.AppSettings["EloquaTrackingTable"];
                CloudTable table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExistsAsync();
                TableBatchOperation batchOperation = new TableBatchOperation();
                //Check if data is exist
                var querys = from mvdata in table.CreateQuery<EloquaTrackingEntity>()
                             where mvdata.InstanceId == eloquaTrackingModel.instanceid &&  mvdata.CustomerId == eloquaTrackingModel.customerid 
                             select mvdata;
                if (querys.FirstOrDefault() == null)
                {
                    EloquaTrackingEntity entity = new EloquaTrackingEntity(eloquaTrackingModel.instanceid, eloquaTrackingModel.customerid, eloquaTrackingModel.imageurl, eloquaTrackingModel.assetid, eloquaTrackingModel.assetname, eloquaTrackingModel.assettype, eloquaTrackingModel.sasurl,eloquaTrackingModel.renditionheigt,eloquaTrackingModel.renditionwidth);

                    batchOperation.Insert(entity);

                    table.ExecuteBatch(batchOperation);
                }
                else {


                    EloquaTrackingEntity entity = querys.FirstOrDefault();
                    entity.AssetId = eloquaTrackingModel.assetid;
                    entity.AssetName = eloquaTrackingModel.assetname;
                    entity.CustomerId = eloquaTrackingModel.customerid;
                    entity.AssetType = eloquaTrackingModel.assettype;
                    entity.AssetId = eloquaTrackingModel.assetid;
                    entity.ImageUrl = eloquaTrackingModel.imageurl;
                    entity.SasUrl = eloquaTrackingModel.sasurl;
                    entity.CreatedDate = DateTime.Now.Date;
                    entity.RenditionHeight = eloquaTrackingModel.renditionheigt;
                    entity.RenditionWidth = eloquaTrackingModel.renditionwidth;
                    batchOperation.Replace(entity);
                    table.ExecuteBatch(batchOperation);
                    return "updated successfully";
                }
                    return "success";
                
                
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public static string[] EloquaGetUrlforNotify(string instanceid)
        {
            string[] murl = new string[5];
            try {
                CloudTableClient tableClient = account.CreateCloudTableClient();
                var tableName = ConfigurationManager.AppSettings["EloquaTrackingTable"];
                CloudTable table = tableClient.GetTableReference(tableName);
                var query = new TableQuery<EloquaTrackingEntity>().Where(TableQuery.GenerateFilterCondition("InstanceId",
                                                                      QueryComparisons.Equal, instanceid));

                var data = table.ExecuteQuery(query);
                var getdata = data.FirstOrDefault();
               
                murl[0] = getdata.ImageUrl;
                murl[1] = getdata.SasUrl;
                murl[2] = getdata.AssetType;
                murl[3] = getdata.RenditionWidth;
                murl[4] = getdata.RenditionHeight;

                return murl;
            }catch(Exception ex)
            {
               murl[0] = ex.Message.ToString();
                murl[1] = ex.StackTrace.ToString();
                return murl;
            }


        }
        public static string EloquaGetOauthBase64(string clientid)
        {
            try {
                CloudTableClient tableClient = account.CreateCloudTableClient();

                var tableName = ConfigurationManager.AppSettings["EloquaClientInfoTable"];

                CloudTable table = tableClient.GetTableReference(tableName);

                // var tableQuery = new TableQuery<EloquaTrackingEntity>();
                //string domainFilter = TableQuery.GenerateFilterCondition("DomainName", QueryComparisons.Equal, domain);
                //string emailFilter = TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email);
                // string _instanceid = TableQuery.GenerateFilterCondition("InstanceId", QueryComparisons.Equal, instanceid);
                var query = new TableQuery<EloquaOauthClientEntity>().Where(TableQuery.GenerateFilterCondition("ClientId",
                                                                      QueryComparisons.Equal, clientid));


                var data = table.ExecuteQuery(query);
                var getdata = data.FirstOrDefault();
                string secretcode = "";
                if (getdata != null)
                {
                     secretcode = getdata.ClientEncodedCode;
                }
                else
                {
                    secretcode = "Not Found";
                }
                
                return secretcode;
            }catch(Exception ex)
            {
                return ex.Message.ToString();
            }


        }

        public static string EloquaSaveOauthBase64(EloquaClientOauthModel model)
        {
            var message = "";
            try
            {
                CloudTableClient tableClient = account.CreateCloudTableClient();
                var tableName = ConfigurationManager.AppSettings["EloquaClientInfoTable"];
                CloudTable table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExistsAsync();
                TableBatchOperation batchOperation = new TableBatchOperation();
                CloudTable tableQuery = tableClient.GetTableReference(tableName);
                var querys = from mvdata in tableQuery.CreateQuery<EloquaOauthClientEntity>()
                             where mvdata.ClientId == model.clientid 
                             select mvdata;
                if (querys.FirstOrDefault() == null)
                {
                    EloquaOauthClientEntity mediavaletmdData = new EloquaOauthClientEntity(model.clientid, model.clientsecretcode,
                    model.clientbase64);
                    batchOperation.Insert(mediavaletmdData);
                    table.ExecuteBatch(batchOperation);
                    message = "saved successfully";

                }
                else
                {
                    EloquaOauthClientEntity entity = querys.FirstOrDefault();
                    entity.ClientId = model.clientid;
                    entity.ClientSecret = model.clientsecretcode;
                    entity.ClientEncodedCode = model.clientbase64;
                    batchOperation.Replace(entity);
                    table.ExecuteBatch(batchOperation);
                    return "updated successfully";
                }
                return message;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }


        }

     public static string DeleteEloquaClientDetails(string clientid)
        {
            string returnmessage = "";
            try
            {

                CloudTableClient tableClient = account.CreateCloudTableClient();
                var tableName = ConfigurationManager.AppSettings["EloquaClientInfoTable"];
                CloudTable table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExistsAsync();
                TableBatchOperation batchOperation = new TableBatchOperation();
                CloudTable tableQuery = tableClient.GetTableReference(tableName);

                // Create a retrieve operation that expects a customer entity.
                TableOperation retrieveOperation = TableOperation.Retrieve<EloquaOauthClientEntity>("ClientId", clientid);

                // Execute the operation.
                TableResult retrievedResult = table.Execute(retrieveOperation);

                // Assign the result to a CustomerEntity.
                EloquaOauthClientEntity deleteEntity = (EloquaOauthClientEntity)retrievedResult.Result;

                // Create the Delete TableOperation.
                if (deleteEntity != null)
                {
                    TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                    // Execute the operation.
                    table.Execute(deleteOperation);

                    Console.WriteLine("Entity deleted.");
                }

                else
                    Console.WriteLine("Could not retrieve the entity.");
                return returnmessage;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public static string DeleteEloquaClientDetailsWithInstanceid(string instanceid)
        {
            string returnmessage = "";
            try
            {

                CloudTableClient tableClient = account.CreateCloudTableClient();
                var tableName = ConfigurationManager.AppSettings["EloquaClientInfoTable"];
                CloudTable table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExistsAsync();
                TableBatchOperation batchOperation = new TableBatchOperation();
                CloudTable tableQuery = tableClient.GetTableReference(tableName);
                // Create a retrieve operation that expects a customer entity.
                TableOperation retrieveOperation = TableOperation.Retrieve<EloquaOauthClientEntity>("InstanceId", instanceid);
                // Execute the operation.
                TableResult retrievedResult = table.Execute(retrieveOperation);

                // Assign the result to a CustomerEntity.
                EloquaOauthClientEntity deleteEntity = (EloquaOauthClientEntity)retrievedResult.Result;

                // Create the Delete TableOperation.
                if (deleteEntity != null)
                {
                    TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                    // Execute the operation.
                    table.Execute(deleteOperation);

                    Console.WriteLine("Entity deleted.");
                }

                else
                    Console.WriteLine("Could not retrieve the entity.");
                return returnmessage;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
    }
}