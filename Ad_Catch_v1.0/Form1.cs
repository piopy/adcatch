using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Principal;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Net;

namespace Ad_Catch_v1._0
{
    public partial class Form1 : Form
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        public event FormClosedEventHandler FormClosed;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenu contextMenu1= new System.Windows.Forms.ContextMenu();
        private System.Windows.Forms.MenuItem menuItem1;
        public int status = 0;
        //cambiare host/hostparental/hostnospotify per generare versioni diverse
        public byte[] host= Properties.Resources.host;
        //

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // check admin or root
            bool isElevated;
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            //

            updatehostNoPopup();
            
            #region menu e notifyicon
            this.menuItem1=new System.Windows.Forms.MenuItem();
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "Exit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);

            this.contextMenu1.MenuItems.AddRange(
                    new System.Windows.Forms.MenuItem[] { this.menuItem1 });

           
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(new System.ComponentModel.Container());
            //notifyIcon1.Icon = new Icon(Directory.GetCurrentDirectory() + "\\files\\ico_small.ico"); OLD METHOD

            notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            notifyIcon1.Text = "Ad_Catch_v1.8";
            notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            notifyIcon1.ContextMenu = this.contextMenu1;
            #endregion

            this.FormClosed += FormClosed;
            

            if (!isElevated)
            {
                label1.Visible = true;
                startToolStripMenuItem.Enabled = false;
                //hostUpdatesToolStripMenuItem.Enabled = false;
                
            }

            
        }

        public void updatehost()
        {
            try {
                
                WebClient client = new WebClient();
                byte[] host2 = client.DownloadData("http://someonewhocares.org/hosts/hosts");
                this.host = host2;
                DownloadForm d = new DownloadForm();
                d.Show();
                if (d.annullata()) this.host = Properties.Resources.host;
            }
            catch (WebException) { MessageBox.Show("Unable to connect to internet.\nPlease check your connection and retry"); this.host = Properties.Resources.host; }
            
        }

        public void updatehostNoPopup()
        {
            try
            {
                WebClient client = new WebClient();
                byte[] host2 = client.DownloadData("http://someonewhocares.org/hosts/hosts");
                this.host = host2;
            }
            catch (WebException) { this.host = Properties.Resources.host; }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please restart program as Administrator");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            start();
            
        }

        private void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            { this.Show(); this.WindowState = FormWindowState.Normal;  }
           
            this.Activate();
            notifyIcon1.Visible = false;
        }

        private void menuItem1_Click(object Sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //now is the new minimize method
            if(this.WindowState == FormWindowState.Minimized)
            { 
            this.Hide();
            notifyIcon1.Visible = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("© Copyright 2016, BurningHAM18. \n License: GNU GPL");
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stop();
            
        }

        private void label3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/BurningHAM18");
        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            // check admin or root
            bool isElevated;
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            //
            if (isElevated&&this.status==1)
            {
                stop();
            }

            
        }

        private void start()
        {
            string location = "C:\\Windows\\System32\\drivers\\etc\\hosts";
            string backup = "C:\\Windows\\System32\\drivers\\etc\\hosts.backupAdCatch";
            

            if (this.status == 1) MessageBox.Show("Service already started!");
            if (this.status == 0)
            {
                if (!File.Exists(location) || host==null) { MessageBox.Show("Host file not found"); }
                else if (File.Exists(location) && host != null)
                {
                    if (File.Exists(backup)) MessageBox.Show("Service already started!");
                    else
                    {
                        System.IO.File.Move(location, backup);  //crea backup
                        try { File.Delete(location); }
                        catch (FileNotFoundException) { }
                        File.WriteAllBytes(location, host);

                        MessageBox.Show("Service successfully started!");
                        this.status = 1;
                    }
                }
            }

        }

        private void stop()
        {
            string location = "C:\\Windows\\System32\\drivers\\etc\\hosts";
            string backup = "C:\\Windows\\System32\\drivers\\etc\\hosts.backupAdCatch";

            if (this.status == 0) MessageBox.Show("Service already stopped!");
            if (this.status == 1)
            {
                if (!File.Exists(location)) { MessageBox.Show("Host file not found"); }
                if (!File.Exists(backup)) { MessageBox.Show("Backup file not found"); }
                else if (File.Exists(location) && File.Exists(backup))
                {
                    File.Delete(location);                      //
                    System.IO.File.Copy(backup, location);      //ripristina backup
                    File.Delete(backup);                        //
                    MessageBox.Show("Service successfully stopped!");
                    this.status = 0;
                }
            }
        }

        private void startAdCatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            start();
        }

        private void stopAdCatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stop();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("© Copyright 2016, BurningHAM18. \n License: GNU GPL");
        }

        private void programUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/BurningHAM18/adcatch/releases");
        }

        private void hostUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updatehost();
        }
    }
}
