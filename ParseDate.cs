using System;
using System.Collections.Generic;
using System.Globalization;

namespace ParseSites
{
    public static class ParseDate
    {
        //private enum MonthEng
        //{
        //    NotSet = 0,
        //    Jan = 01,
        //    Feb = 02,
        //    Mar = 03,
        //    Apr = 04,
        //    May = 05,
        //    Jun = 06,
        //    Jul = 07,
        //    Aug = 08,
        //    Sep = 09,
        //    Oct = 10,
        //    Nov = 11,
        //    Dec = 12
        //}
        private enum MonthRu
        {
            NotSet = 0,
            Янв = 01,
            Фев = 02,
            Март = 03,
            Апрель = 04,
            Май = 05,
            Июнь = 06,
            Июль = 07,
            Авг = 08,
            Сен = 09,
            Окт = 10,
            Ноя = 11,
            Дек = 12
        }
        
        public static string GetDate(string inputDate)
        {
            try
            {
                // Example "Сен. 1, 2015".
                var monthDict = new List<MonthRu>
            {
                MonthRu.Янв,
                MonthRu.Фев,
                MonthRu.Март,
                MonthRu.Апрель,
                MonthRu.Май,
                MonthRu.Июнь,
                MonthRu.Июль,
                MonthRu.Авг,
                MonthRu.Сен,
                MonthRu.Окт,
                MonthRu.Ноя,
                MonthRu.Дек
            };
                if (inputDate == null) return DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
               
                var arr = inputDate.Split(' ');
                var createDate = arr[2] + "-" + (int) monthDict.Find(x => x.ToString()
                                                                          == arr[0].Replace(".", "")) + "-" +
                                 arr[1].Replace(",", "");
                DateTime resData;

                return DateTime.TryParse(createDate, out resData)
                    ? resData.ToString("yyyy-MM-dd HH:mm:ss")
                    : Convert.ToString(DateTime.MinValue, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }
        
    }
}
