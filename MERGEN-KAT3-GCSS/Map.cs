using GMap.NET.MapProviders;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MERGEN_KAT3_GCSS
{
    public class Map
    {
        private GMapControl gMapControl;
        private GMapMarker marker;
        private GMapOverlay markers;

        private double lastLat = 0;
        private double lastLng = 0;
        private const double threshold = 0.00001; // Yaklaşık 1 metre

        public Map(GMapControl control)
        {
            gMapControl = control;
        }

        public void InitializeMap()
        {
            gMapControl.MapProvider = GMapProviders.OpenStreetMap;
            gMapControl.Position = new PointLatLng(38.707675, 35.519550);
            gMapControl.MinZoom = 5;
            gMapControl.MaxZoom = 25;
            gMapControl.Zoom = 16;
            gMapControl.ShowCenter = false;

            // Marker ve overlay'ı sadece bir kere oluştur
            markers = new GMapOverlay("markers");
            marker = new GMarkerGoogle(gMapControl.Position, GMarkerGoogleType.red_dot);
            markers.Markers.Add(marker);
            gMapControl.Overlays.Add(markers);
        }


        public void UpdatePosition(TelemetryData telemetry)
        {
            if (telemetry == null)
                return;

            double lat = telemetry.Gps1Latitude;
            double lng = telemetry.Gps1Longitude;

            double deltaLat = Math.Abs(lat - lastLat);
            double deltaLng = Math.Abs(lng - lastLng);

            // Eşik kontrolü (1 metreden az fark varsa güncelleme)
            if (deltaLat < threshold && deltaLng < threshold)
                return;

            lastLat = lat;
            lastLng = lng;

            // Sadece pozisyonu güncelle
            marker.Position = new PointLatLng(lat, lng);
            // gMapControl.Position = marker.Position; // Kamera ortalama opsiyonel
        }
    }
}
