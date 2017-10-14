using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Windows;

namespace WpfKorisnickiProgram.Klase
{
    class karticeDal
    {
        private karticeDB db = new karticeDB();
        public List<kartice_tab> VratiKartice()
        {
            try
            {
                return db.kartice_tab.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool UnesiKarticu(kartice_tab k)
        {
            try
            {
                db.kartice_tab.Add(k);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                db.Entry(k).State = EntityState.Detached;
                MessageBox.Show("Greska sa serverom.");
                return false;
            }
        }
        public bool IzmeniKarticu(kartice_tab k)
        {
            kartice_tab kar = db.kartice_tab.Find(k.KarticaId);
            try
            {
                //kup.ImeKompanije = k.ImeKompanije; kup.PIB = k.PIB; kup.Adresa = k.Adresa; kup.Drzava = k.Drzava; kup.Grad = k.Grad; kup.Opstina = k.Opstina;
                //kup.PostanskiBroj = k.PostanskiBroj; kup.Telefon = k.Telefon; kup.FAX = k.FAX; kup.Email = k.Email; kup.Bilans = k.Bilans;
                db.Entry(k).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                db.Entry(k).State = EntityState.Detached;
                MessageBox.Show("Greska sa serverom.");
                return false;
            }
        }
        public bool ObrisiKarticu(kartice_tab k)
        {
            try
            {
                db.kartice_tab.Remove(k);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool IzvrsiUplatu(kartice_tab k)
        {
            kartice_tab kt = db.kartice_tab.Find(k.KarticaId);
            try
            {                               
                db.Entry(k).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                db.Entry(k).State = EntityState.Detached;
                MessageBox.Show("Greska sa serverom.");
                return false;
            }
        }
    }
}
