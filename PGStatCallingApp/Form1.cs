using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using PGStatLibrary;

namespace PGStatCallingApp
{
    public partial class Form1 : Form
    {
        PGStat pg;

        public Form1()
        {
            InitializeComponent();
            pg = new PGStat(); //Create an object of PGStat class

            pg.PG_EVT_StartConnecting += new System.EventHandler(PG_OnConnecting);
            pg.PG_EVT_Connected += new System.EventHandler(PG_OnConnected);
            pg.PG_EVT_Ping += PG_OnPing;
            pg.PG_EVT_Disconnected += new System.EventHandler(PG_OnDisconnected);
            pg.PG_EVT_Unpluged += new System.EventHandler(PG_OnUnpluged);
            pg.PG_EVT_ACommandDataReceived += new System.EventHandler<ACommandEventArgs>(PG_CommandDataReceived);
            pg.PG_EVT_AdcgetDataReceived += Pg_PG_EVT_AdcgetDataReceived;
            pg.PG_EVT_AdcgetTotalDataReceived += Pg_PG_EVT_AdcgetTotalDataReceived;
            pg.PG_EVT_AivsetDataReceived += Pg_PG_EVT_AivsetDataReceived;
            pg.PG_EVT_ACVDataReceived += Pg_PG_EVT_ACVDataReceived;
            pg.PG_EVT_AIVDataReceived += Pg_PG_EVT_AIVDataReceived;
            pg.PG_EVT_AChronoDataReceived += Pg_PG_EVT_AChronoDataReceived;
            pg.PG_EVT_LogReceived += Pg_PG_EVT_LogReceived;
            pg.SetNotificationVerbosity(3);
            IV_VSelect.SelectedIndex = 1;
            IV_ISelect.SelectedIndex = 3;

            dataGridView1.DataSource = dataSet.Tables["DataTable1"];
            chart1.Series.Clear();
            chart1.ChartAreas[0].Axes[0].Title = "x";
            chart1.ChartAreas[0].Axes[1].Title = "y";
            chart1.Series.Add("Data");
            chart1.Series["Data"].ChartType = SeriesChartType.Point;
            chart1.Series["Data"].MarkerStyle = MarkerStyle.Circle;
            chart1.Series["Data"].Color = Color.Maroon;
            chart1.Series["Data"].XValueMember = "x";
            chart1.Series["Data"].YValueMembers = "y";
            chart1.DataSource = dataSet.Tables["DataTable1"];
            chart1.DataBind();
        }

        private void AddPointToData(double x, double y)
        {
            this.Invoke(new Action(() =>
            {
                dataSet.Tables["DataTable1"].Rows.Add(x, y);
                chart1.DataBind();
            }));
        }

        private void Pg_PG_EVT_LogReceived(object sender, LogReceivedEventArgs e)
        {
            PrintOutput(e.Info + "\n");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int err = pg.Connect();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pg.Disconnect();
        }

        private void PG_OnConnecting(object sender, EventArgs e)
        {
            label1.ForeColor = Color.LightGreen;
        }

        private void PG_OnConnected(object sender, EventArgs e)
        {
            label2.ForeColor = Color.LightGreen;
        }

        private void PG_OnPing(object sender, PingEventArgs e)
        {
            TogglePingLabel();
        }

        private void TogglePingLabel()
        {
            if (label3.ForeColor == Color.LightGreen)
                label3.ForeColor = Color.Maroon;
            else
                label3.ForeColor = Color.LightGreen;
        }

        private void PG_OnDisconnected(object sender, EventArgs e)
        {
            label2.ForeColor = Color.Maroon;
            if (label3.ForeColor == Color.LightGreen) label3.ForeColor = Color.Maroon;
        }

        private void PG_OnUnpluged(object sender, EventArgs e)
        {
            label4.ForeColor = Color.LightGreen;
        }

