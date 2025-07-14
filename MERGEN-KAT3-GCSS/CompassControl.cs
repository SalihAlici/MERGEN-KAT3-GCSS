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
    public partial class CompassControl : UserControl
    {
        private float angle; // derece cinsinden

        public CompassControl()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.angle = 0f;
            this.BackColor = Color.Transparent;
        }

        /// <summary>
        /// Pusulanın açısını ayarlar (0–360 arası)
        /// </summary>
        public void SetAngle(float degrees)
        {
            angle = degrees % 360f;
            if (angle < 0) angle += 360f;
            Invalidate(); // Yeniden çiz
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawCompass(e.Graphics, this.ClientRectangle);
        }

        private void DrawCompass(Graphics g, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int cx = bounds.Width / 2;
            int cy = bounds.Height / 2;
            int radius = Math.Min(cx, cy) - 10;

            var circleRect = new Rectangle(cx - radius, cy - radius, radius * 2, radius * 2);

            // 1) Zemin: hafif gölgeli daire
            using (var bgBrush = new PathGradientBrush(new GraphicsPath(
                     new Point[] {
                         new Point(cx, cy - radius),
                         new Point(cx + radius, cy),
                         new Point(cx, cy + radius),
                         new Point(cx - radius, cy)
                     },
                     new byte[] {
                         (byte)PathPointType.Start,
                         (byte)PathPointType.Line,
                         (byte)PathPointType.Line,
                         (byte)PathPointType.Line
                     })))
            {
                bgBrush.CenterColor = Color.White;
                bgBrush.SurroundColors = new[] { Color.LightGray };
                g.FillEllipse(bgBrush, circleRect);
            }

            // 2) Dış çerçeve
            using (var pen = new Pen(Color.DimGray, 3))
                g.DrawEllipse(pen, circleRect);

            // 3) Derece işaretleri
            for (int deg = 0; deg < 360; deg += 10)
            {
                float rad = deg * (float)Math.PI / 180f;
                float cos = (float)Math.Cos(rad), sin = (float)Math.Sin(rad);

                int len = (deg % 90 == 0 ? 15 : deg % 30 == 0 ? 10 : 5);
                float x1 = cx + cos * (radius - 5);
                float y1 = cy + sin * (radius - 5);
                float x2 = cx + cos * (radius - 5 - len);
                float y2 = cy + sin * (radius - 5 - len);

                using (var tickPen = new Pen(Color.White, deg % 90 == 0 ? 2 : 1))
                    g.DrawLine(tickPen, x1, y1, x2, y2);
            }

            // 4) Yön harfleri (sadece ilk harf, kalın font)
            DrawDirection(g, "N", cx, cy - radius + 30);
            DrawDirection(g, "E", cx + radius - 30, cy);
            DrawDirection(g, "S", cx, cy + radius - 30);
            DrawDirection(g, "W", cx - radius + 30, cy);

            // 5) İğne: iki renkli üçgen
            DrawNeedle(g, cx, cy, radius - 40);

            // 6) Ortadaki düğme
            g.FillEllipse(Brushes.DimGray, cx - 8, cy - 8, 16, 16);
            g.FillEllipse(Brushes.LightGray, cx - 5, cy - 5, 10, 10);
        }

        private void DrawDirection(Graphics g, string text, float x, float y)
        {
            using (var font = new Font("Arial", 16, FontStyle.Bold))
            {
                SizeF sz = g.MeasureString(text, font);
                g.DrawString(text, font, Brushes.Black, x - sz.Width / 2, y - sz.Height / 2);
            }
        }

        private void DrawNeedle(Graphics g, int cx, int cy, int length)
        {
            // Kuzey ucunu kırmızı, güney ucunu beyaz çizelim
            float rad = angle * (float)Math.PI / 180f;
            float sin = (float)Math.Sin(rad), cos = (float)Math.Cos(rad);

            // Üçgenlerin noktaları
            PointF tip = new PointF(cx + sin * length, cy - cos * length);
            PointF left = new PointF(cx + cos * 10, cy + sin * 10);
            PointF right = new PointF(cx - cos * 10, cy - sin * 10);

            // Kuzeye bakan kırmızı üçgen
            using (var redBrush = new SolidBrush(Color.Red))
            {
                g.FillPolygon(redBrush, new[] { tip, left, right });
            }

            // Güneye bakan beyaz üçgen (tersi)
            PointF tip2 = new PointF(cx - sin * (length * 0.6f), cy + cos * (length * 0.6f));
            PointF left2 = new PointF(cx + cos * (-10), cy + sin * (-10));
            PointF right2 = new PointF(cx - cos * (-10), cy - sin * (-10));
            using (var whiteBrush = new SolidBrush(Color.White))
            {
                g.FillPolygon(whiteBrush, new[] { tip2, left2, right2 });
            }
        }
    }
}
