using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kurs_URIS.Models
{
    public class Kurs
    {
        public int KursID { get; set; }

        [Required]
        [MaxLength(100)]
        public string NazivKursa { get; set; }

        [Required]
        public int BrStudenata { get; set; }

        [Required]
        [MaxLength(100)]
        public string RasporedPredavanja { get; set; }

        [Required]
        [MaxLength(100)]
        public string StudijskiProgram { get; set; }

        [Required]
        [MaxLength(10)]
        public string SkolskaGodina { get; set; }

        [Required]
        public double TezinskiFaktorOcene { get; set; }

        [Required]
        [MaxLength(100)]
        public string PolitikaUpisaOcena { get; set; }

        [Required]
        [MaxLength(100)]
        public string PolitikaUpisaDepartman { get; set; }

        [Required]
        [MaxLength(100)]
        public string PolitikaUpisaFCFS { get; set; }

        [Required]
        public int MinimalanBrStudenata { get; set; }

        [Required]
        [MaxLength(100)]
        public string DefinicijaPreduslova { get; set; }

        [Required]
        [MaxLength(100)]
        public string KriterijumZaPolaganje { get; set; }

        [Required]
        [MaxLength(100)]
        public string Pravilo { get; set; }

        [Required]
        public int PredmetID { get; set; }
    }

    public class KursWithValueObject: Kurs
    {
        public PredmetVO Predmet { get; set; }
    }
}