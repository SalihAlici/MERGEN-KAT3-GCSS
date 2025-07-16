using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MERGEN_KAT3_GCSS
{
    public class DataGridViewHandler
    {
        private readonly BindingList<TelemetryData> _list;
        private readonly DataGridView _dataGridView;
        private readonly string _csvFilePath;
        private readonly object _csvLock = new object();
        private const int MAX_ROWS = 500;

        public DataGridViewHandler(DataGridView dgv, string csvFilePath)
        {
            _dataGridView = dgv;
            _csvFilePath = csvFilePath;
            _list = new BindingList<TelemetryData>();
            _dataGridView.AutoGenerateColumns = false;
            typeof(DataGridView).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic)?
                .SetValue(_dataGridView, true, null);

            _dataGridView.DataSource = _list;

            // GÖSTERMEK İSTEMEDİĞİNİZ KOLONLARI GİZLE
            string[] hiddenColumns =
            {
                "DonmeAcisi",
                "Gps2Latitude",
                "Gps2Longitude",
                "Gps2Altitude",
                "Kuzey"
            };

            _dataGridView.DataBindingComplete += (s, e) =>
            {
                foreach (DataGridViewColumn col in _dataGridView.Columns)
                {
                    // Hem sıralamayı engelle hem de istenmeyenleri gizle
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;

                    if (hiddenColumns.Contains(col.DataPropertyName))
                    {
                        col.Visible = false;
                    }
                }
            };
        }

        public void AddTelemetry(TelemetryData data)
        {
            if (_dataGridView.InvokeRequired)
            {
                _dataGridView.BeginInvoke((MethodInvoker)(() => AddTelemetry(data)));
                return;
            }

            // Scroll pozisyonunu koruma
            int firstDisplayedIndex = _dataGridView.FirstDisplayedScrollingRowIndex;

            _list.Insert(0, data);

            if (_list.Count > MAX_ROWS)
            {
                _list.RemoveAt(_list.Count - 1);
            }

            // Scroll pozisyonunu geri yükle
            if (_dataGridView.RowCount > 0 && firstDisplayedIndex >= 0 && firstDisplayedIndex < _dataGridView.RowCount)
            {
                _dataGridView.FirstDisplayedScrollingRowIndex = firstDisplayedIndex;
            }

            // CSV'ye yaz (tüm veriler korunuyor)
            Task.Run(() => AppendToCsv(data));
        }

        private void AppendToCsv(TelemetryData data)
        {
            lock (_csvLock)
            {
                try
                {
                    var props = typeof(TelemetryData).GetProperties();

                    if (!File.Exists(_csvFilePath))
                    {
                        // CSV başlık satırı (tüm sütunlar dahil)
                        File.WriteAllText(_csvFilePath,
                            string.Join(";", props.Select(p => p.Name)) + Environment.NewLine,
                            Encoding.UTF8);
                    }

                    // Tüm veriler CSV'ye yazılıyor (gizli sütunlar dahil)
                    File.AppendAllText(_csvFilePath,
                        string.Join(";", props.Select(p => p.GetValue(data)?.ToString() ?? "")) + Environment.NewLine,
                        Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CSV Write Error: {ex.Message}");
                }
            }
        }

        public void Clear()
        {
            _list.Clear();
        }
    }
}
