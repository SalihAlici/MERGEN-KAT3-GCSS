using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MERGEN_KAT3_GCSS
{
    public class TelemetryData
    {
        public int PaketNumarasi { get; set; }
        public int UyduStatusu { get; set; }
        public string HataKodu { get; set; }
        public string GondermeSaati { get; set; }
        public float Basinc1 { get; set; }
        public float Basinc2 { get; set; }
        public float Yukseklik1 { get; set; }
        public float Yukseklik2 { get; set; }
        public float IrtifaFarki { get; set; }
        public float InisHizi { get; set; }
        public float Sicaklik { get; set; }
        public float PilGerilimi { get; set; }
        public double Gps1Latitude { get; set; }
        public double Gps1Longitude { get; set; }
        public float Gps1Altitude { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Yaw { get; set; }
        public string RHRH { get; set; }
        public float IoTS1Data { get; set; }
        public float IoTS2Data { get; set; }
        public int TakimNo { get; set; }

        public static TelemetryData Parse(string rawData)
        {
            try
            {
                string[] parts = rawData.Trim().Split('*');
                if (parts.Length < 22) return null;

                return new TelemetryData
                {
                    PaketNumarasi = int.Parse(parts[0]),
                    UyduStatusu = int.Parse(parts[1]),
                    HataKodu = parts[2],
                    GondermeSaati = parts[3],
                    Basinc1 = float.Parse(parts[4], CultureInfo.InvariantCulture),
                    Basinc2 = float.Parse(parts[5], CultureInfo.InvariantCulture),
                    Yukseklik1 = float.Parse(parts[6], CultureInfo.InvariantCulture),
                    Yukseklik2 = float.Parse(parts[7], CultureInfo.InvariantCulture),
                    IrtifaFarki = float.Parse(parts[8], CultureInfo.InvariantCulture),
                    InisHizi = float.Parse(parts[9], CultureInfo.InvariantCulture),
                    Sicaklik = float.Parse(parts[10], CultureInfo.InvariantCulture),
                    PilGerilimi = float.Parse(parts[11], CultureInfo.InvariantCulture),
                    Gps1Latitude = double.Parse(parts[12], CultureInfo.InvariantCulture),
                    Gps1Longitude = double.Parse(parts[13], CultureInfo.InvariantCulture),
                    Gps1Altitude = float.Parse(parts[14], CultureInfo.InvariantCulture),
                    Pitch = float.Parse(parts[15], CultureInfo.InvariantCulture),
                    Roll = float.Parse(parts[16], CultureInfo.InvariantCulture),
                    Yaw = float.Parse(parts[17], CultureInfo.InvariantCulture),
                    RHRH = parts[18],
                    IoTS1Data = float.Parse(parts[19], CultureInfo.InvariantCulture),
                    IoTS2Data = float.Parse(parts[20], CultureInfo.InvariantCulture),
                    TakimNo = int.Parse(parts[21])
                };
            }
            catch
            {
                return null;
            }
        }
    }

}
