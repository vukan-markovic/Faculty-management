using System.ComponentModel.DataAnnotations;

namespace Poruka_URIS.Models
{
    public class Notifikacija
    {
        public int NotifikacijaID { get; set; }

        [Required]
        [MaxLength(500)]
        public string SadrzajNotifikacije { get; set; }

        [Required]
        [MaxLength(30)]
        public string Ucestalost { get; set; }
    }
}