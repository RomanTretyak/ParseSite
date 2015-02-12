using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace ParseSites
{
    class ParseFastTorrentRuSingleItem
    {
        public IEnumerable<string> FindUrlTorrent(string urlPage)
        {
            try
            {
                List<string> linkList;

                if (Request.GetHtmlDocument(urlPage) == null)
                    return null;

                // <div class='c7'>
                var doc = Request.GetHtmlDocument(urlPage);
                var divClassC7 = doc.DocumentNode.SelectNodes("//div[@class='c7']");

                if (divClassC7 == null)
                    return null;

                linkList = new List<string>();
                foreach (var item in divClassC7)
                {
                    linkList.AddRange(from attribute in item.Attributes
                                      where attribute.Name == "href"
                                      select attribute.Value);
                }
                return linkList;
            }
            catch (System.Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        public double GetRatingValue(string url)
        {
            var result = default(double);

            if (Request.GetHtmlDocument(url) != null)
            {
              result =  ReturnRating(Request.GetHtmlDocument(url));
            }
            return result;
        }
        private static double ReturnRating(HtmlDocument doc)
        {
            var rating = default(double);
            HtmlNodeCollection glvCollection = doc.DocumentNode.SelectNodes("//div[@class='margin_b']/table/tr/td/center/div");
            if (glvCollection == null) return rating;

            foreach (HtmlNode itemNode in glvCollection)
            {
                foreach (HtmlAttribute atribute in itemNode.Attributes)
                {
                    if ((atribute.Name == "id") && (atribute.Value == "rating"))
                    {
                        double tmpValue;
                        double.TryParse(itemNode.InnerText, out tmpValue);
                        rating = tmpValue;
                    }
                }
            }
            return rating;
        }
    }
}
