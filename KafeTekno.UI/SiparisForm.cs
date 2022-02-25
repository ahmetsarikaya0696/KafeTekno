using KafeTekno.DATA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KafeTekno.UI
{
    public partial class SiparisForm : Form
    {
        public event EventHandler<MasaTasindiEventArgs> MasaTasindi;
        private readonly KafeVeri db;
        private readonly Siparis siparis;
        private readonly BindingList<SiparisDetay> blSiparisDetaylar;
        public SiparisForm(KafeVeri db, Siparis siparis)
        {
            this.db = db;
            this.siparis = siparis;
            blSiparisDetaylar = new BindingList<SiparisDetay>(siparis.SiparisDetaylar);
            InitializeComponent();
            cboUrunSec.DataSource = db.Urunler;
            dgvSiparisDetaylar.DataSource = blSiparisDetaylar;
            blSiparisDetaylar.ListChanged += BlSiparisDetaylar_ListChanged;
            MasaNoGuncelle();
            OdemeTutariniGuncelle();
        }

        private void BlSiparisDetaylar_ListChanged(object sender, ListChangedEventArgs e)
        {
            OdemeTutariniGuncelle();
        }

        private void OdemeTutariniGuncelle()
        {
            lblOdemeTutari.Text = siparis.ToplamTutarTL;
        }

        private void MasaNoGuncelle()
        {
            Text = $"Masa {siparis.MasaNo:00} (Açılış Zamanı: {siparis.AcilisZamani})";
            lblMasaNo.Text = siparis.MasaNo.ToString();

            cboMasaNoSec.DataSource = Enumerable
                .Range(1, db.MasaAdet)
                .Where(x => !db.AktifSiparisler.Any(s => s.MasaNo == x))
                .ToList();

            #region MyRegion
            //cboMasaNoSec.Items.Clear();
            //for (int i = 1; i <= db.MasaAdet; i++)
            //{
            //    if (!db.AktifSiparisler.Any(x => x.MasaNo == i))
            //    {
            //        cboMasaNoSec.Items.Add(i);
            //    }
            //} 
            #endregion
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (cboUrunSec.SelectedIndex == -1) return;
            Urun urun = (Urun)cboUrunSec.SelectedItem;

            SiparisDetay sd = new SiparisDetay() { UrunAd = urun.UrunAd, BirimFiyat = urun.BirimFiyat, Adet = (int)nudAdet.Value };
            blSiparisDetaylar.Add(sd); // Bindinlist hem siparis detaylarını hem de kendini güncelliyor ve listede yeni eklenen oldukça refresh ediyor.
            EkleFormunuSifirla();
        }

        private void EkleFormunuSifirla()
        {
            cboUrunSec.SelectedIndex = 0;
            nudAdet.Value = 1;
        }

        private void dgvSiparisDetaylar_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult dr = MessageBox.Show(
                "Silmek istediğinize emin misiniz?",
                "Detay Silme Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                 MessageBoxDefaultButton.Button2);

            e.Cancel = dr == DialogResult.No;
        }

        private void btnAnaSayfayaDon_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            MasayiKapat(SiparisDurum.Odendi, siparis.ToplamTutar());
        }

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            MasayiKapat(SiparisDurum.Iptal);
        }

        private void MasayiKapat(SiparisDurum durum, decimal odenenTutar = 0)
        {
            siparis.Durum = durum;
            siparis.KapanisZamani = DateTime.Now;
            siparis.OdenenTutar = odenenTutar;
            db.AktifSiparisler.Remove(siparis);
            db.GecmisSiparisler.Add(siparis);
            Close();
        }

        private void btnTasi_Click(object sender, EventArgs e)
        {
            if (cboMasaNoSec.SelectedItem == null) return;

            int eskiMasaNo = siparis.MasaNo;
            int yeniMasaNo = (int)cboMasaNoSec.SelectedItem;
            siparis.MasaNo = yeniMasaNo;
            MasaNoGuncelle();
            if (MasaTasindi != null)
            {
                MasaTasindi(this,new MasaTasindiEventArgs(eskiMasaNo, yeniMasaNo));
            }
        }
    }
}
