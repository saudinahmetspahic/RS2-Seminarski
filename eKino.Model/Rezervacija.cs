﻿using System;
using System.Collections.Generic;
using System.Text;

namespace eKino.Model
{
    public class Rezervacija
    {
        public int Id { get; set; }
        public DateTime DatumKreirnja { get; set; }

        public int SjedisteRed { get; set; }
        public int SjedisteKolona { get; set; }

        public int? ProjekcijaId { get; set; }
        //public virtual Projekcija Projekcija { get; set; }

        public int? KupacId { get; set; }
        //public virtual Korisnik Kupac { get; set; }
    }
}
