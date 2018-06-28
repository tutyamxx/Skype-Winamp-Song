using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SKYPE4COMLib;
using System.Diagnostics;

namespace Skype_Mood_Changer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Timer tmr;
        private int scrll { get; set; }

        Skype oSkype = new Skype();

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hwnd, string lpString, int cch);

        const string lpClassName = "Winamp v1.x";
        const string strTtlEnd = " - Winamp";

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("skype").Length == 0)
            {
                MessageBox.Show("You must start Skype first!", "Wooops!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(0);

                return;
            }

            tmr = new Timer();
            tmr.Tick += new EventHandler(this.TimerTick);
            tmr.Interval = 200;
            tmr.Start();

            string DefaultMood = oSkype.CurrentUserProfile.MoodText;
            Skype_Winamp_Song.Properties.Settings.Default.CurrentMood = DefaultMood;
            Skype_Winamp_Song.Properties.Settings.Default.Save();

            string ListeningTo = "Listening to: ";
            oSkype.CurrentUserProfile.RichMoodText = ListeningTo + GetSongTitle();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            oSkype.CurrentUserProfile.MoodText = Skype_Winamp_Song.Properties.Settings.Default.CurrentMood;
            Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = true;
            this.Hide();
        }

        private void tmr_ChangeCurrentSong_Tick(object sender, EventArgs e)
        {
            string ListeningTo = "Listening to: ";
            oSkype.CurrentUserProfile.RichMoodText = ListeningTo + GetSongTitle();
   
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string ListeningTo = "Listening to: ";
            oSkype.CurrentUserProfile.RichMoodText = ListeningTo + GetSongTitle();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            oSkype.CurrentUserProfile.RichMoodText = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string CustomMoodText = textBox1.Text;

            if (CustomMoodText == ""
            || CustomMoodText == " "
            || CustomMoodText.Length == 0)
            {
                return;
            }

            oSkype.CurrentUserProfile.RichMoodText = CustomMoodText;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            ScrollLabel();
        }

        private void ScrollLabel()
        {
            string strString = "-=[ Skype Mood Winamp Song Changer -=- Made by tuty ]=-";

            scrll = scrll + 1;

            int iLmt = strString.Length - scrll;
            if (iLmt < 20)
            {
                scrll = 0;
            }
            string str = strString.Substring(scrll, 20);
            label1.Text = str;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            ScrollLabel();
        }

        static string GetSongTitle()
        {
            IntPtr hwnd = FindWindow(lpClassName, null);

            if (hwnd.Equals(IntPtr.Zero))
            {
                return "[Winamp not running]";
            }

            string lpText = new string((char)0, 100);
            int intLength = GetWindowText(hwnd, lpText, lpText.Length);

            if ((intLength <= 0) || (intLength > lpText.Length))
            {
                return "[unknown]";
            }

            string strTitle = lpText.Substring(0, intLength);

            int intName = strTitle.IndexOf(strTtlEnd);
            int intLeft = strTitle.IndexOf("[");
            int intRight = strTitle.IndexOf("]");

            if ((intName >= 0) && (intLeft >= 0) && (intName < intLeft) && (intRight >= 0) && (intLeft + 1 < intRight))
            {
                return strTitle.Substring(intLeft + 1, intRight - intLeft - 1);
            }

            if ((strTitle.EndsWith(strTtlEnd)) &&
                  (strTitle.Length > strTtlEnd.Length))
                strTitle = strTitle.Substring(0,
                    strTitle.Length - strTtlEnd.Length);

            int intDot = strTitle.IndexOf(".");

            if ((intDot > 0) && IsNumeric(strTitle.Substring(0, intDot)))
            {
                strTitle = strTitle.Remove(0, intDot + 1);
            }

            return strTitle.Trim();
        }

        static bool IsNumeric(string Value)
        {
            try
            {
                double.Parse(Value);

                return true;
            }

            catch
            {
                return false;
            }
        }
    }
}
