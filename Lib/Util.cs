using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Mvapi.Lib
{
    public class Util
    {

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




    }
}