using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PGStatLibrary;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Diagnostics;

//test 1
namespace Tinplate
{
    public partial class Form1 : Form
    {
        public UserData CompatibleDefaultUserData = new UserData();
        public static string ProgramName = "STMS";
        public static string Version = "100.0.0.1";
        public static PGStat pg;
        public static bool isFormSetting = false;
        bool ProcessKilled = false;
        bool pingstatus = false;
        string xmlFile = "";
        WaitingForm waitingform = new WaitingForm();
        DataRow DataEmptyRow;
        string TinSampleSettingsxml = "./TinSampleSettings.xml";
        public bool IsAdmin = false;
        public int n_refresh_data_log = 1;

        public Form1()
        {
            InitializeComponent();
            SetCustomBorder();
            

            pg = new PGStat(); //Create an object of PGStat class

            pg.PG_EVT_StartConnecting += new System.EventHandler(PG_OnConnecting);
            pg.PG_EVT_Connected += new System.EventHandler(PG_OnConnected);
            pg.PG_EVT_Ping += PG_OnPing;
            pg.PG_EVT_Disconnected += new System.EventHandler(PG_OnDisconnected);
            pg.PG_EVT_Unpluged += new System.EventHandler(PG_OnUnpluged);
            pg.PG_EVT_ACommandDataReceived += new System.EventHandler<ACommandEventArgs>(PG_CommandDataReceived);
            pg.PG_EVT_AIVDataReceived += Pg_PG_EVT_AIVDataReceived;
            pg.PG_EVT_OffsetRemovalFinished += Pg_PG_EVT_OffsetRemovalFinished;
            pg.PG_EVT_OffsetRemovalStarted += Pg_PG_EVT_OffsetRemovalStarted;
            pg.SetNotificationVerbosity(1);
            pg.PG_EVT_AProcessFinished += Pg_PG_EVT_AProcessFinished;
            int settingsver = pg.LoadSettings("./settings.bin");

            if (settingsver == 0)
                MessageBox.Show("The setting file was not found!");
            else if (settingsver == -1)
                MessageBox.Show("Unable to load the setting file correctly!");

            pingLED.BackgroundImageLayout = ImageLayout.Stretch;

            this.comboBox1.DataBindings.Add(new System.Windows.Forms.Binding("SelectedIndex", this.inputsBindingSource, "VRange", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.comboBox2.DataBindings.Add(new System.Windows.Forms.Binding("SelectedIndex", this.inputsBindingSource, "IRange", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));

            tinSampleSettings1.Clear();
            DataTable t0 = tinSampleSettings1.Tables["Coefficients"];
            DataRow r0 = t0.NewRow();
            tinSampleSettings1.Tables["Coefficients"].Rows.Add(r0);

            FillDeafault();

            if (File.Exists(TinSampleSettingsxml))
            {
                tinSampleSettings1.Clear();
                tinSampleSettings1.ReadXml(TinSampleSettingsxml, XmlReadMode.ReadSchema);
            }
            else
                tinSampleSettings1.WriteXml(TinSampleSettingsxml);

            try
            {
                SyncWithControl();
            }
            catch
            {
                FillDeafault();
                tinSampleSettings1.WriteXml(TinSampleSettingsxml);
                SyncWithControl();
            }

            DataTable t1 = userData1.Tables["Inputs"];
            DataRow r1 = t1.NewRow();
            userData1.Tables["Inputs"].Rows.Add(r1);

            DataEmptyRow = userData1.Tables["Data"].NewRow();


            waitingform.Text = "Offset Removal";
            waitingform.ControlBox = false;
            waitingform.Owner = this;

            AdminLogout();
            //#if DEBUG
            //            tinSampleSettingsToolStripMenuItem.Text = "Logout Admin";
            //            AdminLogin();
            //#endif//moosa
            InitializeFigure();
            UpdateTitle();
        }

        private void FillDeafault()
        {
            tinSampleSettings1.Tables["Coefficients"].Rows[0]["CorrectionCoef"] = (decimal)(0.6666);
            tinSampleSettings1.Tables["Coefficients"].Rows[0]["SampleArea"] = (decimal)(20.428); //in cm^2
            tinSampleSettings1.Tables["Coefficients"].Rows[0]["SetCurrentOffset"] = (decimal)(0);
            tinSampleSettings1.Tables["Coefficients"].Rows[0]["SetCurrentGain"] = (decimal)(100);
            tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadVoltageOffset"] = (decimal)(0);
            tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadVoltageGain"] = (decimal)(100);
            tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadCurrentOffset"] = (decimal)(0);
            tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadCurrentGain"] = (decimal)(100);
            tinSampleSettings1.Tables["Coefficients"].Rows[0]["VFilter"] = (decimal)(3);
            tinSampleSettings1.Tables["Coefficients"].Rows[0]["IFilter"] = (decimal)(2);
            tinSampleSettings1.Tables["Coefficients"].Rows[0]["PostFilter"] = (decimal)(2);
        }

        private void SyncWithControl()
        {
            numericUpDown11.Value = Convert.ToDecimal(tinSampleSettings1.Tables["Coefficients"].Rows[0]["CorrectionCoef"]);
            numericUpDown10.Value = Convert.ToDecimal(tinSampleSettings1.Tables["Coefficients"].Rows[0]["SampleArea"]);
            numericUpDown4.Value = Convert.ToDecimal(tinSampleSettings1.Tables["Coefficients"].Rows[0]["SetCurrentOffset"]);
            numericUpDown15.Value = Convert.ToDecimal(tinSampleSettings1.Tables["Coefficients"].Rows[0]["SetCurrentGain"]);
            numericUpDown5.Value = Convert.ToDecimal(tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadVoltageOffset"]);
            numericUpDown8.Value = Convert.ToDecimal(tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadVoltageGain"]);
            numericUpDown7.Value = Convert.ToDecimal(tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadCurrentOffset"]);
            numericUpDown9.Value = Convert.ToDecimal(tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadCurrentGain"]);
            numericUpDown12.Value = Convert.ToDecimal(tinSampleSettings1.Tables["Coefficients"].Rows[0]["VFilter"]);
            numericUpDown13.Value = Convert.ToDecimal(tinSampleSettings1.Tables["Coefficients"].Rows[0]["IFilter"]);
            numericUpDown14.Value = Convert.ToDecimal(tinSampleSettings1.Tables["Coefficients"].Rows[0]["PostFilter"]);
        }

        private void SetCustomBorder()
        {
            this.FormBorderStyle = FormBorderStyle.None;

            PictureBox Border = new PictureBox();
            Border.Parent = this;
            Border.Location = new Point(0, 0);
            Border.Size = new Size(100, 32);
            Border.Dock = DockStyle.Top;
            Border.BackColor = Color.FromArgb(60,5,5);
            Border.Image = global::Tinplate.Properties.Resources.border;
            //Border.Image = global::Tinplate.Properties.Resources.cangleiconpng;
            Border.SizeMode = PictureBoxSizeMode.StretchImage;
            Border.Paint += Border_Paint;
            Border.DoubleClick += Border_DoubleClick;
            Border.MouseDown += Border_MouseDown;
            Border.MouseMove += Border_MouseMove;
            Border.MouseUp += Border_MouseUp;

            Font drawFont2 = new Font("Marlett", 10, FontStyle.Bold);

            Button MinimizeButton = new Button();
            MinimizeButton.Parent = Border;
            MinimizeButton.Size = new Size(Border.Height, Border.Height);
            MinimizeButton.FlatStyle = FlatStyle.Flat;
            MinimizeButton.FlatAppearance.BorderSize = 0;
            MinimizeButton.Dock = DockStyle.Right;
            MinimizeButton.Font = drawFont2;
            MinimizeButton.ForeColor = Color.White;
            char c = '\u0030';
            MinimizeButton.Text = c.ToString();
            MinimizeButton.Click += MinimizeButton_Click;

            Button RestoreButton = new Button();
            RestoreButton.Parent = Border;
            RestoreButton.Size = new Size(Border.Height, Border.Height);
            RestoreButton.FlatStyle = FlatStyle.Flat;
            RestoreButton.FlatAppearance.BorderSize = 0;
            RestoreButton.Dock = DockStyle.Right;
            RestoreButton.Font = drawFont2;
            RestoreButton.ForeColor = Color.White;
            if (this.WindowState == FormWindowState.Maximized)
                c = '\u0032';
            else
                c = '\u0031';
            RestoreButton.Text = c.ToString();
            RestoreButton.Click += RestoreButton_Click;
            RestoreButton.Paint += RestoreButton_Paint;

            Button CloseButton = new Button();
            CloseButton.Parent = Border;
            CloseButton.Size = new Size(Border.Height, Border.Height);
            CloseButton.FlatStyle = FlatStyle.Flat;
            CloseButton.FlatAppearance.BorderSize = 0;
            CloseButton.Dock = DockStyle.Right;
            CloseButton.Font = drawFont2;
            CloseButton.ForeColor = Color.White;
            c = '\u0072';
            CloseButton.Text = c.ToString();
            CloseButton.Click += CloseButton_Click;

            LeaveFullScreenMode();
            EnterFullScreenMode();

        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            if (System.Windows.Forms.Application.MessageLoop)
            {
                // WinForms app
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                // Console app
                System.Environment.Exit(0);
            }

            try
            {
                System.Environment.Exit(0);
            }
            catch
            {

            }
        }

        private void RestoreButton_Paint(object sender, PaintEventArgs e)
        {
            Button RestoreButton = (Button)sender;
            char c = '\u0032';
            if (this.WindowState == FormWindowState.Maximized)
                c = '\u0032';
            else
                c = '\u0031';
            RestoreButton.Text = c.ToString();
        }

        private void RestoreButton_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
                LeaveFullScreenMode();
            else
                EnterFullScreenMode();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        int mouseX = 0; int mouseY = 0; bool isMove = false;
        int thisLocationX = 0; int thisLocationY = 0;
        private void Border_MouseDown(object sender, MouseEventArgs e)
        {
            isMove = true;
            mouseX = System.Windows.Forms.Control.MousePosition.X;
            mouseY = System.Windows.Forms.Control.MousePosition.Y;
            thisLocationX = this.Location.X;
            thisLocationY = this.Location.Y;
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMove)
            {
                int dx = System.Windows.Forms.Control.MousePosition.X - mouseX;
                int dy = System.Windows.Forms.Control.MousePosition.Y - mouseY;
                this.SetDesktopLocation(thisLocationX + dx, thisLocationY + dy);
            }
        }

        private void Border_MouseUp(object sender, MouseEventArgs e)
        {
            isMove = false;
        }

        bool FirstDoubleClick = true;
        private void Border_DoubleClick(object sender, EventArgs e)
        {
            if (FirstDoubleClick)
            {
                if (this.WindowState == FormWindowState.Maximized)
                    LeaveFullScreenMode();
                else
                    EnterFullScreenMode();
            }

            doubleclicktimer.Start();
            FirstDoubleClick = !FirstDoubleClick;
        }

        private void Border_Paint(object sender, PaintEventArgs e)
        {
            // Create string to draw.
            String drawString;
            drawString = ProgramName + Version;

            // Create font and brush.
            Font drawFont = new Font("Arial", 12, FontStyle.Bold);

            SolidBrush drawBrush = new SolidBrush(Color.FromArgb(20, 20, 20));

            // Create point for upper-left corner of drawing.
            int x0 = 5;
            int y0 = 3;
            float y = 9.0F;

            // Set format of string.
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = StringFormatFlags.NoWrap;

            // Draw string to screen.
            float aspectratio = (float)global::Tinplate.Properties.Resources.irasollogo.Width / (float)global::Tinplate.Properties.Resources.irasollogo.Height;
            int logoheight = 28;
            int logowidth = (int)Math.Round(logoheight * aspectratio);
            e.Graphics.DrawImage(global::Tinplate.Properties.Resources.irasollogo, (int)x0, (int)y0, logowidth, logoheight);

            e.Graphics.DrawString(drawString, drawFont, drawBrush, logowidth + 9, y, drawFormat);
        }

        private void EnterFullScreenMode()
        {
            //this.Hide();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //this.WindowState = FormWindowState.Normal;
            this.ControlBox = true;
            //this.ControlBox = true;
            this.WindowState = FormWindowState.Maximized;
            this.ControlBox = false;
            //this.ControlBox = false;
            //this.Show();
        }

        private void LeaveFullScreenMode()
        {
            //this.Text = " ";
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //this.ControlBox = true;
                //this.WindowState = FormWindowState.Normal;
                this.WindowState = FormWindowState.Maximized;
                this.ControlBox = false;
            }
        }

        private void MaximizeWindow()
        {
            var rectangle = Screen.FromControl(this).Bounds;
            this.FormBorderStyle = FormBorderStyle.None;
            Size = new Size(rectangle.Width, rectangle.Height);
            Location = new Point(0, 0);
            Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;
            this.Size = new Size(workingRectangle.Width, workingRectangle.Height);
        }

        private void ResizableWindow()
        {
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
        }

        ChartArea CA;
        Series S1;
        VerticalLineAnnotation VA_t_min, VA_t_max;
        //TextAnnotation VA_t1, VA_t2;
        //RectangleAnnotation RA1, RA2;

        private void InitializeFigure()
        {
            chart1.Series.Clear();

            chart1.MouseWheel += chart1_MouseWheel;
            chart1.ChartAreas[0].CursorX.LineColor = Color.Blue;
            chart1.ChartAreas[0].CursorY.LineColor = Color.Blue;
            chart1.ChartAreas[0].CursorX.LineWidth = 1;
            chart1.ChartAreas[0].CursorY.LineWidth = 1;

            chart1.ChartAreas[0].Axes[0].Title = "Time";
            chart1.ChartAreas[0].Axes[1].Title = "Voltage";

            chart1.Series.Add("Data");
            chart1.Series["Data"].ChartType = SeriesChartType.Point;
            chart1.Series["Data"].MarkerStyle = MarkerStyle.None;
            chart1.Series["Data"].Color = Color.Maroon;
            //chart1.DataSource = userData1.Tables["Data"];

            //Get the Year for each Country.
            //int[] x = (from p in userData1.Tables["Data"].AsEnumerable()
            //where p.Field<string>("ShipCountry") == country
            //orderby p.Field<int>("Time") ascending
            //           select p.Field<int>("Time")).ToArray();

            //Get the Total of Orders for each Country.
            //int[] y = (from p in userData1.Tables["Data"].AsEnumerable()
            //where p.Field<string>("ShipCountry") == country
            //orderby p.Field<int>("Year") ascending
            //select p.Field<int>("Voltage")).ToArray();


            //chart1.Series["Data"].Points.DataBindXY(userData1.Tables["Data"].Rows, "Time", userData1.Tables["Data"].Rows, "Voltage");
            //chart1.Series["Data"].XValueMember = "Time";
            //chart1.Series["Data"].YValueMembers = "Voltage";


            chart1.Series.Add("Fitting");
            chart1.Series["Fitting"].ChartType = SeriesChartType.Line;
            chart1.Series["Fitting"].Color = Color.DarkBlue;
            //chart1.DataSource = userData1.Tables["Fitting"];
            //chart1.Series["Fitting"].Points.DataBindXY(userData1.Tables["Fitting"].Rows, "Time", userData1.Tables["Fitting"].Rows, "Voltage");
            //chart1.Series["Fitting"].XValueMember = "Time";
            //chart1.Series["Fitting"].YValueMembers = "Voltage";

            //chart1.DataBind();
            //chart1.Series["Data"].Points.DataBindXY(new[] { -1, 1, 3, 3.5}, new[] { 5, 3, -0.3, 0.5 });
            //chart1.Legends[1].Enabled = false;

            chart1.Series.Add("Derivatives1");
            chart1.Series["Derivatives1"].ChartType = SeriesChartType.Line;
            chart1.Series["Derivatives1"].Color = Color.Red;
            chart1.Series["Derivatives1"].IsVisibleInLegend = false;

            chart1.Series.Add("Derivatives2");
            chart1.Series["Derivatives2"].ChartType = SeriesChartType.Line;
            chart1.Series["Derivatives2"].Color = Color.Red;
            chart1.Series["Derivatives2"].IsVisibleInLegend = false;

            chart1.Series.Add("Derivatives3");
            chart1.Series["Derivatives3"].ChartType = SeriesChartType.Line;
            chart1.Series["Derivatives3"].Color = Color.BlueViolet;
            chart1.Series["Derivatives3"].IsVisibleInLegend = false;

            chart1.Series.Add("Derivatives4");
            chart1.Series["Derivatives4"].ChartType = SeriesChartType.Line;
            chart1.Series["Derivatives4"].Color = Color.BlueViolet;
            chart1.Series["Derivatives4"].IsVisibleInLegend = false;

            chart1.Series.Add("T1");
            chart1.Series["T1"].ChartType = SeriesChartType.Point;
            chart1.Series["T1"].Color = Color.Red;
            chart1.Series["T1"].IsVisibleInLegend = false;
            chart1.Series["T1"].MarkerStyle = MarkerStyle.Cross;
            chart1.Series["T1"].MarkerSize = 15;

            chart1.Series.Add("T2");
            chart1.Series["T2"].ChartType = SeriesChartType.Point;
            chart1.Series["T2"].Color = Color.Red;
            chart1.Series["T2"].IsVisibleInLegend = false;
            chart1.Series["T2"].MarkerStyle = MarkerStyle.Cross;
            chart1.Series["T2"].MarkerSize = 15;

            CA = chart1.ChartAreas[0];  // pick the right ChartArea..
            S1 = chart1.Series[0];      // ..and Series!

            // the vertical line
            VA_t_min = new VerticalLineAnnotation();
            VA_t_min.AxisX = CA.AxisX;
            VA_t_min.AllowMoving = true;
            VA_t_min.IsInfinitive = true;
            VA_t_min.ClipToChartArea = CA.Name;
            VA_t_min.Name = "tmin";
            VA_t_min.LineColor = Color.DarkGray;
            VA_t_min.LineWidth = 2;
            VA_t_min.X = tStart;
            chart1.Annotations.Add(VA_t_min);
            // the vertical line
            VA_t_max = new VerticalLineAnnotation();
            VA_t_max.AxisX = CA.AxisX;
            VA_t_max.AllowMoving = true;
            VA_t_max.IsInfinitive = true;
            VA_t_max.ClipToChartArea = CA.Name;
            VA_t_max.Name = "tmax";
            VA_t_max.LineColor = Color.DarkGray;
            VA_t_max.LineWidth = 2;
            VA_t_max.X = tEnd;
            chart1.Annotations.Add(VA_t_max);

            /*
            // the rectangle
            RA1 = new RectangleAnnotation();
            RA1.AxisX = CA.AxisX;
            RA1.IsSizeAlwaysRelative = false;
            RA1.Width = 20;         // use your numbers!
            RA1.Height = 50;        // use your numbers!
            RA1.Name = "mask1";
            RA1.LineColor = Color.Brown;
            RA1.BackColor = Color.Brown;
            RA1.AxisY = CA.AxisY;
            RA1.X = 0;
            RA1.Y = -1;
            RA1.Text = "dfsfgd";
            RA1.ForeColor = Color.White;
            RA1.Font = new System.Drawing.Font("Arial", 8f);
            chart1.Annotations.Add(RA1);
            */


        }

        

        private void AddPointToData(double x, double y)
        {
            //this.Invoke(new Action(() =>
            //{
                userData1.Tables["Data"].Rows.Add(x, y);
            //}));
        }

        private void AddEmptyRowToData()
        {
            userData1.Tables["Data"].Rows.Add();
        }

        private void PrintOutput(string value)
        {
            this.Invoke(new Action(() =>
            {
                int linenumber = richTextBox1.Lines.Count();
                if (linenumber == 0) linenumber = 1;
                if (linenumber % n_refresh_data_log == 1)
                {
                    richTextBox1.AppendText((linenumber).ToString() + ":  " + value);
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                    //richTextBox1.Refresh();
                }
            }));
        }

        private void label2_Click(object sender, EventArgs e)
        {
            //0215100118
            //0215100111
            //02151007630  qatar airline
        }

        private void PG_OnConnecting(object sender, EventArgs e)
        {
            StatusLabel.Text = "Device not found";
            StatusLabel.ForeColor = Color.FromArgb(192,0,0);
            pingLED.BackgroundImage = new Bitmap(Properties.Resources.greenbuttonoff);
            pingstatus = false;
        }

        private void PG_OnConnected(object sender, EventArgs e)
        {
            StatusLabel.Text = "Connected";
            StatusLabel.ForeColor = Color.LightGreen;
            pg.ivset(2047);
        }

        private void PG_OnPing(object sender, PingEventArgs e)
        {
            TogglePingLabel();
            PrintVoltageCurrent(e.Voltage, e.Current);
        }

        private void PrintVoltageCurrent(double voltage, double current)
        {
            this.Invoke(new Action(() =>
            {
                VoltageLabel.Text = "Voltage: " + voltage.ToString("0.0000") + " (V)";
                CurrentLabel.Text = "Current: " + current.ToString("0.0000") + " (mA)";
            }));
        }

        private void TogglePingLabel()
        {
            if (pingstatus)
            {
                pingLED.BackgroundImage = new Bitmap(Properties.Resources.greenbuttonoff);
                pingstatus = false;
            }
            else
            {
                pingLED.BackgroundImage = new Bitmap(Properties.Resources.greenbuttonon);
                pingstatus = true;
            }
        }

        private void PG_OnDisconnected(object sender, EventArgs e)
        {
            StatusLabel.Text = "Disconnected";
            StatusLabel.ForeColor = Color.Maroon;
            pingLED.BackgroundImage = new Bitmap(Properties.Resources.greenbuttonoff);
            pingstatus = false;
        }

        private void PG_OnUnpluged(object sender, EventArgs e)
        {
            StatusLabel.Text = "Device not found";
            StatusLabel.ForeColor = Color.Red;
        }

        private void PG_CommandDataReceived(object sender, ACommandEventArgs e)
        {
            //MessageBox.Show(e.Ans);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pg.IsBusy())
            {
                MessageBox.Show("Application is busy.");
                return;
            }

            ProcessKilled = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton1.Checked = true;

            //Connect to Sample if "Use dummy" is not checked
            if (useDummyToolStripMenuItem.Checked)
                radioButton3.Checked = true;
            else
                radioButton2.Checked = true;

            string dateandtime = DateTime.Now.ToString(@"MM\/dd\/yyyy HH\:mm");
            textBox4.Text = dateandtime;
            userData1.Tables["Inputs"].Rows[0]["IsFittingFound"] = false;
            userData1.Tables["Data"].Clear();
            userData1.Tables["Fitting"].Clear();
            userData1.Tables["Derivatives1"].Clear();
            userData1.Tables["Derivatives2"].Clear();
            userData1.Tables["Derivatives3"].Clear();
            userData1.Tables["Derivatives4"].Clear();
            userData1.Tables["T1"].Clear();
            userData1.Tables["T2"].Clear();
            chart1.Series["Fitting"].Points.DataBindXY(userData1.Tables["Fitting"].Rows, "Time", userData1.Tables["Fitting"].Rows, "Voltage");
            chart1.Series["Derivatives1"].Points.DataBindXY(userData1.Tables["Derivatives1"].Rows, "Time", userData1.Tables["Derivatives1"].Rows, "Voltage");
            chart1.Series["Derivatives2"].Points.DataBindXY(userData1.Tables["Derivatives2"].Rows, "Time", userData1.Tables["Derivatives2"].Rows, "Voltage");
            chart1.Series["Derivatives3"].Points.DataBindXY(userData1.Tables["Derivatives3"].Rows, "Time", userData1.Tables["Derivatives3"].Rows, "Voltage");
            chart1.Series["Derivatives4"].Points.DataBindXY(userData1.Tables["Derivatives4"].Rows, "Time", userData1.Tables["Derivatives4"].Rows, "Voltage");
            chart1.Series["T1"].Points.DataBindXY(userData1.Tables["T1"].Rows, "Time", userData1.Tables["T1"].Rows, "Voltage");
            chart1.Series["T2"].Points.DataBindXY(userData1.Tables["T2"].Rows, "Time", userData1.Tables["T2"].Rows, "Voltage");

            richTextBox1.Clear();

            int settingsver = pg.LoadSettings("./settings.bin");

            pg.PGmode(3);

            int nData = (int)(1000.0 * (int)numericUpDown2.Value / (int)numericUpDown6.Value) + 1;
            n_refresh_data_log = (int)(nData * 5 / 100);
            double SetCurrentOffset = Convert.ToDouble(tinSampleSettings1.Tables["Coefficients"].Rows[0]["SetCurrentOffset"]);
            double SetCurrentGain = Convert.ToDouble(tinSampleSettings1.Tables["Coefficients"].Rows[0]["SetCurrentGain"]);
            pg.IV_Input.Initial_Potential = -((double)numericUpDown1.Value + SetCurrentOffset) * SetCurrentGain / 100.0; //Current + set current offset
            pg.IV_Input.Final_Potential = pg.IV_Input.Initial_Potential; //The same as Initial_Potential
            pg.IV_Input.Step = nData;
            //pg.IV_Input.Ideal_Voltage = 0;
            pg.IV_Input.Voltage_Range_Mode = comboBox1.SelectedIndex;
            pg.IV_Input.Current_Range_Mode = comboBox2.SelectedIndex;
            //pg.IV_Input.Current_Multiplier_Mode = 0;
            //pg.IV_Input.Voltage_Multiplier_Mode = 0;
            //pg.IV_Input.Pretreatment_Voltage = 0;
            pg.IV_Input.Equilibration_Time = 0;
            //pg.IV_Input.Post_Processing_Prob_Off = false;
            pg.IV_Input.Interval_Time = (int)numericUpDown6.Value; //(ms)
            pg.IV_Input.Voltage_Filter = 0;
            pg.IV_Input.Is_Relative_Reference = false;
            pg.IV_Input.IsCalculateOffsetBeforeStart = false;
            pg.IV_Input.VFilter = (int)numericUpDown12.Value;
            pg.IV_Input.IFilter = (int)numericUpDown13.Value;
            pg.IV_Input.PostFilter = (int)numericUpDown14.Value;
            pg.AIV_old();
            ChartRefereshTimer.Start();
        }


        private void Pg_PG_EVT_AIVDataReceived(object sender, AIVEventArgs e)
        {
            if (!isOffsetRemovalProc)
            {
                double ReadVoltageOffset =Convert.ToDouble(tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadVoltageOffset"]);
                double ReadVoltageGain = Convert.ToDouble(tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadVoltageGain"]);
                double ReadCurrentOffset =Convert.ToDouble(tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadCurrentOffset"]);
                double ReadCurrentGain = Convert.ToDouble(tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadCurrentGain"]);
                double MeasuredVoltage = (e.MeasuredVoltage - ReadVoltageOffset) * ReadVoltageGain / 100.0;
                double Current = (e.Current - ReadCurrentOffset / 1000.0) * ReadCurrentGain / 100.0;
                PrintOutput((e.Time * 1000).ToString() + "(ms) , " + MeasuredVoltage.ToString() + "(V), " + (Current * 1000).ToString() + "(mA)\n");
                AddPointToData(e.Time, MeasuredVoltage);
            }
            else
            {
                OffsetRemovalProc_Vmean += e.MeasuredVoltage;
                OffsetRemovalProc_Imean += e.Current;
            }
        }

        private void TryToConnect()
        {
            int err = pg.Connect();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //TryToConnect();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryToConnect();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pg.Disconnect();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeInputs();
            TryToConnect();
        }

        private void InitializeInputs()
        {
            //Input Parameters
            textBox1.Text = ".";
            textBox2.Text = ".";
            textBox1.Text = "";
            textBox2.Text = "";
            numericUpDown1.Value = 100;
            numericUpDown2.Value = 200;
            numericUpDown6.Value = 100;
            checkBox1.Checked = true;
            String SharifSolarPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SharifSolar");
            bool exists = System.IO.Directory.Exists(SharifSolarPath);
            if (!exists)
                System.IO.Directory.CreateDirectory(SharifSolarPath);
            textBox3.Text = SharifSolarPath;
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 1;

            numericUpDown3.Value = 100;
            checkBox2.Checked = false;
            userData1.Tables["Inputs"].Rows[0]["IsUsingCustomCurrent"] = false;

            //Fitting Parameters
            userData1.Tables["Inputs"].Rows[0]["IsFittingFound"] = false;
            userData1.Tables["Inputs"].Rows[0]["t1"] = 0;
            userData1.Tables["Inputs"].Rows[0]["t2"] = 0;
            userData1.Tables["Inputs"].Rows[0]["nSPLine"] = 500;

            ///////////////////////////////////Add any new default value above this part////////////////////////////////////////////
            /////This is to ensure that we can load older file versions in newer app versions
            CompatibleDefaultUserData.Clear();
            userData1.WriteXml("./EmptyProject.xml");
            CompatibleDefaultUserData.ReadXml("./EmptyProject.xml", XmlReadMode.IgnoreSchema);
            ///////////////////////////////////Add any new default value above this part////////////////////////////////////////////
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void Save()
        {
            if (xmlFile != "")
                userData1.WriteXml(xmlFile);
            else
                SaveAs();
        }
        private void SaveAs()
        {
            saveFileDialog1.Filter = "XML file (*.xml)|*.xml";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                xmlFile = saveFileDialog1.FileName;
                UpdateTitle();
                //string t1 = userData1.Tables["Inputs"].Rows[0][0].ToString();
                userData1.WriteXml(xmlFile);
            }
        }

        private void UpdateTitle()
        {
            if (xmlFile=="")
                this.Text = "Untitled";
            else
                this.Text = xmlFile;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenXMLFile();
        }

        private void CreateRecovery()
        {

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateRecovery();
            Application.Exit();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateRecovery();
            InitializeInputs();
            xmlFile = "";
            UpdateTitle();
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!isFormSetting)
            {
                FormSettings FormSetting = new FormSettings();
                FormSetting.Show(this);
                isFormSetting = true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void offsetRemovalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1.pg.OffsetRemoval();
        }

        private void Pg_PG_EVT_OffsetRemovalStarted(object sender, OffsetRemovalStartedEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                waitingform.ShowDialog();
            }));
        }

        private void Pg_PG_EVT_OffsetRemovalFinished(object sender, OffsetRemovalFinishedEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                waitingform.Hide();
            }));
        }

        private void galvanostatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1.pg.G_OffsetRemoval();
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pg.SaveSettings("./settings.bin");
        }

        private void saveSettingsAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Settings file (*.bin)|*.bin";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                pg.SaveSettings(saveFileDialog1.FileName);
        }

