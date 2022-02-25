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
    public partial class UrunlerForm : Form
    {
        private readonly KafeVeri db;
        private readonly BindingList<Urun> blUrunler;
        private Urun duzenlenen;

        public UrunlerForm(KafeVeri db)
        {
            this.db = db;
            blUrunler = new BindingList<Urun>(db.Urunler);
            InitializeComponent();
            dgvUrunler.DataSource = blUrunler;

        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string ad = txtUrunAd.Text.Trim();
            if (string.IsNullOrEmpty(ad))
            {
                MessageBox.Show("Bir ürün adı belirlemediniz.");
                return;
            }

            if (duzenlenen == null)
            {
                blUrunler.Add(new Urun() { UrunAd = ad, BirimFiyat = nudBirimFiyat.Value });
            }
            else
            {
                duzenlenen.UrunAd = ad;
                duzenlenen.BirimFiyat = nudBirimFiyat.Value;
            }
            FormuSifirla();
        }

        private void FormuSifirla()
        {
            txtUrunAd.Clear();
            nudBirimFiyat.Value = 0;
            btnIptal.Hide();
            dgvUrunler.Enabled = true;
            btnEkle.Text = "Kaydet";
            duzenlenen = null;
        }

        private void dgvUrunler_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;

            DataGridViewRow satir = dgvUrunler.Rows[e.RowIndex];
            duzenlenen = (Urun)satir.DataBoundItem;
            txtUrunAd.Text = duzenlenen.UrunAd;
            nudBirimFiyat.Value = duzenlenen.BirimFiyat;
            dgvUrunler.Enabled = false;
            btnEkle.Text = "Kaydet";
            // Btn iptali göster
            btnIptal.Show();
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            FormuSifirla();
        }
    }
}
