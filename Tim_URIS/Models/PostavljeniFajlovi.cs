using System.ComponentModel.DataAnnotations;

namespace Tim_URIS.Models
{
    public class PostavljeniFajlovi
    {
        public int PostavljeniFajloviID { get; set; }

        [Required]
        [MaxLength(100)]
        public string NazivPostavljenogFajla { get; set; }

        public int StudentID { get; set; }

        public int TimID { get; set; }
    }

    public class PostavljeniFajloviWithValueObject: PostavljeniFajlovi
    {
        public StudentInfoVO Student { get; set; }
    }
}