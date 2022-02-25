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

namespace KafeTekno.UI
{
    public partial class AnaForm : Form
    {
        KafeVeri db = new DATA.KafeVeri();
        public AnaForm()
        {
            InitializeComponent();
            OrnekUrunleriYuke();
            MasalariOlustur();
            #region MyRegion
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
                lvi.ImageKey = "bos";
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
            lvi.ImageKey = "dolu";
            new SiparisForm(db, siparis).ShowDialog(); // dialog kapanana kadar bekler

            if (siparis.Durum != SiparisDurum.Aktif)
                lvi.ImageKey = "bos";
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
    }
}
