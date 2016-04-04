using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.IO;

namespace Reboot
{
    public partial class Form1 : Form
    {
        readonly int NEED_REBOOT_TIMES = 50;
        readonly int REBOOT_TIME = 5;
        private static readonly string CONFIG_FILE = "config.txt";

        public Form1()
        {
            InitializeComponent();
            LoadDataToUI();
        }

        private void LoadDataToUI()
        {
            StreamReader stmRdr = new StreamReader(CONFIG_FILE);
            string needRebootTimes = "";
            string start = "";
            string rebootTime = "";

            string line = stmRdr.ReadLine();
            while (line != null)
            {
                line = line.Trim();
                if (line.IndexOf("NeedRebootTimes=") != -1)
                {
                    needRebootTimes = line.Replace("NeedRebootTimes=", "");
                }
                else if (line.IndexOf("Start=") != -1)
                {
                    start = line.Replace("Start=", "");
                }
                else if (line.IndexOf("RebootTime=") != -1)
                {
                    rebootTime = line.Replace("RebootTime=", "");
                }

                line = stmRdr.ReadLine();
            }

            stmRdr.Close();
            stmRdr.Dispose();

            lblNeedRebootTimes.Text = needRebootTimes;
            lblTime.Text = rebootTime;
            
            if (bool.Parse(start) && int.Parse(needRebootTimes) > 0)
            {
                btnRebootM.Enabled = false;
                btnStop.Enabled = true;
                btnCancel.Enabled = true;
                timer1.Enabled = true;
            }
        }

        private void btnReboot_Click(object sender, EventArgs e)
        {
            if (timer2.Enabled)
            {
                lblTime.Text = REBOOT_TIME.ToString();
                timer2.Enabled = false;
                btnReboot.Text = "重新啟動(1)";
            }
            else
            {
                lblTime.Text = REBOOT_TIME.ToString();
                timer2.Enabled = true;
                btnReboot.Text = "取消(1)";
            }
        }

        private static void DoReboot()
        {
            ManagementClass mc = new ManagementClass("Win32_OperatingSystem");


            mc.Scope.Options.EnablePrivileges = true;


            foreach (ManagementObject mo in mc.GetInstances())
            {

                mo.InvokeMethod("Reboot", null, null);

            }


            mc.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int minute = int.Parse(lblTime.Text);

            if (minute == 0)
            {
                timer1.Enabled = false;
                StreamReader stmRdr = new StreamReader(CONFIG_FILE);
                string needRebootTimes = "";
                string start = "";
                string rebootTime = "";

                string line = stmRdr.ReadLine();
                while (line != null)
                {
                    line = line.Trim();
                    if (line.IndexOf("NeedRebootTimes=") != -1)
                    {
                        needRebootTimes = line.Replace("NeedRebootTimes=", "");
                    }
                    else if (line.IndexOf("Start=") != -1)
                    {
                        start = line.Replace("Start=", "");
                    }
                    else if (line.IndexOf("RebootTime=") != -1)
                    {
                        rebootTime = line.Replace("RebootTime=", "");
                    }

                    line = stmRdr.ReadLine();
                }

                stmRdr.Close();
                stmRdr.Dispose();

                if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(needRebootTimes) && !string.IsNullOrEmpty(rebootTime))
                {
                    if (bool.Parse(start) && int.Parse(needRebootTimes) > 0 && int.Parse(rebootTime) > 0)
                    {
                        UpdateConfig(bool.Parse(start), (int.Parse(needRebootTimes) - 1), int.Parse(rebootTime));
                        DoReboot();
                    }
                }
            }
            else
            {
                lblTime.Text = (minute - 1).ToString();
            }
        }

        private void UpdateConfig(bool start, int rebootTimes, int rebootTime)
        {
            StreamWriter stmWdr = new StreamWriter(CONFIG_FILE, false);

            stmWdr.WriteLine(string.Format("NeedRebootTimes={0}", rebootTimes));
            stmWdr.WriteLine(string.Format("Start={0}", start));
            stmWdr.WriteLine(string.Format("RebootTime={0}", rebootTime));

            stmWdr.Close();
            stmWdr.Dispose();
        }

        private void btnRebootM_Click(object sender, EventArgs e)
        {
            btnRebootM.Enabled = false;
            btnStop.Enabled = true;
            btnCancel.Enabled = true;
            UpdateConfig(true, int.Parse(lblNeedRebootTimes.Text), int.Parse(lblTime.Text));
            timer1.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnRebootM.Enabled = true;
            btnStop.Enabled = false;
            btnCancel.Enabled = true;
            timer1.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //重設
            DoReset();
        }

        private void DoReset()
        {
            btnRebootM.Enabled = true;
            btnStop.Enabled = false;
            btnCancel.Enabled = false;
            timer1.Enabled = false;

            lblNeedRebootTimes.Text = NEED_REBOOT_TIMES.ToString();
            lblTime.Text = REBOOT_TIME.ToString();

            UpdateConfig(false, NEED_REBOOT_TIMES, REBOOT_TIME);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            int minute = int.Parse(lblTime.Text);

            if (minute == 0)
            {
                DoReboot();
            }
            else
            {
                lblTime.Text = (minute - 1).ToString();
            }
        }
    }
}
