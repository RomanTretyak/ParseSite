using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ParseSites
{
    static class Constant
    {
        public const string SiteName = "http://fast-torrent.ru";
        public const string SiteSection = "/zarubezhnyj-film/";
        public const string Html = ".html";
    }

    class ParseFastTorrentRu
    {
        public void StartParse(int startPage = 1, int endPage = 1)
        {
            // Для запису в лог id сторінки на якій сталася помилка.
            int currentPage = 0;

            try
            {
                // Сycle through the pages of this site.
                for (var i = startPage; i < endPage + 1; i++)
                {
                    currentPage = i;
                    string buildUri = Constant.SiteName + Constant.SiteSection + i + Constant.Html;

                    if (Request.GetHtmlDocument(buildUri) != null)
                    {
                        FillingModelFilm(Request.GetHtmlDocument(buildUri));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError("page -" + currentPage + " " + ex);
                throw;
            }
        }

        private void FillingModelFilm(HtmlDocument doc)
        {
            try
            {
                // Cycle frame film
                HtmlNodeCollection glvCollection = doc.DocumentNode.SelectNodes("//div[@class='film-item is-video']");
                if (glvCollection != null)
                {
                    foreach (HtmlNode currentNodeItem in glvCollection)
                    {
                        if (currentNodeItem != null)
                        {
                            var currentFilm = new Film();

                            // Parse <div class='film-image'>.
                            DivFilmImage(currentNodeItem, currentFilm);

                            // Год через out виключно для прикладу.
                            string year;
                            FilmYear(out year, currentNodeItem);
                            currentFilm.Year = ParseDate.GetDate(year);
                            //var a = ParseDate.GetDate(year);

                            if (string.IsNullOrEmpty(currentFilm.HrefDetail) == false)
                            {
                                var cParseTorrent = new ParseFastTorrentRuSingleItem();
                                currentFilm.Rating = cParseTorrent.GetRatingValue(Constant.SiteName + currentFilm.HrefDetail);
                            }

                            // Original name.
                            currentFilm.OriginalName = GetOriginalName(currentNodeItem);

                            // Get type film.
                            currentFilm.Type = GetFilmType(currentNodeItem);

                            var allRoles = GetRoles(currentNodeItem);

                            // Get announce.
                            currentFilm.Announce = GetAnnounce(currentNodeItem);
                            currentFilm.CurrentDateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                            SaveInBase(currentFilm, allRoles);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        private void SaveInBase(Film currentFilm, List<Structure.SRole> allRoles)
        {
            try
            {
                if ((!allRoles.Any()) || (currentFilm == null)) return;
                // Base save.
                using (var db = new BaseContext())
                {
                    var operationsBase = new OperationsBase();
                    // Export into Model.Role of the SRole and add to context,
                    // return IEnumerable from Film.Roles.
                    var roleList = ExportToModelRole(currentFilm, allRoles, db);
                    currentFilm.Roles = roleList;

                    operationsBase.SaveFilmInBase(currentFilm, db);
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        public List<Role> ExportToModelRole(Film film, List<Structure.SRole> allRoles, BaseContext db)
        {
            try
            {
                List<Role> roleList = new List<Role>();

                foreach (var itemRole in allRoles)
                {
                    Role role = new Role()
                    {
                        Film = film,
                        Name = itemRole.Name,
                        Type = itemRole.Type
                    };
                    db.Role.Add(role);
                    //db.SaveChanges();
                    roleList.Add(role);
                    // Not save.
                }
                return roleList;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        private void FilmYear(out string year, HtmlNode glvNode)
        {
            try
            {
                year = default(string);
                HtmlNode filmInfo = GetDivClassName(glvNode, "film-info");
                if (filmInfo != null)
                {
                    foreach (HtmlAttribute atribute in filmInfo.ChildNodes[0].Attributes)
                    {
                        if (atribute.Name == "content")
                            year = atribute.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        private string GetOriginalName(HtmlNode glvNode)
        {
            try
            {
                string result = default(string);
                HtmlNode nodeSingle = glvNode.SelectSingleNode(".//div[@class='film-wrap']/h2");

                if ((nodeSingle != null) && (nodeSingle.ChildNodes != null))
                {
                    foreach (HtmlNode childNode in nodeSingle.ChildNodes)
                    {
                        if (childNode != null)
                        {
                            foreach (HtmlAttribute atribute in childNode.Attributes)
                            {
                                if (atribute.Value == "alternativeHeadline")
                                    result = childNode.InnerText; break;
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        private string GetFilmType(HtmlNode glvNode)
        {
            try
            {
                List<string> listType = new List<string>();
                HtmlNode nodeFilm = glvNode.SelectSingleNode(".//div[@class='film-genre']/div");

                if ((nodeFilm != null) && (nodeFilm.ChildNodes != null))
                {
                    foreach (HtmlNode nodeItem in nodeFilm.ChildNodes)
                    {
                        if (nodeItem != null)
                        {
                            if (nodeItem.InnerText.Contains("\t") != true)
                                listType.Add(nodeItem.InnerText);
                        }
                    }
                }

                var result = string.Join(", ", listType.ToArray());

                return result;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }
        public static string ReplaceCode(string input)
        {
            if (string.IsNullOrEmpty(input) == false)
            {
                input = Regex.Replace(input, @"\t|\n|\r", "").Trim();
            }
            return input;
        }

        private string GetAnnounce(HtmlNode glvNode)
        {
            try
            {
                string result = default(string);
                HtmlNodeCollection filmAnounceCollection = glvNode.SelectNodes(".//div[@class='film-announce']");
                if (filmAnounceCollection != null)
                {
                    foreach (HtmlNode singleNode in filmAnounceCollection)
                    {
                        if (singleNode != null)
                            result = singleNode.InnerText.ReplaceCode();
                        //result = ReplaceCode(singleNode.InnerText);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        private List<Structure.SRole> GetRoles(HtmlNode glvNode)
        {
            // All roles.
            try
            {
                List<Structure.SRole> listRoles = new List<Structure.SRole>();
                HtmlNodeCollection alignCollections = glvNode.SelectNodes(".//div[@class='align-l']");

                if (alignCollections != null)
                {
                    foreach (HtmlNode nodeItem in alignCollections)
                    {
                        if (nodeItem != null)
                        {
                            Structure.SRole role = new Structure.SRole();

                            FillRoles(listRoles, nodeItem, role);
                        }
                    }
                }
                return listRoles;

            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        private void FillRoles(List<Structure.SRole> roles, HtmlNode nodeItem, Structure.SRole role)
        {
            try
            {
                if (nodeItem.HasChildNodes)
                {
                    foreach (HtmlNode nodeA in nodeItem.ChildNodes)
                    {
                        if (nodeA.Name == "strong")
                            role.Type = nodeA.InnerText;
                        foreach (HtmlAttribute atribute in nodeA.Attributes)
                        {
                            if (atribute.Value == "actor")
                            {
                                role.Name = nodeA.InnerText;
                                roles.Add(role);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(nodeItem + ex.ToString());
                throw;
            }
        }

        private HtmlNode GetDivClassName(HtmlNode glvNode, string className)
        {
            try
            {
                string path = String.Format(".//div[@class='{0}']", className);
                HtmlNodeCollection nodeCollection = glvNode.SelectNodes(path);
                if (nodeCollection != null)
                {
                    foreach (var itemDiv in nodeCollection)
                    {
                        if (itemDiv != null)
                        {
                            return itemDiv;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        private void DivFilmImage(HtmlNode glvNode, Film film)
        {
            try
            {
                HtmlNodeCollection filmImageCollection = glvNode.SelectNodes(".//div[@class='film-image']");

                if (filmImageCollection != null)
                {
                    foreach (HtmlNode itemDiv in filmImageCollection)
                    {
                        if (itemDiv != null)
                        {
                            // Назви однакові, впринципі поле ImageName не обовязкове.
                            film.ImageName = GetImageName(itemDiv);
                            film.Name = film.ImageName;

                            GetValueDivFilmImage(film, itemDiv);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        private void GetValueDivFilmImage(Film film, HtmlNode itemDiv)
        {
            try
            {
                foreach (HtmlNode childDiv in itemDiv.ChildNodes)
                {
                    foreach (HtmlAttribute htmlAtribute in childDiv.Attributes)
                    {
                        if (htmlAtribute.Name == "href")
                        {
                            film.HrefDetail = htmlAtribute.Value;
                        }

                        if (htmlAtribute.Name == "style")
                        {
                            film.ImageSrc = SubstringImageSrc(htmlAtribute);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        private string SubstringImageSrc(HtmlAttribute htmlAtribute)
        {
            try
            {
                string tmpStr = default(string);
                if (htmlAtribute.Value != null)
                {
                    tmpStr = htmlAtribute.Value;
                    tmpStr = tmpStr.Substring(tmpStr.IndexOf("http", StringComparison.Ordinal));
                    // Substring last symbol.
                    tmpStr = tmpStr.Substring(0, tmpStr.Length - 1);
                }
                // Если возможно качаем картинку.
                var cDownload = new DownloadFile();

                return cDownload.DownloadImage(tmpStr);
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }

        private string GetImageName(HtmlNode itemDiv)
        {
            try
            {
                string imageName = default(string);

                foreach (HtmlAttribute atribute in itemDiv.Attributes)
                {
                    if (atribute.Name == "alt")
                    {
                        imageName = atribute.Value;
                    }
                }
                return imageName;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }
    }
}
