using System;
using System.Linq;

namespace ParseSites
{
    class OperationsBase
    {
        public void SaveFilmInBase(Film currentFilm, BaseContext db)
        {
            try
            {
                // Base save.     
                // Потрібна перевірка на співпадіння в базі
                // ще подумаю можливо якась процедура в базі.

                var dataFilm = db.Film.FirstOrDefault(t => t.Name.Contains(currentFilm.Name));

                if (dataFilm != null)
                {
                    // Якщо в базі вже є такий запис то пропускаємо.
                    return;
                }
                //db.SaveChanges();
                db.Film.Add(currentFilm);
                db.SaveChanges();
                // Якщо вже існує потрібен апдейт існуючого, тільки по полях null
                
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }
        }
    }
}
