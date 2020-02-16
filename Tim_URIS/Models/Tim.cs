using System.ComponentModel.DataAnnotations;

namespace Tim_URIS.Models
{
    public class Tim
    {
        public int TimID { get; set; }

        [Required]
        [MaxLength(30)]
        public string NazivTima { get; set; }

        [Required]
        public int PredavacID { get; set; }

        public int Ocena { get; set; }    
    }

    public class TimWithValueObject: Tim
    {
        public PredavacInfoVO Predavac { get; set; }
    }
}