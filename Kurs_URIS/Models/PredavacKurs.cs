using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kurs_URIS.Models
{
    public class PredavacKurs
    {
        public int PredavacKursID { get; set; }

        [Required]
        public int KursID { get; set; }

        [Required]
        public int PredavacID { get; set; }
    }

    public class PredavacKursWithValueObject
    {
        [Required]
        public int KursID { get; set; }
        public List<PredavacKursaVO> Predavac { get; set; }
    }
}