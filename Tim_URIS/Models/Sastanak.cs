using System;
using System.ComponentModel.DataAnnotations;

namespace Tim_URIS.Models
{
    public class Sastanak
    {
        public int SastanakID { get; set; }

        [Required]
        public int TimID { get; set; }

        [Required]
        public DateTime VremeSastanka { get; set; }

        [Required]
        [MaxLength(50)]
        public string MestoSastanka { get; set; }

        [Required]
        [MaxLength(200)]
        public string PovodSastanka { get; set; }

        public int PredavacID { get; set; }
    }

    public class SastanakWithValueObject: Sastanak
    {
        public PredavacInfoVO Predavac { get; set; }
    }
}