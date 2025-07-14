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
    public partial class QuarterCircularProgress : UserControl
    {
        private int angle;
        private string valueLabel;
        private Label centerLabel; // Ortadaki rastgele harfleri gösterecek label
        private static readonly Random random = new Random();
        private Timer updateTimer;

        public string ValueLabel
        {
            get { return valueLabel; }
            set
            {
                valueLabel = value;
                Invalidate();
            }
        }

        public string CenterText
        {
            get { return centerLabel.Text; }
            set { centerLabel.Text = value; }
        }

        public int Angle
        {
            get { return angle; }
            set
            {
                // Açı 0-90 arasında sınırlı
                angle = Math.Max(0, Math.Min(90, value));
                ValueLabel = $"{angle}°";
                Invalidate();
            }
        }

        public QuarterCircularProgress()
        {
            // InitializeComponent();  // Bu satırı kaldır
            SetupControl();

            Angle = random.Next(0, 91);
            CenterText = GenerateRandomString(3);

            updateTimer = new Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            this.BackColor = Color.Transparent;
            this.Size = new Size(480, 480);
        }

        private void SetupControl()
        {
            centerLabel = new Label();
            centerLabel.BackColor = Color.Transparent;
            centerLabel.Font = new Font("Arial", 20F, FontStyle.Bold, GraphicsUnit.Point);
            centerLabel.ForeColor = Color.White;
            centerLabel.TextAlign = ContentAlignment.MiddleCenter;
            centerLabel.AutoSize = false;
            centerLabel.Location = new Point(0, 0);
            centerLabel.Size = new Size(100, 40);
            this.Controls.Add(centerLabel);
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            Angle = random.Next(0, 91);
            CenterText = GenerateRandomString(3);
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

        // Çeyrek dairenin hemen altına label'ı yerleştiren metot 
        private void UpdateLabelPosition(Rectangle arcRect)
        {
            int labelWidth = arcRect.Width / 3;
            int labelHeight = arcRect.Height / 6;
            int rightX = arcRect.Right;
            int rightY = arcRect.Y + arcRect.Height / 2; // Sağ uç, dikey ortalama
            int labelX = rightX - (labelWidth / 2) - 75;
            int labelY = rightY + 2;
            centerLabel.Bounds = new Rectangle(labelX, labelY, labelWidth, labelHeight);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int padding = 30;
            int diameter = Width - padding * 2;
            int radius = diameter / 2;
            int centerX = padding + radius;
            int centerY = padding + radius;

            // Çeyrek çemberin boyutları (%65 oranında)
            int quarterCircleDiameter = (int)(diameter * 0.85);
            int quarterCircleRadius = quarterCircleDiameter / 2;
            Rectangle arcRect = new Rectangle(
                padding + (diameter - quarterCircleDiameter) / 2,
                padding + (diameter - quarterCircleDiameter) / 2,
                quarterCircleDiameter,
                quarterCircleDiameter);

            // Radial gradient dolgu
            using (GraphicsPath quarterPath = new GraphicsPath())
            {
                quarterPath.AddArc(arcRect, 270, 90);
                quarterPath.AddLine(arcRect.Right, arcRect.Y + arcRect.Height / 2, centerX, centerY);
                quarterPath.AddLine(centerX, centerY, arcRect.X + arcRect.Width / 2, arcRect.Y);
                quarterPath.CloseFigure();

                using (PathGradientBrush brush = new PathGradientBrush(quarterPath))
                {
                    brush.CenterColor = Color.LightBlue;
                    brush.SurroundColors = new Color[] { Color.White };
                    g.FillPath(brush, quarterPath);
                }
            }

            UpdateLabelPosition(arcRect);

            // Çeyrek daire çizimi
            using (Pen arcPen = new Pen(Color.White, 3))
            {
                g.DrawArc(arcPen, arcRect, 270, 90);
                g.DrawLine(arcPen, centerX, centerY, centerX + quarterCircleRadius, centerY);
                g.DrawLine(arcPen, centerX, centerY, centerX, centerY - quarterCircleRadius);
            }

            // 30° aralıklarla yarıçap çizgileri ve açı etiketleri
            for (int i = 0; i <= 90; i += 30)
            {
                double rad = (-i) * Math.PI / 180;
                int endX = centerX + (int)(quarterCircleRadius * Math.Cos(rad));
                int endY = centerY + (int)(quarterCircleRadius * Math.Sin(rad));

                using (Pen pen = new Pen(Color.Gray, 1))
                {
                    g.DrawLine(pen, centerX, centerY, endX, endY);
                }

                int offset = 15;
                int textX = centerX + (int)((quarterCircleRadius + offset) * Math.Cos(rad)) - 5;
                int textY = centerY + (int)((quarterCircleRadius + offset) * Math.Sin(rad)) - 5;
                using (Font smallFont = new Font("Arial", 10, FontStyle.Regular))
                using (SolidBrush brush = new SolidBrush(Color.Gray))
                {
                    g.DrawString($"{i}°", smallFont, brush, textX, textY);
                }
            }

            // Güncel açı çizgisi
            double currentRad = (-angle) * Math.PI / 180;
            int currentEndX = centerX + (int)(quarterCircleRadius * Math.Cos(currentRad));
            int currentEndY = centerY + (int)(quarterCircleRadius * Math.Sin(currentRad));
            using (Pen currentPen = new Pen(Color.Red, 2))
            {
                g.DrawLine(currentPen, centerX, centerY, currentEndX, currentEndY);
            }

            // Güncel açı metni
            int extraOffset = 37;
            string currentAngleText = $"{angle}°";
            using (Font largeFont = new Font("Arial", 16, FontStyle.Bold))
            using (SolidBrush redBrush = new SolidBrush(Color.DarkOrange))
            {
                SizeF textSize = g.MeasureString(currentAngleText, largeFont);
                float currentTextX = centerX + (float)((quarterCircleRadius + extraOffset) * Math.Cos(currentRad)) - textSize.Width / 2;
                float currentTextY = centerY + (float)((quarterCircleRadius + extraOffset) * Math.Sin(currentRad)) - textSize.Height / 2;
                g.DrawString(currentAngleText, largeFont, redBrush, currentTextX, currentTextY);
            }
        }
    }
}
