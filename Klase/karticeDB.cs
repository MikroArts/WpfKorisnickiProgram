namespace WpfKorisnickiProgram.Klase
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class karticeDB : DbContext
    {
        public karticeDB()
            : base("name=karticeDB")
        {
        }

        public virtual DbSet<kartice_tab> kartice_tab { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<kartice_tab>()
                .Property(e => e.korisnik)
                .IsUnicode(false);

            modelBuilder.Entity<kartice_tab>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<kartice_tab>()
                .Property(e => e.faks)
                .IsUnicode(false);

            modelBuilder.Entity<kartice_tab>()
                .Property(e => e.brtelefona)
                .IsUnicode(false);
        }
    }
}
