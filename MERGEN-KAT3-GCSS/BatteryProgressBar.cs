using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MERGEN_KAT3_GCSS
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public partial class BatteryProgressBar : UserControl
    {
        private int _percentage;
        private Timer batteryUpdateTimer;
        private int currentIndex = 0;
        private int[] batteryLevels;

        public int Percentage
        {
            get => _percentage;
            set
            {
                if (_percentage != value)
                {
                    _percentage = Math.Max(0, Math.Min(100, value));
                    Invalidate(); // Yeniden çiz
                }
            }
        }

        private Color GetFillColor(int percentage)
        {
            if (percentage > 50) return Color.Lime;
            else if (percentage > 20) return Color.Orange;
            else return Color.Red;
        }

        public BatteryProgressBar()
        {
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Size = new Size(120, 40);

            // Rastgele pil seviyeleri oluştur
            batteryLevels = new int[20];
            Random rand = new Random();
            for (int i = 0; i < batteryLevels.Length; i++)
                batteryLevels[i] = rand.Next(0, 101);

            // Timer ayarla
            batteryUpdateTimer = new Timer();
            batteryUpdateTimer.Interval = 1000; // 1 saniye
            batteryUpdateTimer.Tick += BatteryUpdateTimer_Tick;
            batteryUpdateTimer.Start();
        }

        private void BatteryUpdateTimer_Tick(object sender, EventArgs e)
        {
            Percentage = batteryLevels[currentIndex];
            currentIndex = (currentIndex + 1) % batteryLevels.Length;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int tipWidth = 10;
            int bodyWidth = Width - tipWidth - 2;
            int bodyHeight = Height - 4;

            Rectangle bodyRect = new Rectangle(1, 2, bodyWidth, bodyHeight);
            Rectangle tipRect = new Rectangle(bodyRect.Right, bodyRect.Top + bodyHeight / 3, tipWidth, bodyHeight / 3);

            int fillWidth = (int)((_percentage / 100f) * (bodyWidth - 2));
            Rectangle fillRect = new Rectangle(bodyRect.Left + 1, bodyRect.Top + 1, fillWidth, bodyHeight - 2);

            // Dolum alanı: renk doluluk oranına göre
            using (SolidBrush brush = new SolidBrush(GetFillColor(_percentage)))
            {
                g.FillRectangle(brush, fillRect);
            }

            // Dolum kenar çizgisi (ince beyaz)
            using (Pen pen = new Pen(Color.White, 1))
            {
                g.DrawRectangle(pen, fillRect);
            }

            // Pil gövdesi kenarı (kalın, beyaz)
            using (Pen pen = new Pen(Color.White, 3))
            {
                g.DrawRectangle(pen, bodyRect.Left, bodyRect.Top, bodyRect.Width - 1, bodyRect.Height - 1);
            }

            // Çıkıntı dolgu beyaz
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, tipRect);
            }

            // Çıkıntı kenarı (kalın, beyaz)
            using (Pen pen = new Pen(Color.White, 3))
            {
                g.DrawRectangle(pen, tipRect.Left, tipRect.Top, tipRect.Width - 1, tipRect.Height - 1);
            }

            // Yüzde metni ortalanmış ve beyaz
            using (Font font = new Font("Arial", 12, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(Color.White))
            {
                string text = $"{_percentage}%";
                SizeF textSize = g.MeasureString(text, font);
                float x = bodyRect.Left + (bodyRect.Width - textSize.Width) / 2;
                float y = bodyRect.Top + (bodyRect.Height - textSize.Height) / 2;
                g.DrawString(text, font, textBrush, x, y);
            }
        }
    }
}
