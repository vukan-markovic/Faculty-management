using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Predmet_URIS.Models
{
    public class PohadjaniPredmet
    {
        public int PohadjaniPredmetID { get; set; }
        public int BrojBodova { get; set; }
        public int PredmetID { get; set; }
        public int Ocena { get; set; }
        public int StudentID { get; set; }
        
    }
    public class PohadjaniPredmetWithValueObject:PohadjaniPredmet
    {
        public StudentNaPredmetuVO Student { get; set; }
    }
    public class StudentiNaPredmetu
    {
        public int PredmetID { get; set; }
        public List<StudentNaPredmetuVO> Studenti { get; set; }
    }
}