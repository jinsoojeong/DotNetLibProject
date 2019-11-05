using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

using NetLibrary.SimpleHttpNet;

namespace RestTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            textBox1.Text = "http://localhost:54164/Service1.svc/GetData";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = textBox1.Text;
            string param = textBox2.Text;

            SimpleHttpNet net = new SimpleHttpNet();

            HttpQuery http_query_get = new HttpQuery(url);
            http_query_get.time_out = 100000;
            http_query_get.AddParam("name", "1");

            string response = string.Empty;
            net.Request(HttpNetRequestType.POST, http_query_get, out response);
            Console.WriteLine(response);

            textBox3.Text = response;
        }
    }
}
