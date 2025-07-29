using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;

namespace MERGEN_KAT3_GCSS
{
    public class Charts
    {
        private readonly Chart[] _charts;
        private const int MAX_POINTS = 100;

        public Charts(Chart ch1, Chart ch2, Chart ch3, Chart ch4, Chart ch5, Chart ch6, Chart ch7, Chart ch8)
        {
            _charts = new[] { ch1, ch2, ch3, ch4, ch5, ch6, ch7, ch8 };

            // Chart optimizasyonları
            foreach (var chart in _charts)
            {
                // Double buffering
                typeof(Chart).GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic)?
                    .SetValue(chart, true, null);

                foreach (var series in chart.Series)
                {
                    
                    series.BorderWidth = 1;
                }
            }
        }

        public void Update(TelemetryData data)
        {
            if (_charts[0].InvokeRequired)
            {
                _charts[0].BeginInvoke((MethodInvoker)(() => Update(data)));
                return;
            }

            string timeLabel = FormatTimeLabel(data.GondermeSaati);

         
            UpdateChart(_charts[0], timeLabel, data.Basinc);

           
            UpdateChart(_charts[1], timeLabel, data.Yukseklik);

            
            UpdateChart(_charts[2], timeLabel, data.DikeyHiz);

            
            UpdateChart(_charts[3], timeLabel, data.Mesafe);

            
            UpdateChart(_charts[4], timeLabel, data.SicaklikIc);

            
            UpdateChart(_charts[5], timeLabel, data.SicaklikDis);

            
            UpdateChart(_charts[6], timeLabel, data.PilGerilimi);

            
            UpdateChart(_charts[7], timeLabel, data.YatayHiz);
        }

        private void UpdateChart(Chart chart, string timeLabel, params double[] values)
        {
            for (int i = 0; i < values.Length && i < chart.Series.Count; i++)
            {
                var series = chart.Series[i];
                if (series.Points.Count > MAX_POINTS)
                {
                    series.Points.RemoveAt(0);
                }
                series.Points.AddXY(timeLabel, values[i]);
            }
        }

        private string FormatTimeLabel(string rawTime)
        {
            string timePart = rawTime.Contains(";") ? rawTime.Split(';')[1] : rawTime;
            return timePart.Replace('/', ':');
        }
    }
}
