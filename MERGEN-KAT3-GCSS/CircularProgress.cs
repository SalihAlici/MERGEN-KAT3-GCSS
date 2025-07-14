using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MERGEN_KAT3_GCSS
{
    public partial class CircularProgress : UserControl
    {
        private int currentAngle;
        private string currentText;
        private Label centerLabel;
        private Timer updateTimer;
        private Random random;

        public CircularProgress()
        {
            // InitializeComponent();
            SetupControl();
            random = new Random();
            updateTimer = new Timer
            {
                Interval = 1000 // Her saniye güncelleme
            };
            updateTimer.Tick += UpdateRandomAngle;
            updateTimer.Start();
        }

        private void SetupControl()
        {
            this.centerLabel = new Label();
            this.SuspendLayout();

            // centerLabel ayarları
            this.centerLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.centerLabel.Font = new Font("Arial", 28, FontStyle.Bold);
            this.centerLabel.AutoSize = false;
            this.centerLabel.BackColor = Color.Transparent;
            this.centerLabel.ForeColor = Color.Black; // İstersen değiştir

            // UserControl ayarları
            this.Controls.Add(this.centerLabel);
            this.BackColor = Color.Transparent;
            this.Size = new Size(480, 480);
            this.Padding = new Padding(5);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.UserPaint, true);

            this.ResumeLayout(false);
        }

        private void UpdateRandomAngle(object sender, EventArgs e)
        {
            int newAngle;
            do
            {
                newAngle = random.Next(0, 360); // 0-359 rastgele açı
            }
            while (newAngle == currentAngle);

            currentAngle = newAngle;
            currentText = GenerateRandomString(4);
            centerLabel.Text = currentText;
            Invalidate(); // Yeniden çiz
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(stringChars);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int padding = 40; // Önceki 30'dan küçülttüm, böylece daha fazla alan kalır
            int diameter = Math.Min(Width, Height) - padding * 2;

            int radius = diameter / 2 - 20; // 40 yerine 10 yaptım, yarıçap daha büyük
            int centerX = Width / 2;
            int centerY = Height / 2;

            Rectangle arcRect = new Rectangle(centerX - radius, centerY - radius, radius * 2, radius * 2);

            // Daireyi radial gradient ile doldur
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(arcRect);
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.LightBlue;
                    brush.SurroundColors = new Color[] { Color.White };
                    g.FillEllipse(brush, arcRect);
                }
            }

            // Kenar çizgisi
            using (Pen arcPen = new Pen(Color.White, 3))
            {
                g.DrawEllipse(arcPen, arcRect);
            }

            // Güncel açıyı gösteren kırmızı çizgi
            double radians = -currentAngle * Math.PI / 180;
            int lineEndX = centerX + (int)(radius * Math.Cos(radians));
            int lineEndY = centerY + (int)(radius * Math.Sin(radians));
            using (Pen linePen = new Pen(Color.Red, 2))
            {
                g.DrawLine(linePen, centerX, centerY, lineEndX, lineEndY);
            }

            // 45° aralıklarla tick çizgileri ve etiketler
            for (int i = 0; i < 360; i += 45)
            {
                double tickRad = -i * Math.PI / 180;
                int tickEndX = centerX + (int)(radius * Math.Cos(tickRad));
                int tickEndY = centerY + (int)(radius * Math.Sin(tickRad));
                using (Pen tickPen = new Pen(Color.Gray, 1))
                {
                    g.DrawLine(tickPen, centerX, centerY, tickEndX, tickEndY);
                }

                int tickOffset = 12;
                using (Font tickFont = new Font("Arial", 10, FontStyle.Regular))
                {
                    string tickText = $"{i}°";
                    SizeF textSize = g.MeasureString(tickText, tickFont);
                    float labelX = centerX + (radius + tickOffset) * (float)Math.Cos(tickRad) - textSize.Width / 2;
                    float labelY = centerY + (radius + tickOffset) * (float)Math.Sin(tickRad) - textSize.Height / 2;
                    using (SolidBrush tickBrush = new SolidBrush(Color.Gray))
                    {
                        g.DrawString(tickText, tickFont, tickBrush, labelX, labelY);
                    }
                }
            }

            // Güncel açıyı metin olarak göster
            int extraOffset = 40;
            using (Font redFont = new Font("Arial", 16, FontStyle.Bold))
            using (SolidBrush redBrush = new SolidBrush(Color.DarkOrange))
            {
                string angleText = $"{currentAngle}°";
                SizeF textSize = g.MeasureString(angleText, redFont);
                float textX = centerX + (radius + extraOffset) * (float)Math.Cos(radians) - textSize.Width / 2;
                float textY = centerY + (radius + extraOffset) * (float)Math.Sin(radians) - textSize.Height / 2;
                g.DrawString(angleText, redFont, redBrush, textX, textY);
            }

            // Ortadaki label'ı büyüt
            int labelSize = (int)(radius * 1.2); // Önceki 0.8 iken şimdi 1.2, label büyüdü
            centerLabel.Bounds = new Rectangle(centerX - labelSize / 2, centerY - labelSize / 2, labelSize, labelSize);
            centerLabel.Font = new Font("Arial", Math.Max(labelSize / 4, 14), FontStyle.Bold);
        }

    }
}
