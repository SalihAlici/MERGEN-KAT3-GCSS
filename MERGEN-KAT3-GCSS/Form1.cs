using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MERGEN_KAT3_GCSS
{
    public partial class Form1 : Form
    {

        Map map;
        public Form1()
        {
            InitializeComponent();
            map = new Map(gMapControl1);
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
            map.InitializeMap();
        }
    }
}
