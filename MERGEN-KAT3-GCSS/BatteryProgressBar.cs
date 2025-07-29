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


    public partial class BatteryProgressBar : UserControl
    {
        private int _percentage;

        public int Percentage
        {
            get => _percentage;
            set
            {
                if (_percentage != value)
                {
                    _percentage = Math.Max(0, Math.Min(100, value));
                    Invalidate();
                }
            }
        }

        public BatteryProgressBar()
        {
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Size = new Size(120, 40);
        }

        public void Update(TelemetryData telemetry)
        {
            this.Percentage = telemetry.PilDurumu;
        }

        private Color GetFillColor(int percentage)
        {
            if (percentage > 50) return Color.Lime;
            else if (percentage > 20) return Color.Orange;
            else return Color.Red;
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

            using (SolidBrush brush = new SolidBrush(GetFillColor(_percentage)))
            {
                g.FillRectangle(brush, fillRect);
            }

            using (Pen pen = new Pen(Color.White, 1))
            {
                g.DrawRectangle(pen, fillRect);
            }

            using (Pen pen = new Pen(Color.White, 3))
            {
                g.DrawRectangle(pen, bodyRect.Left, bodyRect.Top, bodyRect.Width - 1, bodyRect.Height - 1);
            }

            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, tipRect);
            }

            using (Pen pen = new Pen(Color.White, 3))
            {
                g.DrawRectangle(pen, tipRect.Left, tipRect.Top, tipRect.Width - 1, tipRect.Height - 1);
            }

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
