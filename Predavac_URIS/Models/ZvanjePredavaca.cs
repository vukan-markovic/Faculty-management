using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Predavac_URIS.Models
{
    public class ZvanjePredavaca
    {
        public int ZvanjePredavacaID { get; set; }

        [Required]
        [MaxLength(100)]
        public string NazivZvanjaPredavaca { get; set; }
    }
}