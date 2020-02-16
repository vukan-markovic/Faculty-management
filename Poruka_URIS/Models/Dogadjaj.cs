using System.ComponentModel.DataAnnotations;

namespace Poruka_URIS.Models
{
    public class Dogadjaj
    {
        public int DogadjajID { get; set; }

        [Required]
        [MaxLength(30)]
        public string NazivDogadjaja { get; set; }

        [Required]
        public int NotifikacijaID { get; set; }
    }
}