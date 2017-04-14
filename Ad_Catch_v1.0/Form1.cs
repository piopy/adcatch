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
        #region AdCatch 
        string targ = "\n127.0.0.1 rad.msn.com\n127.0.0.1 live.rads.msn.com\n127.0.0.1 ads1.msn.com\n127.0.0.1 static.2mdn.net\n127.0.0.1 g.msn.com\n127.0.0.1 a.ads2.msads.net\n127.0.0.1 b.ads2.msads.net\n127.0.0.1 ac3.msn.com\n";

        #endregion
        //spotiregion 24.09
        string spoti = "\n0.0.0.0		pubads.g.doubleclick.net\n0.0.0.0		securepubads.g.doubleclick.net\n0.0.0.0		doubleclick.com\n0.0.0.0		doubleclick.de\n0.0.0.0		doubleclick.net\n            \n#nuovi spoty\n0.0.0.0		lon3-accesspoint-a21.lon3.spotify.com\n0.0.0.0		adclick.g.doubleclick.net\n0.0.0.0		\\/\\/adclick.g.doubleclick.net\n0.0.0.0		sto3-weblb-wg6.sto3.spotify.com\n0.0.0.0		mil04s04-in-f14.1e100.net\n0.0.0.0		adeventtracker.spotify.com\n0.0.0.0		asn.advolution.de\n0.0.0.0		spclient.wg.spotify.com\n0.0.0.0		ads.pubmatic.com\n0.0.0.0		gads.pubmatic.com\n0.0.0.0		open.spotify.com\n            \n#\n";

        //endspotiregion
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenu contextMenu1= new System.Windows.Forms.ContextMenu();
        private System.Windows.Forms.MenuItem menuItem1;
        public int status = 0;
        public bool elevated;
        //cambiare host/hostparental/hostnospotify per generare versioni diverse
        public byte[] host= Properties.Resources.host;
        //
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //check if is running

            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                MessageBox.Show("Another instance of this application is running.");
                Close();
                return;
            }

            // check admin or root
            bool isElevated;
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            //
            //update
            targ += spoti;
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
            
            //
            System.IntPtr icH = Properties.Resources.ico_stopped_small.GetHicon();
            Icon icoStopped = Icon.FromHandle(icH);
            icH = Properties.Resources.ico_small.GetHicon();
            Icon icoStarted = Icon.FromHandle(icH);

            if (status == 0) notifyIcon1.Icon = icoStopped;
            else notifyIcon1.Icon = icoStarted;

                //notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
                notifyIcon1.Text = "Ad_Catch";
            notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            notifyIcon1.ContextMenu = this.contextMenu1;
            #endregion

            this.FormClosed += FormClosed;

            if (status == 0) this.BackgroundImage = Properties.Resources.ico_stopped;
            else this.BackgroundImage = Properties.Resources.ico;

            if (!isElevated)
            {
                label1.Visible = true;
                startToolStripMenuItem.Enabled = false;
                //hostUpdatesToolStripMenuItem.Enabled = false;
                
            }
            elevated = isElevated;
            
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
                System.IntPtr icH = Properties.Resources.ico_stopped_small.GetHicon();
                Icon icoStopped = Icon.FromHandle(icH);
                icH = Properties.Resources.ico_small.GetHicon();
                Icon icoStarted = Icon.FromHandle(icH);

                if (status == 0) notifyIcon1.Icon = icoStopped;
                else notifyIcon1.Icon = icoStarted;
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

            if (File.Exists(backup) && this.status == 0)
            {
                File.Delete(location);
                System.IO.File.Copy(backup, location);
                File.Delete(backup);
                this.status = 0;
            }
            if (this.status == 1) MessageBox.Show("Service already started!");
            if (this.status == 0)
            {
                if (!File.Exists(location) || host==null) { MessageBox.Show("Host file not found"); }
                else if (File.Exists(location) && host != null)
                {
                    if (File.Exists(backup)) MessageBox.Show("Service already started!");
                    else
                    {
                        string oldhostcontent = File.ReadAllText(location);
                        
                        System.IO.File.Move(location, backup);  //crea backup
                        try { File.Delete(location); }
                        catch (FileNotFoundException) { }
                        File.WriteAllBytes(location, host);
                        System.IO.File.AppendAllText(location, targ);
                        System.IO.File.AppendAllText(location, oldhostcontent); //mette cio che c' era prima nell host
                        this.BackgroundImage = Properties.Resources.ico;
                        this.BackColor= System.Drawing.SystemColors.ActiveCaption;
                        //MessageBox.Show("Service successfully started!");
                        this.status = 1;
                    }
                }
            }

        }

        private void stop()
        {
            string location = "C:\\Windows\\System32\\drivers\\etc\\hosts";
            string backup = "C:\\Windows\\System32\\drivers\\etc\\hosts.backupAdCatch";
            if (File.Exists(backup) && this.status==0)
            {
                File.Delete(location);
                System.IO.File.Copy(backup, location);
                File.Delete(backup);
                this.status = 1;
            }
            if (this.status == 0) MessageBox.Show("Service already stopped!");
            if (this.status == 1)
            {
                if (!File.Exists(location)) { MessageBox.Show("Host file not found"); }
                if (!File.Exists(backup)) { MessageBox.Show("Backup file not found"); }
                else if (File.Exists(location) && File.Exists(backup))
                {
                    try { File.Delete(location); }
                    catch (Exception egeneric)
                    {
                        MessageBox.Show("Something went wrong. Try to manually restore backup");
                        System.Diagnostics.Process.Start("C:\\Windows\\System32\\drivers\\etc\\");
                    }//
                    System.IO.File.Copy(backup, location);      //ripristina backup
                    File.Delete(backup);                        //
                    this.BackgroundImage = Properties.Resources.ico_stopped;
                    this.BackColor = System.Drawing.SystemColors.ActiveBorder;
                    // MessageBox.Show("Service successfully stopped!");
                    this.status = 0;
                }
            }
        }

        //private void startAdCatchToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    start();
        //}

        //private void stopAdCatchToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    stop();
        //}

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

        private void OnOffButtonClick(object sender, EventArgs e)
        {
            if (!elevated) return;
            else
            {
                if (startToolStripMenuItem.Text == "Start")
                {
                    start();
                    startToolStripMenuItem.Text = "Stop";
                }
                else if (startToolStripMenuItem.Text == "Stop")
                {
                    stop();
                    startToolStripMenuItem.Text = "Start";
                }
            }

        }
    }
}
