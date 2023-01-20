using GetFit.Classes;
using GetFit.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetFit.Formlar
{
    public partial class Raporlar : Form
    {
        UygulamaDbContext db = new UygulamaDbContext();
        Yiyecek secilenYiyecek;
        Ogun secilenOgun;
        Kullanici secilenKisi;
        public Raporlar()
        {
            InitializeComponent();
            YenenleriGetir();
            EncokYenenGetir();
            listBox1.SelectedIndex = -1;
            lstYenen.SelectedIndex = 0;
            OgunleriListele();
            KisileriGetir();
            OguneGoreKategoriListele();
        }

        private void KisileriGetir()
        {
            cmbKisiler.DataSource = db.Kullanicilar.ToList();
        }

        private void OgunleriListele()
        {
            cmbOgun.DataSource = db.Ogunler.ToList();

            cmbOgun2.DataSource = db.Ogunler.ToList();
        }

        private void OguneGoreYiyecekSayileriGetir()
        {
            secilenYiyecek = (Yiyecek)lstYenen.SelectedItem;
            secilenOgun = (Ogun)cmbOgun.SelectedItem;

            var sorgu = db.KullaniciYiyecekOgunler.Where(x => x.YiyecekId == secilenYiyecek.Id && x.OgunId == secilenOgun.Id).OrderBy(x => x.OgunId).GroupBy(x => x.OgunId).Select(x => new { OgunId = x.Key, YiyecekSayisi = x.Count() });
            listBox2.DataSource = sorgu.ToList();
        }

        private void YenenleriGetir()
        {
            var yiyecekler = db.Yiyecekler
             .Where(row => row.Id == db.KullaniciYiyecekOgunler
                                  .Where(kullaniciOgunYiyecekRow => kullaniciOgunYiyecekRow.YiyecekId == row.Id)
                                  .Select(kullaniciOgunYiyecekRow => kullaniciOgunYiyecekRow.YiyecekId)
                                  .FirstOrDefault());

            lstYenen.DataSource = yiyecekler.ToList();
        }


        private void EncokYenenGetir()
        {
            int enCok = 1;
            int enCokId = 1;
            var enTop = db.KullaniciYiyecekOgunler.OrderBy(x => x.YiyecekId)
                .GroupBy(x => x.YiyecekId)
                .Select(x => new { YiyecekId = x.Key, YiyecekSayisi = x.Count() });
            var sorgu = enTop.OrderByDescending(x => x.YiyecekSayisi).Take(3).Select(x => x.YiyecekId).ToList();
            List<string> adlar = new List<string>();
            foreach (var item in sorgu)
            {
                string ad = db.Yiyecekler.Find(item).Ad;
                adlar.Add(ad);
            }

            listBox1.DataSource = adlar;
        }

        private void cmbOgun_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            OguneGoreYiyecekSayileriGetir();
        }

        
        private void cmbOgun2_SelectedIndexChanged(object sender, EventArgs e)
        {
            OguneGoreKategoriListele();
        }

        private void OguneGoreKategoriListele()
        {
            if (cmbKisiler.SelectedIndex == -1) return;
            secilenKisi = (Kullanici)cmbKisiler.SelectedItem;
            if (cmbOgun2.SelectedIndex == 0)
            {
                KategoriAdlariListele(1);
            }
            if (cmbOgun2.SelectedIndex == 1)
            {
                KategoriAdlariListele(2);
            }
            if (cmbOgun2.SelectedIndex == 2)
            {
                KategoriAdlariListele(3);
            }
            if (cmbOgun2.SelectedIndex == 3)
            {
                KategoriAdlariListele(4);
            }
        }

        private void KategoriAdlariListele(int ogunId)
        {
            var sorgu = db.KullaniciYiyecekOgunler.Where(x => x.KullaniciId == secilenKisi.Id && x.OgunId == ogunId && x.Tarih <= dtpBitis.Value && x.Tarih >= dtpBaslangic.Value).Select(x => x.YiyecekId);
            List<int> kategoriIdler = new List<int>();
            List<string> kategoriAdlar = new List<string>();

            foreach (var item in sorgu)
            {
                int sorgu2 = db.Yiyecekler.Find(item).KategoriId;
                kategoriIdler.Add(sorgu2);
            }
            foreach (var item in kategoriIdler)
            {
                string sorgu3 = db.Kategoriler.Find(item).KategoriAd;
                if (!kategoriAdlar.Contains(sorgu3))
                {
                    kategoriAdlar.Add(sorgu3);
                }
            }
            lstKategori.DataSource = kategoriAdlar;
        }

        private void Raporlar_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void pcbGeri_Click(object sender, EventArgs e)
        {
            YemekDuzenlemeEkrani yemekDuzenlemeEkrani = new YemekDuzenlemeEkrani();
            yemekDuzenlemeEkrani.Show();
            this.Hide();
        }
    }
}