        private void PG_CommandDataReceived(object sender, ACommandEventArgs e)
        {
            MessageBox.Show(e.Ans);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pg.ACommand("you?");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pg.DisablePing();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pg.EnablePing();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            pg.dummy(1);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            pg.dummy(0);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pg.sample(1);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            pg.sample(0);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int[,] output = pg.dcget();
            richTextBox1.Clear();
            for (int i = 0; i < pg.dcgetNdata; i++)
                PrintOutput("dcget: (" + i.ToString("000") + ")" + output[i, 0].ToString() + "  ,  " + output[i, 1].ToString() + "\n");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            pg.Adcget();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            pg.AdcgetTotal();
        }

        private void Pg_PG_EVT_AdcgetDataReceived(object sender, AdcgetEventArgs e)
        {
            PrintOutput("Adcget: (" + e.Index.ToString("000") + "/" + e.Count.ToString("000") + ")   " + e.Voltage.ToString() + "  ,  " + e.Current.ToString() + "\n");
        }

        private void Pg_PG_EVT_AdcgetTotalDataReceived(object sender, AdcgetTotalEventArgs e)
        {
            for (int i = 0; i < e.Count; i++)
                PrintOutput("AdcgetTotal: (" + i.ToString("000") + "/" + e.Count.ToString("000") + ")   " + e.Voltage[i].ToString() + "  ,  " + e.Current[i].ToString() + "\n");
        }

