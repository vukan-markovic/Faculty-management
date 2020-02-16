using Korisnik_URIS.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Korisnik_URIS.Models
{
    public class Korisnik
    {
        public int KorisnikID { get; set; }

        [Required]
        [MaxLength(30)]
        public string Username { get; set; }

        [Required]
        [MaxLength(50)]
        public string Sifra { get; set; }

        [Required]
        [MaxLength(30)]
        public string Ime { get; set; }

        [Required]
        [MaxLength(30)]
        public string Prezime { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        [MaxLength(30)]
        [ValidateRolesState]
        public string TrenutnaUloga { get; set; }
    }
}