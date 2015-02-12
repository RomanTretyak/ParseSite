using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace ParseSites
{
    class DownloadFile
    {
       public string CurrentFolder = Directory.GetCurrentDirectory();
       private const string ImageFolderName = @"\Image";

        public string DownloadTorrentFile(IEnumerable<string> linkTorrentFile)
        {
            if (linkTorrentFile != null)
            {
                foreach (var item in linkTorrentFile)
                {
                   // item
                }
            }
            return default(string);
        }
       public string DownloadImage(string url)
       {
           try
           {
               if (string.IsNullOrEmpty(url)) return url;
               var fileName = url.Split('/').Last();

               FolderExist(CurrentFolder + ImageFolderName);
               var pathImage = CurrentFolder + ImageFolderName + @"\" + fileName;

               // Create a new WebClient instance.
               using (var myWebClient = new WebClient())
               {
                   if (File.Exists(pathImage) == false)
                       myWebClient.DownloadFile(url, pathImage);
                   return pathImage;
               }
           }
           catch (Exception ex)
           {
               Logger.GetLogger().LogError(ex.ToString());
               throw;
           }
       }
       private void FolderExist(string folderPath)
       {
           try
           {
               if (string.IsNullOrEmpty(folderPath)) return;

               if (Directory.Exists(folderPath) == false)
               {
                   // Create folder
                   Directory.CreateDirectory(folderPath);
               }
           }
           catch (Exception ex)
           {
               Logger.GetLogger().LogError(ex.ToString());
               throw;
           }
       }
       
    }
}
