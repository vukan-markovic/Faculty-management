using System.ComponentModel.DataAnnotations;

namespace Tim_URIS.Models
{
    public class StudentSastanak
    {
        public int StudentSastanakID { get; set; }

        [Required]
        public int SastanakID { get; set; }


        [Required]
        public int StudentID { get; set; }
    }

    public class StudentSastanakWithValueObject : StudentSastanak
    {
        public StudentInfoVO Student { get; set; }
    }
}