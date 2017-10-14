namespace WpfKorisnickiProgram.Klase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class kartice_tab
    {
        [Key]
        public int KarticaId { get; set; }

        public int brkartice { get; set; }
        
        public double? stanje { get; set; }

        [Required]
        [StringLength(50)]
        public string korisnik { get; set; }

        [StringLength(50)]
        public string email { get; set; }

        [StringLength(50)]
        public string faks { get; set; }

        [StringLength(50)]
        public string brtelefona { get; set; }

        public double? Uplata { get; set; }
    }
}
