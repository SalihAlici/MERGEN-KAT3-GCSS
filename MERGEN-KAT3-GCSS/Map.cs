using GMap.NET.MapProviders;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MERGEN_KAT3_GCSS
{
    public class Map
    {
        private GMapControl gMapControl;

        private GMapOverlay markersOverlay;
        private GMapOverlay routesOverlay;

        private GMapMarker currentPositionMarker;

        private List<PointLatLng> routePoints = new List<PointLatLng>();

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

            // Marker ve rota için ayrı overlay'lar
            markersOverlay = new GMapOverlay("markers");
            routesOverlay = new GMapOverlay("routes");

            gMapControl.Overlays.Add(routesOverlay);
            gMapControl.Overlays.Add(markersOverlay);

            // Başlangıçta marker oluştur (şu anki pozisyon için)
            currentPositionMarker = new GMarkerGoogle(gMapControl.Position, GMarkerGoogleType.red_dot);
            markersOverlay.Markers.Add(currentPositionMarker);
        }

        public void UpdatePosition(TelemetryData telemetry)
        {
            if (telemetry == null)
                return;

            double lat = telemetry.Gps1Latitude;
            double lng = telemetry.Gps1Longitude;

            double deltaLat = Math.Abs(lat - lastLat);
            double deltaLng = Math.Abs(lng - lastLng);

            // Çok küçük değişiklik varsa işlem yapma
            if (deltaLat < threshold && deltaLng < threshold)
                return;

            lastLat = lat;
            lastLng = lng;

            var newPoint = new PointLatLng(lat, lng);

            // Yeni noktayı rota listesine ekle
            routePoints.Add(newPoint);

            // Rota overlay'ini temizle (eski çizgileri kaldır)
            routesOverlay.Routes.Clear();

            // Yeni rota çizgisi oluştur
            var route = new GMapRoute(routePoints, "route")
            {
                Stroke = new Pen(Color.Blue, 2)  // Rota çizgi kalınlığı ve rengi
            };

            routesOverlay.Routes.Add(route);

            // Şu anki marker pozisyonunu güncelle
            currentPositionMarker.Position = newPoint;

            // İstersen harita merkezini güncelle
            gMapControl.Position = newPoint;

            // Haritayı yeniden çiz
            gMapControl.Refresh();
        }
    }
}
