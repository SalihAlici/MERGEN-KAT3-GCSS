# MERGEN-KAT3-GCSS

Ground station and antenna tracking software for a model satellite. It displays and logs telemetry received from the satellite, computes antenna angles from the GPS positions of the satellite and the ground station, and points the ground station antenna at the satellite with motors. It also handles encrypted messaging with the satellite and receives photo packets.

Written in C# with Windows Forms (.NET Framework 4.7.2). The user interface is in Turkish.

> **The latest version is in the [`QR`](../../tree/QR) branch.** The `master` branch contains the first interface design. The features below describe the latest version in `QR`.

![Satellite tab](docs/uydu_sekmesi.png)
*Satellite tab: telemetry charts, map, 3D model, compass and data table. Values shown are sample data.*

![Antenna tab](docs/anten_sekmesi.png)
*Antenna tab: ground station and satellite positions, RSSI indicators, tracking and manual control.*

## Features

- **Two serial ports (9600 baud):**
  - One for telemetry and communication, one for the antenna motor controller.
  - Reading and processing run on separate threads.
  - Telemetry packets are framed as `$ ... #`, photo packets as `< ... >`. Corrupt packets are discarded.
- **Telemetry (25 fields):**
  - Packet number, satellite status, error code, transmission time.
  - Pressure, altitude, vertical and horizontal speed, distance.
  - Internal and external temperature, battery voltage and battery percentage.
  - Satellite GPS, pitch / roll / yaw, rotation angle.
  - Message, team number, ground station GPS.
- **Charts, table and logging:**
  - 8 time charts.
  - Every packet is added to the data table and appended to `TELEMETRİ-KAT3.csv` on the desktop.
- **Map:** GMap.NET + OpenStreetMap; the satellite position and its track are drawn.
- **3D simulation:** a satellite and parachute model rotates with the received angles (OpenTK).
- **Antenna tracking:**
  - **Geometry:** horizontal and vertical distance, azimuth and elevation are computed from the two GPS positions (`AngleCalculator`).
  - **Automatic tracking:** started with "Start Tracking!"; angles are sent to the motor controller every second as `azimuth,elevation`.
  - **Manual control:** direction buttons step the azimuth (0–360°) and elevation (0–180°); a "home" command returns to the start position.
  - **Tracking modes:** the interface offers GPS, RSSI and Hybrid tracking options.
- **Communication:**
  - Two ground station modules; uplink 865.125 MHz, downlink 868.125 MHz.
  - RSSI indicators for azimuth and elevation.
- **Encrypted messages:** messages to the satellite are AES-encrypted and sent as `A,<hex>`; encrypted messages from the satellite are decrypted and displayed.
- **Photo reception:** `<packet_no,hex_data>` packets are appended to `photo_data.txt` on the desktop.
- **ARAS (fault detection) indicator:** error-code bits are shown as lights.

## Telemetry format

```
$packet*status*error_code*time*pressure*altitude*vertical_speed*distance*horizontal_speed*temp_internal*temp_external*battery*battery_pct*
 gps1_lat*gps1_lon*gps1_alt*pitch*roll*yaw*rotation_angle*message*team_no*gps2_lat*gps2_lon*gps2_alt#
```

`gps1` is the satellite position and `gps2` is the ground station position. The decimal separator is a dot.

## Files

| File | Contents |
|---|---|
| `Form1.cs` | Main window, ports, tracking and manual control buttons, AES encryption |
| `Data.cs` | Serial reading, telemetry and photo packet framing, sending commands |
| `TelemetryData.cs` | Packet parser |
| `AngleCalculator.cs` | Azimuth and elevation from GPS positions |
| `AntennaController.cs` | Motor commands, automatic tracking loop |
| `Charts.cs`, `DataGridViewHandler.cs` | Charts, data table and CSV logging |
| `Map.cs`, `_3DSimulation.cs` | Map and 3D model |
| `Aras.cs` | Error-code indicator |
| `CompassControl.cs`, `CircularProgress.cs`, `QuarterCircularProgress.cs`, `BatteryProgressBar.cs` | Custom gauges |

## Build and run

**Requirements:**
- Windows 10 / 11
- Visual Studio 2019 or 2022 with the ".NET desktop development" workload (including .NET Framework 4.7.2)
- Internet connection for the map

**Steps:**
1. Clone the repository and check out the latest version:
   ```
   git clone https://github.com/SalihAlici/MERGEN-KAT3-GCSS.git
   cd MERGEN-KAT3-GCSS
   git checkout QR
   ```
2. Open `MERGEN-KAT3-GCSS.sln` in Visual Studio.
3. Restore NuGet packages: right-click the solution → *Restore NuGet Packages* (they may also be restored on the first build).
4. Build and run (F5).
5. Select the communication module port under *Communication Modules* and the antenna motor controller port under *Motor Controller*, then connect. Ports open at 9600 baud.
6. Enter the ground station position on the *ANTEN* tab. Press *Start Tracking!* for automatic tracking, or use the direction buttons to point the antenna manually.

Telemetry is logged to a CSV file on the desktop.

## Libraries

GMap.NET (map), OpenTK (3D), System.Windows.Forms.DataVisualization (charts), System.Security.Cryptography (AES).

## Author

[SalihAlici](https://github.com/SalihAlici)
