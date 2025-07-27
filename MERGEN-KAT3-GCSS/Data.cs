using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        _3DSimulation _simulation;
        private readonly AngleCalculator _angleCalculator;
        private readonly Aras _aras;
        private readonly AntennaController _antennaController;
        // Thread-safe kuyruklar
        private readonly BlockingCollection<TelemetryData> _processingQueue = new BlockingCollection<TelemetryData>(1000);
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        // İşlemci thread'leri
        private Thread _serialReaderThread;
        private Thread _dataProcessorThread;

        public Data(SerialPort port, Map map, Form1 form, Charts charts,_3DSimulation simulation,
                   DataGridViewHandler dataGridHandler,  Aras aras, AngleCalculator angleCalculator,AntennaController antennaController)
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

            InitializeThreads();
        }

        private void InitializeThreads()
        {
            _serialReaderThread = new Thread(ReadSerialData)
            {
                Name = "SerialReaderThread",
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };

            _dataProcessorThread = new Thread(ProcessData)
            {
                Name = "DataProcessorThread",
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };

            _serialReaderThread.Start();
            _dataProcessorThread.Start();
        }

        private void ReadSerialData()
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    if (_port.IsOpen && _port.BytesToRead > 0)
                    {
                        string data = _port.ReadLine().Trim();
                        Console.WriteLine(data);
                        if (TelemetryData.TryParse(data, out var telemetry))
                        {   
                            _processingQueue.Add(telemetry, _cts.Token);
                        }
                    }
                    Thread.Sleep(1); // CPU kullanımını azalt
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Serial Read Error: {ex.Message}");
                    Thread.Sleep(100); // Hata durumunda bekle
                }
            }
        }

        private void ProcessData()
        {
            foreach (var telemetry in _processingQueue.GetConsumingEnumerable(_cts.Token))
            {
                try
                {
                    // UI güncellemelerini tek Invoke'da topla
                    _form.Invoke((MethodInvoker)(() =>
                    {
                        _simulation.UpdateRotation(telemetry.Yaw, telemetry.Pitch, telemetry.Roll);
                        _map.UpdatePosition(telemetry);
                        _charts.Update(telemetry);
                        _angleCalculator.CalculateAngles(telemetry);
                        _antennaController.UpdateAnglesFromTelemetry(telemetry);
                        _dataGridHandler.AddTelemetry(telemetry);
                        _aras.Update(telemetry.HataKodu);
                    }));

                    // Güncelleme hızını sınırla (max 30 FPS)
                    Thread.Sleep(33);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"UI Update Error: {ex.Message}");
                }
            }
        }

        public void Connect()
        {
            if (!_port.IsOpen)
            {
                _port.Open();
                Console.WriteLine("Serial port connected");
            }
        }

        public void Disconnect()
        {
            if (_port.IsOpen)
            {
                _port.Close();
                Console.WriteLine("Serial port disconnected");
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _processingQueue.CompleteAdding();

            _serialReaderThread?.Join(1000);
            _dataProcessorThread?.Join(1000);

            _cts.Dispose();
            _processingQueue.Dispose();
        }
    }
}
