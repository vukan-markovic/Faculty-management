using System.ComponentModel.DataAnnotations;

namespace Poruka_URIS.Models
{
    public class Poruka
    {
        public int PorukaID { get; set; }

        [Required]
        [MaxLength(1000)]
        public string SadrzajPoruke { get; set; }

        [Required]
        [MaxLength(30)]
        public string VrstaPoruke { get; set; }

        [Required]
        [MaxLength(30)]
        public string VidljivostPoruke { get; set; }

        public int PredavacID { get; set; }

        public int TimID { get; set; }

        public int StudentID { get; set; }

        [Required]
        public int NotifikacijaID { get; set; }
    }

    public class PorukaWithValueObject : Poruka
    {
        public NotifikovanStudentVO Student { get; set; }
        public PredavacPosiljalacVO Predavac { get; set; }
        public NotifikovanTimVO Tim { get; set; }
    }
}