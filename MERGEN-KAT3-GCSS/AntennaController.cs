using System;
using System.Globalization;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MERGEN_KAT3_GCSS
{



    public class AntennaController
    {
        double azimuth;
        double elevation;
        private AngleCalculator ang;
        private readonly SerialPort serialPort;
        private CancellationTokenSource trackingTokenSource; // Takip kontrolü için iptal tokeni

        public AntennaController(SerialPort serialPort)
        {
            this.serialPort = serialPort;
            ang = new AngleCalculator();
        }

        public void UpdateAnglesFromTelemetry(TelemetryData telemetry)
        {
            if (telemetry == null) return;
            Console.WriteLine("UpdateAngels calisti");
            var (_, _, az, el) = ang.CalculateAngles(telemetry);
            azimuth = az;
            elevation = el;
        }
        public void SetCoordinatesForAngles(double lat0, double lon0, double alt0, double lat1, double lon1, double alt1)
        {
            ang.SetCoordinates(lat0, lon0, alt0, lat1, lon1, alt1);
        }
        public void UpdateAnglesManually()
        {
           // Console.WriteLine("UpdateAnglesManually çağrıldı.");
            var (_, _, az, el) = ang.ManuelCalculateAngles();
            azimuth = az;
            elevation = el;
            
           
        }

        public void SendMotorAngles()
        {
            if (serialPort == null || !serialPort.IsOpen)
                return;

            
            //string command = $"{azimuth:F2},{elevation:F2}";
            string command = string.Format(CultureInfo.InvariantCulture, "{0:F2},{1:F2}", azimuth, elevation);

            serialPort.WriteLine(command);
            System.Console.WriteLine("Gönderilen komut: " + command);
        }

        public void SendMotorData(double stepAci, double servoAci)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                // Kültüre duyarlı biçimde, 3 ondalık hassasiyetle veri oluştur
                string data = $"{stepAci.ToString("0.###", CultureInfo.InvariantCulture)},{servoAci.ToString("0.###", CultureInfo.InvariantCulture)}";
                serialPort.WriteLine(data);  // Veriyi gönder
            }
            else
            {
                MessageBox.Show("Motor seri bağlantısı açık değil!", "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void SendMotorCommand(string command)
        {
            if ((serialPort != null && serialPort.IsOpen))
            {
                serialPort.WriteLine(command);
                //Console.WriteLine("Motor komutu gönderildi: " + command);
            }
        }

        public async Task StartTrackingAsync()
        {
            if (trackingTokenSource != null)
            {
                Console.WriteLine("Takip zaten aktif!");
                return;
            }

            trackingTokenSource = new CancellationTokenSource();
            CancellationToken token = trackingTokenSource.Token;

            try
            {
                Console.WriteLine("Takip başlatıldı.");

                while (!token.IsCancellationRequested)
                {
                    SendMotorAngles();
                    await Task.Delay(1000, token); // 1 saniye bekle (iptal destekli)
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Takip iptal edildi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Takip sırasında hata: {ex.Message}");
            }
            finally
            {
                trackingTokenSource.Dispose();
                trackingTokenSource = null;
            }
        }

        /// <summary>
        /// Takibi durdurur.
        /// </summary>
        public void StopTracking()
        {
            if (trackingTokenSource == null)
            {
                Console.WriteLine("Takip zaten durdurulmuş.");
                return;
            }

            trackingTokenSource.Cancel();
        }
    }

}
