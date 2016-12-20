namespace Mvapi.Lib
{
   public  interface IWebReqRes
    {
     
       string  GetRequest(string accessToken, string contentType, string url);
        string postdata(string Token, string URL, string body);
    }
}
