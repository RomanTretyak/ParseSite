using System.Collections.Generic;

namespace ParseSites
{
    public class Film
    {
        public int FilmId { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string Type { get; set; }
        public string Year { get; set; }
        public string ImageName { get; set; }
        public string ImageSrc { get; set; }
        public string Country { get; set; }
        public string Announce { get; set; }
        public double Rating { get; set; }
        public string HrefDetail { get; set; }
        public string CurrentDateTime { get; set; }
        public string Variable1 { get; set; }
        public string Variable2 { get; set; }
        public virtual IEnumerable<Role> Roles { get; set; }

    }
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public virtual Film Film { get; set; }
    }
}
