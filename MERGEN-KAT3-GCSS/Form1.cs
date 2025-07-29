using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography;

namespace MERGEN_KAT3_GCSS
{
    public partial class Form1 : Form
    {
        private Data dataHandler;
        private DataGridViewHandler dataGridViewHandler;
        Map map;
        private Charts charts;
        private Aras aras;
        _3DSimulation simulation;
        AngleCalculator angleCalculator;
        private AntennaController antCont;
        private double latitude0, longitude0, altitude0, latitude1, longitude1, altitude1;
        private CancellationTokenSource trackingTokenSource;
        private double motorAzimuth = 0, motorElevation = 0;

        private readonly byte[] key = Encoding.UTF8.GetBytes("1234567890123456"); // 16 byte = AES-128
        private readonly byte[] iv = Encoding.UTF8.GetBytes("6543210987654321");  // 16 byte IV
        public Form1()
        {
            InitializeComponent();
            map = new Map(gMapControl1);
            charts = new Charts(chart1, chart2, chart3, chart4, chart5, chart6, chart7, chart8);
            aras = new Aras(pictureBox3, pictureBox4, pictureBox5, pictureBox6);
            string csvPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TELEMETRİ-KAT3.csv");
            dataGridViewHandler = new DataGridViewHandler(dataGridView1, csvPath);
            antCont = new AntennaController(serialPort2);
            angleCalculator = new AngleCalculator();
            simulation = new _3DSimulation(glControl1);
        
            LoadAvailablePorts();
        }

        private void cikisbutton_Click(object sender, EventArgs e)
        {
            DialogResult firstResponse = MessageBox.Show("Uygulamadan çıkmak istediğinize emin misiniz?",
                                                          "Çıkış Onayı",
                                                          MessageBoxButtons.YesNo,
                                                          MessageBoxIcon.Question);
            if (firstResponse == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            // map.InitializeMap();
            pictureBox3.BackColor = Color.Chartreuse;
            pictureBox4.BackColor = Color.Chartreuse;
            pictureBox5.BackColor = Color.Chartreuse;
            pictureBox6.BackColor = Color.Chartreuse;
           

        }
        private void LoadAvailablePorts()
        {
            string[] ports = SerialPort.GetPortNames();

            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            comboBox1.Items.AddRange(ports);
            comboBox2.Items.AddRange(ports);

            if (ports.Length > 0)
            {
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
            }
        }

      /*  private void buttonStracking_Click(object sender, EventArgs e)
        {
            antCont.SendMotorAngles();
            Console.WriteLine("basıldı");
        } */

        private async void buttonStracking_Click(object sender, EventArgs e)
        {
            if (trackingTokenSource == null)
            {
                trackingTokenSource = new CancellationTokenSource();
                await StartTrackingAsync(trackingTokenSource.Token);
            }
            else
            {
                trackingTokenSource.Cancel();
                trackingTokenSource = null;
                textBox2.AppendText("\r\nTakip durduruldu.\r\n");
            }
        }
        private async Task StartTrackingAsync(CancellationToken token)
        {
            textBox2.AppendText("\r\nTakip başlatıldı.\r\n");
            try
            {
                while (!token.IsCancellationRequested)
                {
                    antCont.SendMotorAngles();
                    await Task.Delay(1000, token);
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                MessageBox.Show("Takip hatası: " + ex.Message);
            }
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            double degree = Convert.ToDouble(textBoxDegree.Text, CultureInfo.InvariantCulture);
            motorAzimuth = (motorAzimuth + degree) % 360;
            antCont.SendMotorData(motorAzimuth, motorElevation);
            UpdateLabels();
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            double degree = Convert.ToDouble(textBoxDegree.Text, CultureInfo.InvariantCulture);
            motorElevation = Math.Max(motorElevation - degree, 0);
            antCont.SendMotorData(motorAzimuth, motorElevation);
            UpdateLabels();
        }

        private void UpButton_Click(object sender, EventArgs e)
        {
            double degree = Convert.ToDouble(textBoxDegree.Text, CultureInfo.InvariantCulture);
            motorElevation = Math.Min(motorElevation + degree, 180);
            antCont.SendMotorData(motorAzimuth, motorElevation);
            UpdateLabels();
        }

        private void LeftButton_Click(object sender, EventArgs e)
        {
            double degree = Convert.ToDouble(textBoxDegree.Text, CultureInfo.InvariantCulture);
            motorAzimuth = (motorAzimuth - degree + 360) % 360;
            antCont.SendMotorData(motorAzimuth, motorElevation);
            UpdateLabels();

           
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            motorAzimuth = 0;
            motorElevation = 0;
            antCont.SendMotorData(motorAzimuth, motorElevation);
            antCont.SendMotorCommand("home");
            UpdateLabels();

        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.FromArgb(24, 30, 54)); // Arka plan rengi
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Perspektif ve kamera ayarları.
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.04f, (float)glControl1.Width / glControl1.Height, 1, 10000);
            Matrix4 lookAt = Matrix4.LookAt(25, 0, 0, 0, 0, 0, 0, 1, 0);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookAt);

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);