        delegate void SetStringCallback(string value);
        private void PrintOutput(string value)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.richTextBox1.InvokeRequired)
            {
                SetStringCallback d = new SetStringCallback(PrintOutput);
                this.Invoke(d, new object[] { value });
            }
            else
            {
                richTextBox1.AppendText(value);
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                // scroll it automatically
                richTextBox1.ScrollToCaret();
                richTextBox1.Refresh();
                Thread.Sleep(50);
            }

        }

        private void button13_Click(object sender, EventArgs e)
        {
            double[] output = pg.ivset((int)numericUpDown1.Value);
            PrintOutput("ivset:   Real V=" + output[0].ToString() + "  ,  Real I=" + output[1].ToString() + "\n");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            pg.Aivset((int)numericUpDown1.Value);
        }

        private void Pg_PG_EVT_AivsetDataReceived(object sender, AivsetEventArgs e)
        {
            PrintOutput("ivset:   Real V=" + e.Voltage.ToString() + "  ,  Real I=" + e.Current.ToString() + "\n");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            int settingsver = pg.LoadSettings("./settings.bin");
            pg.CV_Input.Initial_Potential = 0.5;
            pg.CV_Input.Switching_Potential = -1.0;
            pg.CV_Input.Final_Potential = 1.0;
            pg.CV_Input.Step = 100;
            pg.CV_Input.Number_of_Scans = 2;
            pg.CV_Input.Ideal_Voltage = 0;
            pg.CV_Input.Voltage_Range_Mode = 0;
            pg.CV_Input.Current_Range_Mode = 2;
            pg.CV_Input.Current_Multiplier_Mode = 0;
            pg.CV_Input.Voltage_Multiplier_Mode = 0;
            pg.CV_Input.Pretreatment_Voltage = 0;
            pg.CV_Input.Equilibration_Time = 10;
            pg.CV_Input.Post_Processing_Prob_Off = false;
            pg.CV_Input.Interval_Time = 50; //(ms)
            pg.CV_Input.Voltage_Filter = 0;
            pg.CV_Input.Is_Relative_Reference = false;
            pg.ACV();
        }

        private void Pg_PG_EVT_ACVDataReceived(object sender, ACVEventArgs e)
        {
            PrintOutput(e.Voltage.ToString() + " , " + e.Current.ToString() + " , " + e.MeasuredVoltage.ToString() + "\n");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            int settingsver = pg.LoadSettings("./settings.bin");
            pg.IV_Input.Initial_Potential = (double)IVInitialVoltage.Value;
            pg.IV_Input.Final_Potential = (double)IVFinalVoltage.Value;
            pg.IV_Input.Step = (int)IVNumberOfSteps.Value;
            pg.IV_Input.Voltage_Range_Mode = IV_VSelect.SelectedIndex;
            pg.IV_Input.Current_Range_Mode = IV_ISelect.SelectedIndex;
            pg.IV_Input.Equilibration_Time = (int)IVEquilibrationTime.Value; //(ms)
            pg.IV_Input.Interval_Time = (int)IVIntervalTime.Value; //(ms)
            pg.IV_Input.Voltage_Filter = 0;
            pg.IV_Input.Is_Relative_Reference = IVRelativeRefrence.Checked;
            pg.IV_Input.Digital_Filter = (int)DigitalLowPassFilter.Value;
            pg.IV_Input.Current_Range_Mode_Min = (int)Current_Range_Mode_Min.Value;
            pg.IV_Input.Current_Range_Mode_Max = (int)Current_Range_Mode_Max.Value;
            pg.IV_Input.Auto_Range_NCheck = (int)Auto_Range_NCheck.Value;
            pg.IV_Input.Auto_Range_Time = (int)Auto_Range_Time.Value;
            counter = 0;
            dataSet.Tables["DataTable1"].Clear();
            pg.AIV();
        }

        int counter;
        private void Pg_PG_EVT_AIVDataReceived(object sender, AIVEventArgs e)
        {
            counter++;
            PrintOutput(counter.ToString() + ":   V set =" + e.Voltage.ToString("0.00000") + "     I=" + e.Current.ToString("0.00000") + "     V=" + e.MeasuredVoltage.ToString("0.00000") + "\n");
            AddPointToData(e.MeasuredVoltage, e.Current);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            int settingsver = pg.LoadSettings("./settings.bin");
            pg.Chrono_Input.V1 = -1.0;
            pg.Chrono_Input.T1 = 1.0;
            pg.Chrono_Input.Step = 1;
            pg.Chrono_Input.Ideal_Voltage = 0;
            pg.Chrono_Input.Voltage_Range_Mode = 0;
            pg.Chrono_Input.Current_Range_Mode = 2;
            pg.Chrono_Input.Current_Multiplier_Mode = 0;
            pg.Chrono_Input.Voltage_Multiplier_Mode = 0;
            pg.Chrono_Input.Pretreatment_Voltage = 0;
            pg.Chrono_Input.Equilibration_Time = 10;
            pg.Chrono_Input.Post_Processing_Prob_Off = false;
            pg.Chrono_Input.Interval_Time = 50; //(ms)
            pg.Chrono_Input.Voltage_Filter = 0;
            pg.Chrono_Input.Is_Relative_Reference = false;
            pg.AChrono();
        }

        private void Pg_PG_EVT_AChronoDataReceived(object sender, AChronoEventArgs e)
        {
            PrintOutput(e.Time.ToString() + " , " + e.Current.ToString() + " , " + e.MeasuredVoltage.ToString() + "\n");
        }

        private void button19_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            pg.Ninput01((int)Ninput01.Value);
            pg.Ninput02((int)Ninput02.Value);
            pg.Ninput03((int)Ninput03.Value);
            //Ninput06(0);
            pg.input00((int)input00.Value); //V_init
            pg.Ninput04((int)Ninput04.Value); //nDataCycle1
            pg.input01((int)input01.Value); //deltaV
            pg.tinput00((int)tinput00.Value); //nWait at V_init (the number of pulse)
            pg.Ninput00((int)Ninput00.Value); //nCycle
            pg.tinput01((int)tinput01.Value); //nVSet
            pg.Ninput05((int)Ninput05.Value); //nDataCycles

            int expectednData = 4*((int)Ninput04.Value * ((int)tinput00.Value + 1) + 1);
            int ns = pg.teststart((int)Method.Value, (int)Time.Value, expectednData);

            PrintOutput("Expected = " + expectednData.ToString() + "\n");
            PrintOutput("nBytes = " + ns.ToString() + "\n");
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                pg.Command(textBox1.Text);
            }
        }

    }
}
