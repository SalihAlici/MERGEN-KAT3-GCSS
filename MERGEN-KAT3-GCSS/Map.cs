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
using System.Windows.Forms;

namespace MERGEN_KAT3_GCSS
{
    public class Map : IDisposable
    {
        private readonly GMapControl _gMapControl;
        private GMapMarker _startMarker;
        private GMapMarker _currentMarker;
        private GMapOverlay _routesOverlay;
        private GMapRoute _route;
        private readonly List<PointLatLng> _pathPoints = new List<PointLatLng>();
        private bool _disposed = false;
        private bool _isFirstValidPosition = true;
        private PointLatLng? _lastValidPosition = null; // Nullable olarak tanımlandı
        private const double MovementThreshold = 0.00001;

        public Map(GMapControl control)
        {
            _gMapControl = control ?? throw new ArgumentNullException(nameof(control));

            if (_gMapControl.IsHandleCreated)
            {
                InitializeMap();
            }
            else
            {
                _gMapControl.HandleCreated += (sender, e) => InitializeMap();
            }
        }

        public void InitializeMap()
        {
            if (_gMapControl.InvokeRequired)
            {
                _gMapControl.Invoke((MethodInvoker)InitializeMap);
                return;
            }

            try
            {
                _gMapControl.MapProvider = GMapProviders.OpenStreetMap;
                _gMapControl.MinZoom = 5;
                _gMapControl.MaxZoom = 25;
                _gMapControl.Zoom = 16;
                _gMapControl.ShowCenter = false;

                _gMapControl.Overlays.Clear();

                var markersOverlay = new GMapOverlay("markers");

                _startMarker = new GMarkerGoogle(new PointLatLng(0, 0), GMarkerGoogleType.blue_dot)
                {
                    IsHitTestVisible = false,
                    IsVisible = false,
                    ToolTipText = "Başlangıç Noktası"
                };
                markersOverlay.Markers.Add(_startMarker);

                _currentMarker = new GMarkerGoogle(new PointLatLng(0, 0), GMarkerGoogleType.red_dot)
                {
                    IsHitTestVisible = false,
                    IsVisible = false,
                    ToolTipText = "Güncel Konum"
                };
                markersOverlay.Markers.Add(_currentMarker);

                _gMapControl.Overlays.Add(markersOverlay);

                _routesOverlay = new GMapOverlay("routes");
                _route = new GMapRoute(_pathPoints, "path")
                {
                    Stroke = new Pen(Color.Red, 3)
                };
                _routesOverlay.Routes.Add(_route);
                _gMapControl.Overlays.Add(_routesOverlay);

                _gMapControl.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Harita başlatma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UpdatePosition(TelemetryData telemetry)
        {
            if (telemetry == null || _disposed || !_gMapControl.IsHandleCreated)
                return;

            if (IsInvalidCoordinate(telemetry.Gps1Latitude, telemetry.Gps1Longitude))
            {
               // Console.WriteLine("Geçersiz koordinat - Son geçerli konum kullanılıyor");

                // Null kontrolü ile son geçerli konumu kullan
                if (_lastValidPosition.HasValue)
                {
                    _currentMarker.Position = _lastValidPosition.Value;
                    _gMapControl.UpdateMarkerLocalPosition(_currentMarker);
                }
                return;
            }

            var newPosition = new PointLatLng(telemetry.Gps1Latitude, telemetry.Gps1Longitude);
            _lastValidPosition = newPosition;

            if (_gMapControl.InvokeRequired)
            {
                _gMapControl.BeginInvoke((MethodInvoker)(() => UpdatePosition(telemetry)));
            }
            else
            {
                if (_isFirstValidPosition)
                {
                    _startMarker.Position = newPosition;
                    _startMarker.IsVisible = true;
                    _currentMarker.IsVisible = true;
                    _isFirstValidPosition = false;
                    _gMapControl.Position = newPosition;
                }

                _currentMarker.Position = newPosition;
                _pathPoints.Add(newPosition);

                _routesOverlay.Routes.Clear();
                _route = new GMapRoute(new List<PointLatLng>(_pathPoints), "path")
                {
                    Stroke = new Pen(Color.Red, 3)
                };
                _routesOverlay.Routes.Add(_route);

                _gMapControl.UpdateRouteLocalPosition(_route);
                _gMapControl.UpdateMarkerLocalPosition(_startMarker);
                _gMapControl.UpdateMarkerLocalPosition(_currentMarker);
            }
        }

        private bool IsInvalidCoordinate(double lat, double lng)
        {
            return (lat == 0 && lng == 0) ||
                   Math.Abs(lat) > 90 ||
                   Math.Abs(lng) > 180;
        }

        public void ClearPath()
        {
            if (_disposed || !_gMapControl.IsHandleCreated)
                return;

            if (_gMapControl.InvokeRequired)
            {
                _gMapControl.Invoke((MethodInvoker)ClearPath);
            }
            else
            {
                _pathPoints.Clear();
                _routesOverlay.Routes.Clear();
                _route = new GMapRoute(new List<PointLatLng>(), "path")
                {
                    Stroke = new Pen(Color.Red, 3)
                };
                _routesOverlay.Routes.Add(_route);

                _startMarker.IsVisible = false;
                _currentMarker.IsVisible = false;
                _isFirstValidPosition = true;
                _lastValidPosition = null;

                _gMapControl.UpdateRouteLocalPosition(_route);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _startMarker = null;
            _currentMarker = null;
            _route = null;
            _routesOverlay = null;
            _pathPoints.Clear();
            _lastValidPosition = null;

            if (_gMapControl.IsHandleCreated && !_gMapControl.IsDisposed)
            {
                _gMapControl.Overlays.Clear();
            }
        }
    }
}
