using System.ComponentModel.DataAnnotations;

namespace Poruka_URIS.Models
{
    public class StudentPorukaPrimalac
    {
        public int StudentPorukaPrimalacID { get; set; }

        [Required]
        public int PorukaID { get; set; }

        [Required]
        public int StudentID { get; set; }
    }

    public class StudentPorukaPrimalacWithValueObject : StudentPorukaPrimalac
    {
        public NotifikovanStudentVO Student { get; set; }
    }
}