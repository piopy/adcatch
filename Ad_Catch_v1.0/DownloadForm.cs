using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ad_Catch_v1._0
{
    public partial class DownloadForm : Form
    {
        private bool _annullata = false;
        public DownloadForm()
        {
            InitializeComponent();
            label1.Visible = false;
            timer.Start();

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            bar.Increment(3);
            if (bar.Value == 100)
            {
                button1.Enabled = true;
                timer.Stop();
                label1.Visible = true;
                this.UseWaitCursor = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        public bool annullata()
        {
            return _annullata;
        }

        private void closeHandler(object sender, FormClosingEventArgs e)
        {
            if (bar.Value != 100) { timer.Stop();  this._annullata = true; }
                

            timer.Stop();
        }

        
    }
}