            // Model rotasyonları uygulanıyor.
               GL.Rotate(simulation.x, 0.0, 0.0, -1.0);  
               GL.Rotate(simulation.z, 0.0, 1.0, 0.0);  
               GL.Rotate(simulation.y, -1.0, 0.0, 0.0);   

          

           





            // Modelin çizimi: alternatif model seçimine göre.

            simulation.DrawCube();
            

            glControl1.SwapBuffers();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"SelectedIndexChanged: {tabControl1.SelectedIndex} at {DateTime.Now.Millisecond}");

        }

        private void SendMessagebutton_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                string plainText = textBox1.Text;
               

                string dataToSend = "A," + plainText;  // Başına "A," ekledik
                serialPort1.WriteLine(dataToSend);       // HEX string olarak gönder
            }
            else
            {
                MessageBox.Show("Seri port açık değil!");
            }
        }

        

        

        private void UpdateLabels()
        {
            labelAntennaAzimuth.Text = "Azimuth: " + motorAzimuth.ToString("0.###", CultureInfo.InvariantCulture);
            labelAntennaElevation.Text = "Elevation: " + motorElevation.ToString("0.###", CultureInfo.InvariantCulture);
        }
        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboBox1.Items.AddRange(ports);
        }

        private void comboBox2_DropDown(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboBox2.Items.AddRange(ports);
        }

        private void lSerialConnectButton_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                // Port açık ise kapat
                try
                {
                    dataHandler?.Disconnect();  // Eğer varsa bağlantıyı kes
                    serialPort1.Close();
                    MessageBox.Show("Port kapatıldı.");
                    Console.WriteLine("Port kapatıldı: " + serialPort1.PortName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Port kapatılırken hata oluştu: " + ex.Message);
                }
                return;
            }

            

            // Port kapalı ise açmaya çalış

            string selectedPort = comboBox1.SelectedItem?.ToString();
            
            if (string.IsNullOrEmpty(selectedPort) || selectedPort == "Port Yok")
            {
              
                MessageBox.Show("Port seçin.");
                return;
            }

            serialPort1.PortName = selectedPort;
            serialPort1.BaudRate = 9600;

            try
            {
                dataHandler = new Data(serialPort1, map, this,charts,simulation, dataGridViewHandler,  aras,angleCalculator,antCont,batteryProgressBar2,compassControl1); // event bağlanıyor
                dataHandler.Connect();
                // serialPort1.Open();

                MessageBox.Show("Port açıldı: " + selectedPort);
                Console.WriteLine("Port açık: " + selectedPort);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Port açılırken hata oluştu: " + ex.Message);
            }
        }

        private void mSerialConnectButton_Click(object sender, EventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                // Port açık ise kapat
                try
                {
                    serialPort2.Close();
                    MessageBox.Show("Port kapatıldı.");
                    Console.WriteLine("Port kapatıldı: " + serialPort2.PortName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Port kapatılırken hata oluştu: " + ex.Message);
                }
                return;
            }

            // Port kapalı ise açmaya çalış
            string selectedPort = comboBox2.SelectedItem?.ToString();
            
            if (string.IsNullOrEmpty(selectedPort) || selectedPort == "Port Yok")
            {   
                MessageBox.Show("Port seçin.");
                return;
            }

            serialPort2.PortName = selectedPort;
            serialPort2.BaudRate = 9600;

            try
            {
                serialPort2.Open();
                MessageBox.Show("Port açıldı: " + selectedPort);
                Console.WriteLine("Port açık: " + selectedPort);

               

              
            }
            catch (Exception ex)
            {
                MessageBox.Show("Port açılırken hata oluştu: " + ex.Message);
            }
        }

      
       


        private void fhApplyButton_Click(object sender, EventArgs e)
        {
            latitude0 = Convert.ToDouble(tbGSlat.Text, CultureInfo.InvariantCulture);
            longitude0 = Convert.ToDouble(tbGSlon.Text, CultureInfo.InvariantCulture);
            altitude0 = Convert.ToDouble(tbGSalt.Text, CultureInfo.InvariantCulture);
            latitude1 = Convert.ToDouble(tbMSlat.Text, CultureInfo.InvariantCulture);
            longitude1 = Convert.ToDouble(tbMSlon.Text, CultureInfo.InvariantCulture);
            altitude1 = Convert.ToDouble(tbMSalt.Text, CultureInfo.InvariantCulture);

            labelGSlong.Text = "GS Longitude: " + longitude0.ToString("0.######");
            labelGSlat.Text = "GS Latitude: " + latitude0.ToString("0.######");
            labelGSalt.Text = "GS Altitude: " + altitude0.ToString("0.##");
            labelMSlat.Text = "MS Latitude: " + latitude1.ToString("0.######");
            labelMSlong.Text = "MS Longitude: " + longitude1.ToString("0.######");
            labelMSalt.Text = "MS Altitude: " + altitude1.ToString("0.##");

            //angleCalculator.SetCoordinates(latitude0, longitude0, altitude0, latitude1, longitude1, altitude1);
            antCont.SetCoordinatesForAngles(latitude0, longitude0, altitude0, latitude1, longitude1, altitude1);
            antCont.UpdateAnglesManually();
            //angleCalculator.ManuelCalculateAngles(); 
            antCont.SendMotorAngles();
        }
    }
}
