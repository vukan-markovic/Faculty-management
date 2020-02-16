using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Predmet_URIS.Models
{
    public class Predmet
    {
        public int PredmetID { get; set; }
        [Required]
        [MaxLength(255)]
        public string NazivPredmeta { get; set; }
        [Required]
        public string OznakaPredmeta { get; set; }
        [Required]
        public int Godina { get; set; }
        [Required]
        public int ECTSBodovi { get; set; }
        [Required]
        public int DepartmanID { get; set; }
    }
    public class PredmetWithValueObject : Predmet
    {
        public DepartmanPredmetaVO Departman { get; set; }
    }
    
}