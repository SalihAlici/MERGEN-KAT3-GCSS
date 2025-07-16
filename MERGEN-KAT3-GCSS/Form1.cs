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
using System.IO.Ports;
using System.Runtime.InteropServices;

namespace MERGEN_KAT3_GCSS
{
    public partial class Form1 : Form
    {
        private Data dataHandler;
        private DataGridViewHandler dataGridViewHandler;
        Map map;
        private Aras aras;
        public Form1()
        {
            InitializeComponent();
            map = new Map(gMapControl1);
            aras = new Aras(pictureBox3, pictureBox4, pictureBox5, pictureBox6);
            string csvPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TELEMETRİ-KAT3.csv");
            dataGridViewHandler = new DataGridViewHandler(dataGridView1, csvPath);

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
            string selectedPort = comboBox2.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedPort) || selectedPort == "Port Yok")
            {
                MessageBox.Show("Port seçin.");
                return;
            }

            serialPort1.PortName = selectedPort;
            serialPort1.BaudRate = 9600;

            try
            {
                dataHandler = new Data(serialPort1, map, this, dataGridViewHandler,  aras); // event bağlanıyor
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

        }
    }
}
