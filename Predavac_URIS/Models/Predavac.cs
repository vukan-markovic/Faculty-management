using System;
using System.ComponentModel.DataAnnotations;

namespace Predavac_URIS.Models
{
    public class Predavac
    {
        public int PredavacID { get; set; }

        [Required]
        public DateTime DatumRodjenjaPredavaca { get; set; }

        [Required]
        [MaxLength(20)]
        public string MestoRodjenjaPredavaca { get; set; }

        [Required]
        [MaxLength(50)]
        public string KatedraPredavaca { get; set; }

        [Required]
        public int ZvanjePredavacaID { get; set; }

        [Required]
        public int KorisnikID { get; set; }

        [Required]
        public int DepartmanID { get; set; }
    }

    public class PredavacWithValueObject: Predavac
    {
        public KorisnikInfoVO Korisnik { get; set; }
        public DepartmanVO Departman { get; set; }
    }
}