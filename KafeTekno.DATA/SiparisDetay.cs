using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafeTekno.DATA
{

    /*
     * SiparisDetay
* UrunAd: string
* BirimFiyat: decimal
* Adet: int
* TutarTL: string-readonly
- Tutar(): decimal*/

    public class SiparisDetay
    {
        public string UrunAd { get; set; }
        public decimal BirimFiyat { get; set; }
        public int Adet { get; set; }
        public string TutarTL => $"₺{Tutar():n2}";
        public decimal Tutar()
        {
            return Adet * BirimFiyat;
        }
    }
}
