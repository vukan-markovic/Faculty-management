using System;
using System.ComponentModel.DataAnnotations;

namespace Student_URIS.Models
{
    public class Student
    {
        public int StudentID { get; set; }

        [Required]
        public DateTime DatumRodjenjaStudenta { get; set; }

        [Required]
        [MaxLength(20)]
        public string MestoRodjenjaStudenta { get; set; }

        [Required]
        [MaxLength(10)]
        public string BrojIndeksaStudenta { get; set; }

        [Required]
        [MaxLength(50)]
        public string KatedraStudenta { get; set; }

        [Required]
        [MaxLength(100)]
        public string StudijskiProgramStudenta { get; set; }

        [Required]
        [MaxLength(30)]
        public string VrstaStudija { get; set; }

        [Required]
        [MaxLength(100)]
        public string StepenStudija { get; set; }

        [Required]
        [MaxLength(100)]
        public string GodinaStudija { get; set; }

        [Required]
        public int RbrUpisaneGodine { get; set; }

        [Required]
        public int GodinaUpisaFakulteta { get; set; }

        [Required]
        [MaxLength(100)]
        public string NacinFinansiranja { get; set; }

        [Required]
        [MaxLength(100)]
        public string SifraStudenta { get; set; }

        [Required]
        public int KorisnikID { get; set; }

        [Required]
        public int DepartmanID { get; set; }
    }

    public class StudentWithValueObjects : Student
    {
        public KorisnikInfoVO Korisnik { get; set; }
        public DepartmanInfoVO Departman { get; set; }
    }
}