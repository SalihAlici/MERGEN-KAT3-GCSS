using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace MERGEN_KAT3_GCSS
{
    public class Data : IDisposable
    {
        private readonly SerialPort _port;
        private readonly Form1 _form;
        private readonly Map _map;
        private readonly Charts _charts;
        private readonly DataGridViewHandler _dataGridHandler;
        private readonly _3DSimulation _simulation;
        private readonly AngleCalculator _angleCalculator;
        private readonly Aras _aras;
        private readonly BatteryProgressBar _battery;
        private readonly CompassControl _compass;
        private readonly AntennaController _antennaController;

        private readonly BlockingCollection<TelemetryData> _processingQueue = new BlockingCollection<TelemetryData>(1000);

        private Thread _serialReaderThread;
        private Thread _dataProcessorThread;

        private readonly object _serialLock = new object();
        private volatile bool _running = true;

        private string _serialBuffer = "";  // Gelen veriyi biriktirmek için buffer

        public Data(SerialPort port, Map map, Form1 form, Charts charts, _3DSimulation simulation,
                    DataGridViewHandler dataGridHandler, Aras aras, AngleCalculator angleCalculator,
                    AntennaController antennaController, BatteryProgressBar battery, CompassControl compass)
        {
            _port = port;
            _map = map;
            _form = form;
            _charts = charts;
            _simulation = simulation;
            _angleCalculator = angleCalculator;
            _dataGridHandler = dataGridHandler;
            _antennaController = antennaController;
            _aras = aras;
            _battery = battery;
            _compass = compass;

            StartThreads();
        }

        private void StartThreads()
        {
            _running = true;

            _serialReaderThread = new Thread(ReadSerialData)
            {
                Name = "SerialReaderThread",
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };
            _serialReaderThread.Start();

            _dataProcessorThread = new Thread(ProcessData)
            {
                Name = "DataProcessorThread",
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            _dataProcessorThread.Start();
        }

        private void ReadSerialData()
        {
            while (_running)
            {
                try
                {
                    if (_port != null && _port.IsOpen && _port.BytesToRead > 0)
                    {
                        lock (_serialLock)
                        {
                            // Gelen tüm mevcut veriyi oku ve buffer'a ekle
                            _serialBuffer += _port.ReadExisting();
                        }

                        int startIndex, endIndex;
                        // Buffer'da tam paket var mı kontrol et
                        while ((startIndex = _serialBuffer.IndexOf('$')) != -1 &&
                               (endIndex = _serialBuffer.IndexOf('#', startIndex)) != -1)
                        {
                            // Başlangıç ve bitiş arasındaki paketi al (baş ve son karakter hariç)
                            string packet = _serialBuffer.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();

                            // İşlenen kısmı buffer'dan çıkar
                            _serialBuffer = _serialBuffer.Substring(endIndex + 1);

                            Console.WriteLine($"[Tam Paket Çıkarıldı] {packet}");

                            // Telemetri verisini parse et ve kuyruğa ekle
                            if (TelemetryData.TryParse(packet, out var telemetry))
                                _processingQueue.Add(telemetry);
                            else
                                Console.WriteLine("[Parse Hatası] Paket parse edilemedi.");
                        }

                        // Çok büyük buffer temizleme (ör: 10k karakter üstü)
                        if (_serialBuffer.Length > 10000)
                        {
                            Console.WriteLine("[UYARI] Buffer büyüdü, temizleniyor.");
                            _serialBuffer = "";
                        }
                    }

                    Thread.Sleep(5); // CPU yükünü azalt
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Serial Read Error: {ex.Message}");
                    Thread.Sleep(50);
                }
            }
        }

        private void ProcessData()
        {
            while (_running)
            {
                try
                {
                    if (_processingQueue.TryTake(out var telemetry, 50))
                    {
                        _form.Invoke((MethodInvoker)(() =>
                        {
                            int index = _form.tabControl1.SelectedIndex;

                            _angleCalculator.CalculateAngles(telemetry);
                            _antennaController.UpdateAnglesFromTelemetry(telemetry);
                            _battery.Update(telemetry);
                            _map.UpdatePosition(telemetry);
                            _charts.Update(telemetry);
                            _dataGridHandler.AddTelemetry(telemetry);
                            _aras.Update(telemetry.HataKodu);
                            _form.HandleNewTelemetryData(telemetry);

                            if (index == 0)
                            {
                                _simulation.UpdateRotation(telemetry.Yaw, telemetry.Pitch, telemetry.Roll);
                                _compass.SetAngle(telemetry);
                            }
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"UI Update Error: {ex.Message}");
                }
            }
        }

        public void SendCommand(string command)
        {
            if (_port == null || !_port.IsOpen) return;

            lock (_serialLock)
            {
                try
                {
                    _port.DiscardInBuffer();
                    _port.WriteLine(command);
                    Console.WriteLine($"Sent command: {command}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Serial Write Error: {ex.Message}");
                }
            }
        }

        public void Connect()
        {
            try
            {
                if (!_port.IsOpen)
                {
                    _port.Open();
                    _port.Encoding = System.Text.Encoding.ASCII;
                    _port.DiscardInBuffer();
                    _port.DiscardOutBuffer();
                    Console.WriteLine("Serial port connected");

                    if (_serialReaderThread == null || !_serialReaderThread.IsAlive ||
                        _dataProcessorThread == null || !_dataProcessorThread.IsAlive)
                    {
                        StartThreads();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connect Error: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_port.IsOpen)
                    _port.Close();

                Console.WriteLine("Serial port disconnected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Disconnect Error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _running = false;

            _serialReaderThread?.Join(500);
            _dataProcessorThread?.Join(500);

            _processingQueue.Dispose();
        }
    }
}
