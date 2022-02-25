using KafeTekno.UI.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KafeTekno.DATA;
using Newtonsoft.Json;
using System.IO;

namespace KafeTekno.UI
{
    public partial class AnaForm : Form
    {
        KafeVeri db;
        public AnaForm()
        {
            VerileriOku();
            InitializeComponent();
            //OrnekUrunleriYuke();
            MasalariOlustur();
            #region EskiKod
            //ImageList il = new ImageList();
            //il.ImageSize = new Size(64, 64);
            //il.Images.Add("bos", Resources.bos);
            //il.Images.Add("dolu", Resources.dolu);
            //lvwMasalar.LargeImageList = il;
            //lvwMasalar.Items.Add(new ListViewItem(new[] { "01", "Adana", "Akdeniz" }, "dolu"));
            //lvwMasalar.Items.Add(new ListViewItem(new[] { "01", "Adana", "Akdeniz" }, "bos")); 
            #endregion
        }


        private void OrnekUrunleriYuke()
        {
            db.Urunler.Add(new Urun { UrunAd = "Kola", BirimFiyat = 7.00m });
            db.Urunler.Add(new Urun { UrunAd = "Ayran", BirimFiyat = 5.00m });
        }

        private void MasalariOlustur()
        {
            lvwMasalar.LargeImageList = BuyukImajListesi();
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                ListViewItem lvi = new ListViewItem("Masa " + i);
                lvi.ImageKey = db.AktifSiparisler.Any(x => x.MasaNo == i) ? "dolu" : "bos";
                lvi.Tag = i;
                lvwMasalar.Items.Add(lvi);
            }
        }

        private ImageList BuyukImajListesi()
        {
            ImageList il = new ImageList();
            il.ImageSize = new Size(64, 64);
            il.Images.Add("bos", Resources.bos);
            il.Images.Add("dolu", Resources.dolu);
            return il;

        }

        private void lvwMasalar_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem lvi = lvwMasalar.SelectedItems[0];
            int masaNo = (int)lvi.Tag;
            Siparis siparis = SiparisBulYaDaOlustur(masaNo);
            SiparisForm sf = new SiparisForm(db, siparis);
            sf.MasaTasindi += Sf_MasaTasindi;
            sf.ShowDialog();
            lvi.ImageKey = "dolu";

            if (siparis.Durum != SiparisDurum.Aktif)
                lvi.ImageKey = "bos";
        }

        private void Sf_MasaTasindi(object sender, MasaTasindiEventArgs e)
        {
            MasaTasi(e.EskiMasaNo, e.YeniMasaNo);
        }

        private void MasaTasi(int eskiMasaNo, int yeniMasaNo)
        {
            foreach (ListViewItem lvi in lvwMasalar.Items)
            {
                int masaNo = (int)lvi.Tag;
                if (masaNo == eskiMasaNo)
                {
                    lvi.ImageKey = "bos";
                    lvi.Selected = false;
                }
                else if (masaNo == yeniMasaNo)
                {
                    lvi.ImageKey = "dolu";
                    lvi.Selected = true;
                }
            }
        }

        private Siparis SiparisBulYaDaOlustur(int masaNo)
        {
            Siparis siparis = db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == masaNo);
            if (siparis == null)
            {
                siparis = new Siparis() { MasaNo = masaNo };
                db.AktifSiparisler.Add(siparis);
            }
            return siparis;
        }

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            new GecmisSiparislerForm(db).ShowDialog();
        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            new UrunlerForm(db).ShowDialog();
        }
        private void VerileriOku()
        {
            try
            {
                string fromJson = File.ReadAllText("veriler.json");
                db = JsonConvert.DeserializeObject<KafeVeri>(fromJson);
            }
            catch (Exception)
            {
                db = new KafeVeri();
                OrnekUrunleriYuke();
            }
        }
        private void VerileriKaydet()
        {
            string toJson = JsonConvert.SerializeObject(db);
            File.WriteAllText("veriler.json", toJson);
        }

        private void AnaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            VerileriKaydet();
        }
    }
}
