using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Student_URIS.Models
{
    public class Test
    {
        public int TestID { get; set; }

        [Required]
        public int KursID { get; set; }

        [Required]
        [MaxLength(100)]
        public string NazivTesta { get; set; }
    }

    public class TestWithValueObject: Test
    {
        public KursVO Kurs { get; set; }
    }
}