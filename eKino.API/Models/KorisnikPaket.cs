﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eKino.API.Models
{
    public class KorisnikPaket
    {
        [ForeignKey(nameof(KorisnikId))]
        public int KorisnikId { get; set; }
        public virtual Korisnik Korisnik { get; set; }

        [ForeignKey(nameof(PaketId))]
        public int PaketId { get; set; }
        public virtual Paket Paket { get; set; }

    }
}
