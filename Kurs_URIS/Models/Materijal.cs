using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kurs_URIS.Models
{
    public class Materijal
    {
        public int MaterijalID { get; set; }

        [Required]
        [MaxLength(100)]
        public string NazivMaterijala { get; set; }

        [Required]
        public int KursID { get; set; }
    }
}