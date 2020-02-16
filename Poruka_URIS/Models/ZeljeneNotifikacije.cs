using System.ComponentModel.DataAnnotations;

namespace Poruka_URIS.Models
{
    public class ZeljeneNotifikacije
    {
        public int ZeljeneNotifikacijeID { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        public int NotifikacijaID { get; set; }
    }

    public class ZeljeneNotifikacijeWithValueObject: ZeljeneNotifikacije
    {
        public NotifikovanStudentVO Student { get; set; }
    }
}