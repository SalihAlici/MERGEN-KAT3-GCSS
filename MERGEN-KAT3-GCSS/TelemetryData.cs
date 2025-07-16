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
        // VERİLER TAM SİZİN VERDİĞİNİZ SIRAYLA
        public int PaketNumarasi { get; set; }
        public int UyduStatusu { get; set; }
        public string HataKodu { get; set; }
        public string GondermeSaati { get; set; }
        public float Basinc1 { get; set; }
        public float Basinc2 { get; set; }
        public float Yukseklik1 { get; set; }
        public float Yukseklik2 { get; set; }
        public float DikeyHiz { get; set; }
        public float Mesafe { get; set; }
        public float YatayHiz { get; set; }
        public float SicaklikIc1 { get; set; }
        public float SicaklikIc2 { get; set; }
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
        public float Kuzey { get; set; }

        public static bool TryParse(string rawData, out TelemetryData telemetry)
        {
            telemetry = null;
            if (string.IsNullOrWhiteSpace(rawData)) return false;

            string[] parts = rawData.Trim().Split('*');
            if (parts.Length < 28) return false; // Toplam 28 alan kontrolü

            telemetry = new TelemetryData
            {
                // SİZİN VERDİĞİNİZ SIRAYLA parse işlemleri
                PaketNumarasi = SafeParseInt(parts[0]),
                UyduStatusu = SafeParseInt(parts[1]),
                HataKodu = parts[2],
                GondermeSaati = parts[3],
                Basinc1 = SafeParseFloat(parts[4]),
                Basinc2 = SafeParseFloat(parts[5]),
                Yukseklik1 = SafeParseFloat(parts[6]),
                Yukseklik2 = SafeParseFloat(parts[7]),
                DikeyHiz = SafeParseFloat(parts[8]),
                Mesafe = SafeParseFloat(parts[9]),
                YatayHiz = SafeParseFloat(parts[10]),
                SicaklikIc1 = SafeParseFloat(parts[11]),
                SicaklikIc2 = SafeParseFloat(parts[12]),
                SicaklikDis = SafeParseFloat(parts[13]),
                PilGerilimi = SafeParseFloat(parts[14]),
                PilDurumu = SafeParseFloat(parts[15]),
                Gps1Latitude = SafeParseDouble(parts[16]),
                Gps1Longitude = SafeParseDouble(parts[17]),
                Gps1Altitude = SafeParseFloat(parts[18]),
                Pitch = SafeParseFloat(parts[19]),
                Roll = SafeParseFloat(parts[20]),
                Yaw = SafeParseFloat(parts[21]),
                DonmeAcisi = SafeParseFloat(parts[22]),
                Mesaj = parts[23],
                TakimNo = SafeParseInt(parts[24]),
                Gps2Latitude = SafeParseDouble(parts[25]),
                Gps2Longitude = SafeParseDouble(parts[26]),
                Gps2Altitude = SafeParseFloat(parts[27]),
                Kuzey = SafeParseFloat(parts[28])
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
            // VERİ SIRALAMASI AYNEN KORUNDU
            return $"{PaketNumarasi};{UyduStatusu};{HataKodu};{GondermeSaati};" +
                   $"{Basinc1};{Basinc2};{Yukseklik1};{Yukseklik2};{DikeyHiz};" +
                   $"{Mesafe};{YatayHiz};{SicaklikIc1};{SicaklikIc2};{SicaklikDis};" +
                   $"{PilGerilimi};{PilDurumu};{Gps1Latitude};{Gps1Longitude};" +
                   $"{Gps1Altitude};{Pitch};{Roll};{Yaw};{DonmeAcisi};" +
                   $"{Mesaj};{TakimNo};{Gps2Latitude};{Gps2Longitude};{Gps2Altitude};{Kuzey}";
        }
    }

}
