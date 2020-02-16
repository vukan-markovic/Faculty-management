using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Departman_URIS.Models
{
    public class PredavacVO
    {
        public string KatedraPredavaca { get; set; }
        public DateTime DatumRodjenjaPredavaca { get; set; }
        public string MestoRodjenjaPredavaca { get; set; }
        public int ZvanjePredavacaID { get; set; }
        public int KorisnikID { get; set; }

    }
}