        private void loadSettingsFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Settings file (*.bin)|*.bin";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                pg.LoadSettings(openFileDialog1.FileName);
        }

        private void loadSettingsFromDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pg.LoadSettingsFromDevice("./microsettings.bin");
            pg.LoadSettings("./microsettings.bin");
            
        }

        private void resetSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pg.ResetFactory();
        }

        private void saveSettingsToDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pg.MicroSaveSettings("./settings.bin");
        }

        private void saveDeviceSettingsAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Settings file (*.bin)|*.bin";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                pg.LoadSettingsFromDevice(saveFileDialog1.FileName);
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Data files (*.dat)|*.dat";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                ExportData(saveFileDialog1.FileName);
        }

        private void MakePDF(string FileName)
        {
            iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 70f, 70f, 70f, 70f);
            iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(FileName, FileMode.Create));

            doc.Open();

            try
            {
                iTextSharp.text.Paragraph paragraph;
                using (MemoryStream stream = new MemoryStream())
                {
                    var TitleColour = new iTextSharp.text.BaseColor(70, 0, 0);
                    iTextSharp.text.Font titleFont = iTextSharp.text.FontFactory.GetFont("Arial", 24, TitleColour);
                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Report created by " + ProgramName + Version, titleFont);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    doc.Add(title);

                    paragraph = new iTextSharp.text.Paragraph(" ");
                    doc.Add(paragraph);
                    paragraph = new iTextSharp.text.Paragraph("User's Name:      " + textBox1.Text.ToString());
                    doc.Add(paragraph);
                    paragraph = new iTextSharp.text.Paragraph("Sample Code:      " + textBox2.Text.ToString());
                    doc.Add(paragraph);
                    paragraph = new iTextSharp.text.Paragraph("Date and Time:      " + textBox4.Text.ToString());
                    doc.Add(paragraph);


                    chart1.SaveImage(stream, ChartImageFormat.Png);
                    Image img = Image.FromStream(stream);
                    iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(img, iTextSharp.text.BaseColor.WHITE);

                    png.ScaleAbsoluteHeight(doc.PageSize.Height/3f);
                    png.ScaleAbsoluteWidth(doc.PageSize.Width/1.2f);

                    //Resize image depend upon your need
                    //jpg.ScaleToFit(140f, 120f);

                    //Give space before image
                    //jpg.SpacingBefore = 10f;

                    //Give some space after the image
                    png.SpacingAfter = 1f;

                    png.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    doc.Add(png);
                }

                bool AddHeader = true;
                if (AddHeader)
                {
                    paragraph = new iTextSharp.text.Paragraph("       Input parameters");
                    doc.Add(paragraph);
                    paragraph = new iTextSharp.text.Paragraph(" ");
                    doc.Add(paragraph);
                    iTextSharp.text.pdf.PdfPTable intable = new iTextSharp.text.pdf.PdfPTable(2);
                    intable.AddCell("Current");
                    intable.AddCell(numericUpDown1.Value.ToString() + "(mA)");
                    if (checkBox2.Checked)
                    {
                        intable.AddCell("Real Current*");
                        intable.AddCell(numericUpDown3.Value.ToString() + "(mA)");
                    }
                    intable.AddCell("Total Time");
                    intable.AddCell(numericUpDown2.Value.ToString() + "(s)");
                    intable.AddCell("Sampling Interval");
                    intable.AddCell(numericUpDown6.Value.ToString() + "(ms)");
                    intable.AddCell("t1");
                    intable.AddCell(UD_T1.Value.ToString("0.000") + "(s)");
                    intable.AddCell("t2");
                    intable.AddCell(UD_T2.Value.ToString("0.000") + "(s)");
                    intable.AddCell("Sample Area");
                    intable.AddCell(numericUpDown10.Value.ToString() + "(cm^2)");
                    intable.AddCell("Free Tin");
                    intable.AddCell(FreeTin.Text.ToString() + "(g m^-2)");
                    intable.AddCell("Alloy Tin");
                    intable.AddCell(AlloyTin.Text.ToString() + "(g m^-2)");
                    intable.AddCell("Total Tin");
                    intable.AddCell(TotalTin.Text.ToString() + "(g m^-2)");

                    doc.Add(intable);

                    if (checkBox2.Checked)
                    {
                        paragraph = new iTextSharp.text.Paragraph("        *Real current is used for calculations");
                        doc.Add(paragraph);
                    }

                    doc.NewPage();
                    paragraph = new iTextSharp.text.Paragraph(" ");
                    doc.Add(paragraph);
                    paragraph = new iTextSharp.text.Paragraph("       Table of data");
                    doc.Add(paragraph);
                    paragraph = new iTextSharp.text.Paragraph(" ");
                    doc.Add(paragraph);

                }

                iTextSharp.text.pdf.PdfPTable table = new iTextSharp.text.pdf.PdfPTable(2);
                table.AddCell("Time (sec)");
                table.AddCell("Voltage (volt)");

                foreach (DataRow row in userData1.Tables["Data"].Rows)
                {
                    table.AddCell(row["Time"].ToString());
                    table.AddCell(row["Voltage"].ToString());
                }
                
                doc.Add(table);
            }
            catch (Exception ex)
            { }

            finally
            {
                doc.Close();
                Process.Start(FileName);
            }
        }

        private void ExportData(string FileName, bool AddHeader = false)
        {
            StreamWriter FileProtocol = new StreamWriter(FileName);
            if (AddHeader)
            {
                FileProtocol.Write("User's Name:\t" + textBox1.Text.ToString() + "\n");
                FileProtocol.Write("Sample Code:\t" + textBox2.Text.ToString() + "\n");
                FileProtocol.Write("Date and Time:\t" + textBox4.Text.ToString() + "\n");

                FileProtocol.Write("Current:\t" + numericUpDown1.Value.ToString() + "(mA)\n");
                FileProtocol.Write("Time:\t" + numericUpDown2.Value.ToString() + "(s)\n");
                FileProtocol.Write("Sampling Interval:\t" + numericUpDown6.Value.ToString() + "(ms)\n");

                FileProtocol.Write("t1:\t" + UD_T1.Value.ToString("0.000") + "(s)\n");
                FileProtocol.Write("t2:\t" + UD_T2.Value.ToString("0.000") + "(s)\n");

                if (checkBox2.Checked)
                {
                    FileProtocol.Write("Real current is used for calculations\n");
                    FileProtocol.Write("Real Current:\t" + numericUpDown3.Value.ToString() + "(mA)\n");
                }
                FileProtocol.Write("Sample Area:\t" + numericUpDown10.Value.ToString() + "(cm^2)\n");
                FileProtocol.Write("Free Tin:\t" + FreeTin.Text.ToString() + "(g m^-2)\n");
                FileProtocol.Write("Alloy Tin:\t" + AlloyTin.Text.ToString() + "(g m^-2)\n");
                FileProtocol.Write("Total Tin:\t" + TotalTin.Text.ToString() + "(g m^-2)\n");

                FileProtocol.Write("\n");
                FileProtocol.Write("time (s)   voltage (V)\n");
            }

            foreach (DataRow row in userData1.Tables["Data"].Rows)
                FileProtocol.Write(row["Time"].ToString() + "\t" + row["Voltage"].ToString() + "\n");
            FileProtocol.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2(this);
            f2.ShowDialog(this);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenXMLFile();
        }

        private void ValidateOlderXMLFiles()
        {
            /*int itable = -1;
            foreach (DataTable table in userData1.Tables)
            {
                itable++;
                int irow = -1;
                foreach (DataRow row in table.Rows)
                {
                    irow++;
                    foreach (DataColumn column in table.Columns)
                    {
                        object item = row[column];
                        if (item is DBNull)
                            userData1.Tables[itable].Rows[irow][column.ColumnName] = CompatibleDefaultUserData.Tables[itable].Rows[irow][column.ColumnName];
                    }
                }
            }*/

            foreach (DataColumn column in userData1.Tables["Inputs"].Columns)
            {
                object item = userData1.Tables["Inputs"].Rows[0][column];
                if (item is DBNull)
                    userData1.Tables["Inputs"].Rows[0][column.ColumnName] = CompatibleDefaultUserData.Tables["Inputs"].Rows[0][column.ColumnName];
            }
        }

        private void OpenXMLFile()
        {
            openFileDialog1.Filter = "XML file (*.xml)|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                inLoadingProccess = true;
                xmlFile = openFileDialog1.FileName;
                int nspline1 = Convert.ToInt32(userData1.Tables["Inputs"].Rows[0]["nSPLine"]);
                userData1.Clear();
                userData1.ReadXml(xmlFile, XmlReadMode.IgnoreSchema);

                ValidateOlderXMLFiles();
                int nspline2 = Convert.ToInt32(userData1.Tables["Inputs"].Rows[0]["nSPLine"]);
                double t1 = Convert.ToDouble(userData1.Tables["Inputs"].Rows[0]["t1"]);
                double t2 = Convert.ToDouble(userData1.Tables["Inputs"].Rows[0]["t2"]);
                bool DummyIsUsingFittingTimes = Convert.ToBoolean(userData1.Tables["Inputs"].Rows[0]["IsUsingFittingTimes"]);
                string FittingMethod = Convert.ToString(userData1.Tables["Inputs"].Rows[0]["FittingMethod"]);

                UpdateTitle();
                inLoadingProccess = false;
                ProccessData();

                if (FittingMethod == "Curvature")
                    CurvatureMethod.Checked = true;

                if (FittingMethod == "Tangential")
                    TangentialMethod.Checked = true;

                if (!DummyIsUsingFittingTimes)
                {
                    SetT1T2(t1, t2);
                }
            }
        }

        private void SetT1T2(double T1, double T2)
        {
            UD_T1.Value = (decimal)T1;
            SelectionModeT1 = false;

            UD_T2.Value = (decimal)T2;
            SelectionModeT2 = false;
            IsUsingFittingTimes = false;
            userData1.Tables["Inputs"].Rows[0]["IsUsingFittingTimes"] = IsUsingFittingTimes;

            UpdateFittingDiagram();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Save();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSample();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSample();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSample();
        }

        private void UpdateSample()
        {
            bool pingHistory = pg.IsPingEnabled();
            pg.DisablePing();
            Thread.Sleep(500);

            if (radioButton1.Checked)
            {
                pg.dummy(0);
                pg.sample(0);
            }
            else if (radioButton2.Checked)
            {
                pg.dummy(0);
                pg.sample(1);
            }
            else if (radioButton3.Checked)
            {
                pg.sample(0);
                pg.dummy(1);
            }

            if (pingHistory) pg.EnablePing();
        }

        bool inLoadingProccess = false;
        double tStart = 0;
        double tEnd = 0;
        private void UpdateFittingDiagram()
        {
            try
            {
                if (inLoadingProccess) return;
                this.Invoke(new Action(() =>
                {
                    this.UD_T1.ValueChanged -= new System.EventHandler(this.UD_T1_ValueChanged);
                    this.UD_T2.ValueChanged -= new System.EventHandler(this.UD_T2_ValueChanged);
                    UD_T1.Value = (decimal)(Convert.ToDouble(userData1.Tables["Inputs"].Rows[0]["t1"]));
                    UD_T2.Value = (decimal)(Convert.ToDouble(userData1.Tables["Inputs"].Rows[0]["t2"]));
                    this.UD_T1.ValueChanged += new System.EventHandler(this.UD_T1_ValueChanged);
                    this.UD_T2.ValueChanged += new System.EventHandler(this.UD_T2_ValueChanged);
                }));

                bool IsFittingFound = Convert.ToBoolean(userData1.Tables["Inputs"].Rows[0]["IsFittingFound"]);
                double Time = Convert.ToDouble(userData1.Tables["Inputs"].Rows[0]["Time"]);
                double Interval = Convert.ToDouble(userData1.Tables["Inputs"].Rows[0]["Interval"]);
                int nData = userData1.Tables["Data"].Rows.Count;
                //int nData = (int)(1000.0 * Time / Interval) + 1;
                int nDataFitting = nData;
                double dt = Time / (nDataFitting - 1);

                userData1.Tables["Fitting"].Clear();
                userData1.Tables["Derivatives1"].Clear();
                userData1.Tables["Derivatives2"].Clear();
                userData1.Tables["Derivatives3"].Clear();
                userData1.Tables["Derivatives4"].Clear();
                userData1.Tables["T1"].Clear();
                userData1.Tables["T2"].Clear();

                foreach (DataPoint p in chart1.Series["Data"].Points)
                {
                    if (p.XValue < FirstOfValidInterval || p.XValue > EndOfValidInterval)
                    {
                        this.Invoke(new Action(() =>
                        {
                            p.Color = System.Drawing.Color.LightGray;
                        }));
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            p.Color = chart1.Series["Data"].Color;
                        }));
                    }

                }

                foreach (DataPoint p in chart1.Series["Fitting"].Points)
                {
                    if (p.XValue < FirstOfValidInterval || p.XValue > EndOfValidInterval)
                    {
                        this.Invoke(new Action(() =>
                        {
                            p.Color = System.Drawing.Color.LightGray;
                        }));
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            p.Color = chart1.Series["Fitting"].Color;
                        }));
                    }
                }

                if (IsFittingFound)
                {
                    double t1 = Convert.ToDouble(userData1.Tables["Inputs"].Rows[0]["t1"]);
                    double t2 = Convert.ToDouble(userData1.Tables["Inputs"].Rows[0]["t2"]);
                    int left1 = Convert.ToInt32(userData1.Tables["Inputs"].Rows[0]["left1"]);
                    int left2 = Convert.ToInt32(userData1.Tables["Inputs"].Rows[0]["left2"]);
                    int right1 = Convert.ToInt32(userData1.Tables["Inputs"].Rows[0]["right1"]);
                    int right2 = Convert.ToInt32(userData1.Tables["Inputs"].Rows[0]["right2"]);
                    int center1 = Convert.ToInt32(userData1.Tables["Inputs"].Rows[0]["center1"]);
                    int center2 = Convert.ToInt32(userData1.Tables["Inputs"].Rows[0]["center2"]);
                    int curvature1 = Convert.ToInt32(userData1.Tables["Inputs"].Rows[0]["curvature1"]);
                    int curvature2 = Convert.ToInt32(userData1.Tables["Inputs"].Rows[0]["curvature2"]);


                    for (int it = 0; it < nData; it++)
                        userData1.Tables["Fitting"].Rows.Add(tArray[it], f[it]);

                    userData1.Tables["T1"].Rows.Add(t1, f[curvature1]);
                    userData1.Tables["T2"].Rows.Add(t2, f[curvature2]);

                    if (TangentialMethod.Checked && IsUsingFittingTimes)
                    {
                        double x1, y1, x2, y2, yAtCurvature;
                        FindLine(center1, curvature1, nDataFitting, out x1, out y1, out x2, out y2, out yAtCurvature);
                        userData1.Tables["Derivatives1"].Rows.Add(x1, y1);
                        userData1.Tables["Derivatives1"].Rows.Add(x2, y2);

                        int ixTangential;
                        FindTangential(tArray[curvature1], yAtCurvature, curvature1 + 4, center2, out ixTangential);
                        FindLine(ixTangential, curvature1, nDataFitting, out x1, out y1, out x2, out y2, out yAtCurvature);
                        userData1.Tables["Derivatives2"].Rows.Add(x1, y1);
                        userData1.Tables["Derivatives2"].Rows.Add(x2, y2);

                        FindLine(center2, curvature2, nDataFitting, out x1, out y1, out x2, out y2, out yAtCurvature);
                        userData1.Tables["Derivatives3"].Rows.Add(x1, y1);
                        userData1.Tables["Derivatives3"].Rows.Add(x2, y2);

                        FindTangential(tArray[curvature2], yAtCurvature, curvature2 + 4, iEndOfValidInterval, out ixTangential);
                        FindLine(ixTangential, curvature2, nDataFitting, out x1, out y1, out x2, out y2, out yAtCurvature);
                        userData1.Tables["Derivatives4"].Rows.Add(x1, y1);
                        userData1.Tables["Derivatives4"].Rows.Add(x2, y2);
                    }

                    chart1.Series["Fitting"].Points.DataBindXY(userData1.Tables["Fitting"].Rows, "Time", userData1.Tables["Fitting"].Rows, "Voltage");
                    chart1.Series["Derivatives1"].Points.DataBindXY(userData1.Tables["Derivatives1"].Rows, "Time", userData1.Tables["Derivatives1"].Rows, "Voltage");
                    chart1.Series["Derivatives2"].Points.DataBindXY(userData1.Tables["Derivatives2"].Rows, "Time", userData1.Tables["Derivatives2"].Rows, "Voltage");
                    chart1.Series["Derivatives3"].Points.DataBindXY(userData1.Tables["Derivatives3"].Rows, "Time", userData1.Tables["Derivatives3"].Rows, "Voltage");
                    chart1.Series["Derivatives4"].Points.DataBindXY(userData1.Tables["Derivatives4"].Rows, "Time", userData1.Tables["Derivatives4"].Rows, "Voltage");
                    chart1.Series["T1"].Points.DataBindXY(userData1.Tables["T1"].Rows, "Time", userData1.Tables["T1"].Rows, "Voltage");
                    chart1.Series["T2"].Points.DataBindXY(userData1.Tables["T2"].Rows, "Time", userData1.Tables["T2"].Rows, "Voltage");

                    UpdateTin();
                }

            }
            catch { }
        }

        private void UpdateTin()
        {
            if (inLoadingProccess) return;
            if (userData1.Tables["Inputs"] is null) return;
            if (userData1.Tables["Inputs"].Rows.Count < 1) return;
            if (userData1.Tables["Inputs"].Rows[0]["IsFittingFound"] is DBNull) return;
            bool IsFittingFound = Convert.ToBoolean(userData1.Tables["Inputs"].Rows[0]["IsFittingFound"]);
            if (!IsFittingFound) return;
            double t1 = Convert.ToDouble(userData1.Tables["Inputs"].Rows[0]["t1"]);
            double t2 = Convert.ToDouble(userData1.Tables["Inputs"].Rows[0]["t2"]);
            t2 = t2 - t1;
            double CorrectionCoef = Convert.ToDouble(numericUpDown11.Value);
            double SampleArea = Convert.ToDouble(numericUpDown10.Value);
            //double C = 0.0185*SampleArea;   //old one
            double C = 6.1517 / SampleArea;
            double I_mA = Convert.ToDouble(userData1.Tables["Inputs"].Rows[0]["Current"]);
            bool IsUsingCustomCurrent = checkBox2.Checked;
            if (IsUsingCustomCurrent)
                I_mA = Convert.ToDouble(numericUpDown3.Value);
            double I_A = I_mA / 1000.0;

            double freetin = I_A * t1 * C;
            double alloytin = CorrectionCoef * I_A * t2 * C;

            FreeTin.Text = freetin.ToString("0.00");
            AlloyTin.Text = alloytin.ToString("0.00");
            Decimal totaltin = Convert.ToDecimal(FreeTin.Text) + Convert.ToDecimal(AlloyTin.Text);
            TotalTin.Text = (totaltin).ToString("0.00");
        }

        private void FindLine(int iTangential, int j, int nData, out double x1, out double y1, out double x2, out double y2, out double yAtCurvature)
        {
            double DeltaInt = j - iTangential;
            int DeltaLeft = (int)(DeltaInt * 0.7);
            int DeltaRight = (int)(DeltaInt * 0.7);
            
            double x0 = tArray[iTangential];
            double y0 = f[iTangential];
            double m = df[iTangential];

            int ix1 = iTangential - DeltaLeft;
            if (ix1 < 0) ix1 = 0;
            if (ix1 > nData - 1) ix1 = nData - 1;

            int ix2 = j + DeltaRight;
            if (ix2 < 0) ix2 = 0;
            if (ix2 > nData - 1) ix2 = nData - 1;

            x1 = tArray[ix1];
            y1 = m * (x1 - x0) + y0;

            x2 = tArray[ix2];
            y2 = m * (x2 - x0) + y0;

            yAtCurvature = m * (tArray[j] - x0) + y0;
        }

        private void FindTangential(double x0, double y0, int imin0, int imax0, out int ixTangential)
        {
            int imin = imin0;
            int imax = imax0;
            if (imin0 > imax0)
            {
                imin = imax0;
                imax = imin0;
            }

            int ntArray = tArray.Length;
            //here is a bug. when we stop the proccess imax sometimes is more than tarray length
            int imax_forbug = Math.Min(ntArray - 1, imax);

            int nTotal = imax_forbug - imin + 1;
            double[] FindRootArray = new double[nTotal];

            int ind = 0;
            for (int ixTang = imin; ixTang <= imax_forbug; ixTang++)
            {
                FindRootArray[ind] = Math.Abs(y0 - f[ixTang] - df[ixTang] * (x0 - tArray[ixTang]));
                ind++;
            }

            ixTangential = imin;
            double minval = FindRootArray[0];
            for (int i = 0; i < nTotal; i++)
                if (minval > FindRootArray[i])
                {
                    minval = df[i];
                    ixTangential = imin + i;
                }
        }

        //int nDataSpline;
        //float[] f;
        //float[] df;
        //float dt_Spline;

        int nDataSpline;
        double[] tArray;
        double[] f;
        double[] df;
        double[] d2f;
        double dt_Spline;

        double FirstOfValidInterval = 0;
        double EndOfValidInterval = 0;
        int iFirstOfValidInterval = 0;
        int iEndOfValidInterval = 0;
       
        bool IsUsingFittingTimes = false;
        private void FindPicks()
        {
            if (TangentialMethod.Checked)
                userData1.Tables["Inputs"].Rows[0]["FittingMethod"] = "Tangential";
            if (CurvatureMethod.Checked)
                userData1.Tables["Inputs"].Rows[0]["FittingMethod"] = "Curvature";
            int nData0 = userData1.Tables["Data"].Rows.Count;
            if (nData0 == 0) return;
            float Interval = (float)Convert.ToDouble(userData1.Tables["Inputs"].Rows[0]["Interval"]) / 1000;
            double[] t0 = new double[nData0];
            double[] f0 = new double[nData0];
            
            int i = 0;
            foreach (DataRow row in userData1.Tables["Data"].Rows)
            {
                t0[i] = (double)row["Time"];
                f0[i] = (double)row["Voltage"];
                i++;
            }
            
            try
            {
                if (tStart == 0 && tEnd == 0)
                {
                    tStart = t0[0];
                    tEnd = t0[nData0 - 1];
                    VA_t_min.X = tStart;
                    VA_t_max.X = tEnd;
                    FirstOfValidInterval = tStart;
                    EndOfValidInterval = tEnd;
                }
            }
            catch
            { }

            //int nData = (int)(nData0 / 10);
            int nData = nData0;
            nDataSpline = (int)CTRL_nSPLine.Value;
            tArray = new double[nData];
            f = new double[nData];
            df = new double[nData];
            d2f = new double[nData];

            double stepSize = (t0[t0.Length - 1] - t0[0]) / (nData - 1);
            dt_Spline = stepSize;
            for (i = 0; i < nData; i++) tArray[i] = t0[0] + i * stepSize;

            int info;
            alglib.spline1dfitreport rep;
            alglib.spline1dinterpolant splinefit;
            try
            {
                alglib.spline1dfitpenalized(t0, f0, (int)(nDataSpline / 2), 0.1, out info, out splinefit, out rep);
                for (i = 0; i < nData; i++)
                    alglib.spline1ddiff(splinefit, tArray[i], out f[i], out df[i], out d2f[i]);
            }
            catch
            {
                MessageBox.Show("Unable to fit the data!");
            }


            /*int nsmooth = 1;
            for (i = 0; i < nData; i++)
            {
                float sum = 0.0f;
                int n0 = (int)((nsmooth - 1) / 2);
                for (int j = 0; j < nsmooth; j++)
                {
                    int i0 = i + j - n0;
                    if (i0 < 0) i0 = 0;
                    if (i0 > nData - 1) i0 = nData - 1;
                    sum += sf0[i0];
                }
                f[i] = sum / nsmooth;
            }*/

            if (VA_t_min.X < VA_t_max.X)
            {
                FirstOfValidInterval = VA_t_min.X;
                EndOfValidInterval = VA_t_max.X;
            }
            else
            {
                FirstOfValidInterval = VA_t_max.X;
                EndOfValidInterval = VA_t_min.X;
            }

            for (i = 0; i < nData; i++)
                if (tArray[i] >= FirstOfValidInterval)
                {
                    iFirstOfValidInterval = i; break;
                }

            for (i = 0; i < nData; i++)
                if (tArray[i] >= EndOfValidInterval)
                {
                    iEndOfValidInterval = i; break;
                }

            Derivative(nData, dt_Spline, f, out df, out d2f);
            double[] Curvature = new double[nData];
            for (i = 0; i < nData; i++)
                Curvature[i] = Math.Abs(d2f[i]) / Math.Pow(1.0 + Math.Pow(df[i], 2.0), 3.0 / 2.0);

            int imax1 = iFirstOfValidInterval;
            double max1 = Math.Abs(df[iFirstOfValidInterval]);
            double tmax1 = tArray[iFirstOfValidInterval];

            for (i = 1; i < nData; i++)
                if (max1 < Math.Abs(df[i]) && i >= iFirstOfValidInterval && i <= iEndOfValidInterval)
                {
                    max1 = Math.Abs(df[i]);
                    imax1 = i;
                    tmax1 = tArray[i];
                }

            int deltaN = (int)(nData / 8);

            int left1 = imax1 - deltaN;
            int center1 = imax1;
            int right1 = imax1 + deltaN;
            if (left1 < 0) left1 = 0;
            if (right1 < 0) right1 = 0;
            if (left1 > nData - 1) left1 = nData - 1;
            if (right1 > nData - 1) right1 = nData - 1;

            double[] pick2 = new double[nData];
            for (i = 0; i < nData; i++)
                if (i >= left1 && i <= right1)
                    pick2[i] = 0;
                else
                    pick2[i] = df[i];

            int imax2 = iFirstOfValidInterval;
            double max2 = Math.Abs(pick2[iFirstOfValidInterval]);
            double tmax2 = tArray[iFirstOfValidInterval];

            for (i = 1; i < nData; i++)
                if (max2 < Math.Abs(pick2[i]) && i >= iFirstOfValidInterval && i <= iEndOfValidInterval)
                {
                    max2 = Math.Abs(pick2[i]);
                    imax2 = i;
                    tmax2 = tArray[i];
                }

            int left2 = imax2 - deltaN;
            int center2 = imax2;
            int right2 = imax2 + deltaN;
            if (left2 < 0) left2 = 0;
            if (right2 < 0) right2 = 0;
            if (left2 > nData - 1) left2 = nData - 1;
            if (right2 > nData - 1) right2 = nData - 1;

            if (imax2 < imax1)
            {
                int iii = imax2;
                imax2 = imax1;
                imax1 = iii;

                iii = left2;
                left2 = left1;
                left1 = iii;

                iii = right2;
                right2 = right1;
                right1 = iii;

                iii = center2;
                center2 = center1;
                center1 = iii;

                double ddd = tmax2;
                tmax2 = tmax1;
                tmax1 = ddd;

                ddd = max2;
                max2 = max1;
                max1 = ddd;
            }

            int imiddle = (int)((imax1 + imax2) / 2);


            int imaxCurv1 = imax1;
            double maxCurv1 = Curvature[imax1];
            double tmaxCurv1 = tArray[imax1];

            for (i = imax1; i <= imiddle; i++)
                if (maxCurv1 < Curvature[i] && i >= iFirstOfValidInterval && i <= iEndOfValidInterval)
                {
                    maxCurv1 = Curvature[i];
                    imaxCurv1 = i;
                    tmaxCurv1 = tArray[i];
                }

            int imaxCurv2 = imiddle;
            double maxCurv2 = Curvature[imiddle];
            double tmaxCurv2 = tArray[imiddle];

            for (i = imiddle; i <= iEndOfValidInterval; i++)
                if (maxCurv2 < Curvature[i] && i >= iFirstOfValidInterval && i <= iEndOfValidInterval)
                {
                    maxCurv2 = Curvature[i];
                    imaxCurv2 = i;
                    tmaxCurv2 = tArray[i];
                }

            if (TangentialMethod.Checked)
            {
                double x1, y1, m1, x2, y2, m2;

                int imin_df = center1;
                double min_df = Math.Abs(df[center1]);
                for (i = center1; i <= center2; i++)
                    if (min_df > Math.Abs(df[i]) && i >= iFirstOfValidInterval && i <= iEndOfValidInterval)
                    {
                        min_df = Math.Abs(df[i]);
                        imin_df = i;
                    }

                x1 = tArray[center1];
                y1 = f[center1];
                m1 = df[center1];

                x2 = tArray[imin_df];
                y2 = f[imin_df];
                m2 = df[imin_df];

                if (m1 == m2)
                    tmaxCurv1 = -((-(m1 * x1) + m2 * x2 + y1 - y2) / (m1 - m2 + 0.000001));
                else
                    tmaxCurv1 = -((-(m1 * x1) + m2 * x2 + y1 - y2) / (m1 - m2));

                imin_df = center2;
                min_df = Math.Abs(df[center2]);
                for (i = center2; i <= iEndOfValidInterval; i++)
                    if (min_df > Math.Abs(df[i]) && i >= iFirstOfValidInterval && i <= iEndOfValidInterval)
                    {
                        min_df = Math.Abs(df[i]);
                        imin_df = i;
                    }

                x1 = tArray[center2];
                y1 = f[center2];
                m1 = df[center2];

                x2 = tArray[imin_df];
                y2 = f[imin_df];
                m2 = df[imin_df];

                if (m1 == m2)
                    tmaxCurv2 = -((-(m1 * x1) + m2 * x2 + y1 - y2) / (m1 - m2 + 0.000001));
                else
                    tmaxCurv2 = -((-(m1 * x1) + m2 * x2 + y1 - y2) / (m1 - m2));

                for (i = 0; i < nData; i++)
                    if (tArray[i] >= tmaxCurv1)
                    {
                        imaxCurv1 = i; break;
                    }

                for (i = 0; i < nData; i++)
                    if (tArray[i] >= tmaxCurv2)
                    {
                        imaxCurv2 = i; break;
                    }
            }

            userData1.Tables["Inputs"].Rows[0]["IsFittingFound"] = true;
            IsUsingFittingTimes = true;
            userData1.Tables["Inputs"].Rows[0]["IsUsingFittingTimes"] = IsUsingFittingTimes;
            userData1.Tables["Inputs"].Rows[0]["t1"] = tmaxCurv1;
            userData1.Tables["Inputs"].Rows[0]["t2"] = tmaxCurv2;
            userData1.Tables["Inputs"].Rows[0]["left1"] = left1;
            userData1.Tables["Inputs"].Rows[0]["left2"] = left2;
            userData1.Tables["Inputs"].Rows[0]["center1"] = center1;
            userData1.Tables["Inputs"].Rows[0]["center2"] = center2;
            userData1.Tables["Inputs"].Rows[0]["right1"] = right1;
            userData1.Tables["Inputs"].Rows[0]["right2"] = right2;
            userData1.Tables["Inputs"].Rows[0]["curvature1"] = imaxCurv1;
            userData1.Tables["Inputs"].Rows[0]["curvature2"] = imaxCurv2;
        }

        private void Derivative(int nData, double dx, double[] f, out double[] df, out double[] d2f)
        {
            int i;
            df = new double[nData];
            d2f = new double[nData];

            df[0] = (f[1] - f[0]) / dx;
            df[1] = (f[2] - f[0]) / 2 / dx;
            df[nData - 2] = (f[nData - 1] - f[nData - 3]) / 2 / dx;
            df[nData - 1] = (f[nData - 1] - f[nData - 2]) / dx;
            for (i = 2; i < 8; i++)
                df[i] = (-f[i + 2] + 8 * f[i + 1] - 8 * f[i - 1] + f[i - 2]) / 12 / dx;
            for (i = nData - 8; i < nData - 2; i++)
                df[i] = (-f[i + 2] + 8 * f[i + 1] - 8 * f[i - 1] + f[i - 2]) / 12 / dx;
            for (i = 8; i < nData - 8; i++)
                df[i] = (-f[i + 8] + 8 * f[i + 4] - 8 * f[i - 4] + f[i - 8]) / 12 / dx / 4;


            d2f[0] = (df[1] - df[0]) / dx;
            d2f[1] = (f[2] - 2* f[1] + f[0]) / dx / dx;
            d2f[nData - 2] = (f[nData - 1] - 2 * f[nData - 2]+ f[nData - 3]) / dx / dx;
            d2f[nData - 1] = (df[nData - 1] - df[nData - 2]) / dx;
            for (i = 2; i < 8; i++)
                d2f[i] = (-f[i + 2] + 16 * f[i + 1] - 30 * f[i] + 16 * f[i - 1] - f[i - 2]) / 12 / dx / dx;
            for (i = nData - 8; i < nData - 2; i++)
                d2f[i] = (-f[i + 2] + 16 * f[i + 1] - 30 * f[i] + 16 * f[i - 1] - f[i - 2]) / 12 / dx / dx;
            for (i = 8; i < nData - 8; i++)
                d2f[i] = (-f[i + 8] + 16 * f[i + 4] - 30 * f[i] + 16 * f[i - 4] - f[i - 8]) / 12 / dx / dx / 4;
        }

        private double dlogisticcalc5(double x, double a, double b, double c, double d, double g)
        {
            return -((b * (a - d) * g * Math.Pow(x / c, -1.0 + b) * Math.Pow(1.0 + Math.Pow(x / c, b), -1.0 - g)) / c);
        }

        private double d2logisticcalc5(double x, double a, double b, double c, double d, double g)
        {
            return (b * (a - d) * g * Math.Pow(x / c, b) * Math.Pow(1.0 + Math.Pow(x / c, b), -2.0 - g) * (1.0 - b + (1.0 + b * g) * Math.Pow(x / c, b))) / Math.Pow(x, 2);
        }

        double OffsetRemovalProc_Vmax = 0;
        double OffsetRemovalProc_Vmin = 0;
        double OffsetRemovalProc_Imax = 0;
        double OffsetRemovalProc_Imin = 0;
        private void Pg_PG_EVT_AProcessFinished(object sender, AProcessFinishedEventArgs e)
        {
            if (e.Process == "AIV")
            {
                if (!isOffsetRemovalProc)
                {
                    //dettach sample and dummy
                    radioButton2.Checked = false;
                    radioButton3.Checked = false;
                    radioButton1.Checked = true;

                    ChartRefereshTimer.Stop();

                    ProccessData();

                    if (checkBox1.Checked)
                    {
                        CreateAutoReport();
                    }
                }
                else
                {
                    double Imean = OffsetRemovalProc_Imean / OffsetRemovalProc_nData;
                    double Vmean = OffsetRemovalProc_Vmean / OffsetRemovalProc_nData;
                    if (Imean > 0)
                        OffsetRemovalProc_Imax = Imean;
                    else
                        OffsetRemovalProc_Imin = Imean;

                    if (Vmean > 0)
                        OffsetRemovalProc_Vmax = Vmean;
                    else
                        OffsetRemovalProc_Vmin = Vmean;


                    if (OffsetRemovalProc_CurrentSet < 0) //I mean -100. Now we can calculate the offset and gain
                    {
                        double offsetI = (OffsetRemovalProc_Imax + OffsetRemovalProc_Imin) / 2;
                        double offsetV = (OffsetRemovalProc_Vmax + OffsetRemovalProc_Vmin) / 2;
                        double newImax = OffsetRemovalProc_Imax - offsetI;
                        double newImin = OffsetRemovalProc_Imin - offsetI;
                        double newVmax = OffsetRemovalProc_Vmax - offsetV;
                        double newVmin = OffsetRemovalProc_Vmin - offsetV;
                        double gainI= 0.1/ newImax;
                        double gainV = 0.1 / newVmax;

                        decimal offsetV_dec = Convert.ToDecimal(offsetV);
                        decimal gainV_dec = Convert.ToDecimal(gainV * 100);
                        decimal offsetI_dec = Convert.ToDecimal(offsetI * 1000);
                        decimal gainI_dec = Convert.ToDecimal(gainI * 100);

                        tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadVoltageOffset"] = offsetV_dec;
                        tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadVoltageGain"] = gainV_dec;
                        tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadCurrentOffset"] = offsetI_dec;
                        tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadCurrentGain"] = gainI_dec;
                        tinSampleSettings1.WriteXml(TinSampleSettingsxml);
                        
                        isOffsetRemovalProc = false;
                    }
                    else
                        StartOffsetRemovalAt(-100);
                }
            }
        }

        
        private void CreateAutoReport()
        {
            String dateAndTime = textBox4.Text;
            dateAndTime = dateAndTime.Replace("/","-");
            dateAndTime = dateAndTime.Replace(":", "-");
            dateAndTime = dateAndTime.Replace(" ", "_");
            string filename =  "Report_" + dateAndTime + ".dat";
            string dir = textBox3.Text;
            string filepath = Path.Combine(dir, Path.GetFileName(filename));
            ExportData(filepath, true);
        }


        private void ProccessData()
        {

            int nData0 = userData1.Tables["Data"].Rows.Count;
            if (nData0 == 0) return;

            if (ProcessKilled)
            {
                userData1.Tables["Data"].Rows.RemoveAt(nData0 - 1);
            }

            nData0 = userData1.Tables["Data"].Rows.Count;
            if (nData0 == 0) return;

            chart1.Series["Data"].Points.DataBindXY(userData1.Tables["Data"].Rows, "Time", userData1.Tables["Data"].Rows, "Voltage");

            double[] t0 = new double[nData0];
            double[] f0 = new double[nData0];
            int i = 0;
            foreach (DataRow row in userData1.Tables["Data"].Rows)
            {
                t0[i] = (double)row["Time"];
                f0[i] = (double)row["Voltage"];
                i++;
            }

            try
            {
                tStart = t0[0];
                tEnd = t0[nData0 - 1];
                this.Invoke(new Action(() =>
                {
                    VA_t_min.X = tStart;
                    VA_t_max.X = tEnd;
                }));
                FirstOfValidInterval = tStart;
                EndOfValidInterval = tEnd;
            }
            catch
            { }

            FindPicks();
            this.Invoke(new Action(() =>
            {
                UpdateFittingDiagram();
            }));
        }

        private double Erf(double x)
        {
            // constants
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x);

            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return sign * y;
        }

        private void LinearFit(int nData, int nParameters, double[,] X, double[] Y, ref double[] FittedParameters)
        {
            double[,] Xd = new double[nParameters, nData];
            for (int iData = 0; iData < nData; iData++)
                for (int iParameters = 0; iParameters < nParameters; iParameters++)
                    Xd[iParameters, iData] = X[iData, iParameters];

            double[,] M = new double[nParameters, nParameters];
            for (int iParameters = 0; iParameters < nParameters; iParameters++)
                for (int jParameters = 0; jParameters < nParameters; jParameters++)
                {
                    double s = 0;
                    for (int kData = 0; kData < nData; kData++)
                    {
                        s = s + Xd[iParameters, kData] * X[kData, jParameters];
                    }
                    M[iParameters, jParameters] = s;
                }

            int info;
            alglib.matinvreport rep;
            try
            {
                alglib.rmatrixinverse(ref M, out info, out rep);
            }
            catch
            {
                MessageBox.Show("Unable to fit!");
            }

            double[] XdY = new double[nParameters];
            for (int iParameters = 0; iParameters < nParameters; iParameters++)
            {
                double s = 0;
                for (int kData = 0; kData < nData; kData++)
                {
                    s = s + Xd[iParameters, kData] * Y[kData];
                }
                XdY[iParameters] = s;
            }

            for (int iParameters = 0; iParameters < nParameters; iParameters++)
            {
                double s = 0;
                for (int kParameters = 0; kParameters < nParameters; kParameters++)
                {
                    s = s + M[iParameters, kParameters] * XdY[kParameters];
                }
                FittedParameters[iParameters] = s;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Data file (*.dat)|*.dat";
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

            double interval = 0;
            double lastTime = 0;
            int counter = 0;
            string filename = openFileDialog1.FileName;
            using (var file = new StreamReader(filename))
            {
                string ln;
                double t = 0;
                double v;

                List<double> T_list = new List<double>();
                List<double> V_list = new List<double>();
                List<double> I_list = new List<double>();

                while ((ln = file.ReadLine()) != null)
                {
                    String[] d = ln.Split();
                    Double.TryParse(d[2], out t);
                    Double.TryParse(d[3], out v);
                    if (counter == 1) interval = t;
                    //PrintOutput((t * 1000).ToString() + "(ms) , " + v.ToString() + "(V), " + d[1].ToString() + "(mA)\n");
                    AddEmptyRowToData();
                    T_list.Add(t);
                    V_list.Add(v);
                    counter++;
                }
                lastTime = t;

                progressBar1.Value = progressBar1.Minimum;
                for (int irow = 0; irow < counter; irow++)
                {
                    userData1.Tables["Data"].Rows[irow]["Time"] = T_list[irow];
                    userData1.Tables["Data"].Rows[irow]["Voltage"] = V_list[irow];
                    int percentage = (int)(100.0 * irow / (counter - 1));
                    if (percentage < progressBar1.Minimum) percentage = progressBar1.Minimum;
                    if (percentage > progressBar1.Maximum) percentage = progressBar1.Maximum;
                    progressBar1.Value = percentage;
                    progressBar1.Refresh();
                }
            }

            chart1.Series["Data"].Points.DataBindXY(userData1.Tables["Data"].Rows, "Time", userData1.Tables["Data"].Rows, "Voltage");
            
            progressBar1.Value = progressBar1.Minimum;
            progressBar1.Refresh();

            numericUpDown2.Value = (decimal)((int)(lastTime));
            numericUpDown6.Value = (decimal)((int)(interval * 1000));

            FindPicks();
            UpdateFittingDiagram();
        }

        private void UD_T1_ValueChanged(object sender, EventArgs e)
        {
            if (inLoadingProccess) return;
            if ((double)UD_T1.Value > tEnd || (double)UD_T1.Value < tStart)
            {
                if ((double)UD_T1.Value > tEnd) UD_T1.Value = (decimal)tEnd;
                if ((double)UD_T1.Value < tStart) UD_T1.Value = (decimal)tStart;
            }
            else
            {
                userData1.Tables["Inputs"].Rows[0]["t1"] = (double)UD_T1.Value;
                int curvature1 = 0;
                for (int i = 0; i < nDataSpline; i++)
                    if (tArray[i] >= (double)UD_T1.Value)
                    {
                        curvature1 = i; break;
                    }
                userData1.Tables["Inputs"].Rows[0]["curvature1"] = curvature1;
            }
            UpdateFittingDiagram();
        }

        private void UD_T2_ValueChanged(object sender, EventArgs e)
        {
            if (inLoadingProccess) return;
            if ((double)UD_T2.Value > tEnd || (double)UD_T2.Value < tStart)
            {
                if ((double)UD_T2.Value > tEnd) UD_T2.Value = (decimal)tEnd;
                if ((double)UD_T2.Value < tStart) UD_T2.Value = (decimal)tStart;
            }
            else
            {
                userData1.Tables["Inputs"].Rows[0]["t2"] = (double)UD_T2.Value;
                int curvature2 = 0;
                for (int i = 0; i < nDataSpline; i++)
                    if (tArray[i] >= (double)UD_T2.Value)
                    {
                        curvature2 = i; break;
                    }
                userData1.Tables["Inputs"].Rows[0]["curvature2"] = curvature2;
            }
            UpdateFittingDiagram();
        }

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePoint = new Point(e.X, e.Y);

            chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(mousePoint, true);
            chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(mousePoint, true);
        }

        private class ZoomFrame
        {
            public double XStart { get; set; }
            public double XFinish { get; set; }
            public double YStart { get; set; }
            public double YFinish { get; set; }
        }

        private readonly Stack<ZoomFrame> _zoomFrames = new Stack<ZoomFrame>();
        private void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;

            try
            {
                if (e.Delta < 0)
                {
                    if (0 < _zoomFrames.Count)
                    {
                        var frame = _zoomFrames.Pop();
                        if (_zoomFrames.Count == 0)
                        {
                            xAxis.ScaleView.ZoomReset();
                            yAxis.ScaleView.ZoomReset();
                        }
                        else
                        {
                            xAxis.ScaleView.Zoom(frame.XStart, frame.XFinish);
                            yAxis.ScaleView.Zoom(frame.YStart, frame.YFinish);
                        }
                    }
                }
                else if (e.Delta > 0)
                {
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;
                    var yMin = yAxis.ScaleView.ViewMinimum;
                    var yMax = yAxis.ScaleView.ViewMaximum;

                    _zoomFrames.Push(new ZoomFrame { XStart = xMin, XFinish = xMax, YStart = yMin, YFinish = yMax });

                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                    var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                    var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }

        private void chart1_AnnotationPositionChanging(object sender,
                    AnnotationPositionChangingEventArgs e)
        {
            // move the rectangle with the line
            //if (sender == VA) RA.X = VA.X - RA.Width / 2;

            // display the current Y-value
            int pt1 = (int)e.NewLocationX;
            //double step = (S1.Points[pt1 + 1].YValues[0] - S1.Points[pt1].YValues[0]);
            //double deltaX = e.NewLocationX - S1.Points[pt1].XValue;
            //double val = S1.Points[pt1].YValues[0] + step * deltaX;
            //chart1.Titles[0].Text = String.Format(
            //                       "X = {0:0.00}   Y = {1:0.00}", e.NewLocationX, val);
            //RA.Text = String.Format("{0:0.00}", 1);
            chart1.Update();
        }

        private void chart1_AnnotationPositionChanged(object sender, EventArgs e)
        {
            if (VA_t_min.X < tStart) VA_t_min.X = tStart;
            if (VA_t_max.X < tStart) VA_t_max.X = tStart;
            if (VA_t_min.X > tEnd) VA_t_min.X = tEnd;
            if (VA_t_max.X > tEnd) VA_t_max.X = tEnd;

            //VA.X = (int)(VA.X + 0.5);
            //RA.X = VA.X - RA.Width / 2;

            FindPicks();
            UpdateFittingDiagram();
        }
        private void chart1_SelectionRangeChanged(object sender, CursorEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;

            var xMin = xAxis.ScaleView.ViewMinimum;
            var xMax = xAxis.ScaleView.ViewMaximum;
            var yMin = yAxis.ScaleView.ViewMinimum;
            var yMax = yAxis.ScaleView.ViewMaximum;

            _zoomFrames.Push(new ZoomFrame { XStart = xMin, XFinish = xMax, YStart = yMin, YFinish = yMax });

        }

        bool SelectionModeT1 = false;
        bool SelectionModeT2 = false;
        private void btn_choos_t1_Click(object sender, EventArgs e)
        {
            if (SelectionModeT2)
            {
                SelectionModeT2 = false;
                UD_T2.BackColor = Color.White;
            }

            if (SelectionModeT1)
                SelectionModeT1 = false;
            else
                SelectionModeT1 = true;

            if (SelectionModeT1)
            {
                chart1.ChartAreas[0].CursorX.LineColor = Color.Red;
                chart1.ChartAreas[0].CursorY.LineColor = Color.Red;
                chart1.ChartAreas[0].CursorX.LineWidth = 2;
                chart1.ChartAreas[0].CursorY.LineWidth = 2;
                UD_T1.BackColor = Color.Red;
            }
            else
            {
                chart1.ChartAreas[0].CursorX.LineColor = Color.Blue;
                chart1.ChartAreas[0].CursorY.LineColor = Color.Blue;
                chart1.ChartAreas[0].CursorX.LineWidth = 1;
                chart1.ChartAreas[0].CursorY.LineWidth = 1;
                UD_T1.BackColor = Color.White;
            }
        }

        private void btn_choos_t2_Click(object sender, EventArgs e)
        {
            if (SelectionModeT1)
            {
                SelectionModeT1 = false;
                UD_T1.BackColor = Color.White;
            }

            if (SelectionModeT2)
                SelectionModeT2 = false;
            else
                SelectionModeT2 = true;

            if (SelectionModeT2)
            {
                chart1.ChartAreas[0].CursorX.LineColor = Color.Red;
                chart1.ChartAreas[0].CursorY.LineColor = Color.Red;
                chart1.ChartAreas[0].CursorX.LineWidth = 2;
                chart1.ChartAreas[0].CursorY.LineWidth = 2;
                UD_T2.BackColor = Color.Red;
            }
            else
            {
                chart1.ChartAreas[0].CursorX.LineColor = Color.Blue;
                chart1.ChartAreas[0].CursorY.LineColor = Color.Blue;
                chart1.ChartAreas[0].CursorX.LineWidth = 1;
                chart1.ChartAreas[0].CursorY.LineWidth = 1;
                UD_T2.BackColor = Color.White;
            }
        }

        int ClickX0 = 0;
        int ClickY0 = 0;
        private void chart1_MouseDown(object sender, MouseEventArgs e)
        {
            ClickX0 = e.X;
            ClickY0 = e.Y;
        }

        private void chart1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!(ClickX0 == e.X && ClickY0 == e.Y)) return;

            if (!(SelectionModeT1 || SelectionModeT2)) return;

            double T = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.X);

            if (SelectionModeT1)
            {
                if (T < (double)UD_T2.Value)
                    UD_T1.Value = (decimal)T;
                else
                {
                    double dummy = Convert.ToDouble(userData1.Tables["Inputs"].Rows[0]["t2"]);
                    UD_T2.Value = (decimal)T;
                    userData1.Tables["Inputs"].Rows[0]["t2"] = T;
                    UD_T1.Value = (decimal)dummy;
                }
                SelectionModeT1 = false;
                IsUsingFittingTimes = false;
                userData1.Tables["Inputs"].Rows[0]["IsUsingFittingTimes"] = IsUsingFittingTimes;
            }

            if (SelectionModeT2)
            {
                if (T > (double)UD_T1.Value)
                    UD_T2.Value = (decimal)T;
                else
                {
                    double dummy = Convert.ToDouble(userData1.Tables["Inputs"].Rows[0]["t1"]);
                    UD_T1.Value = (decimal)T;
                    userData1.Tables["Inputs"].Rows[0]["t1"] = T;
                    UD_T2.Value = (decimal)dummy;
                }
                SelectionModeT2 = false;
                IsUsingFittingTimes = false;
                userData1.Tables["Inputs"].Rows[0]["IsUsingFittingTimes"] = IsUsingFittingTimes;
            }

            chart1.ChartAreas[0].CursorX.LineColor = Color.Blue;
            chart1.ChartAreas[0].CursorY.LineColor = Color.Blue;
            chart1.ChartAreas[0].CursorX.LineWidth = 1;
            chart1.ChartAreas[0].CursorY.LineWidth = 1;
            UD_T1.BackColor = Color.White;
            UD_T2.BackColor = Color.White;

            UpdateFittingDiagram();
        }

        private void TangentialMethod_CheckedChanged(object sender, EventArgs e)
        {
            FindPicks();
            UpdateFittingDiagram();
        }

        private void doubleclicktimer_Tick(object sender, EventArgs e)
        {
            FirstDoubleClick = true;
            doubleclicktimer.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pg.KillProcess();
            ProcessKilled = true;
            ChartRefereshTimer.Stop();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown3.Enabled = checkBox2.Checked;
            UpdateTin();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            UpdateTin();
        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {   
            //tinSampleSettings1.Tables["Coefficients"].Rows[0]["SampleArea"] = numericUpDown10.Value; //in cm^2
           // tinSampleSettings1.WriteXml(TinSampleSettingsxml);
            UpdateTin();
        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            //tinSampleSettings1.Tables["Coefficients"].Rows[0]["CorrectionCoef"] = numericUpDown11.Value;
           // tinSampleSettings1.WriteXml(TinSampleSettingsxml);
            UpdateTin();
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            //tinSampleSettings1.Tables["Coefficients"].Rows[0]["SetCurrentOffset"] = numericUpDown4.Value;
            //tinSampleSettings1.WriteXml(TinSampleSettingsxml);
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            //tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadVoltageOffset"] = numericUpDown5.Value;
            //tinSampleSettings1.WriteXml(TinSampleSettingsxml);
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            //tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadVoltageGain"] = numericUpDown8.Value;
            //tinSampleSettings1.WriteXml(TinSampleSettingsxml);
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            //tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadCurrentOffset"] = numericUpDown7.Value;
            //tinSampleSettings1.WriteXml(TinSampleSettingsxml);
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            //tinSampleSettings1.Tables["Coefficients"].Rows[0]["ReadCurrentGain"] = numericUpDown9.Value;
            //tinSampleSettings1.WriteXml(TinSampleSettingsxml);
        }
        private void tinSampleSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsAdmin)
            {
                tinSampleSettingsToolStripMenuItem.Text = "Login as Admin";
                AdminLogout();
                return;
            }

            AdminPassForm passform = new AdminPassForm(this);
            passform.ShowDialog();
            if (IsAdmin)
            {
                tinSampleSettingsToolStripMenuItem.Text = "Logout Admin";
                AdminLogin();
            }
        }

        private void AutoSelectTime_Click(object sender, EventArgs e)
        {
            FindPicks();
            UpdateFittingDiagram();
        }

        private void AdminLogin()
        {
            IsAdmin = true;
            groupBox7.Visible = true;
            numericUpDown11.Visible = true;
            label47.Visible = true;
            button5.Visible = true;
            button7.Visible = true;
            panel6.Size = new Size(panel6.Size.Width, 790);

            useDummyToolStripMenuItem.Enabled = true;
            offsetRemovalToolStripMenuItem.Enabled = true;
            offsetRemovalToolStripMenuItem1.Enabled = true;
            galvanostatToolStripMenuItem.Enabled = true;
            saveSettingsAsToolStripMenuItem.Enabled = true;
            saveSettingsToolStripMenuItem.Enabled = true;
            saveSettingsToDeviceToolStripMenuItem.Enabled = true;
            loadSettingsFromDeviceToolStripMenuItem.Enabled = true;
            loadSettingsFromFileToolStripMenuItem.Enabled = true;
            saveDeviceSettingsAsToolStripMenuItem.Enabled = true;
            settingsToolStripMenuItem1.Enabled = true;
            resetSettingsToolStripMenuItem.Enabled = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Data files (*.dat)|*.dat|PDF files (.pdf)|*.pdf";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FilterIndex == 1)
                    ExportData(saveFileDialog1.FileName, true);
                else if (saveFileDialog1.FilterIndex == 2)
                    MakePDF(saveFileDialog1.FileName);
            }
        }

        bool isOffsetRemovalProc;
        double OffsetRemovalProc_CurrentSet = 0;
        double OffsetRemovalProc_Vmean;
        double OffsetRemovalProc_Imean;
        int OffsetRemovalProc_Voltage_Range_Mode;
        int OffsetRemovalProc_Current_Range_Mode;
        int OffsetRemovalProc_nData = 0;
        private void offsetRemovalToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            isOffsetRemovalProc = true;
            

            int settingsver = pg.LoadSettings("./settings.bin");

            /*pg.ClearBuffer();
            double[] output = pg.ivset(0);
            PrintOutput("ivset:   Real V=" + output[0].ToString() + "  ,  Real I=" + output[1].ToString() + "\n");

            Thread.Sleep(1000);

            output = pg.ivset(2047);
            PrintOutput("ivset:   Real V=" + output[0].ToString() + "  ,  Real I=" + output[1].ToString() + "\n");

            Thread.Sleep(1000);

            output = pg.ivset(4095);
            PrintOutput("ivset:   Real V=" + output[0].ToString() + "  ,  Real I=" + output[1].ToString() + "\n");*/

            OffsetRemovalProc_Voltage_Range_Mode = comboBox1.SelectedIndex;
            OffsetRemovalProc_Current_Range_Mode = comboBox2.SelectedIndex;
            StartOffsetRemovalAt(100);

            
        }

        private void StartOffsetRemovalAt(double current)
        {
            pg.sample(1);
            pg.sample(0);
            pg.dummy(1);
            OffsetRemovalProc_Imean = 0;
            OffsetRemovalProc_Vmean = 0;

            pg.PGmode(3);

            OffsetRemovalProc_nData = 25;
            int nData = OffsetRemovalProc_nData;
            OffsetRemovalProc_CurrentSet = current;
            pg.IV_Input.Initial_Potential = OffsetRemovalProc_CurrentSet;
            pg.IV_Input.Final_Potential = OffsetRemovalProc_CurrentSet;
            pg.IV_Input.Step = nData;
            //pg.IV_Input.Ideal_Voltage = 0;
            pg.IV_Input.Voltage_Range_Mode = OffsetRemovalProc_Voltage_Range_Mode;
            pg.IV_Input.Current_Range_Mode = OffsetRemovalProc_Current_Range_Mode;
            //pg.IV_Input.Current_Multiplier_Mode = 0;
            //pg.IV_Input.Voltage_Multiplier_Mode = 0;
            //pg.IV_Input.Pretreatment_Voltage = 0;
            pg.IV_Input.Equilibration_Time = 0;
            //pg.IV_Input.Post_Processing_Prob_Off = false;
            pg.IV_Input.Interval_Time = 100; //(ms)
            pg.IV_Input.Voltage_Filter = 0;
            pg.IV_Input.Is_Relative_Reference = false;
            pg.IV_Input.IsCalculateOffsetBeforeStart = false;
            pg.AIV_old();
        }

        private void coefficientsBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            tinSampleSettings1.WriteXml(TinSampleSettingsxml);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox d = new AboutBox();
            d.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ProccessData();
        }

        private void ChartRefereshTimer_Tick(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                chart1.Series["Data"].Points.DataBindXY(userData1.Tables["Data"].Rows, "Time", userData1.Tables["Data"].Rows, "Voltage");
            }));
        }

        private void Input_Validating(object sender, CancelEventArgs e)
        {
            Input_Validating();
        }

        private void Input_Validating(object sender, EventArgs e)
        {
            Input_Validating();
        }

        private void Input_Validating()
        {
            int nmin = 10;
            int nmax = 2000;
            double time = 1000 * (double)numericUpDown2.Value; //ms
            double dt = (double)numericUpDown6.Value; //ms
            int n = (int)(time / dt) + 1;
            if (n < nmin)
            {
                int newn = nmin;
                double newdt = time / (newn - 1);

                if (newdt < 1)
                {
                    newdt = 1;
                    double newtime = newdt * (newn - 1);
                    numericUpDown2.Value = (decimal)Math.Floor(newtime / 1000);
                }

                numericUpDown6.Value = (decimal)(newdt);
            }
            else if (n > nmax)
            {
                int newn = nmax;
                double newdt = time / (newn - 1);

                if (newdt > 2000)
                {
                    newdt = 2000;
                    double newtime = newdt * (newn - 1);
                    numericUpDown2.Value = (decimal)Math.Floor(newtime / 1000);
                }

                numericUpDown6.Value = (decimal)(newdt);
            }
        }

        private void AdminLogout()
        {
            IsAdmin = false;
            groupBox7.Visible = false;
            numericUpDown11.Visible = false;
            label47.Visible = false;
            button5.Visible = false;
            button7.Visible = false;
            panel6.Size = new Size(panel6.Size.Width, 478);

            useDummyToolStripMenuItem.Enabled = false;
            offsetRemovalToolStripMenuItem.Enabled = false;
            offsetRemovalToolStripMenuItem1.Enabled = false;
            galvanostatToolStripMenuItem.Enabled = false;
            saveSettingsAsToolStripMenuItem.Enabled = false;
            saveSettingsToolStripMenuItem.Enabled = false;
            saveSettingsToDeviceToolStripMenuItem.Enabled = false;
            loadSettingsFromDeviceToolStripMenuItem.Enabled = false;
            loadSettingsFromFileToolStripMenuItem.Enabled = false;
            saveDeviceSettingsAsToolStripMenuItem.Enabled = false;
            settingsToolStripMenuItem1.Enabled = false;
            resetSettingsToolStripMenuItem.Enabled = false;
        }

















    }
}
