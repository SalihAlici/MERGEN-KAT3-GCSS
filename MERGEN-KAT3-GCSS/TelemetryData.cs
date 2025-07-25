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
        // GÜNCELLENMİŞ VERİLER
        public int PaketNumarasi { get; set; }
        public int UyduStatusu { get; set; }
        public string HataKodu { get; set; }
        public string GondermeSaati { get; set; }
        public float Basinc { get; set; }
        public float Yukseklik { get; set; }
        public float DikeyHiz { get; set; }
        public float Mesafe { get; set; }
        public float YatayHiz { get; set; }
        public float SicaklikIc { get; set; }
        public float SicaklikDis { get; set; }
        public float PilGerilimi { get; set; }
        public float PilDurumu { get; set; }
        public double Gps1Latitude { get; set; }
        public double Gps1Longitude { get; set; }
        public float Gps1Altitude { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Yaw { get; set; }
        public float DonmeAcisi { get; set; }
        public string Mesaj { get; set; }
        public int TakimNo { get; set; }
        public double Gps2Latitude { get; set; }
        public double Gps2Longitude { get; set; }
        public float Gps2Altitude { get; set; }

        public static bool TryParse(string rawData, out TelemetryData telemetry)
        {
            telemetry = null;
            if (string.IsNullOrWhiteSpace(rawData)) return false;

            string[] parts = rawData.Trim().Split('*');
            if (parts.Length < 25) return false; // GÜNCELLENMİŞ ALAN SAYISI: 25

            telemetry = new TelemetryData
            {
                PaketNumarasi = SafeParseInt(parts[0]),
                UyduStatusu = SafeParseInt(parts[1]),
                HataKodu = parts[2],
                GondermeSaati = parts[3],
                Basinc = SafeParseFloat(parts[4]),
                Yukseklik = SafeParseFloat(parts[5]),
                DikeyHiz = SafeParseFloat(parts[6]),
                Mesafe = SafeParseFloat(parts[7]),
                YatayHiz = SafeParseFloat(parts[8]),
                SicaklikIc = SafeParseFloat(parts[9]),
                SicaklikDis = SafeParseFloat(parts[10]),
                PilGerilimi = SafeParseFloat(parts[11]),
                PilDurumu = SafeParseFloat(parts[12]),
                Gps1Latitude = SafeParseDouble(parts[13]),
                Gps1Longitude = SafeParseDouble(parts[14]),
                Gps1Altitude = SafeParseFloat(parts[15]),
                Pitch = SafeParseFloat(parts[16]),
                Roll = SafeParseFloat(parts[17]),
                Yaw = SafeParseFloat(parts[18]),
                DonmeAcisi = SafeParseFloat(parts[19]),
                Mesaj = parts[20],
                TakimNo = SafeParseInt(parts[21]),
                Gps2Latitude = SafeParseDouble(parts[22]),
                Gps2Longitude = SafeParseDouble(parts[23]),
                Gps2Altitude = SafeParseFloat(parts[24])
            };
            return true;
        }

        private static int SafeParseInt(string s)
        {
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : 0;
        }

        private static float SafeParseFloat(string s)
        {
            return float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float result) ? result : 0f;
        }

        private static double SafeParseDouble(string s)
        {
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double result) ? result : 0.0;
        }

        public override string ToString()
        {
            return $"{PaketNumarasi};{UyduStatusu};{HataKodu};{GondermeSaati};" +
                   $"{Basinc};{Yukseklik};{DikeyHiz};{Mesafe};{YatayHiz};" +
                   $"{SicaklikIc};{SicaklikDis};{PilGerilimi};{PilDurumu};" +
                   $"{Gps1Latitude};{Gps1Longitude};{Gps1Altitude};" +
                   $"{Pitch};{Roll};{Yaw};{DonmeAcisi};" +
                   $"{Mesaj};{TakimNo};{Gps2Latitude};{Gps2Longitude};{Gps2Altitude}";
        }
    }
}
