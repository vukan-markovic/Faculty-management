using System.ComponentModel.DataAnnotations;

namespace Tim_URIS.Models
{
    public class StudentTim
    {
        public int StudentTimID { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        public int TimID { get; set; }

    }

    public class StudentTimWithValueObject: StudentTim
    {
        public StudentInfoVO Student { get; set; }
    }
}