using System.ComponentModel.DataAnnotations;

namespace Student_URIS.Models
{
    public class StudentTest
    {
        public int StudentTestID { get; set; }

        [Required]
        public int TestID { get; set; }

        [Required]
        public int StudentID { get; set; }
    }
}