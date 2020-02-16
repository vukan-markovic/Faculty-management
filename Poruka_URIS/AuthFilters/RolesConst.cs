using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Poruka_URIS.AuthFilters
{
    public class RolesConst
    {
        public const string ROLE_Admin = "Admin";
        public const string ROLE_Predavac = "Predavac";
        public const string ROLE_Student = "Student";
        public const string ROLE_Admin_Predavac = "Admin,Predavac";
        public const string ROLE_Admin_Student = "Admin,Student";
        public const string ROLE_Predavac_Student = "Predavac,Student";
        public const string ROLE_Admin_Student_Predavac = "Admin,Student,Predavac";
        public const string ROLE_Odrzavac = "Odrzavac";
    }
}