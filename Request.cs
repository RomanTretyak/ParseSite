using System;
using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace ParseSites
{
   static class Request
    {
       public static HtmlDocument GetHtmlDocument(string uri)
       {
           try
           {
               var uriSite = new Uri(uri);

               string request = ResponseUrl(uriSite);
               if (string.IsNullOrEmpty(request)) return default(HtmlDocument);

               var doc = new HtmlDocument();
               doc.LoadHtml(request);

               return doc;
           }
           catch (Exception ex)
           {
               Logger.GetLogger().LogError(ex.ToString());
               throw;
           }
       }
        public static string  ResponseUrl(Uri link)
        {
            try
            {
                string result;

                WebRequest request = WebRequest.Create(link.ToString());

                request.Credentials = CredentialCache.DefaultCredentials;

                GetResponse(out result, request);

                // For example :)
                return result ?? string.Empty;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }

        }

        private static void GetResponse(out string result, WebRequest request)
        {
            using (var response = (HttpWebResponse)request.GetResponse())
            {

                using (Stream dataStream = response.GetResponseStream())
                {

                    result = ResponseFromServer(dataStream);

                    if (dataStream != null)
                    {
                        dataStream.Close();
                    }
                    response.Close();
                }
            }

        }

        private static string ResponseFromServer(Stream dataStream)
        {
            string result;

            using (var reader = new StreamReader(dataStream))
            {
                result = reader.ReadToEnd();

                reader.Close();
            }

            return result;
        }
    }
}
