
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;

namespace Mvapi.Lib
{
    public class WebReqRes : IWebReqRes
    {
        public string GetRequest(string accessToken, string contentType,string url)
        {
            var response = "";
            
            var resourceGroupsrequest = (HttpWebRequest)WebRequest.Create(url);
            if (!string.IsNullOrEmpty(accessToken))
            {
                resourceGroupsrequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + accessToken);
            }
            resourceGroupsrequest.ContentType = contentType;// "application/json";
            var resourceGroupsResponse = (HttpWebResponse)resourceGroupsrequest.GetResponse();
            Console.WriteLine(resourceGroupsResponse.StatusDescription);
            var resourceGroupsreceiveStream = resourceGroupsResponse.GetResponseStream();
            // Pipes the stream to a higher level stream reader with the required encoding format. 
            if (resourceGroupsreceiveStream == null) return response;
            var rateCardreadStream = new StreamReader(resourceGroupsreceiveStream, Encoding.UTF8);
            response = rateCardreadStream.ReadToEnd();

            return response;
        }


        public string   postdata(string Token,string URL,string body) {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            if (!string.IsNullOrEmpty(Token))
            {
               client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));               
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            }
            HttpResponseMessage response = client.PostAsync(URL, new StringContent(body, Encoding.UTF8, "application/json")).Result;
            string Result = response.Content.ReadAsStringAsync().Result;
            return Result;
        }

        
    }
}