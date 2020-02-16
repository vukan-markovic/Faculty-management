using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Departman_URIS.Models
{
    public class Departman
    {
        public int DepartmanID { get; set; }
        public string NazivDepartmana { get; set; }
        public int FakultetID { get; set; }
    }
    public class DepartmanWithVO:Departman
    {
        public List<PredavacVO> Predavac { get; set; }
    }
}