using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParseSites;

namespace ParseSites
{
    class Program
    {
        static void Main(string[] args)
        {
            var newParse = new ParseFastTorrentRu();
            newParse.StartParse(1,1000);
        }
    }
}
