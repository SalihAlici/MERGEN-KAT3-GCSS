using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MERGEN_KAT3_GCSS
{
    public class Aras
    {
        // PictureBox dizisi hata lambaları için tutulur
        private PictureBox[] pictureBoxes;

        // PictureBox'ları parametre olarak alır ve saklar
        public Aras(params PictureBox[] pbs)
        {
            pictureBoxes = pbs;
        }

        // Hata durumunu güncelleyen metot
        public void Update(string error)
        {
            bool alarm = false; // Alarm durumu için bayrak başlangıçta false

            // Her PictureBox için hata durumunu kontrol et
            for (int i = 0; i < pictureBoxes.Length; i++)
            {
                if (i < error.Length && error[i] == '1') // Hata varsa 1
                {
                    pictureBoxes[i].BackColor = Color.Red; // Kırmızı renk 
                    alarm = true; // Alarm durumu aktif
                }
                else
                {
                    pictureBoxes[i].BackColor = Color.Chartreuse; // Hata yoksa veya karakter eksikse yeşil
                }
            }

            // Eğer alarm durumu oluşmuşsa uyarı sesi çal
            if (alarm)
            {
                SystemSounds.Exclamation.Play();
            }
        }
    }
}
