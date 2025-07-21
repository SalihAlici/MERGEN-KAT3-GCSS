using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MERGEN_KAT3_GCSS
{
    public class AngleCalculator
    {
        // Dünya yarıçapı (metre cinsinden)
        private const double EarthRadius = 6371000;
        private double Latitude0, Longitude0, Altitude0;
        private double Latitude1, Longitude1, Altitude1;
        public AngleCalculator()
        {
            // Artık referans noktasını constructor'da almıyoruz,
            // çünkü referans noktası GPS2'den gelecek
        }
        public void SetCoordinates(double latitude0, double longitude0, double altitude0,
                               double latitude1, double longitude1, double altitude1)
        {
            Latitude0 = latitude0;
            Longitude0 = longitude0;
            Altitude0 = altitude0;

            Latitude1 = latitude1;
            Longitude1 = longitude1;
            Altitude1 = altitude1;

            Console.WriteLine($"GS Konumu Ayarlandı: Latitude={latitude0}, Longitude={longitude0}, Altitude={altitude0}");

            //  CalculateAngles();
        }

        public (double HorizontalDistance, double VerticalDistance, double Azimuth, double Elevation) ManuelCalculateAngles()
        {
            double lat0_rad = DegreeToRadian(Latitude0);
            double lon0_rad = DegreeToRadian(Longitude0);
            double lat1_rad = DegreeToRadian(Latitude1);
            double lon1_rad = DegreeToRadian(Longitude1);

            double dLat = lat1_rad - lat0_rad;
            double dLon = lon1_rad - lon0_rad;

            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(lat0_rad) * Math.Cos(lat1_rad) * Math.Pow(Math.Sin(dLon / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double horizontalDistance = EarthRadius * c;

            double verticalDistance = Altitude1 - Altitude0;

            double azimuth = Math.Atan2(
                Math.Sin(dLon) * Math.Cos(lat1_rad),
                Math.Cos(lat0_rad) * Math.Sin(lat1_rad) - Math.Sin(lat0_rad) * Math.Cos(lat1_rad) * Math.Cos(dLon)
            ) * (180.0 / Math.PI);
            if (azimuth < 0) azimuth += 360;

            double elevation = Math.Atan2(verticalDistance, horizontalDistance) * (180.0 / Math.PI);


            Console.WriteLine($"Manual Hesaplama Yapıldı: azimuth={azimuth}");
            Console.WriteLine($"Manual Hesaplama Yapıldı: el={elevation}");


            return (horizontalDistance, verticalDistance, azimuth ,elevation);
        }

        public (double horizontalDistance, double verticalDistance, double azimuth, double elevation) CalculateAngles(TelemetryData telemetry)
        {
            // GPS1 = Uydu (hedef)
            Console.WriteLine("UYDU: " + telemetry.Gps1Latitude + " "+  telemetry.Gps1Longitude + " " + telemetry.Gps1Altitude);
            Console.WriteLine("YER ISTASYONU: " + telemetry.Gps2Latitude + " " + telemetry.Gps2Longitude + " " + telemetry.Gps2Altitude);
            double targetLat = telemetry.Gps1Latitude;
            double targetLon = telemetry.Gps1Longitude;
            double targetAlt = telemetry.Gps1Altitude;

            // GPS2 = Yer istasyonu (referans)
            double refLat = telemetry.Gps2Latitude;
            double refLon = telemetry.Gps2Longitude;
            double refAlt = telemetry.Gps2Altitude;

            // Dereceyi radyana çevirme
            double lat0_rad = DegreeToRadian(refLat);
            double lon0_rad = DegreeToRadian(refLon);
            double lat1_rad = DegreeToRadian(targetLat);
            double lon1_rad = DegreeToRadian(targetLon);

            // Koordinat farkları
            double dLat = lat1_rad - lat0_rad;
            double dLon = lon1_rad - lon0_rad;

            // Yatay mesafe hesaplama (Haversine formülü)
            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(lat0_rad) * Math.Cos(lat1_rad) * Math.Pow(Math.Sin(dLon / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double horizontalDistance = EarthRadius * c;

            // Dikey mesafe hesaplama
            double verticalDistance = targetAlt - refAlt;

            // Azimuth hesaplama
            double azimuth = Math.Atan2(
                Math.Sin(dLon) * Math.Cos(lat1_rad),
                Math.Cos(lat0_rad) * Math.Sin(lat1_rad) - Math.Sin(lat0_rad) * Math.Cos(lat1_rad) * Math.Cos(dLon)
            ) * (180.0 / Math.PI);

            // Azimuth'ü 0-360 derece aralığına getirme
            azimuth = NormalizeAngle(azimuth);

            // Elevation hesaplama
            double elevation = Math.Atan2(verticalDistance, horizontalDistance) * (180.0 / Math.PI);
            //Console.WriteLine(azimuth);
            return (horizontalDistance, verticalDistance, azimuth, elevation);
        }


        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private double NormalizeAngle(double angle)
        {
            // Açıyı 0-360 aralığına normalize et
            while (angle < 0) angle += 360;
            while (angle >= 360) angle -= 360;
            return angle;
        }

        public string GenerateServoCommand(TelemetryData telemetry)
        {
            var (_, _, azimuth, elevation) = CalculateAngles(telemetry);

            // Servo motor protokolüne uygun komut oluşturma
            // Örnek format: "AZ 180.50 EL 45.25" (Azimuth ve Elevation değerleri)
            // Gerçek format servo motor kontrolcünüzün beklediği formata göre değişir
            return $"{azimuth:F2},{elevation:F2}";
        }
    }
}
