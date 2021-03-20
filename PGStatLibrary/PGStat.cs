using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Management;
using System.Windows.Forms;
using System.Timers;
using System.Reflection;
using System.Collections;
//using System.Runtime.InteropServices;

namespace PGStatLibrary
{
    //[ClassInterface(ClassInterfaceType.AutoDual)]
    //[ComVisible(true)]

    /// <summary>
    /// The main PGStat class.<para />
    /// Contains all methods for controlling the device.
    /// </summary>
    public class PGStat
    {
        /*********************************************Variables*********************************************/
        private string PGStatVer = "2019.1";
        private static int SettingsVersion = 8;
        private SerialPort CheckPort = new SerialPort("COM1", 921600, Parity.None, 8, StopBits.One);
        private SerialPort Port = new SerialPort("COM1", 921600, Parity.None, 8, StopBits.One);
        private string ReadToChar = "\r";
        private string FoundedPort = "";
        private bool isFounded = false;
        private int CheckPortTimeoutSec = 2000;
        private bool Connected = false;
        private int PortTimeout = 2000;
        private int NotificationsVerbosity = 1;
        private NotifyIcon notification;
        private bool IsAutoReconnectEnabled = true;
        private System.Timers.Timer PingTimer;
        private int DeviceVersion = 0;
        private int BoardType = 2;
        private UInt16 PageNumber = 128;
        private UInt16 PageSize = 256;
        private int InteraPageSleep = 30;
        private UInt16 pass = 0;
        private int ReconnectingCount = 0;
        private bool isPingEnable = true;
        private int DataCountInQueue = 0;
        private static int IVnData = 512;
        private double OpenCircuitVoltage = 0;
        private bool KillProcessRequested = false;
        /****************************************************************************************************/

        /*********************************************Properties*********************************************/
        /// <summary>
        /// The number of data coming from dcget command family.
        /// </summary>
        public readonly int dcgetNdata = IVnData;
        /// <summary>
        /// Device settings. Do not change it.
        /// </summary>
        public bool SettingsIsIVReceiverUnsigned = false;
        /// <summary>
        /// The inputs of the CV method.
        /// </summary>
        public CVProperties CV_Input;
        /// <summary>
        /// The inputs of the IV method.
        /// </summary>
        public IVProperties IV_Input;
        /// <summary>
        /// The inputs of the Chrono method.
        /// </summary>
        public ChronoProperties Chrono_Input;
        /****************************************************************************************************/

        /*********************************************Properties*********************************************/
        /// <summary>
        /// The last value of PGmode.
        /// </summary>
        public int LastPGmode { get; private set; }
        /// <summary>
        /// The last value of vfilter.
        /// </summary>
        public int Lastvfilter { get; private set; }
        /// <summary>
        /// The last value of idcfilter.
        /// </summary>
        public int Lastidcfilter { get; private set; }
        /// <summary>
        /// The last value of idcselect.
        /// </summary>
        public int Lastidcselect { get; private set; }
        /// <summary>
        /// The last value of setselect.
        /// </summary>
        public int Lastsetselect { get; private set; }
        /// <summary>
        /// The last value of idcmlp.
        /// </summary>
        public int Lastidcmlp { get; private set; }
        /// <summary>
        /// The last value of vdcmlp.
        /// </summary>
        public int Lastvdcmlp { get; private set; }
        /// <summary>
        /// The last value of zeroset.
        /// </summary>
        public int Lastzeroset { get; private set; }
        /// <summary>
        /// The last value of acset.
        /// </summary>
        public int Lastacset { get; private set; }
        /// <summary>
        /// The last value of aczero.
        /// </summary>
        public int Lastaczero { get; private set; }
        /// <summary>
        /// The last value of vaczero.
        /// </summary>
        public int Lastvaczero { get; private set; }
        /// <summary>
        /// The last value of input00.
        /// </summary>
        public int Lastinput00 { get; private set; }
        /// <summary>
        /// The last value of input01.
        /// </summary>
        public int Lastinput01 { get; private set; }
        /// <summary>
        /// The last value of input02.
        /// </summary>
        public int Lastinput02 { get; private set; }
        /// <summary>
        /// The last value of input03.
        /// </summary>
        public int Lastinput03 { get; private set; }
        /// <summary>
        /// The last value of input04.
        /// </summary>
        public int Lastinput04 { get; private set; }
        /// <summary>
        /// The last value of input05.
        /// </summary>
        public int Lastinput05 { get; private set; }
        /// <summary>
        /// The last value of input06.
        /// </summary>
        public int Lastinput06 { get; private set; }
        /// <summary>
        /// The last value of input07.
        /// </summary>
        public int Lastinput07 { get; private set; }
        /// <summary>
        /// The last value of input08.
        /// </summary>
        public int Lastinput08 { get; private set; }
        /// <summary>
        /// The last value of input09.
        /// </summary>
        public int Lastinput09 { get; private set; }
        /// <summary>
        /// The last value of input10.
        /// </summary>
        public int Lastinput10 { get; private set; }
        /// <summary>
        /// The last value of input00.
        /// </summary>
        public int Lasttinput00 { get; private set; }
        /// <summary>
        /// The last value of input01.
        /// </summary>
        public int Lasttinput01 { get; private set; }
        /// <summary>
        /// The last value of input02.
        /// </summary>
        public int Lasttinput02 { get; private set; }
        /// <summary>
        /// The last value of input03.
        /// </summary>
        public int Lasttinput03 { get; private set; }
        /// <summary>
        /// The last value of input04.
        /// </summary>
        public int Lasttinput04 { get; private set; }
        /// <summary>
        /// The last value of input05.
        /// </summary>
        public int Lasttinput05 { get; private set; }
        /// <summary>
        /// The last value of input06.
        /// </summary>
        public int Lasttinput06 { get; private set; }
        /// <summary>
        /// The last value of input07.
        /// </summary>
        public int Lasttinput07 { get; private set; }
        /// <summary>
        /// The last value of input08.
        /// </summary>
        public int Lasttinput08 { get; private set; }
        /// <summary>
        /// The last value of input09.
        /// </summary>
        public int Lasttinput09 { get; private set; }
        /// <summary>
        /// The last value of input10.
        /// </summary>
        public int Lasttinput10 { get; private set; }
        /// <summary>
        /// The last value of Ninput00.
        /// </summary>
        public int LastNinput00 { get; private set; }
        /// <summary>
        /// The last value of Ninput01.
        /// </summary>
        public int LastNinput01 { get; private set; }
        /// <summary>
        /// The last value of Ninput02.
        /// </summary>
        public int LastNinput02 { get; private set; }
        /// <summary>
        /// The last value of Ninput03.
        /// </summary>
        public int LastNinput03 { get; private set; }
        /// <summary>
        /// The last value of Ninput04.
        /// </summary>
        public int LastNinput04 { get; private set; }
        /// <summary>
        /// The last value of Ninput05.
        /// </summary>
        public int LastNinput05 { get; private set; }
        /// <summary>
        /// The last value of Ninput06.
        /// </summary>
        public int LastNinput06 { get; private set; }
        /// <summary>
        /// The last value of Ninput07.
        /// </summary>
        public int LastNinput07 { get; private set; }
        /// <summary>
        /// The last value of Ninput08.
        /// </summary>
        public int LastNinput08 { get; private set; }
        /// <summary>
        /// The last value of Ninput09.
        /// </summary>
        public int LastNinput09 { get; private set; }
        /// <summary>
        /// The last value of Ninput10.
        /// </summary>
        public int LastNinput10 { get; private set; }
        /// <summary>
        /// The last value of sn.
        /// </summary>
        public int Lastsn { get; private set; }
        /// <summary>
        /// The last value of Tget.
        /// </summary>
        public int LastTget { get; private set; }
        /// <summary>
        /// The last value of dcset.
        /// </summary>
        public int Lastdcset { get; private set; }
        /// <summary>
        /// The last value of ddsclk.
        /// </summary>
        public int Lastddsclk { get; private set; }
        /// <summary>
        /// The last value of dds.
        /// </summary>
        public int Lastdds { get; private set; }
        /// <summary>
        /// The last value of vac.
        /// </summary>
        public int Lastvac { get; private set; }
        /// <summary>
        /// The last state of the dummy.
        /// </summary>
        public int Lastdummy { get; private set; }
        /// <summary>
        /// The last state of the sample.
        /// </summary>
        public int Lastsample { get; private set; }
        /// <summary>
        /// The last value of EqTime.
        /// </summary>
        public int LastEqTime { get; private set; }
        /// <summary>
        /// The last value of Pretreatment.
        /// </summary>
        public double LastPretreatment { get; private set; }
        /// <summary>
        /// The last value of Open Circuit Potential state. Respectively, false and true are Inactive and Active.
        /// </summary>
        public bool LastOCPState { get; private set; }
        /// <summary>
        /// The last value of RelRef state. Respectively, false and true are Absolute and Relative modes.<para />
        /// If it is true, the potential of the working electrode is relative to the reference electrode.
        /// </summary>
        public bool LastRelRefState { get; private set; }
        /// <summary>
        /// The Open Circuit Potential. It can be voltage or current with respect to the PGMode. The unit for set voltage/current is Volt/Ampere.
        /// </summary>
        public double LastOCP { get; private set; }
        /// <summary>
        /// The last value of IdealVoltage.
        /// </summary>
        public double LastIdealVoltage { get; private set; }
        /****************************************************************************************************/

        /***********************************************Events***********************************************/
        /// <summary>
        /// Process a PG_EVT_StartConnecting event, when PGStat started to search for a connected device.
        /// </summary>
        public event EventHandler PG_EVT_StartConnecting;

        /// <summary>
        /// Process a PG_EVT_Connected event, when PGStat connected to the device.
        /// </summary>
        public event EventHandler PG_EVT_Connected;

        /// <summary>
        /// Process a PG_EVT_Ping event, when PGStat is pinging.
        /// </summary>
        public event EventHandler<PingEventArgs> PG_EVT_Ping;

        /// <summary>
        /// Process a PG_EVT_Disconnected event, when PGStat is disconnected from the device.
        /// </summary>
        public event EventHandler PG_EVT_Disconnected;

        /// <summary>
        /// Process a PG_EVT_Unpluged event, when PGStat is disconnected accidentally.
        /// </summary>
        public event EventHandler PG_EVT_Unpluged;

        /// <summary>
        /// Represents the method that will handle the Offset Removal Started event.<para />
        /// It will be fired once Potentiostat or Galvanostat offset removal started.
        /// </summary>
        public event EventHandler<OffsetRemovalStartedEventArgs> PG_EVT_OffsetRemovalStarted;

        /// <summary>
        /// Represents the method that will handle the Offset Removal Finished event.<para />
        /// It will be fired once Potentiostat or Galvanostat offset removal finished.
        /// </summary>
        public event EventHandler<OffsetRemovalFinishedEventArgs> PG_EVT_OffsetRemovalFinished;

        /// <summary>
        /// Represents the method that will handle the Async Process Started event.<para />
        /// It will be fired once an async process started.
        /// </summary>
        public event EventHandler<AProcessStartedEventArgs> PG_EVT_AProcessStarted;

        /// <summary>
        /// Represents the method that will handle the Async Process Finished event.<para />
        /// It will be fired once an async process finished.
        /// </summary>
        public event EventHandler<AProcessFinishedEventArgs> PG_EVT_AProcessFinished;

        /// <summary>
        /// Represents the method that will handle the Data Received event of ACommand process.<para />
        /// ACommand is the asynchronous mode of the method Command.
        /// </summary>
        public event EventHandler<ACommandEventArgs> PG_EVT_ACommandDataReceived;

        /// <summary>
        /// Represents the method that will handle the Log Received event.<para />
        /// </summary>
        public event EventHandler<LogReceivedEventArgs> PG_EVT_LogReceived;

        /// <summary>
        /// Represents the method that will handle the Data Received event of Adcget process.<para />
        /// Adcget is the asynchronous mode of the method dcget.<para />
        /// Note that the raw data of event, according to the values of the setdcselect and idcselect functions, will be translated to observable values. One can convert the data using ConvertToObservable.
        /// </summary>
        public event EventHandler<AdcgetEventArgs> PG_EVT_AdcgetDataReceived;

        /// <summary>
        /// Represents the method that will handle the Total Data Received event of AdcgetTotal process.<para />
        /// AdcgetTotal is the asynchronous mode of the method dcget.<para />
        /// Note that the raw data of event, according to the values of the setdcselect and idcselect functions, will be translated to observable values. One can convert the data using ConvertToObservable.
        /// </summary>
        public event EventHandler<AdcgetTotalEventArgs> PG_EVT_AdcgetTotalDataReceived;

        /// <summary>
        /// Represents the method that will handle the Data Received event of Aivset process.<para />
        /// Aivset is the asynchronous mode of the method ivset.<para />
        /// Note that the raw data of event, according to the values of the setdcselect and idcselect functions, will be translated to observable values. One can convert the data using ConvertToObservable.
        /// </summary>
        public event EventHandler<AivsetEventArgs> PG_EVT_AivsetDataReceived;

        /// <summary>
        /// Represents the method that will handle the Data Received event of ACV process.<para />
        /// ACV is the asynchronous mode of the method CV.
        /// </summary>
        public event EventHandler<ACVEventArgs> PG_EVT_ACVDataReceived;

        /// <summary>
        /// Represents the method that will handle the Data Received event of AIV process.<para />
        /// AIV is the asynchronous mode of the method IV.
        /// </summary>
        public event EventHandler<AIVEventArgs> PG_EVT_AIVDataReceived;

        /// <summary>
        /// Represents the method that will handle the Data Received event of AChrono process.<para />
        /// AChrono is the asynchronous mode of the method Chrono.
        /// </summary>
        public event EventHandler<AChronoEventArgs> PG_EVT_AChronoDataReceived;
        /****************************************************************************************************/

        /// <summary>
        /// Inputs for CV method.
        /// </summary>
        public struct CVProperties
        {
            /// <summary>
            /// in Volt.
            /// </summary>
            public double Initial_Potential;
            /// <summary>
            /// in Volt.
            /// </summary>
            public double Switching_Potential;
            /// <summary>
            /// in Volt.
            /// </summary>
            public double Final_Potential;
            public int Step;
            public int Number_of_Scans;
            public int Current_Range_Mode;
            public int Voltage_Range_Mode;
            /// <summary>
            /// Time step in milliseconds.
            /// </summary>
            public int Interval_Time;
            public int Voltage_Filter;
            public int Voltage_Multiplier_Mode;
            public int Current_Multiplier_Mode;
            public bool Is_Relative_Reference;
            public double Pretreatment_Voltage;
            public int Equilibration_Time;
            public bool Post_Processing_Prob_Off;
            /// <summary>
            /// in Volt.
            /// </summary>
            public double Ideal_Voltage;
        }

        /// <summary>
        /// Inputs for IV method.
        /// </summary>
        public struct IVProperties
        {
            /// <summary>
            /// in Volt.
            /// </summary>
            public double Initial_Potential;
            /// <summary>
            /// in Volt.
            /// </summary>
            public double Final_Potential;
            public int Step;
            public int Current_Range_Mode;
            public int Voltage_Range_Mode;
            public int Current_Range_Mode_Min;
            public int Current_Range_Mode_Max;
            public int Voltage_Range_Mode_Min;
            public int Voltage_Range_Mode_Max;
            public int Auto_Range_NCheck;
            public int Auto_Range_Time;
            /// <summary>
            /// Time step in milliseconds.
            /// </summary>
            public int Interval_Time;
            public int Voltage_Filter;
            public bool Is_Relative_Reference;
            /// <summary>
            /// Time step in milliseconds.
            /// </summary>
            public int Equilibration_Time;
            /// <summary>
            /// Digital low pass filter.
            /// </summary>
            public int Digital_Filter;
        }

        /// <summary>
        /// Inputs for Chrono method.
        /// </summary>
        public struct ChronoProperties
        {
            /// <summary>
            /// in Volt.
            /// </summary>
            public double V1;
            /// <summary>
            /// in Volt.
            /// </summary>
            public double V2;
            /// <summary>
            /// in Volt.
            /// </summary>
            public double V3;
            /// <summary>
            /// in Volt.
            /// </summary>
            public double V4;
            /// <summary>
            /// in Volt.
            /// </summary>
            public double V5;
            /// <summary>
            /// in Volt.
            /// </summary>
            public double V6;
            /// <summary>
            /// in Volt.
            /// </summary>
            public double V7;
            /// <summary>
            /// in Volt.
            /// </summary>
            public double V8;
            /// <summary>
            /// in Volt.
            /// </summary>
            public double V9;
            /// <summary>
            /// in Volt.
            /// </summary>
            public double V10;
            /// <summary>
            /// in second.
            /// </summary>
            public double T1;
            /// <summary>
            /// in second.
            /// </summary>
            public double T2;
            /// <summary>
            /// in second.
            /// </summary>
            public double T3;
            /// <summary>
            /// in second.
            /// </summary>
            public double T4;
            /// <summary>
            /// in second.
            /// </summary>
            public double T5;
            /// <summary>
            /// in second.
            /// </summary>
            public double T6;
            /// <summary>
            /// in second.
            /// </summary>
            public double T7;
            /// <summary>
            /// in second.
            /// </summary>
            public double T8;
            /// <summary>
            /// in second.
            /// </summary>
            public double T9;
            /// <summary>
            /// in second.
            /// </summary>
            public double T10;

            public bool OCP1;
            public bool OCP2;
            public bool OCP3;
            public bool OCP4;
            public bool OCP5;
            public bool OCP6;
            public bool OCP7;
            public bool OCP8;
            public bool OCP9;
            public bool OCP10;

            /// <summary>
            /// The number of parts Step=[1:10].
            /// </summary>
            public int Step;
            public int Current_Range_Mode;
            public int Voltage_Range_Mode;
            /// <summary>
            /// Time step in milliseconds.
            /// </summary>
            public int Interval_Time;
            public int Voltage_Filter;
            public int Voltage_Multiplier_Mode;
            public int Current_Multiplier_Mode;
            public bool Is_Relative_Reference;
            public double Pretreatment_Voltage;
            public int Equilibration_Time;
            public bool Post_Processing_Prob_Off;
            /// <summary>
            /// in Volt.
            /// </summary>
            public double Ideal_Voltage;
        }

        /// <summary>
        /// The constructor for the PGStat class.<para />
        /// The object contains all methods for controlling the device.
        /// </summary>
        public PGStat()
        {
            CheckPort.NewLine = "\r";

            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_SerialPort'");
            ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += new EventArrivedEventHandler(Unpluged);
            removeWatcher.Start();

            notification = new NotifyIcon();

            PingTimer = new System.Timers.Timer();
            SetPingPeriod(1.0);
            PingTimer.Elapsed += new ElapsedEventHandler(PingTimerTick);

            CV_Input = new CVProperties();
            IV_Input = new IVProperties();
        }

        private bool isPackageCompleted(string MethodName)
        {
            return false;
        }

        private void Notification_Click(object sender, EventArgs e)
        {
            NotifyIcon ThisNotifyIcon = sender as NotifyIcon;
            MessageBox.Show(ThisNotifyIcon.Text);
        }

        /// <summary>
        /// This method determines the verbosity of notifications.<para />
        /// 0: notification off.<para />
        /// 1: Release mode of final software for user.<para />
        /// 2: Debug mode for developer of the software.<para />
        /// 3: Debug mode for developer of the dll.
        /// </summary>
        public int SetNotificationVerbosity(int verbosity = 1)
        {
            int err = -1;
            if (verbosity < 0) return err;
            if (verbosity > 3) return err;
            NotificationsVerbosity = verbosity;
            return 0;
        }

        /// <summary>
        /// This function determines that the PGStat wills
        /// try to reconnect in case of accidentally disconnection.
        /// </summary>
        public void EnableAutoReconnect()
        {
            IsAutoReconnectEnabled = true;
        }

        /// <summary>
        /// This function determines that the PGStat avoids
        /// automatical reconnection in the case of accidentally disconnection.
        /// </summary>
        public void DisableAutoReconnect()
        {
            IsAutoReconnectEnabled = false;
        }

        /// <summary>
        /// Returns true if PGStat is connected to the device.
        /// </summary>
        public bool IsConnected()
        {
            return Connected;
        }

        /// <summary>
        /// Returns true if PGStat is pinging data to the software.
        /// </summary>
        public bool IsPingEnabled()
        {
            return isPingEnable;
        }

        /// <summary>
        /// Determines the period of "pinging process".
        /// </summary>
        public int SetPingPeriod(double sec)
        {
            int err = -1;
            int s = (int)(1000 * sec);
            if (s < 500) return err;
            if (s > 10000) return err;
            PingTimer.Interval = s;
            return 0;
        }

        private void PingTimerTick(object sender, EventArgs e)
        {
            if (Connected && isPingEnable)
            {
                double voltage = 0;
                double current = 0;
                /*
                int[,] output = dcget();
                for (int i = 0; i < dcgetNdata; i++)
                {
                    voltage += output[i, 0] / dcgetNdata;
                    current += output[i, 1] / dcgetNdata;
                }

                voltage = GetDCVConvert(voltage, 0, Lastsetselect);
                current = GetDCIConvert(current, 0, Lastidcselect);
                */
                PingEventArgs e1 = new PingEventArgs(voltage, current);
                PG_EVT_Ping?.Invoke(this, e1);
            }
            else
                PingTimer.Stop();
        }

        /// <summary>
        /// Try to discard both input and output buffers of the connected port.
        /// </summary>
        public void ClearBuffer()
        {
            try
            {
                Port.DiscardOutBuffer();
                Port.DiscardInBuffer();
            }
            catch
            { }
        }

        private void ClearBuffer(SerialPort port)
        {
            try
            {
                port.DiscardOutBuffer();
                port.DiscardInBuffer();
            }
            catch
            { }
        }

        /// <summary>
        /// Kills any running process.
        /// </summary>
        public void KillProcess()
        {
            KillProcessRequested = true;
            ClearBuffer();
            try
            {
                Port.Write(";");
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                SetStatus("Unable to kill the process.", ex.Message, "void KillProcess()");
            }
            ClearBuffer();
        }

        /// <summary>
        /// It turns on the pinging process.
        /// </summary>
        public void EnablePing()
        {
            isPingEnable = true;
            PingTimer.Start();
        }

        /// <summary>
        /// It turns off the pinging process.
        /// </summary>
        public void DisablePing()
        {
            isPingEnable = false; //The timer will be turned off in the next tick.
        }

        private string Command(SerialPort port, string command, int Delay = 100)
        {
            ClearBuffer();
            try
            {
                log(command);
                port.Write(command + ReadToChar);
            }
            catch (Exception ex)
            {
                SetStatus("Unable to send command to device.", ex.Message, "string Command(SerialPort port, string command, int Delay = 100)");
            }
            string reply = "";
            Thread.Sleep(Delay);
            try
            {
                reply = port.ReadTo(ReadToChar);
            }
            catch (Exception ex)
            {
                SetStatus("Unable to receive command from device.", ex.Message, "string Command(SerialPort port, string command, int Delay = 100)");
            }
            return reply;
        }

        private bool ACommand(SerialPort port, string command)
        {
            ClearBuffer();
            try
            {
                log(command);
                port.Write(command + ReadToChar);
                return true;
            }
            catch (Exception ex)
            {
                SetStatus("Unable to send command to device.", ex.Message, "bool ACommand(SerialPort port, string command, int Delay = 100)");
                return false;
            }
        }

        /// <summary>
        /// This method sends the command to device and returns the reply synchronously.
        /// </summary>
        public string Command(string command, int Delay = 100)
        {
            if (command.StartsWith("dcget") || command.StartsWith("ivset") || command.StartsWith("cv") || command.StartsWith("chrono"))
            {
                SetStatus("Unable to send command to device.", "You are not allowed to send this command via this method. It returns more than one output.", "string Command(string command, int Delay = 100)");
                return "Err";
            }

            if (!IsConnected())
            {
                SetStatus("Unable to send command to device. PGStat has not connected to device yet.");
                return "Err";
            }

            if (IsBusy())
            {
                SetStatus("Unable to send command to device.", "Port is busy by an async mode process.", "void Command(string command, int Delay = 100)");
                return "Err";
            }

            bool pingstatusHistory = IsPingEnabled();
            if (pingstatusHistory)
            {
                DisablePing();
                Thread.Sleep(200);
            }

            string ans = Command(Port, command, Delay);

            if (pingstatusHistory) EnablePing();

            return ans;
        }

        /// <summary>
        /// This method sends the command to device in asynchronous mode.<para />
        /// The reply will be accessible from the ACommandEventArgs of the callback PG_EVT_ACommandDataReceived.<para />
        /// If the command is sent successfully, True is returned. 
        /// </summary>
        public bool ACommand(string command)
        {
            if (command == "dcget" || command.StartsWith("ivset") || command.StartsWith("cv"))
            {
                SetStatus("Unable to send command to device.", "You are not allowed to send this command via this method. It returns more than one output.", "bool Command(string command, int Delay = 100)");
                return false;
            }

            if (!IsConnected())
            {
                SetStatus("Unable to send command to device. PGStat has not connected to device yet.");
                return false;
            }

            if (IsPingEnabled())
            {
                SetStatus("Unable to send command in this mode.", "The port is busy by the Ping process. You should disable ping by calling the function DisablePing().", "bool ACommand(string command, int Delay = 100)");
                return false;
            }

            if (IsBusy())
            {
                SetStatus("Unable to send command to device.", "Port is busy by another async mode process.", "bool ACommand(string command, int Delay = 100)");
                return false;
            }

            try
            {
                DataCountInQueue = 1;
                Thread t = new Thread(new ParameterizedThreadStart(ACommand_Process));
                t.Start(command);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ACommand_Process(object command)
        {
            AProcessStartedEventArgs e1 = new AProcessStartedEventArgs("ACommand");
            PG_EVT_AProcessStarted?.Invoke(this, e1);
            log((string)command);
            ACommand(Port, (string)command);
            Thread.Sleep(200);
            string ans = Port.ReadExisting();
            DataCountInQueue--;
            ACommandEventArgs e2 = new ACommandEventArgs(ans);
            PG_EVT_ACommandDataReceived?.Invoke(this, e2);
            if (DataCountInQueue == 0)
            {
                AProcessFinishedEventArgs e3 = new AProcessFinishedEventArgs("ACommand");
                PG_EVT_AProcessFinished?.Invoke(this, e3);
            }
        }

        private void log(string info)
        {
            if (NotificationsVerbosity > 2)
            {
                LogReceivedEventArgs e1 = new LogReceivedEventArgs(info);
                PG_EVT_LogReceived?.Invoke(this, e1);
            }
        }

        private bool SendAndReceiveOK(string command)
        {
            string response = Command(command);
            log(response);
            if (response == "OK")
                return true;
            else
                return false;
        }

        /// <summary>
        /// This function changes the PGmode.<para />
        /// 0:<para />
        /// 1:<para />
        /// 2:
        /// </summary>
        public void PGmode(int value)
        {
            string order = "PGmode " + value.ToString();
            if (SendAndReceiveOK(order)) LastPGmode = value;
        }

        /// <summary>
        /// This function changes the voltage filter.<para />
        /// 0:<para />
        /// 1:<para />
        /// 2:
        /// </summary>
        public void vfilter(int value)
        {
            string order = "vfilter " + value.ToString();
            if (SendAndReceiveOK(order)) Lastvfilter = value;
        }

        /// <summary>
        /// This function changes the DC current filter.<para />
        /// 0:<para />
        /// 1:<para />
        /// 2:
        /// </summary>
        public void idcfilter(int value)
        {
            string order = "idcfilter " + value.ToString();
            if (SendAndReceiveOK(order)) Lastidcfilter = value;
        }

        /// <summary>
        /// This function changes the DC current range.<para />
        /// 0:<para />
        /// 1:<para />
        /// 2:<para />
        /// 3:
        /// </summary>
        public void idcselect(int value)
        {
            string order = "idcselect " + value.ToString();
            if (SendAndReceiveOK(order)) Lastidcselect = value;
        }

        /// <summary>
        /// This function changes the voltage range.<para />
        /// 0:<para />
        /// 1:<para />
        /// 2:<para />
        /// 3:
        /// </summary>
        public void setselect(int value)
        {
            string order = "setselect " + value.ToString();
            if (SendAndReceiveOK(order)) Lastsetselect = value;
        }

        /// <summary>
        /// This function changes the DC current multiplier.<para />
        /// 0:<para />
        /// 1:<para />
        /// 2:<para />
        /// 3:
        /// </summary>
        public void idcmlp(int value)
        {
            string order = "idcmlp " + value.ToString();
            if (SendAndReceiveOK(order)) Lastidcmlp = value;
        }

        /// <summary>
        /// This function changes the DC voltage multiplier.<para />
        /// 0:<para />
        /// 1:<para />
        /// 2:<para />
        /// 3:
        /// </summary>
        public void vdcmlp(int value)
        {
            string order = "vdcmlp " + value.ToString();
            if (SendAndReceiveOK(order)) Lastvdcmlp = value;
        }

        /// <summary>
        /// This function sets the zero level of DC voltage.<para />
        /// The input value should be set between 0 and 4096.
        /// </summary>
        public void zeroset(int value)
        {
            string order = "zeroset " + value.ToString();
            if (SendAndReceiveOK(order)) Lastzeroset = value;
        }

        /// <summary>
        /// This function sets the AC voltage.
        /// </summary>
        public void acset(int value)
        {
            string order = "acset " + value.ToString();
            if (SendAndReceiveOK(order)) Lastacset = value;
        }

        /// <summary>
        /// This function sets the zero level of the AC current.
        /// </summary>
        public void aczero(int value)
        {
            string order = "aczero " + value.ToString();
            if (SendAndReceiveOK(order)) Lastaczero = value;
        }

        /// <summary>
        /// This function sets the zero level of the AC voltage.
        /// </summary>
        public void vaczero(int value)
        {
            string order = "vaczero " + value.ToString();
            if (SendAndReceiveOK(order)) Lastvaczero = value;
        }

        /// <summary>
        /// Sets the input00.
        /// </summary>
        public void input00(int value)
        {
            string order = "input 00 " + value.ToString();
            if (SendAndReceiveOK(order)) Lastinput00 = value;
        }

        /// <summary>
        /// Sets the input01.
        /// </summary>
        public void input01(int value)
        {
            string order = "input 01 " + value.ToString();
            if (SendAndReceiveOK(order)) Lastinput01 = value;
        }

        /// <summary>
        /// Sets the input02.
        /// </summary>
        public void input02(int value)
        {
            string order = "input 02 " + value.ToString();
            if (SendAndReceiveOK(order)) Lastinput02 = value;
        }

        /// <summary>
        /// Sets the input03.
        /// </summary>
        public void input03(int value)
        {
            string order = "input 03 " + value.ToString();
            if (SendAndReceiveOK(order)) Lastinput03 = value;
        }

        /// <summary>
        /// Sets the input04.
        /// </summary>
        public void input04(int value)
        {
            string order = "input 04 " + value.ToString();
            if (SendAndReceiveOK(order)) Lastinput04 = value;
        }

        /// <summary>
        /// Sets the input05.
        /// </summary>
        public void input05(int value)
        {
            string order = "input 05 " + value.ToString();
            if (SendAndReceiveOK(order)) Lastinput05 = value;
        }

        /// <summary>
        /// Sets the input06.
        /// </summary>
        public void input06(int value)
        {
            string order = "input 06 " + value.ToString();
            if (SendAndReceiveOK(order)) Lastinput06 = value;
        }

        /// <summary>
        /// Sets the input07.
        /// </summary>
        public void input07(int value)
        {
            string order = "input 07 " + value.ToString();
            if (SendAndReceiveOK(order)) Lastinput07 = value;
        }

        /// <summary>
        /// Sets the input08.
        /// </summary>
        public void input08(int value)
        {
            string order = "input 08 " + value.ToString();
            if (SendAndReceiveOK(order)) Lastinput08 = value;
        }

        /// <summary>
        /// Sets the input09.
        /// </summary>
        public void input09(int value)
        {
            string order = "input 09 " + value.ToString();
            if (SendAndReceiveOK(order)) Lastinput09 = value;
        }

        /// <summary>
        /// Sets the input10.
        /// </summary>
        public void input10(int value)
        {
            string order = "input 10 " + value.ToString();
            if (SendAndReceiveOK(order)) Lastinput10 = value;
        }

        /// <summary>
        /// Sets the tinput00.
        /// </summary>
        public void tinput00(int value)
        {
            string order = "tinput 00 " + value.ToString();
            if (SendAndReceiveOK(order)) Lasttinput00 = value;
        }

        /// <summary>
        /// Sets the tinput01.
        /// </summary>
        public void tinput01(int value)
        {
            string order = "tinput 01 " + value.ToString();
            if (SendAndReceiveOK(order)) Lasttinput01 = value;
        }

        /// <summary>
        /// Sets the tinput02.
        /// </summary>
        public void tinput02(int value)
        {
            string order = "tinput 02 " + value.ToString();
            if (SendAndReceiveOK(order)) Lasttinput02 = value;
        }

        /// <summary>
        /// Sets the tinput03.
        /// </summary>
        public void tinput03(int value)
        {
            string order = "tinput 03 " + value.ToString();
            if (SendAndReceiveOK(order)) Lasttinput03 = value;
        }

        /// <summary>
        /// Sets the tinput04.
        /// </summary>
        public void tinput04(int value)
        {
            string order = "tinput 04 " + value.ToString();
            if (SendAndReceiveOK(order)) Lasttinput04 = value;
        }

        /// <summary>
        /// Sets the tinput05.
        /// </summary>
        public void tinput05(int value)
        {
            string order = "tinput 05 " + value.ToString();
            if (SendAndReceiveOK(order)) Lasttinput05 = value;
        }

        /// <summary>
        /// Sets the tinput06.
        /// </summary>
        public void tinput06(int value)
        {
            string order = "tinput 06 " + value.ToString();
            if (SendAndReceiveOK(order)) Lasttinput06 = value;
        }

        /// <summary>
        /// Sets the tinput07.
        /// </summary>
        public void tinput07(int value)
        {
            string order = "tinput 07 " + value.ToString();
            if (SendAndReceiveOK(order)) Lasttinput07 = value;
        }

        /// <summary>
        /// Sets the tinput08.
        /// </summary>
        public void tinput08(int value)
        {
            string order = "tinput 08 " + value.ToString();
            if (SendAndReceiveOK(order)) Lasttinput08 = value;
        }

        /// <summary>
        /// Sets the tinput09.
        /// </summary>
        public void tinput09(int value)
        {
            string order = "tinput 09 " + value.ToString();
            if (SendAndReceiveOK(order)) Lasttinput09 = value;
        }

        /// <summary>
        /// Sets the tinput10.
        /// </summary>
        public void tinput10(int value)
        {
            string order = "tinput 10 " + value.ToString();
            if (SendAndReceiveOK(order)) Lasttinput10 = value;
        }

        /// <summary>
        /// Sets the Ninput00.
        /// </summary>
        public void Ninput00(int value)
        {
            string order = "Ninput 00 " + value.ToString();
            if (SendAndReceiveOK(order)) LastNinput00 = value;
        }

        /// <summary>
        /// Sets the Ninput01.
        /// </summary>
        public void Ninput01(int value)
        {
            string order = "Ninput 01 " + value.ToString();
            if (SendAndReceiveOK(order)) LastNinput01 = value;
        }

        /// <summary>
        /// Sets the Ninput02.
        /// </summary>
        public void Ninput02(int value)
        {
            string order = "Ninput 02 " + value.ToString();
            if (SendAndReceiveOK(order)) LastNinput02 = value;
        }

        /// <summary>
        /// Sets the Ninput03.
        /// </summary>
        public void Ninput03(int value)
        {
            string order = "Ninput 03 " + value.ToString();
            if (SendAndReceiveOK(order)) LastNinput03 = value;
        }

        /// <summary>
        /// Sets the Ninput04.
        /// </summary>
        public void Ninput04(int value)
        {
            string order = "Ninput 04 " + value.ToString();
            if (SendAndReceiveOK(order)) LastNinput04 = value;
        }

        /// <summary>
        /// Sets the Ninput05.
        /// </summary>
        public void Ninput05(int value)
        {
            string order = "Ninput 05 " + value.ToString();
            if (SendAndReceiveOK(order)) LastNinput05 = value;
        }

        /// <summary>
        /// Sets the Ninput06.
        /// </summary>
        public void Ninput06(int value)
        {
            string order = "Ninput 06 " + value.ToString();
            if (SendAndReceiveOK(order)) LastNinput06 = value;
        }

        /// <summary>
        /// Sets the Ninput07.
        /// </summary>
        public void Ninput07(int value)
        {
            string order = "Ninput 07 " + value.ToString();
            if (SendAndReceiveOK(order)) LastNinput07 = value;
        }

        /// <summary>
        /// Sets the Ninput08.
        /// </summary>
        public void Ninput08(int value)
        {
            string order = "Ninput 08 " + value.ToString();
            if (SendAndReceiveOK(order)) LastNinput08 = value;
        }

        /// <summary>
        /// Sets the Ninput09.
        /// </summary>
        public void Ninput09(int value)
        {
            string order = "Ninput 09 " + value.ToString();
            if (SendAndReceiveOK(order)) LastNinput09 = value;
        }

        /// <summary>
        /// Sets the Ninput10.
        /// </summary>
        public void Ninput10(int value)
        {
            string order = "Ninput 10 " + value.ToString();
            if (SendAndReceiveOK(order)) LastNinput10 = value;
        }

        /// <summary>
        /// Start the method.
        /// method = 0 : CV
        /// method = 1 : Pulse
        /// method = 2 : Chrono
        /// </summary>
        public void start(int method, int wait)
        {
            string order = "start " + method.ToString() + " " + wait.ToString();
            if (!ACommand(Port, order)) return;
        }

        /// <summary>
        /// Sets the sn.
        /// </summary>
        public void sn(int value)
        {
            string order = "sn " + value.ToString();
            if (SendAndReceiveOK(order)) Lastsn = value;
        }

        /// <summary>
        /// Sets the Tget.
        /// </summary>
        public void Tget(int value)
        {
            string order = "T_get " + value.ToString();
            if (SendAndReceiveOK(order)) LastTget = value;
        }

        /// <summary>
        /// Sets the dcset.
        /// </summary>
        public void dcset(int value)
        {
            string order = "dcset " + value.ToString();
            if (SendAndReceiveOK(order)) Lastdcset = value;
        }

        /// <summary>
        /// Sets the ddsclk.
        /// </summary>
        public void ddsclk(int value)
        {
            string order = "ddsclk " + value.ToString();
            if (SendAndReceiveOK(order)) Lastddsclk = value;
        }

        /// <summary>
        /// Sets the dds.
        /// </summary>
        public void dds(int value)
        {
            string order = "dds " + value.ToString();
            if (SendAndReceiveOK(order)) Lastdds = value;
        }

        /// <summary>
        /// Sets the vac.
        /// </summary>
        public void vac(int value)
        {
            string order = "vac " + value.ToString();
            if (SendAndReceiveOK(order)) Lastvac = value;
        }

        /// <summary>
        /// DCpreget command.
        /// </summary>
        public void DCpreget()
        {
            string order = "DCpreget";
            SendAndReceiveOK(order);
        }

        /// <summary>
        /// ACpreset command.
        /// </summary>
        public void ACpreset(int value)
        {
            string order = "ACpreset";
            SendAndReceiveOK(order);
        }

        /// <summary>
        /// This method connect or disconnect the dummy probe;<para />
        /// The dummy is a 1KOhm resistor.<para />
        /// 0: Disconnect<para />
        /// 1: Connect
        /// </summary>
        public void dummy(int value)
        {
            string order = "dummyon " + value.ToString();
            if (SendAndReceiveOK(order))
            {
                Lastdummy = value;
            }
        }

        /// <summary>
        /// This method connect or disconnect the sample probe;<para />
        /// 0: Disconnect<para />
        /// 1: Connect
        /// </summary>
        public void sample(int value)
        {
            string order = "sampleon " + value.ToString();
            if (SendAndReceiveOK(order)) Lastsample = value;
        }

        /// <summary>
        /// Sets the RelRef state. Respectively, false and true are Absolute and Relative modes.<para />
        /// If it is true, the potential of the working electrode is relative to the reference electrode.
        /// </summary>
        public void RelRefState(bool value)
        {
            LastRelRefState = value;
        }

        /// <summary>
        /// Sets the Open Circuit Potential state. Respectively, false and true are Inactive and Active.
        /// </summary>
        public void OCP(bool value)
        {
            LastOCPState = value;
        }

        /// <summary>
        /// Sets the Open Circuit Potential. It can be voltage or current with respect to the PGMode. The unit for set voltage/current is Volt/Ampere.
        /// </summary>
        public void OCP(double value)
        {
            LastOCP = value;
        }

        /// <summary>
        /// Sets the equilibrium time in (ms). It is the waiting time just after the pretreatment voltage/current is set.
        /// </summary>
        public void EqTime(int value)
        {
            LastEqTime = value;
        }

        /// <summary>
        /// Sets the pretreatment. It can be voltage or current with respect to the set PGMode.
        /// </summary>
        public void Pretreatment(double value)
        {
            LastPretreatment = value;
        }

        /// <summary>
        /// Sets the ideal value for voltage/current in Volt/Ampere with respect to the set PGMode.
        /// </summary>
        public void IdealVoltage(double value)
        {
            LastIdealVoltage = value;
        }

        /// <summary>
        /// Convert an observable value to raw data.
        /// </summary>
        public int ToRawData(double value)
        {
            int VSelect = Lastsetselect;
            int ISelect = Lastidcselect;
            int PGMode = LastPGmode;
            double VltOrCurrentInGalvanostat = value;

            int ivlt;
            if (PGMode != 3)
            {
                double vlt = VltOrCurrentInGalvanostat;
                if (VSelect == 0)
                    vlt = vlt / Settings.SetDCV_Select0;
                else
                    vlt = vlt / Settings.SetDCV_Select1;
                ivlt = (int)(vlt / Settings.SetDCV_Domain * 2047) + Settings.SetDCV_Offset;
                if (ivlt > 4095) ivlt = 4095;
                if (ivlt < 0) ivlt = 0;
            }
            else
            {
                double amp = -VltOrCurrentInGalvanostat;
                if (ISelect == 0)
                    ivlt = (int)(amp * 2047 * Settings.GetDCI_Select0 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 1)
                    ivlt = (int)(0.001 * amp * 2047 * Settings.GetDCI_Select1 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 2)
                    ivlt = (int)(0.001 * amp * 2047 * Settings.GetDCI_select2 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 3)
                    ivlt = (int)(0.001 * amp * 2047 * Settings.GetDCI_Select3 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 4)
                    ivlt = (int)(0.000001 * amp * 2047 * Settings.GetDCI_Select4 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 5)
                    ivlt = (int)(0.000001 * amp * 2047 * Settings.GetDCI_Select5 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 6)
                    ivlt = (int)(0.000001 * amp * 2047 * Settings.GetDCI_Select6 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
                else
                    ivlt = (int)(0.000000001 * amp * 2047 * Settings.GetDCI_Select7 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);


                if (ivlt > 4095) ivlt = 4095;
                if (ivlt < 0) ivlt = 0;
            }
            return ivlt;
        }

        /// <summary>
        /// Returns the raw data of Voltage and Current.<para />
        /// Note, the raw data for both input and output values, according to the values of the setdcselect and idcselect functions, will be translated to observable values. One can convert the data using ConvertToObservable.
        /// </summary>
        public int[,] dcget()
        {
            int[,] Output = new int[dcgetNdata, 2];
            if (!ACommand(Port, "dcget")) return Output;

            try
            {
                for (int iData = 0; iData < dcgetNdata; iData++)
                {
                    int word;
                    int AllBytes0 = Port.ReadByte();
                    int AllBytes1 = Port.ReadByte();
                    int AllBytes2 = Port.ReadByte();
                    int AllBytes3 = Port.ReadByte();

                    word = AllBytes0 + AllBytes1 * 256;
                    if (!SettingsIsIVReceiverUnsigned)
                    {
                        if (word >= 0 && word < 2048)
                            word = word + 2048;
                        else
                            word = word - 2048 - 61440;
                    }
                    Output[iData, 0] = word;

                    word = AllBytes2 + AllBytes3 * 256;
                    if (!SettingsIsIVReceiverUnsigned)
                    {
                        if (word >= 0 && word < 2048)
                            word = word + 2048;
                        else
                            word = word - 2048 - 61440;
                    }
                    Output[iData, 1] = word;
                }
            }
            catch { }
            return Output;
        }

        private void Adcget_Process()
        {
            AProcessStartedEventArgs e1 = new AProcessStartedEventArgs("Adcget");
            PG_EVT_AProcessStarted?.Invoke(this, e1);
            if (!ACommand(Port, "dcget")) return;

            try
            {
                for (int iData = 0; iData < dcgetNdata; iData++)
                {
                    int word, voltage, current;
                    int AllBytes0 = Port.ReadByte();
                    int AllBytes1 = Port.ReadByte();
                    int AllBytes2 = Port.ReadByte();
                    int AllBytes3 = Port.ReadByte();

                    word = AllBytes0 + AllBytes1 * 256;
                    if (!SettingsIsIVReceiverUnsigned)
                    {
                        if (word >= 0 && word < 2048)
                            word = word + 2048;
                        else
                            word = word - 2048 - 61440;
                    }
                    voltage = word;

                    word = AllBytes2 + AllBytes3 * 256;
                    if (!SettingsIsIVReceiverUnsigned)
                    {
                        if (word >= 0 && word < 2048)
                            word = word + 2048;
                        else
                            word = word - 2048 - 61440;
                    }
                    current = word;

                    DataCountInQueue--;
                    AdcgetEventArgs e2 = new AdcgetEventArgs(dcgetNdata, iData, voltage, current);
                    PG_EVT_AdcgetDataReceived?.Invoke(this, e2);
                }
            }
            catch { }

            if (DataCountInQueue == 0)
            {
                AProcessFinishedEventArgs e3 = new AProcessFinishedEventArgs("Adcget");
                PG_EVT_AProcessFinished?.Invoke(this, e3);
            }
        }

        private void AdcgetTotal_Process()
        {
            AProcessStartedEventArgs e1 = new AProcessStartedEventArgs("AdcgetTotal");
            PG_EVT_AProcessStarted?.Invoke(this, e1);
            if (!ACommand(Port, "dcget")) return;
            try
            {
                int[] VotageArray = new int[dcgetNdata];
                int[] CurrentArray = new int[dcgetNdata];
                for (int iData = 0; iData < dcgetNdata; iData++)
                {
                    int word;
                    int AllBytes0 = Port.ReadByte();
                    int AllBytes1 = Port.ReadByte();
                    int AllBytes2 = Port.ReadByte();
                    int AllBytes3 = Port.ReadByte();

                    word = AllBytes0 + AllBytes1 * 256;
                    if (!SettingsIsIVReceiverUnsigned)
                    {
                        if (word >= 0 && word < 2048)
                            word = word + 2048;
                        else
                            word = word - 2048 - 61440;
                    }
                    VotageArray[iData] = word;

                    word = AllBytes2 + AllBytes3 * 256;
                    if (!SettingsIsIVReceiverUnsigned)
                    {
                        if (word >= 0 && word < 2048)
                            word = word + 2048;
                        else
                            word = word - 2048 - 61440;
                    }
                    CurrentArray[iData] = word;
                    DataCountInQueue--;
                }
                AdcgetTotalEventArgs e2 = new AdcgetTotalEventArgs(dcgetNdata, VotageArray, CurrentArray);
                PG_EVT_AdcgetTotalDataReceived?.Invoke(this, e2);
            }
            catch { }

            if (DataCountInQueue == 0)
            {
                AProcessFinishedEventArgs e3 = new AProcessFinishedEventArgs("Adcget");
                PG_EVT_AProcessFinished?.Invoke(this, e3);
            }
        }

        /// <summary>
        /// This method start the command dcget in asynchronous mode.<para />
        /// The reply will be accessible from the AdcgetEventArgs of the callback PG_EVT_AdcgetDataReceived.<para />
        /// The data will be passed one by one including its index.<para />
        /// This method is suitable in the case of real time plotting. Note, the plotter should be fast enough.<para />
        /// If the command is sent successfully, True is returned.
        /// </summary>
        public bool Adcget()
        {
            try
            {
                DataCountInQueue = dcgetNdata;
                Thread t = new Thread(new ThreadStart(Adcget_Process));
                t.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// This method start the command dcget in asynchronous mode.<para />
        /// The reply will be accessible from the AdcgetTotalEventArgs of the callback PG_EVT_AdcgetTotalDataReceived.<para />
        /// The whole data will be collected in memory and the total array will be passed to AdcgetTotalEventArgs.<para />
        /// If the command is sent successfully, True is returned.
        /// </summary>
        public bool AdcgetTotal()
        {
            try
            {
                DataCountInQueue = dcgetNdata;
                Thread t = new Thread(new ThreadStart(AdcgetTotal_Process));
                t.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// This method set the desire voltage/current in potentiostat/galvanostat mode and returns the real voltage and real current.<para />
        /// The input is a raw data not real voltage/current. The raw data is between 0 and 4095. The middle point is 2047.<para />
        /// Note, the raw data for both input and output values, according to the values of the setdcselect and idcselect functions, will be translated to observable values. One can convert the data using ConvertToObservable.
        /// </summary>
        public double[] ivset(int value)
        {
            double[] Output = new double[2];
            string order = "ivset " + value.ToString();
            if (!ACommand(Port, order))
            {
                return Output;
            }

            try
            {
                byte[] AllBytes1 = new byte[4];
                byte[] AllBytes2 = new byte[4];
                int word;

                AllBytes1[0] = (byte)Port.ReadByte();
                AllBytes1[1] = (byte)Port.ReadByte();
                AllBytes1[2] = (byte)Port.ReadByte();
                AllBytes1[3] = (byte)Port.ReadByte();
                AllBytes2[0] = (byte)Port.ReadByte();
                AllBytes2[1] = (byte)Port.ReadByte();
                AllBytes2[2] = (byte)Port.ReadByte();
                AllBytes2[3] = (byte)Port.ReadByte();

                int nData = IVnData;
                word = AllBytes1[0] | (AllBytes1[1] << 8) | (AllBytes1[2] << 16) | (AllBytes1[3] << 24);
                Output[0] = (double)word / (double)nData;

                word = AllBytes2[0] | (AllBytes2[1] << 8) | (AllBytes2[2] << 16) | (AllBytes2[3] << 24);
                Output[1] = (double)word / (double)nData;
            }
            catch { }
            return Output;
        }

        private void Aivset_Process(object value)
        {
            AProcessStartedEventArgs e1 = new AProcessStartedEventArgs("Aivset");
            PG_EVT_AProcessStarted?.Invoke(this, e1);
            int intval = (int)value;
            string order = "ivset " + intval.ToString();
            if (!ACommand(Port, order)) return;

            try
            {
                byte[] AllBytes1 = new byte[4];
                byte[] AllBytes2 = new byte[4];
                int word;
                double voltage, current;

                AllBytes1[0] = (byte)Port.ReadByte();
                AllBytes1[1] = (byte)Port.ReadByte();
                AllBytes1[2] = (byte)Port.ReadByte();
                AllBytes1[3] = (byte)Port.ReadByte();
                AllBytes2[0] = (byte)Port.ReadByte();
                AllBytes2[1] = (byte)Port.ReadByte();
                AllBytes2[2] = (byte)Port.ReadByte();
                AllBytes2[3] = (byte)Port.ReadByte();

                int nData = IVnData;
                word = AllBytes1[0] | (AllBytes1[1] << 8) | (AllBytes1[2] << 16) | (AllBytes1[3] << 24);
                voltage = (double)word / (double)nData;

                word = AllBytes2[0] | (AllBytes2[1] << 8) | (AllBytes2[2] << 16) | (AllBytes2[3] << 24);
                current = (double)word / (double)nData;

                DataCountInQueue--;
                AivsetEventArgs e2 = new AivsetEventArgs(voltage, current);
                PG_EVT_AivsetDataReceived?.Invoke(this, e2);
            }
            catch { }

            if (DataCountInQueue == 0)
            {
                AProcessFinishedEventArgs e3 = new AProcessFinishedEventArgs("Aivset");
                PG_EVT_AProcessFinished?.Invoke(this, e3);
            }
        }

        private void resetdevice()
        {
            ddsclk(1);
            Thread.Sleep(10);
            dds(0);
            Thread.Sleep(10);
            acset(0);
            Thread.Sleep(10);
            vac((Int16)Settings.GalvanostatI_Select3);
            Thread.Sleep(10);
            zeroset(Settings.Zeroset0);
            Thread.Sleep(10);
            DCpreget();
            Thread.Sleep(10);
        }

        /// <summary>
        /// This method start the command ivset in asynchronous mode.<para />
        /// The reply will be accessible from the AivsetEventArgs of the callback PG_EVT_AivsetDataReceived.<para />
        /// The raw values of voltage and current will be collected in an array and passed to AivsetEventArgs.<para />
        /// If the command is sent successfully, True is returned.
        /// </summary>
        public bool Aivset(int val)
        {
            try
            {
                DataCountInQueue = 1;
                Thread t = new Thread(new ParameterizedThreadStart(Aivset_Process));
                t.Start(val);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CalculateSpecificIVOffsets(int ISelect, int Imlt, int Vmlt, ref double Ioffset, ref double Voffset)
        {
            int OldSample = Lastsample;
            int OldDummy = Lastdummy;

            if (BoardType > 1)
            {
                if (ISelect == 0)
                    dummy(1);
                else
                    dummy(0);
            }
            else
                dummy(0);

            sample(0);
            dummy(1);
            vdcmlp(Vmlt);
            idcmlp(Imlt);
            idcselect(ISelect);
            if (ISelect > 4)
                Thread.Sleep(1000);
            else
                Thread.Sleep(100);
            double[] dummyout = ivset(2047);
            Thread.Sleep(1000);
            dummyout = ivset(2047);

            Voffset = dummyout[0];
            Ioffset = dummyout[1];

            if (OldDummy == 1)
                dummy(1);
            else
                dummy(0);

            if (OldSample == 1)
                sample(1);

            Thread.Sleep(500);
        }

        private void SetIVFilter(int EISfilterMode, double tPeriod, int EISDCCurrentRangeMode, int EISVoltageRangeMode)
        {
            int IFilter;
            int VFilter;
            double tPeriodMicro = 1000000.0 * tPeriod;
            if (EISfilterMode == 0)
            {
                double R;
                if (EISVoltageRangeMode == 0) R = 5.0; //5 k
                else R = 1.0; //1 k
                double C_V = tPeriodMicro / Math.Acos(-1.0) / R / 20;
                if (C_V <= Settings.FilterC_V1)
                    VFilter = 0;
                else if (C_V > Settings.FilterC_V1 && C_V < Settings.FilterC_V2)
                    VFilter = 1;
                else
                    VFilter = 2;

                if (EISDCCurrentRangeMode == 0) R = Settings.GetDCI_Select0;
                else if (EISDCCurrentRangeMode == 1) R = Settings.GetDCI_Select1;
                else if (EISDCCurrentRangeMode == 2) R = Settings.GetDCI_select2;
                else if (EISDCCurrentRangeMode == 3) R = Settings.GetDCI_Select3;
                else if (EISDCCurrentRangeMode == 4) R = Settings.GetDCI_Select4;
                else if (EISDCCurrentRangeMode == 5) R = Settings.GetDCI_Select5;
                else if (EISDCCurrentRangeMode == 6) R = Settings.GetDCI_Select6;
                else if (EISDCCurrentRangeMode == 7) R = Settings.GetDCI_Select7;
                double C_I = 100.0 * (tPeriodMicro - 0.1) / R; //1000.0 * tPeriodMicro / R / 10;
                if (C_I <= Settings.FilterC_I1)
                    IFilter = 0;
                else if (C_I > Settings.FilterC_I1 && C_I < Settings.FilterC_I2)
                    IFilter = 1;
                else
                    IFilter = 2;
                //SetLabel(ref Label_Filter_C_V, C_V, "V");
                //SetLabel(ref Label_Filter_C_I, C_I, "A");
            }
            else
            {
                IFilter = 0;
                VFilter = 1;
                //Label_Filter_C_V.Text = "-------";
                //Label_Filter_C_I.Text = "-------";
            }

            vfilter(VFilter);
            idcfilter(IFilter);
        }

        private void GetVOffsetFromSettings(int vmlp, ref double Voffset)
        {
            Voffset = Settings.GetDCV_OffsetMLP0;
            if (vmlp == 1)
                Voffset = Settings.GetDCV_OffsetMLP1;
            else if (vmlp == 2)
                Voffset = Settings.GetDCV_OffsetMLP2;
            else if (vmlp == 3)
                Voffset = Settings.GetDCV_OffsetMLP3;
            else if (vmlp == 4)
                Voffset = Settings.GetDCV_OffsetMLP4;
            else if (vmlp == 5)
                Voffset = Settings.GetDCV_offsetMLP5;
            else if (vmlp == 6)
                Voffset = Settings.GetDCV_OffsetMLP6;
        }

        public int SetDCVConvert(double VltOrCurrentInGalvanostat, int VSelect, int ISelect, int PGMode)
        {
            int ivlt;
            if (PGMode != 3)
            {
                double vlt = VltOrCurrentInGalvanostat;
                if (VSelect == 0)
                    vlt = vlt / Settings.SetDCV_Select0;
                else
                    vlt = vlt / Settings.SetDCV_Select1;
                ivlt = (int)(vlt / Settings.SetDCV_Domain * 2047) + Settings.SetDCV_Offset;
                if (ivlt > 4095) ivlt = 4095;
                if (ivlt < 0) ivlt = 0;
            }
            else
            {
                double GetDCI_Domain = Settings.SetDCV_Domain / (-5);
                double amp = -VltOrCurrentInGalvanostat;
                if (ISelect == 0)
                    ivlt = (int)(amp * 2047 * Settings.GetDCI_Select0 / GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 1)
                    ivlt = (int)(0.001 * amp * 2047 * Settings.GetDCI_Select1 / GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 2)
                    ivlt = (int)(0.001 * amp * 2047 * Settings.GetDCI_select2 / GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 3)
                    ivlt = (int)(0.001 * amp * 2047 * Settings.GetDCI_Select3 / GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 4)
                    ivlt = (int)(0.000001 * amp * 2047 * Settings.GetDCI_Select4 / GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 5)
                    ivlt = (int)(0.000001 * amp * 2047 * Settings.GetDCI_Select5 / GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 6)
                    ivlt = (int)(0.000001 * amp * 2047 * Settings.GetDCI_Select6 / GetDCI_Domain + Settings.SetDCV_Offset);
                else
                    ivlt = (int)(0.000000001 * amp * 2047 * Settings.GetDCI_Select7 / GetDCI_Domain + Settings.SetDCV_Offset);

                if (ivlt > 4095) ivlt = 4095;
                if (ivlt < 0) ivlt = 0;
            }
            return ivlt;
        }

        public int SetDCVConvert_dV(double VltOrCurrentInGalvanostat, int VSelect, int ISelect, int PGMode)
        {
            int ivlt;
            if (PGMode != 3)
            {
                double vlt = VltOrCurrentInGalvanostat;
                if (VSelect == 0)
                    vlt = vlt / Settings.SetDCV_Select0;
                else
                    vlt = vlt / Settings.SetDCV_Select1;
                ivlt = (int)(vlt / Settings.SetDCV_Domain * 2047) + Settings.SetDCV_Offset;
            }
            else
            {
                double amp = -VltOrCurrentInGalvanostat;
                if (ISelect == 0)
                    ivlt = (int)(amp * 2047 * Settings.GetDCI_Select0 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 1)
                    ivlt = (int)(0.001 * amp * 2047 * Settings.GetDCI_Select1 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 2)
                    ivlt = (int)(0.001 * amp * 2047 * Settings.GetDCI_select2 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 3)
                    ivlt = (int)(0.001 * amp * 2047 * Settings.GetDCI_Select3 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 4)
                    ivlt = (int)(0.000001 * amp * 2047 * Settings.GetDCI_Select4 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 5)
                    ivlt = (int)(0.000001 * amp * 2047 * Settings.GetDCI_Select5 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
                else if (ISelect == 6)
                    ivlt = (int)(0.000001 * amp * 2047 * Settings.GetDCI_Select6 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
                else
                    ivlt = (int)(0.000000001 * amp * 2047 * Settings.GetDCI_Select7 / Settings.GetDCI_Domain + Settings.SetDCV_Offset);
            }
            return ivlt;
        }

        public double Inverse_SetDCVConvert_dV(int VltOrCurrentInGalvanostat_Int, int VSelect, int ISelect, int PGMode)
        {
            double vlt;
            if (PGMode != 3)
            {
                int ivlt = VltOrCurrentInGalvanostat_Int;
                vlt = (ivlt - Settings.SetDCV_Offset) * Settings.SetDCV_Domain / 2047;

                if (VSelect == 0)
                    vlt = vlt * Settings.SetDCV_Select0;
                else
                    vlt = vlt * Settings.SetDCV_Select1;

            }
            else
            {
                int iamp = VltOrCurrentInGalvanostat_Int;

                if (ISelect == 0)
                    vlt = (iamp - Settings.SetDCV_Offset) * Settings.GetDCI_Domain / Settings.GetDCI_Select0 / 2047;
                else if (ISelect == 1)
                    vlt = (iamp - Settings.SetDCV_Offset) * Settings.GetDCI_Domain / Settings.GetDCI_Select1 / 2047 / 0.001;
                else if (ISelect == 2)
                    vlt = (iamp - Settings.SetDCV_Offset) * Settings.GetDCI_Domain / Settings.GetDCI_select2 / 2047 / 0.001;
                else if (ISelect == 3)
                    vlt = (iamp - Settings.SetDCV_Offset) * Settings.GetDCI_Domain / Settings.GetDCI_Select3 / 2047 / 0.001;
                else if (ISelect == 4)
                    vlt = (iamp - Settings.SetDCV_Offset) * Settings.GetDCI_Domain / Settings.GetDCI_Select4 / 2047 / 0.000001;
                else if (ISelect == 5)
                    vlt = (iamp - Settings.SetDCV_Offset) * Settings.GetDCI_Domain / Settings.GetDCI_Select5 / 2047 / 0.000001;
                else if (ISelect == 6)
                    vlt = (iamp - Settings.SetDCV_Offset) * Settings.GetDCI_Domain / Settings.GetDCI_Select6 / 2047 / 0.000001;
                else
                    vlt = (iamp - Settings.SetDCV_Offset) * Settings.GetDCI_Domain / Settings.GetDCI_Select7 / 2047 / 0.000000001;

                vlt = -vlt;
            }
            return vlt;
        }

        /*private double GetDCVConvertWithNewOffset(double ivlt, int Vmlp, double Offset)
        {
            double vlt;
            vlt = (ivlt - Offset) * Settings.GetDCV_Domain / 2047;

            if (Vmlp < 7)
                vlt = vlt / Math.Pow(2, Vmlp);
            else
                vlt = vlt * 2;

            return vlt;
        }*/

        private double GetDCVConvertWithNewOffset(double ivlt, int Vmlp, double Offset, int SetSelect)
        {
            //return GetDCVConvert(ivlt, Vmlp, SetSelect);
            double vlt;
            double domain = Settings.GetDCV_Domain * Math.Pow(5.55, 1 - SetSelect);
            vlt = (ivlt - Offset) * domain / 2047;

            if (Vmlp < 7)
                vlt = vlt / Math.Pow(2, Vmlp);
            else
                vlt = vlt * 2;

            return vlt;
        }

        private double GetDCVConvert(double ivlt, int Vmlp, int SetSelect)
        {
            double domain = Settings.GetDCV_Domain * Math.Pow(5.55, 1 - SetSelect);
            double vlt = (ivlt - Settings.GetDCV_OffsetMLP0) * domain / 2047;

            if (Vmlp == 1)
                vlt = (ivlt - Settings.GetDCV_OffsetMLP1) * domain / 2047;
            else if (Vmlp == 2)
                vlt = (ivlt - Settings.GetDCV_OffsetMLP2) * domain / 2047;
            else if (Vmlp == 3)
                vlt = (ivlt - Settings.GetDCV_OffsetMLP3) * domain / 2047;
            else if (Vmlp == 4)
                vlt = (ivlt - Settings.GetDCV_OffsetMLP4) * domain / 2047;
            else if (Vmlp == 5)
                vlt = (ivlt - Settings.GetDCV_offsetMLP5) * domain / 2047;
            else if (Vmlp == 6)
                vlt = (ivlt - Settings.GetDCV_OffsetMLP6) * domain / 2047;

            if (Vmlp < 7)
                vlt = vlt / Math.Pow(2, Vmlp);
            else
                vlt = vlt * 2;

            return vlt;
        }

        private double GetDCIConvert(double iamp, int ISelect, int Imlp)
        {
            double amp;
            if (ISelect == 0)
            {
                amp = (iamp - Settings.GetDCI_Offset0d) * Settings.GetDCI_Domain / 2047;
                amp = amp / Settings.GetDCI_Select0;
            }
            else if (ISelect == 1)
            {
                amp = (iamp - Settings.GetDCI_Offset1d) * Settings.GetDCI_Domain / 2047;
                amp = amp / Settings.GetDCI_Select1;
            }
            else if (ISelect == 2)
            {
                amp = (iamp - Settings.GetDCI_Offset2) * Settings.GetDCI_Domain / 2047;
                amp = amp / Settings.GetDCI_select2;
            }
            else if (ISelect == 3)
            {
                amp = (iamp - Settings.GetDCI_Offset3d) * Settings.GetDCI_Domain / 2047;
                amp = amp / Settings.GetDCI_Select3;
            }
            else if (ISelect == 4)
            {
                amp = (iamp - Settings.GetDCI_Offset4d) * Settings.GetDCI_Domain / 2047;
                amp = amp / Settings.GetDCI_Select4;
            }
            else if (ISelect == 5)
            {
                amp = (iamp - Settings.GetDCI_Offset5d) * Settings.GetDCI_Domain / 2047;
                amp = amp / Settings.GetDCI_Select5;
            }
            else if (ISelect == 6)
            {
                amp = (iamp - Settings.GetDCI_Offset6d) * Settings.GetDCI_Domain / 2047;
                amp = amp / Settings.GetDCI_Select6;
            }
            else
            {
                amp = (iamp - Settings.GetDCI_Offset7d) * Settings.GetDCI_Domain / 2047;
                amp = amp / Settings.GetDCI_Select7;
            }

            if (Imlp < 7)
                amp = amp / Math.Pow(2, Imlp);
            else
                amp = amp * 2;

            return -amp;
        }

        private double GetDCIConvertWithNewOffset(double iamp, int ISelect, int Imlp, double Offset)
        {
            double amp;
            amp = (iamp - Offset) * Settings.GetDCI_Domain / 2047;
            if (ISelect == 0)
                amp = amp / Settings.GetDCI_Select0;
            else if (ISelect == 1)
                amp = amp / Settings.GetDCI_Select1;
            else if (ISelect == 2)
                amp = amp / Settings.GetDCI_select2;
            else if (ISelect == 3)
                amp = amp / Settings.GetDCI_Select3;
            else if (ISelect == 4)
                amp = amp / Settings.GetDCI_Select4;
            else if (ISelect == 5)
                amp = amp / Settings.GetDCI_Select5;
            else if (ISelect == 6)
                amp = amp / Settings.GetDCI_Select6;
            else
                amp = amp / Settings.GetDCI_Select7;

            if (Imlp < 7)
                amp = amp / Math.Pow(2, Imlp);
            else
                amp = amp * 2;

            return -amp;
        }

        private int GetGalvanostatZerosetOffset(int CurrentRangeMode)
        {
            int zeroset = 2047;
            if (CurrentRangeMode == 0)
                zeroset = (int)Settings.GalvanostatI_Select0;
            else if (CurrentRangeMode == 1)
                zeroset = (int)Settings.GalvanostatI_Select1;
            else if (CurrentRangeMode == 2)
                zeroset = (int)Settings.GalvanostatI_Select1;
            else if (CurrentRangeMode == 3)
                zeroset = (int)Settings.GalvanostatI_Select1;
            else if (CurrentRangeMode == 4)
                zeroset = (int)Settings.GalvanostatI_Select1;
            else if (CurrentRangeMode == 5)
                zeroset = (int)Settings.GalvanostatI_Select1;
            else if (CurrentRangeMode == 6)
                zeroset = (int)Settings.GalvanostatI_Select1;
            else if (CurrentRangeMode == 7)
                zeroset = (int)Settings.GalvanostatI_Select1;

            return zeroset;
        }

        private void SetPreTreatmentVoltage(int PGMode, int CurrentRangeMode, int Imlp, int VoltageRangeMode, int Vmlp, bool RelRef, double PretreatmentVoltage, int EqTime)
        {
            idcselect(CurrentRangeMode);

            if (CurrentRangeMode <= 1 && PGMode == 1)
            {
                PGmode(0);
            }

            setselect(VoltageRangeMode);
            acset(0);

            int Zeroset;
            if (VoltageRangeMode == 0)
                Zeroset = Settings.Zeroset0;
            else
                Zeroset = Settings.Zeroset1;

            zeroset(Zeroset);
            vdcmlp(Vmlp);
            idcmlp(Imlp);
            //double ThisIV_Voffset = 0;
            //double ThisIV_Ioffset = 0;
            //CalculateSpecificIVOffsets(CurrentRangeMode, Imlp, Vmlp, ref ThisIV_Ioffset, ref ThisIV_Voffset);
            //GetVOffsetFromSettings(Vmlp, ref ThisIV_Voffset);

            double setvolt0 = PretreatmentVoltage;
            if (RelRef) setvolt0 = -setvolt0;
            int ivlt = SetDCVConvert(setvolt0, VoltageRangeMode, CurrentRangeMode, PGMode);

            double[] output = ivset(ivlt);

            SetStatus("Pretreatment voltage is set. Please wait " + EqTime.ToString() + "(s).");
            Thread.Sleep(EqTime);
        }


        ///// <summary>
        ///// This method start the command GIV in asynchronous mode.<para />
        ///// The reply will be accessible from the ACVEventArgs of the callback PG_EVT_ACVDataReceived.<para />
        ///// It returns true when the command is sent to device successfully.
        ///// </summary>
        //private void AIV_V2020_Process()
        //{
        //    //input 02 {0:4095}   normally V or I
        //    //tinput 05 {0:4095}   t
        //    //Ninput 01 {0:4095}   number
        //    //start 0 {0:65000}   start a method
        //    //0 cv
        //    //1 pulse
        //    //2 chrono
        //    //3
        //    //start <method index>  <sampling time>
        //    //
        //    //cv
        //    //input 00 V_init which is V0 also in IV
        //    //Ninput 04 nDataCycle1
        //    //input 01 deltaV    : input 1 1000   input 1 -1000
        //    //tinput 00 nWait at V_init (the number of pulse)
        //    //Ninput 00 nCycle
        //    //tinput 01 nVSet
        //    //Ninput 05 nDataCycles
        //    //start 0 100
        //    //  ndata =  Ninput04*(tinput00 + 1) + 1    Ninput00*Ninput05
        //    //  time_counter
        //    //
        //    //pulse
        //    //input 00 V_init
        //    //input 01 V0
        //    //input 02 V1
        //    //tinput 00 nWait at V_init (the number of pulse)
        //    //Ninput 00 nCycle
        //    //input 03 DeltaV1
        //    //input 04 DeltaV0
        //    //tinput 01 T0
        //    //tinput 02 T1
        //    //start 1 100
        //
        //    //chrono
        //    //Ninput 00 nCycle (nParts)
        //    //input 00 V0
        //    //tinput 00 T0
        //    //input 01 V1
        //    //tinput 01 T1
        //    //input 02 V2
        //    //tinput 02 T2
        //    // ...
        //    //start 2 100
        //    //
        //    //Only for current
        //    //Ninput 1 selectmin
        //    //Ninput 2 selectmax
        //    //Ninput 3 nc (thereshold to change) nc = 4 is good
        //    //autoselect 1
        //    //
        //    //how to read?
        //    //four first bits is current selector and second four bits value
        //}

        public int teststart(int method, int time, int ndata)
        {
            start(method, time);


            string lastValue1 = "";
            while (Port.BytesToRead >= ndata) ;

            Thread.Sleep(3000);

            lastValue1 = Port.ReadExisting();

            int ns = lastValue1.Length;
            return ns;
        }

        private void AIV_ProcessOK()
        {
            bool pingstatusHistory = IsPingEnabled();
            DisablePing();

            //Ninput00(1);
            //Ninput04(20); //nDataCycle1
            //start(2, 2000);

            //setselect(1);
            Ninput01(2);
            Ninput02(2);
            Ninput03(4);
            //Ninput06(0);
            input00(1000); //V_init
            Ninput04(100); //nDataCycle1
            input01(20); //deltaV
            tinput00(0); //nWait at V_init (the number of pulse)
            Ninput00(1); //nCycle
            tinput01(0); //nVSet
            Ninput05(0); //nDataCycles


            start(0, 20000);

            for (int cnt = 0; cnt <= 100; cnt++)
            {
                //ReadNewData(ref VSelect, ref Voltage, ref ISelect, ref Current);
                byte[] Bytes = new byte[4];
                Bytes[0] = (byte)Port.ReadByte();
                Bytes[1] = (byte)Port.ReadByte();
                Bytes[2] = (byte)Port.ReadByte();
                Bytes[3] = (byte)Port.ReadByte();
                int nData = IVnData;
            }

            Thread.Sleep(1000);
            string lastValue1 = "";
            lastValue1 = Port.ReadExisting();

            int ns = lastValue1.Length;

            AIVEventArgs e2 = new AIVEventArgs(ns, ns, ns, ns, ns, ns, ns);
            PG_EVT_AIVDataReceived?.Invoke(this, e2);

            if (DataCountInQueue == 0)
            {
                AProcessFinishedEventArgs e3 = new AProcessFinishedEventArgs("AIV");
                PG_EVT_AProcessFinished?.Invoke(this, e3);
            }

            if (pingstatusHistory) EnablePing();
        }

        private void AIV_Process()
        {
            bool pingstatusHistory = IsPingEnabled();
            DisablePing();

            int PG_mode = LastPGmode;
            //if (IV_Input.Equilibration_Time > 0) SetPreTreatmentVoltage(PG_mode, IV_Input.Current_Range_Mode, IV_Input.Current_Multiplier_Mode, IV_Input.Voltage_Range_Mode, IV_Input.Voltage_Multiplier_Mode, IV_Input.Is_Relative_Reference, IV_Input.Pretreatment_Voltage, IV_Input.Equilibration_Time);

            resetdevice();
            string DataAndTime = "Date: " + string.Format("{0:dd/MM/yyyy}", DateTime.Today) + "  Time: " + string.Format("{0:HH:mm:ss tt}", DateTime.Now);
            OpenCircuitVoltage = 0;

            AProcessStartedEventArgs e1 = new AProcessStartedEventArgs("AIV");
            PG_EVT_AProcessStarted?.Invoke(this, e1);
            SetStatus("IV process is started ...");
            int cntMax = IV_Input.Step;
            int MyVFilter = 2; int MyIFilter = 2;
            //PGmode(CV_Input.PGmode);
            //if (CV_Input.PGmode == 3){ MyVFilter = 2; MyIFilter = 0; }
            vfilter(MyVFilter);
            idcfilter(MyIFilter);
            idcselect(IV_Input.Current_Range_Mode);
            //if (CV_Input.Current_Range_Mode <= 1 && CV_Input.PGmode == 1) {PGmode(0); CV_Input.PGmode = 0;}
            setselect(IV_Input.Voltage_Range_Mode);
            SetIVFilter(0, (double)IV_Input.Interval_Time * cntMax / 100000, IV_Input.Current_Range_Mode, IV_Input.Voltage_Range_Mode);
            if (IV_Input.Voltage_Filter < 3) vfilter(IV_Input.Voltage_Filter);//For IV and chrono and pulse
            acset(0);
            int Myzeroset;
            if (IV_Input.Voltage_Range_Mode == 0)
                Myzeroset = Settings.Zeroset0;
            else
                Myzeroset = Settings.Zeroset1;

            //if (PG_mode == 3)
            //Myzeroset = GetGalvanostatZerosetOffset(CV_Input.Current_Range_Mode);

            zeroset(Myzeroset);
            //vdcmlp(IV_Input.Voltage_Multiplier_Mode);
            //idcmlp(IV_Input.Current_Multiplier_Mode);
            double ThisIV_Voffset = 0;
            double ThisIV_Ioffset = 0;
            //CalculateSpecificIVOffsets(IV_Input.Current_Range_Mode, IV_Input.Current_Multiplier_Mode, IV_Input.Voltage_Multiplier_Mode, ref ThisIV_Ioffset, ref ThisIV_Voffset);
            //GetVOffsetFromSettings(IV_Input.Voltage_Multiplier_Mode, ref ThisIV_Voffset);
            double setvolt0 = IV_Input.Initial_Potential;
            if (IV_Input.Is_Relative_Reference) setvolt0 = -setvolt0;
            int ivlt = SetDCVConvert(setvolt0, IV_Input.Voltage_Range_Mode, IV_Input.Current_Range_Mode, PG_mode);
            //double[] Output = ivset(ivlt);
            //double Vmean = Output[0];
            //double Imean = Output[1];

            double ivvltfrom = IV_Input.Initial_Potential;
            double ivvltto = IV_Input.Final_Potential;
            double IVChronoVset = ivvltfrom;
            double IVChronoDVset = (ivvltto - ivvltfrom) / (cntMax - 1);
            if (IV_Input.Is_Relative_Reference)
            {
                ivvltfrom = -ivvltfrom;
                ivvltto = -ivvltto;
            }
            int ivltFrom = SetDCVConvert(ivvltfrom, IV_Input.Voltage_Range_Mode, IV_Input.Current_Range_Mode, PG_mode);
            int ivltTo = SetDCVConvert(ivvltto, IV_Input.Voltage_Range_Mode, IV_Input.Current_Range_Mode, PG_mode);
            int iTimeStep = (int)(IV_Input.Interval_Time * 31.250) - 1;
            double IVChronoTimeStep = IV_Input.Interval_Time;

            int deltaint = (int)((ivltTo - ivltFrom) / (cntMax - 1));

            //    //cv
            //    //input 00 V_init which is V0 also in IV
            //    //Ninput 04 nDataCycle1
            //    //input 01 deltaV    : input 1 1000   input 1 -1000
            //    //tinput 00 nWait at V_init (the number of pulse)
            //    //Ninput 00 nCycle
            //    //tinput 01 nVSet
            //    //Ninput 05 nDataCycles
            //    //start 0 100
            //microtimesteps = Interval_Time * 1000000 / 500 - 1;

            double minimum_dt = 500 * 0.001;//ms //50 micro secounds is the minimum of micro that we checked
            int nPerSet = (int)Math.Floor((double)IV_Input.Interval_Time / minimum_dt) + 1;//The number of interval in DeltaT
            double dt = (double)IV_Input.Interval_Time / (nPerSet - 1);
            int microtime_dt = (int)(dt * 2000) - 1;
            int nwait = (int)((double)IV_Input.Equilibration_Time / dt) + 1;
            if (nwait < 0) nwait = 0;

            if (PG_mode == 3)
            {
                Ninput01(IV_Input.Current_Range_Mode_Min);
                Ninput02(IV_Input.Current_Range_Mode_Max);
            }
            else
            {
                Ninput01(IV_Input.Voltage_Range_Mode_Min);
                Ninput02(IV_Input.Voltage_Range_Mode_Max);
            }

            Ninput03(IV_Input.Auto_Range_NCheck);
            Ninput06(IV_Input.Auto_Range_Time);
            input00(ivltFrom); //V_init
            Ninput04(cntMax); //nDataCycle1
            input01(deltaint); //deltaV
            tinput00(nwait - 1); //nWait at V_init (the number of pulse)
            Ninput00(1); //nCycle
            tinput01(nPerSet - 1); //nSet
            Ninput05(0); //nDataCycles

            Thread.Sleep(500);
            ClearBuffer();
            
            start(0, microtime_dt); //50 micro secounds
                                    //start(0, 99); //50 micro secounds
                                    
            int VSelect = 0;
            int Voltage = 0;
            int ISelect = 0;
            int Current = 0;
            
            int TheNumberOfData = 1 + (nwait - 1) + cntMax * nPerSet;
            int expectedBytes = TheNumberOfData*4;
            DataCountInQueue = TheNumberOfData;
            double MaxTime = dt * (TheNumberOfData - 1) / 1000.0;
            int IVVsetcnt = 0;
            int time_counter = -(nwait - 1);
            int cnt = 0;
            int Qind = 0;
            double[] volt_Que = new double[IV_Input.Digital_Filter];
            double[] current_Que = new double[IV_Input.Digital_Filter];
            while (IVVsetcnt < TheNumberOfData)
            {
                if (Port.BytesToRead < 4) continue;
                //ReadNewData(ref VSelect, ref Voltage, ref ISelect, ref Current);
                byte[] Bytes = new byte[4];
                Bytes[0] = (byte)Port.ReadByte();
                Bytes[1] = (byte)Port.ReadByte();
                Bytes[2] = (byte)Port.ReadByte();
                Bytes[3] = (byte)Port.ReadByte();
                int nData = IVnData;

                VSelect = (Bytes[1] & 0xf0) >> 4;
                Voltage = Bytes[0] | ((Bytes[1] & 0x0f) << 8);
                Voltage = (Voltage >> 11) == 0 ? Voltage : -1 ^ 0xFFF | Voltage;
                ISelect = (Bytes[3] & 0xf0) >> 4;
                Current = Bytes[2] | ((Bytes[3] & 0x0f) << 8);
                Current = (Current >> 11) == 0 ? Current : -1 ^ 0xFFF | Current;
                
                double volt = GetDCVConvertWithNewOffset(Voltage, 0, ThisIV_Voffset, VSelect);
                double current = GetDCIConvertWithNewOffset(Current, ISelect, 0, ThisIV_Ioffset);
                if (IV_Input.Is_Relative_Reference)
                {
                    volt = -volt;
                    current = -current;
                }

                if (IVVsetcnt == 0)
                {
                    for (int i = 0; i < IV_Input.Digital_Filter; i++)
                    {
                        volt_Que[i] = volt;
                        current_Que[i] = current;
                    }
                }
                else
                {
                    if (Qind == IV_Input.Digital_Filter) Qind = 0;
                    volt_Que[Qind] = volt;
                    current_Que[Qind] = current;
                }
                
                if (time_counter >= nPerSet) time_counter = 0;
                if (time_counter == 0)
                {
                    cnt++;
                    double Mytime = IVChronoTimeStep * IVVsetcnt / 1000.0;
                    double MyVoltage = IVChronoVset + IVVsetcnt * IVChronoDVset;

                    double meanvolt = 0;
                    double meancurrent = 0;
                    for (int i = 0; i < IV_Input.Digital_Filter; i++)
                    {
                        meanvolt += volt_Que[i];
                        meancurrent += current_Que[i];
                    }

                    meanvolt = meanvolt / IV_Input.Digital_Filter;
                    meancurrent = meancurrent / IV_Input.Digital_Filter;
                    AIVEventArgs e2 = new AIVEventArgs(cntMax, cnt, MaxTime, Mytime, MyVoltage, meancurrent, meanvolt);
                    PG_EVT_AIVDataReceived?.Invoke(this, e2);
                }
                DataCountInQueue--;
                IVVsetcnt++;
                time_counter++;
                Qind++;
            }

            //Thread.Sleep(3000);
            //string lastValue1 = Port.ReadExisting();
            
            if (DataCountInQueue == 0)
            {
                AProcessFinishedEventArgs e3 = new AProcessFinishedEventArgs("AIV");
                PG_EVT_AProcessFinished?.Invoke(this, e3);
            }

            if (pingstatusHistory) EnablePing();
        }

        private void ReadNewData(ref int VSelect, ref int Voltage, ref int ISelect, ref int Current)
        {
            byte[] Bytes = new byte[4];
            Bytes[0] = (byte)Port.ReadByte();
            Bytes[1] = (byte)Port.ReadByte();
            Bytes[2] = (byte)Port.ReadByte();
            Bytes[3] = (byte)Port.ReadByte();
            int nData = IVnData;
            
            VSelect = (Bytes[1] & 240) >> 4;
            Voltage = Bytes[0] | ((Bytes[1] & 15) << 8);
            ISelect = (Bytes[3] & 240) >> 4;
            Current = Bytes[2] | ((Bytes[3] & 15) << 8);
        }

        private void AIV_Process_old()
        {
            bool pingstatusHistory = IsPingEnabled();
            DisablePing();

            int PG_mode = LastPGmode;
            if (IV_Input.Equilibration_Time > 0) SetPreTreatmentVoltage(PG_mode, IV_Input.Current_Range_Mode, 0, IV_Input.Voltage_Range_Mode, 0, IV_Input.Is_Relative_Reference, IV_Input.Initial_Potential, IV_Input.Equilibration_Time);

            resetdevice();
            string DataAndTime = "Date: " + string.Format("{0:dd/MM/yyyy}", DateTime.Today) + "  Time: " + string.Format("{0:HH:mm:ss tt}", DateTime.Now);
            OpenCircuitVoltage = 0;

            AProcessStartedEventArgs e1 = new AProcessStartedEventArgs("AIV");
            PG_EVT_AProcessStarted?.Invoke(this, e1);
            SetStatus("IV process is started ...");
            int cntMax = IV_Input.Step;
            int MyVFilter = 2; int MyIFilter = 2;
            //PGmode(CV_Input.PGmode);
            //if (CV_Input.PGmode == 3){ MyVFilter = 2; MyIFilter = 0; }
            vfilter(MyVFilter);
            idcfilter(MyIFilter);
            idcselect(IV_Input.Current_Range_Mode);
            //if (CV_Input.Current_Range_Mode <= 1 && CV_Input.PGmode == 1) {PGmode(0); CV_Input.PGmode = 0;}
            setselect(IV_Input.Voltage_Range_Mode);
            SetIVFilter(0, (double)IV_Input.Interval_Time * IV_Input.Step / 100000, IV_Input.Current_Range_Mode, IV_Input.Voltage_Range_Mode);
            if (IV_Input.Voltage_Filter < 3) vfilter(IV_Input.Voltage_Filter);//For IV and chrono and pulse
            acset(0);
            int Myzeroset;
            if (IV_Input.Voltage_Range_Mode == 0)
                Myzeroset = Settings.Zeroset0;
            else
                Myzeroset = Settings.Zeroset1;

            //if (PG_mode == 3)
            //Myzeroset = GetGalvanostatZerosetOffset(CV_Input.Current_Range_Mode);

            zeroset(Myzeroset);
            //vdcmlp(IV_Input.Voltage_Multiplier_Mode);
            //idcmlp(IV_Input.Current_Multiplier_Mode);
            double ThisIV_Voffset = 0;
            double ThisIV_Ioffset = 0; 

            CalculateSpecificIVOffsets(IV_Input.Current_Range_Mode, 0, 0, ref ThisIV_Ioffset, ref ThisIV_Voffset);
            //GetVOffsetFromSettings(IV_Input.Voltage_Multiplier_Mode, ref ThisIV_Voffset);
            double setvolt0 = IV_Input.Initial_Potential;
            if (IV_Input.Is_Relative_Reference) setvolt0 = -setvolt0;
            int ivlt = SetDCVConvert(setvolt0, IV_Input.Voltage_Range_Mode, IV_Input.Current_Range_Mode, PG_mode);
            double[] Output = ivset(ivlt);
            double Vmean = Output[0];
            double Imean = Output[1];

            /*
            if (CV_Input.isOCP && CV_Input.isOCPAutoStart)
            {
                BtnFindOCP_Click(null, null);
                BtnUseSuggestedVOCP_Click(null, null);
            }
            else
            {
                if (CV_Input.Equilibration_Time > 0)
                {
                    Thread.Sleep(CV_Input.Equilibration_Time * 1000);
                }
            }
            */

            double ivvltfrom = IV_Input.Initial_Potential;
            double ivvltto = IV_Input.Final_Potential;
            double IVChronoVset = ivvltfrom;
            double IVChronoDVset = (ivvltto - ivvltfrom) / (IV_Input.Step - 1);
            if (IV_Input.Is_Relative_Reference)
            {
                ivvltfrom = -ivvltfrom;
                ivvltto = -ivvltto;
            }
            int ivltFrom = SetDCVConvert(ivvltfrom, IV_Input.Voltage_Range_Mode, IV_Input.Current_Range_Mode, PG_mode);
            int ivltTo = SetDCVConvert(ivvltto, IV_Input.Voltage_Range_Mode, IV_Input.Current_Range_Mode, PG_mode);
            int iTimeStep = (int)(IV_Input.Interval_Time * 31.250) - 1;
            double IVChronoTimeStep = IV_Input.Interval_Time;

            int deltaint = (int)((ivltTo - ivltFrom) / (IV_Input.Step - 1));
            string order = "iv";
            int startorder = ivltFrom - deltaint;
            if (startorder < 0) startorder = 0;
            if (ivltTo > 4095) ivltTo = 4095;
            order = order + string.Format("{0: 0000}", startorder);
            order = order + string.Format("{0: 0000}", ivltTo);
            order = order + string.Format("{0: 0000}", IV_Input.Step + 1);
            order = order + string.Format("{0: 000000}", iTimeStep);
            if (!ACommand(Port, order)) return; //iv 0000 4094 0100 003125
            for (int d = 0; d < 8; d++) Port.ReadByte();

            DataCountInQueue = cntMax;
            double MaxTime = IVChronoTimeStep * (cntMax - 1) / 1000.0;
            int IVVsetcnt = 0;
            KillProcessRequested = false;
            for (int cnt = 1; cnt <= cntMax; cnt++)
            {
                if (KillProcessRequested)
                {
                    DataCountInQueue = 0;
                    break;
                }
                byte[] AllBytes1 = new byte[4];
                byte[] AllBytes2 = new byte[4];
                try
                {
                    AllBytes1[0] = (byte)Port.ReadByte();
                    AllBytes1[1] = (byte)Port.ReadByte();
                    AllBytes1[2] = (byte)Port.ReadByte();
                    AllBytes1[3] = (byte)Port.ReadByte();
                    AllBytes2[0] = (byte)Port.ReadByte();
                    AllBytes2[1] = (byte)Port.ReadByte();
                    AllBytes2[2] = (byte)Port.ReadByte();
                    AllBytes2[3] = (byte)Port.ReadByte();
                }
                catch { }
                int nData = IVnData;
                int word;
                word = AllBytes1[0] | (AllBytes1[1] << 8) | (AllBytes1[2] << 16) | (AllBytes1[3] << 24);
                Vmean = (double)word / (double)nData;
                word = AllBytes2[0] | (AllBytes2[1] << 8) | (AllBytes2[2] << 16) | (AllBytes2[3] << 24);
                Imean = (double)word / (double)nData;
                //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " i=" + cnt.ToString());
                double volt = GetDCVConvertWithNewOffset(Vmean, 0, ThisIV_Voffset, IV_Input.Voltage_Range_Mode);
                double current = GetDCIConvertWithNewOffset(Imean, IV_Input.Current_Range_Mode, 0, ThisIV_Ioffset);
                if (IV_Input.Is_Relative_Reference)
                {
                    volt = -volt;
                    current = -current;
                }
                //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " iVmean=" + Vmean.ToString() + " V=" + volt.ToString());
                //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " iImean=" + Imean.ToString() + " I=" + current.ToString());
                double Mytime = IVChronoTimeStep * (cnt - 1) / 1000.0;
                double MyVoltage = IVChronoVset + IVVsetcnt * IVChronoDVset;
                DataCountInQueue--;
                AIVEventArgs e2 = new AIVEventArgs(cntMax, cnt, MaxTime, Mytime, MyVoltage, current, volt);
                PG_EVT_AIVDataReceived?.Invoke(this, e2);
                IVVsetcnt++;
            }

            if (true)
            {
                sample(0);
            }

            if (DataCountInQueue == 0)
            {
                AProcessFinishedEventArgs e3 = new AProcessFinishedEventArgs("AIV");
                PG_EVT_AProcessFinished?.Invoke(this, e3);
            }

            if (pingstatusHistory) EnablePing();
        }

        private void ACV_Process()
        {
            bool pingstatusHistory = IsPingEnabled();
            DisablePing();

            int PG_mode = LastPGmode;
            if (CV_Input.Equilibration_Time > 0) SetPreTreatmentVoltage(PG_mode, CV_Input.Current_Range_Mode, CV_Input.Current_Multiplier_Mode, CV_Input.Voltage_Range_Mode, CV_Input.Voltage_Multiplier_Mode, CV_Input.Is_Relative_Reference, CV_Input.Pretreatment_Voltage, CV_Input.Equilibration_Time);

            resetdevice();
            string DataAndTime = "Date: " + string.Format("{0:dd/MM/yyyy}", DateTime.Today) + "  Time: " + string.Format("{0:HH:mm:ss tt}", DateTime.Now);
            OpenCircuitVoltage = 0;

            AProcessStartedEventArgs e1 = new AProcessStartedEventArgs("ACV");
            PG_EVT_AProcessStarted?.Invoke(this, e1);
            SetStatus("CV process is started ...");
            
            int CVnFirst = (int)((CV_Input.Final_Potential - CV_Input.Initial_Potential) / (CV_Input.Final_Potential - CV_Input.Switching_Potential) * (CV_Input.Step - 1));
            int CVItt = CV_Input.Number_of_Scans * 2 - 1;
            int cntMax = CV_Input.Step * CVItt + CVnFirst;
            int MyVFilter = 2; int MyIFilter = 2;
            //PGmode(CV_Input.PGmode);
            //if (CV_Input.PGmode == 3){ MyVFilter = 2; MyIFilter = 0; }
            vfilter(MyVFilter);
            idcfilter(MyIFilter);
            idcselect(CV_Input.Current_Range_Mode);
            //if (CV_Input.Current_Range_Mode <= 1 && CV_Input.PGmode == 1) {PGmode(0); CV_Input.PGmode = 0;}
            setselect(CV_Input.Voltage_Range_Mode);
            SetIVFilter(0, (double)CV_Input.Interval_Time * CV_Input.Step / 100000, CV_Input.Current_Range_Mode, CV_Input.Voltage_Range_Mode);
            if (CV_Input.Voltage_Filter < 3) vfilter(CV_Input.Voltage_Filter);//For IV and chrono and pulse
            acset(0);
            int Myzeroset;
            if (CV_Input.Voltage_Range_Mode == 0)
                Myzeroset = Settings.Zeroset0;
            else
                Myzeroset = Settings.Zeroset1;
            //if (CV_Input.PGmode == 3) GetGalvanostatZerosetOffset(CV_Input.Current_Range_Mode);
            zeroset(Myzeroset);
            vdcmlp(CV_Input.Voltage_Multiplier_Mode);
            idcmlp(CV_Input.Current_Multiplier_Mode);
            double ThisIV_Voffset = 0;
            double ThisIV_Ioffset = 0;
            CalculateSpecificIVOffsets(CV_Input.Current_Range_Mode, CV_Input.Current_Multiplier_Mode, CV_Input.Voltage_Multiplier_Mode, ref ThisIV_Ioffset, ref ThisIV_Voffset);
            GetVOffsetFromSettings(CV_Input.Voltage_Multiplier_Mode, ref ThisIV_Voffset);
            double setvolt0 = CV_Input.Initial_Potential;
            if (CV_Input.Is_Relative_Reference) setvolt0 = -setvolt0;
            int ivlt = SetDCVConvert(setvolt0, CV_Input.Voltage_Range_Mode, CV_Input.Current_Range_Mode, PG_mode);
            double[] Output = ivset(ivlt);
            double Vmean = Output[0];
            double Imean = Output[1];

            /*
            if (CV_Input.isOCP && CV_Input.isOCPAutoStart)
            {
                BtnFindOCP_Click(null, null);
                BtnUseSuggestedVOCP_Click(null, null);
            }
            else
            {
                if (CV_Input.Equilibration_Time > 0)
                {
                    Thread.Sleep(CV_Input.Equilibration_Time * 1000);
                }
            }
            */
            
            double ivvltfrom = CV_Input.Switching_Potential;
            double cvvltstart = CV_Input.Initial_Potential;
            double ivvltto = CV_Input.Final_Potential;
            double IVChronoVset = cvvltstart;
            double IVChronoDVset = (ivvltto - cvvltstart) / (CVnFirst - 1); //It will be set some lines later
            if (CV_Input.Is_Relative_Reference)
            {
                cvvltstart = -cvvltstart;
                ivvltfrom = -ivvltfrom;
                ivvltto = -ivvltto;
            }
            int ivltStart = SetDCVConvert(cvvltstart, CV_Input.Voltage_Range_Mode, CV_Input.Current_Range_Mode, PG_mode);
            int ivltFrom = SetDCVConvert(ivvltfrom, CV_Input.Voltage_Range_Mode, CV_Input.Current_Range_Mode, PG_mode);
            int ivltTo = SetDCVConvert(ivvltto, CV_Input.Voltage_Range_Mode, CV_Input.Current_Range_Mode, PG_mode);
            int iTimeStep = (int)(CV_Input.Interval_Time * 31.250) - 1;
            double IVChronoTimeStep = CV_Input.Interval_Time;
            double deltadouble = (ivvltto - ivvltfrom) / (CV_Input.Step - 1);
            int CurrentVer = DeviceVer();
            double deltadouble2 = deltadouble;
            if (CurrentVer > 1) deltadouble2 = 100.0 * deltadouble;
            double sign = Math.Sign(deltadouble2);
            int deltaint = SetDCVConvert_dV(Math.Abs(deltadouble2), CV_Input.Voltage_Range_Mode, CV_Input.Current_Range_Mode, PG_mode);
            deltaint -= 2047;
            deltaint = -(int)(sign * Math.Abs(deltaint));
            input00(CV_Input.Number_of_Scans);
            input01(deltaint);
            input02(CVnFirst);
            input03(ivltStart);
            input04(CV_Input.Step);
            tinput00(iTimeStep);
            
            DataCountInQueue = cntMax;
            if (!ACommand(Port, "cv")) return;

            double MaxTime = IVChronoTimeStep * (cntMax - 1) / 1000.0;
            int IVVsetcnt = 0;
            KillProcessRequested = false;
            for (int cnt = 1; cnt <= cntMax; cnt++)
            {
                if (KillProcessRequested)
                {
                    DataCountInQueue = 0;
                    break;
                }
                byte[] AllBytes1 = new byte[4];
                byte[] AllBytes2 = new byte[4];
                try
                {
                    AllBytes1[0] = (byte)Port.ReadByte();
                    AllBytes1[1] = (byte)Port.ReadByte();
                    AllBytes1[2] = (byte)Port.ReadByte();
                    AllBytes1[3] = (byte)Port.ReadByte();
                    AllBytes2[0] = (byte)Port.ReadByte();
                    AllBytes2[1] = (byte)Port.ReadByte();
                    AllBytes2[2] = (byte)Port.ReadByte();
                    AllBytes2[3] = (byte)Port.ReadByte();
                }
                catch { }
                int nData = IVnData;
                int word;
                word = AllBytes1[0] | (AllBytes1[1] << 8) | (AllBytes1[2] << 16) | (AllBytes1[3] << 24);
                Vmean = (double)word / (double)nData;
                word = AllBytes2[0] | (AllBytes2[1] << 8) | (AllBytes2[2] << 16) | (AllBytes2[3] << 24);
                Imean = (double)word / (double)nData;
                //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " i=" + cnt.ToString());
                double volt = GetDCVConvertWithNewOffset(Vmean, CV_Input.Voltage_Multiplier_Mode, ThisIV_Voffset, CV_Input.Voltage_Range_Mode);
                double current = GetDCIConvertWithNewOffset(Imean, CV_Input.Current_Range_Mode, CV_Input.Current_Multiplier_Mode, ThisIV_Ioffset);
                if (CV_Input.Is_Relative_Reference)
                {
                    volt = -volt;
                    current = -current;
                }
                //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " iVmean=" + Vmean.ToString() + " V=" + volt.ToString());
                //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " iImean=" + Imean.ToString() + " I=" + current.ToString());
                double Mytime = IVChronoTimeStep * (cnt-1) / 1000.0;
                double MyVoltage = IVChronoVset + IVVsetcnt * IVChronoDVset;
                DataCountInQueue--;
                ACVEventArgs e2 = new ACVEventArgs(cntMax, CVnFirst, CV_Input.Step, cnt, MaxTime, Mytime, MyVoltage, current, volt);
                PG_EVT_ACVDataReceived?.Invoke(this, e2);
                IVVsetcnt++;
                if ((cnt - CVnFirst) % CV_Input.Step == 0 && cnt >= CVnFirst)
                {
                    //if (((int)((double)(cnt - CVnFirst) / CV_Input.Step)) % 2 == 0)
                    //{
                    double ddummy = ivvltfrom;
                    ivvltfrom = ivvltto;
                    ivvltto = ddummy;
                    //}
                    ivvltfrom = IVChronoVset + (IVVsetcnt - 1) * IVChronoDVset;
                    IVChronoDVset = (ivvltto - ivvltfrom) / (CV_Input.Step - 1);
                    IVVsetcnt = 0;
                    IVChronoVset = ivvltfrom;
                    double IVChronoDVsetSign = Math.Sign((ivvltto - ivvltfrom) / (CV_Input.Step - 1));
                    IVChronoDVset = IVChronoDVsetSign * Math.Abs(IVChronoDVset);
                    if (CV_Input.Is_Relative_Reference)
                    {
                        ivvltfrom = -ivvltfrom;
                        ivvltto = -ivvltto;
                    }
                }
            }

            if (CV_Input.Post_Processing_Prob_Off)
            {
                sample(0);
            }
            else
            {
                try
                {
                    int iVlt = SetDCVConvert(CV_Input.Ideal_Voltage, CV_Input.Voltage_Range_Mode, CV_Input.Current_Range_Mode, PG_mode);
                    double[] dummyout = ivset(iVlt);
                }
                catch { }
            }

            if (DataCountInQueue == 0)
            {
                AProcessFinishedEventArgs e3 = new AProcessFinishedEventArgs("ACV");
                PG_EVT_AProcessFinished?.Invoke(this, e3);
            }

            if (pingstatusHistory) EnablePing();
        }

        private void AChrono_Process()
        {
            bool pingstatusHistory = IsPingEnabled();
            DisablePing();

            int PG_mode = LastPGmode;
            if (Chrono_Input.Equilibration_Time > 0) SetPreTreatmentVoltage(PG_mode, Chrono_Input.Current_Range_Mode, Chrono_Input.Current_Multiplier_Mode, Chrono_Input.Voltage_Range_Mode, Chrono_Input.Voltage_Multiplier_Mode, Chrono_Input.Is_Relative_Reference, Chrono_Input.Pretreatment_Voltage, Chrono_Input.Equilibration_Time);

            resetdevice();
            string DataAndTime = "Date: " + string.Format("{0:dd/MM/yyyy}", DateTime.Today) + "  Time: " + string.Format("{0:HH:mm:ss tt}", DateTime.Now);
            OpenCircuitVoltage = 0;

            AProcessStartedEventArgs e1 = new AProcessStartedEventArgs("AChrono");
            PG_EVT_AProcessStarted?.Invoke(this, e1);
            SetStatus("Chrono process is started ...");
            int cntMax = Chrono_Input.Step;
            int MyVFilter = 2; int MyIFilter = 2;
            //PGmode(PG_mode);
            //if (PG_mode == 3){ MyVFilter = 4; MyIFilter = 0; }
            vfilter(MyVFilter);
            idcfilter(MyIFilter);
            idcselect(Chrono_Input.Current_Range_Mode);
            //if (CV_Input.Current_Range_Mode <= 1 && CV_Input.PGmode == 1) {PGmode(0); CV_Input.PGmode = 0;}
            setselect(Chrono_Input.Voltage_Range_Mode);
            SetIVFilter(0, (double)Chrono_Input.Interval_Time * Chrono_Input.Step / 100000, Chrono_Input.Current_Range_Mode, Chrono_Input.Voltage_Range_Mode);
            if (Chrono_Input.Voltage_Filter < 3) vfilter(Chrono_Input.Voltage_Filter);//For IV and chrono and pulse
            acset(0);
            int Myzeroset;
            if (Chrono_Input.Voltage_Range_Mode == 0)
                Myzeroset = Settings.Zeroset0;
            else
                Myzeroset = Settings.Zeroset1;
            //if (CV_Input.PGmode == 3) GetGalvanostatZerosetOffset(CV_Input.Current_Range_Mode);
            zeroset(Myzeroset);
            vdcmlp(Chrono_Input.Voltage_Multiplier_Mode);
            idcmlp(Chrono_Input.Current_Multiplier_Mode);
            double ThisIV_Voffset = 0;
            double ThisIV_Ioffset = 0;
            CalculateSpecificIVOffsets(Chrono_Input.Current_Range_Mode, Chrono_Input.Current_Multiplier_Mode, Chrono_Input.Voltage_Multiplier_Mode, ref ThisIV_Ioffset, ref ThisIV_Voffset);
            GetVOffsetFromSettings(Chrono_Input.Voltage_Multiplier_Mode, ref ThisIV_Voffset);
            double setvolt0 = Chrono_Input.V1;
            if (Chrono_Input.Is_Relative_Reference) setvolt0 = -setvolt0;
            int ivlt = SetDCVConvert(setvolt0, Chrono_Input.Voltage_Range_Mode, Chrono_Input.Current_Range_Mode, PG_mode);
            double[] Output = ivset(ivlt);
            double Vmean = Output[0];
            double Imean = Output[1];

            /*
            if (CV_Input.isOCP && CV_Input.isOCPAutoStart)
            {
                BtnFindOCP_Click(null, null);
                BtnUseSuggestedVOCP_Click(null, null);
            }
            else
            {
                if (CV_Input.Equilibration_Time > 0)
                {
                    Thread.Sleep(CV_Input.Equilibration_Time * 1000);
                }
            }
            */

            double ivvltfrom = Chrono_Input.V1;
            //double ivvltto = Chrono_Input.Final_Potential;
            double IVChronoVset = ivvltfrom;
            //double IVChronoDVset = 0;
            if (Chrono_Input.Is_Relative_Reference)
            {
                ivvltfrom = -ivvltfrom;
                //ivvltto = -ivvltto;
            }
            int ivltFrom = SetDCVConvert(ivvltfrom, Chrono_Input.Voltage_Range_Mode, Chrono_Input.Current_Range_Mode, PG_mode);
            //int ivltTo = SetDCVConvert(ivvltto, Chrono_Input.Voltage_Range_Mode, Chrono_Input.Current_Range_Mode, PG_mode);

            SetIVFilter(0, Chrono_Input.T1 / 10.0, Chrono_Input.Current_Range_Mode, Chrono_Input.Voltage_Range_Mode);
            if (Chrono_Input.Voltage_Filter < 3)
            {
                MyVFilter = Chrono_Input.Voltage_Filter; //For IV and chrono and pulse
                if (MyVFilter > 0) MyVFilter += 2;
                vfilter(MyVFilter);
            }

            double EndTime = Chrono_Input.T1 * 1000; //ms
            int nThisStep = (int)(Chrono_Input.T1 / Chrono_Input.Interval_Time * 1000.0) + 1;
            double IVChronoTimeStep = Chrono_Input.Interval_Time;
            int iTimeStep = (int)(Chrono_Input.Interval_Time * 31.250) - 1;
            
            //int deltaint = (int)((ivltTo - ivltFrom) / (Chrono_Input.Step - 1));
            string order = "iv";
            int startorder = ivltFrom;
            if (startorder < 0) startorder = 0;
            //if (ivltTo > 4095) ivltTo = 4095;
            order = order + string.Format("{0: 0000}", startorder);
            order = order + string.Format("{0: 0000}", startorder);
            order = order + string.Format("{0: 0000}", nThisStep + 1);
            order = order + string.Format("{0: 000000}", iTimeStep);
            if (!ACommand(Port, order)) return; //iv 0000 4094 0100 003125
            for (int d = 0; d < 8; d++) Port.ReadByte();

            //    //chrono
            //    //Ninput 00 nCycle (nParts)
            //    //input 00 V0
            //    //tinput 00 T0
            //    //input 01 V1
            //    //tinput 01 T1
            //    //input 02 V2
            //    //tinput 02 T2
            //    // ...
            //    //start 2 100

            DataCountInQueue = nThisStep;
            double MaxTime = IVChronoTimeStep * (cntMax - 1) / 1000.0;
            int IVVsetcnt = 0;
            int cnt = 0;
            KillProcessRequested = false;
            for (int cntsteps = 1; cntsteps <= cntMax; cntsteps++)
            {
                if (KillProcessRequested)
                {
                    DataCountInQueue = 0;
                    break;
                }

                for (int cnt0 = 1; cnt0 <= nThisStep; cnt0++)
                {
                    if (KillProcessRequested)
                    {
                        DataCountInQueue = 0;
                        break;
                    }
                    cnt++;
                    byte[] AllBytes1 = new byte[4];
                    byte[] AllBytes2 = new byte[4];
                    try
                    {
                        AllBytes1[0] = (byte)Port.ReadByte();
                        AllBytes1[1] = (byte)Port.ReadByte();
                        AllBytes1[2] = (byte)Port.ReadByte();
                        AllBytes1[3] = (byte)Port.ReadByte();
                        AllBytes2[0] = (byte)Port.ReadByte();
                        AllBytes2[1] = (byte)Port.ReadByte();
                        AllBytes2[2] = (byte)Port.ReadByte();
                        AllBytes2[3] = (byte)Port.ReadByte();
                    }
                    catch { }
                    int nData = IVnData;
                    int word;
                    word = AllBytes1[0] | (AllBytes1[1] << 8) | (AllBytes1[2] << 16) | (AllBytes1[3] << 24);
                    Vmean = (double)word / (double)nData;
                    word = AllBytes2[0] | (AllBytes2[1] << 8) | (AllBytes2[2] << 16) | (AllBytes2[3] << 24);
                    Imean = (double)word / (double)nData;
                    //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " i=" + cnt.ToString());
                    double volt = GetDCVConvertWithNewOffset(Vmean, Chrono_Input.Voltage_Multiplier_Mode, ThisIV_Voffset, Chrono_Input.Voltage_Range_Mode);
                    double current = GetDCIConvertWithNewOffset(Imean, Chrono_Input.Current_Range_Mode, Chrono_Input.Current_Multiplier_Mode, ThisIV_Ioffset);
                    if (Chrono_Input.Is_Relative_Reference)
                    {
                        volt = -volt;
                        current = -current;
                    }
                    //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " iVmean=" + Vmean.ToString() + " V=" + volt.ToString());
                    //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " iImean=" + Imean.ToString() + " I=" + current.ToString());
                    double Mytime = IVChronoTimeStep * (cnt - 1) / 1000.0;
                    //double MyVoltage = IVChronoVset + IVVsetcnt * IVChronoDVset;
                    DataCountInQueue--;
                    AChronoEventArgs e2 = new AChronoEventArgs(cntMax, cnt, MaxTime, Mytime, current, volt);
                    PG_EVT_AChronoDataReceived?.Invoke(this, e2);
                    IVVsetcnt++;
                }
            }

            if (Chrono_Input.Post_Processing_Prob_Off)
            {
                sample(0);
            }
            else
            {
                try
                {
                    int iVlt = SetDCVConvert(Chrono_Input.Ideal_Voltage, Chrono_Input.Voltage_Range_Mode, Chrono_Input.Current_Range_Mode, PG_mode);
                    double[] dummyout = ivset(iVlt);
                }
                catch { }
            }

            if (DataCountInQueue == 0)
            {
                AProcessFinishedEventArgs e3 = new AProcessFinishedEventArgs("AChrono");
                PG_EVT_AProcessFinished?.Invoke(this, e3);
            }

            if (pingstatusHistory) EnablePing();
        }

        /// <summary>
        /// This method start the command CV in asynchronous mode.<para />
        /// The reply will be accessible from the ACVEventArgs of the callback PG_EVT_ACVDataReceived.<para />
        /// It returns true when the command is sent to device successfully.
        /// </summary>
        public bool ACV()
        {
            if (!IsConnected())
            {
                SetStatus("Unable to send command to device. PGStat has not connected to device yet.");
                return false;
            }

            if (IsBusy())
            {
                SetStatus("Unable to send command to device.", "Port is busy by another async mode process.", "bool ACommand(string command, int Delay = 100)");
                return false;
            }

            try
            {
                Thread t = new Thread(new ThreadStart(ACV_Process));
                t.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// This method start the command IV in asynchronous mode.<para />
        /// The reply will be accessible from the AIVEventArgs of the callback PG_EVT_AIVDataReceived.<para />
        /// It returns true when the command is sent to device successfully.
        /// </summary>
        public bool AIV()
        {
            if (!IsConnected())
            {
                SetStatus("Unable to send command to device. PGStat has not connected to device yet.");
                return false;
            }

            if (IsBusy())
            {
                SetStatus("Unable to send command to device.", "Port is busy by another async mode process.", "bool ACommand(string command, int Delay = 100)");
                return false;
            }

            try
            {
                Thread t = new Thread(new ThreadStart(AIV_Process));
                t.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// This method start the command IV in asynchronous mode.<para />
        /// The reply will be accessible from the AIVEventArgs of the callback PG_EVT_AIVDataReceived.<para />
        /// It returns true when the command is sent to device successfully.
        /// </summary>
        public bool AIV_old()
        {
            if (!IsConnected())
            {
                SetStatus("Unable to send command to device. PGStat has not connected to device yet.");
                return false;
            }

            if (IsBusy())
            {
                SetStatus("Unable to send command to device.", "Port is busy by another async mode process.", "bool ACommand(string command, int Delay = 100)");
                return false;
            }

            try
            {
                Thread t = new Thread(new ThreadStart(AIV_Process_old));
                t.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// This method start the command Chrono in asynchronous mode.<para />
        /// The reply will be accessible from the AChronoEventArgs of the callback PG_EVT_AChronoDataReceived.<para />
        /// It returns true when the command is sent to device successfully.
        /// </summary>
        public bool AChrono()
        {
            if (!IsConnected())
            {
                SetStatus("Unable to send command to device. PGStat has not connected to device yet.");
                return false;
            }

            if (IsBusy())
            {
                SetStatus("Unable to send command to device.", "Port is busy by another async mode process.", "bool ACommand(string command, int Delay = 100)");
                return false;
            }

            try
            {
                Thread t = new Thread(new ThreadStart(AChrono_Process));
                t.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string stringver()
        {
            return Command("ver?");
        }

        /// <summary>
        /// Returns the version of the device firmware.
        /// </summary>
        public int DeviceVer()
        {
            string Ver = stringver();
            string[] Parts;
            char[] delimiterChars = { '.' };
            Parts = Ver.Split(delimiterChars);
            int CurrentVer = Convert.ToInt16(Parts[1]);
            return CurrentVer;
        }

        /// <summary>
        /// Returns the version of the PGStat library.
        /// </summary>
        public string version()
        {
            return PGStatVer;
        }

        /// <summary>
        /// Command to check the process status.
        /// Returns true if an async process in the device is currently running.
        /// </summary>
        public bool IsBusy()
        {
            if (DataCountInQueue == 0)
                return false;
            else
                return true;
        }

        private void Unpluged(object sender, EventArrivedEventArgs e)
        {
            //if (PG_EVT_UNPLUGED != null){PG_EVT_UNPLUGED(this, e);}
            PG_EVT_Unpluged?.Invoke(this, EventArgs.Empty);
            if (!Connected) return;
            try
            {
                Disconnect();
            }
            catch (Exception ex)
            {
                SetStatus("Unable to disconnect the connection.", ex.Message, "void Unpluged(object sender, EventArrivedEventArgs e)");
            }
            if (IsAutoReconnectEnabled) ReConnect();
        }

        private void SetStatus(string Value, int time = 1000)
        {
            if (NotificationsVerbosity > 0)
            {
                Icon MyIcon0 = (Icon)Properties.Resources.ResourceManager.GetObject("pg");
                //Icon MyIcon = new Icon(MyIcon0, new Size(256, 256));
                notification.Icon = MyIcon0;
                notification.BalloonTipIcon = ToolTipIcon.None;
                notification.Visible = true;
                notification.Text = "";
                notification.BalloonTipTitle = "";
                notification.BalloonTipText = Value;
                notification.ShowBalloonTip(time);
            }
        }

        private void SetStatus(string Value, string DebugValue, string FunctionName, int time = 1000)
        {
            if (NotificationsVerbosity > 0)
            {
                if (NotificationsVerbosity == 1) SetStatus(Value, time);
                if (NotificationsVerbosity == 2) MessageBox.Show(Value + "\n\nDebugging Message:\n" + DebugValue, "PGStat Message");
                if (NotificationsVerbosity == 3) MessageBox.Show(Value + "\n\nDebugging Message:\n" + DebugValue + "\n\nFunction name:\n" + FunctionName, "PGStat Message");
            }
        }

        /// <summary>
        /// This method disconnects PGStat from the device.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (Port.IsOpen) Port.Close();
            }
            catch { }
            Connected = false;
            FoundedPort = "";
            DeviceVersion = 0;
            isFounded = false;
            PG_EVT_Disconnected?.Invoke(this, EventArgs.Empty);
            SetStatus("Application is disconnected.");
        }

        /// <summary>
        /// This method searchs and tries to connect PGStat to the device.
        /// </summary>
        public int Connect()
        {
            /* -1 An unknown error has occurred
             * 0 Successfully completed (No error)
             * 1 Device not found
             * 2 There is not any port in your device
             */
            int HistoryVerbosity = NotificationsVerbosity;
            SetNotificationVerbosity(0);
            PG_EVT_StartConnecting?.Invoke(this, EventArgs.Empty);
            int err = -1;
            string DeviceName = "";
            string Ver = "";
            isFounded = false;
            Thread.Sleep(500);
            for (int trynum = 0; trynum < 2; trynum++)
            {
                if (!isFounded)
                {
                    string[] ArrayComPortsNames = null;
                    int index = -1;
                    FoundedPort = "";
                    DeviceVersion = 0;

                    string ComPortName = null;
                    ArrayComPortsNames = SerialPort.GetPortNames();
                    do
                    {
                        index++;
                        if (!isFounded)
                        {
                            if (CheckPort.IsOpen) CheckPort.Close();
                            CheckPort.WriteTimeout = CheckPortTimeoutSec; CheckPort.ReadTimeout = CheckPortTimeoutSec;
                            if (ArrayComPortsNames.Length == 0)
                            {
                                err = 2; //
                                SetNotificationVerbosity(HistoryVerbosity);
                                return err;
                            }
                            CheckPort.PortName = ArrayComPortsNames[index];

                            // try to open the selected port:
                            try
                            {
                                CheckPort.Open();
                                ClearBuffer(CheckPort);
                                string Order = Command(CheckPort, "you?");
                                if (Order.StartsWith("EIS"))
                                {
                                    FoundedPort = CheckPort.PortName;
                                    //DeviceVersion = Convert.ToInt32(Commands[1]);
                                    DeviceVersion = 1;
                                    isFounded = true;
                                    Ver = Command(CheckPort, "ver?");
                                    string[] Parts;
                                    char[] delimiterChars = { '.' };
                                    Parts = Ver.Split(delimiterChars);
                                    int CurrentVer;
                                    if (Parts[0] == "b")
                                    {
                                        BoardType = 2;
                                        CurrentVer = Convert.ToInt16(Parts[1]);
                                    }
                                    else
                                    {
                                        BoardType = 1;
                                        CurrentVer = Convert.ToInt16(Ver);
                                    }
                                    Port = CheckPort;
                                    //CheckUpdates(CurrentVer);
                                    DeviceName = Order;
                                }
                                else
                                {
                                    if (CheckPort.IsOpen) CheckPort.Close();
                                }
                            }
                            catch {}
                        }
                    }
                    while (!((ArrayComPortsNames[index] == ComPortName) ||
                                        (index == ArrayComPortsNames.GetUpperBound(0))));

                    if (CheckPort.IsOpen) CheckPort.Close();
                    if (isFounded)
                    {
                        Port.WriteTimeout = PortTimeout; Port.ReadTimeout = PortTimeout;
                        if (Port.IsOpen) Port.Close();
                        Port.PortName = FoundedPort;
                        Port.ReadBufferSize = 2000000000;
                        Port.Open();
                        ClearBuffer();
                        Connected = true;
                        PingTimer.Enabled = true;
                        PingTimer.Start();
                        err = 0;
                        SetNotificationVerbosity(HistoryVerbosity);
                        SetStatus("Application is Connected to Device.");
                        PG_EVT_Connected?.Invoke(this, EventArgs.Empty);
                        return err;
                    }
                    else
                    {
                        if (trynum > 0)
                        {
                            SetNotificationVerbosity(HistoryVerbosity);
                            SetStatus("Device was not found.");
                            err = 1;
                            return 1;
                        }
                    }
                }
            }
            SetNotificationVerbosity(HistoryVerbosity);
            return err;
        }

        private void ReConnect()
        {
            SetStatus("Reconnecting ...");
            
            for (int i = 0; i < 10; i++)
            {
                if (!Connected)
                {
                    try
                    {
                        Thread.Sleep(1000);
                        Port.Open();
                        KillProcess();
                        KillProcess();
                        Command(Port, "");
                        string Order = Command(Port, "you?");
                        if (Order.StartsWith("EIS"))
                        {
                            Connected = true;
                            isFounded = true;
                        }
                    }
                    catch
                    { }
                }
            }

            if (Connected)
            {
                PingTimer.Interval = 1000;
                PingTimer.Start();
                PG_EVT_Connected?.Invoke(this, EventArgs.Empty);
                SetStatus("Successfully reconnected ...");
            }
            else
                SetStatus("Sorry. We could not reconnect the system.\nCheck the USB cable and reconnect manually.");

        }

        /// <summary>
        /// Restore device to factory settings.<para />
        /// Please contact with the manufacturer to do this.
        /// </summary>
        public void ResetFactory()
        {
            Settings.IsIVReceiverUnsigned = FactoryDefault.isIVReceiverUnsigned;
            Settings.isDigitalEISReceiverUnsigned = FactoryDefault.isDigitalEISReceiverUnsigned;

            Settings.SetDCV_Offset = FactoryDefault.SetDCV_Offset;
            Settings.SetDCV_Domain = FactoryDefault.SetDCV_Domain;
            Settings.SetDCV_factor = FactoryDefault.SetDCV_factor;

            Settings.GetDCV_OffsetMLP0 = FactoryDefault.GetDCV_OffsetMLP0;
            Settings.GetDCV_OffsetMLP1 = FactoryDefault.GetDCV_OffsetMLP1;
            Settings.GetDCV_OffsetMLP2 = FactoryDefault.GetDCV_OffsetMLP2;
            Settings.GetDCV_OffsetMLP3 = FactoryDefault.GetDCV_OffsetMLP3;
            Settings.GetDCV_OffsetMLP4 = FactoryDefault.GetDCV_OffsetMLP4;
            Settings.GetDCV_offsetMLP5 = FactoryDefault.GetDCV_offsetMLP5;
            Settings.GetDCV_OffsetMLP6 = FactoryDefault.GetDCV_OffsetMLP6;
            Settings.GetDCV_Domain = FactoryDefault.GetDCV_Domain;
            Settings.GetDCV_factor = FactoryDefault.GetDCV_factor;

            Settings.SetDCI_Offset = FactoryDefault.SetDCI_Offset;
            Settings.SetDCI_Domain = FactoryDefault.SetDCI_Domain;
            Settings.SetDCI_Select0 = FactoryDefault.SetDCI_Select0;
            Settings.SetDCI_Select1 = FactoryDefault.SetDCI_Select1;
            Settings.SetDCI_Select2 = FactoryDefault.SetDCI_Select2;
            Settings.SetDCI_factor = FactoryDefault.SetDCI_factor;

            Settings.GetDCI_Offset0d = FactoryDefault.GetDCI_Offset0d;
            Settings.GetDCI_Offset1d = FactoryDefault.GetDCI_Offset1d;
            Settings.GetDCI_Offset2 = FactoryDefault.GetDCI_Offset2;
            Settings.GetDCI_Offset3d = FactoryDefault.GetDCI_Offset3d;
            Settings.GetDCI_Offset4d = FactoryDefault.GetDCI_Offset4d;
            Settings.GetDCI_Offset5d = FactoryDefault.GetDCI_Offset5d;
            Settings.GetDCI_Offset6d = FactoryDefault.GetDCI_Offset6d;
            Settings.GetDCI_Offset7d = FactoryDefault.GetDCI_Offset7d;
            Settings.GetDCI_Domain = FactoryDefault.GetDCI_Domain;
            Settings.GetDCI_Select0 = FactoryDefault.GetDCI_Select0;
            Settings.GetDCI_Select1 = FactoryDefault.GetDCI_Select1;
            Settings.GetDCI_select2 = FactoryDefault.GetDCI_select2;
            Settings.GetDCI_Select3 = FactoryDefault.GetDCI_Select3;
            Settings.GetDCI_Select4 = FactoryDefault.GetDCI_Select4;
            Settings.GetDCI_Select5 = FactoryDefault.GetDCI_Select5;
            Settings.GetDCI_Select6 = FactoryDefault.GetDCI_Select6;
            Settings.GetDCI_Select7 = FactoryDefault.GetDCI_Select7;
            Settings.GetDCI_factor = FactoryDefault.GetDCI_factor;

            Settings.SetDigitalACV_Offset = FactoryDefault.SetDigitalACV_Offset;
            Settings.SetDigitalACV_Domain = FactoryDefault.SetDigitalACV_Domain;
            Settings.SetDigitalACV_factor = FactoryDefault.SetDigitalACV_factor;

            Settings.GetDigitalACV_Offset = FactoryDefault.GetDigitalACV_Offset;
            Settings.GetDigitalACV_Domain = FactoryDefault.GetDigitalACV_Domain;
            Settings.GetDigitalACV_factor = FactoryDefault.GetDigitalACV_factor;

            Settings.SetDigitalf_Min = FactoryDefault.SetDigitalf_Min;
            Settings.SetDigitalf_Max = FactoryDefault.SetDigitalf_Max;
            Settings.SetDigitalf_clock = FactoryDefault.SetDigitalf_clock;
            Settings.SetDigitalf_factor = FactoryDefault.SetDigitalf_factor;

            Settings.GetDigitalf_Min = FactoryDefault.GetDigitalf_Min;
            Settings.GetDigitalf_Max = FactoryDefault.GetDigitalf_Max;
            Settings.GetDigitalf_clock = FactoryDefault.GetDigitalf_clock;
            Settings.GetDigitalf_factor = FactoryDefault.GetDigitalf_factor;

            Settings.AnalogCommon_intOffset = FactoryDefault.AnalogCommon_intOffset;
            Settings.AnalogCommon_Domain = FactoryDefault.AnalogCommon_Domain;
            Settings.AnalogCommon_factor = FactoryDefault.AnalogCommon_factor;

            Settings.Zeroset0 = FactoryDefault.Zeroset0;
            Settings.Zeroset1 = FactoryDefault.Zeroset1;
            Settings.isEIS = FactoryDefault.isEIS;
            Settings.isMSH = FactoryDefault.isMSH;
            Settings.isChrono = FactoryDefault.isChrono;
            Settings.isIV0 = FactoryDefault.isIV0;
            Settings.isCV = FactoryDefault.isCV;
            Settings.isPulse = FactoryDefault.isPulse;

            Settings.GalvanostatI_Select4 = FactoryDefault.GalvanostatI_Select4;
            Settings.GalvanostatI_Select5 = FactoryDefault.GalvanostatI_Select5;
            Settings.GalvanostatI_Select6 = FactoryDefault.GalvanostatI_Select6;
            Settings.GalvanostatI_Select7 = FactoryDefault.GalvanostatI_Select7;

            Settings.GalvanostatI_Select0 = FactoryDefault.GalvanostatI_Select0;
            Settings.GalvanostatI_Select1 = FactoryDefault.GalvanostatI_Select1;
            Settings.GalvanostatI_Select2 = FactoryDefault.GalvanostatI_Select2;
            Settings.GalvanostatI_Select3 = FactoryDefault.GalvanostatI_Select3;

            Settings.SetDCV_Select0 = FactoryDefault.SetDCV_Select0;
            Settings.SetDCV_Select1 = FactoryDefault.SetDCV_Select1;

            Settings.GetDCV_Select0 = FactoryDefault.GetDCV_Select0;
            Settings.GetDCV_Select1 = FactoryDefault.GetDCV_Select1;

            Settings.ACMultFactor_Select0 = FactoryDefault.ACMultFactor_Select0;
            Settings.ACMultFactor_Select1 = FactoryDefault.ACMultFactor_Select1;

            Settings.FilterC_V1 = FactoryDefault.FilterC_V1;
            Settings.FilterC_V2 = FactoryDefault.FilterC_V2;
            Settings.FilterC_I1 = FactoryDefault.FilterC_I1;
            Settings.FilterC_I2 = FactoryDefault.FilterC_I2;

            Settings.SetACVMaxS0 = FactoryDefault.SetACVMaxS0;
            Settings.SetACVResoloution = FactoryDefault.SetACVResoloution;

            Settings.GetACVMaxS0 = FactoryDefault.GetACVMaxS0;
            Settings.GetACVResoloution0 = FactoryDefault.GetACVResoloution0;

            Settings.VTau_L = FactoryDefault.VTau_L;
            Settings.VTau_H = FactoryDefault.VTau_H;
            Settings.ITau_L0 = FactoryDefault.ITau_L0;
            Settings.ITau_H0 = FactoryDefault.ITau_H0;
            Settings.ITau_L1 = FactoryDefault.ITau_L1;
            Settings.ITau_H1 = FactoryDefault.ITau_H1;
            Settings.ITau_L2 = FactoryDefault.ITau_L2;
            Settings.ITau_H2 = FactoryDefault.ITau_H2;
        }

        /// <summary>
        /// Write the settings in a binary file.
        /// </summary>
        public void SaveSettings(string FileName)
        {
            try
            {
                FileStream FileProtocol = new FileStream(FileName, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(FileProtocol);
                byte counter = 0;
                bw.Write(++counter); bw.Write(Settings.Version);

                bw.Write(++counter); bw.Write(Settings.IsIVReceiverUnsigned);
                bw.Write(++counter); bw.Write(Settings.isDigitalEISReceiverUnsigned);

                bw.Write(++counter); bw.Write(Settings.SetDCV_Offset);
                bw.Write(++counter); bw.Write(Settings.SetDCV_Domain);
                bw.Write(++counter); bw.Write(Settings.SetDCV_factor);

                bw.Write(++counter); bw.Write(Settings.GetDCV_OffsetMLP0);
                bw.Write(++counter); bw.Write(Settings.GetDCV_OffsetMLP1);
                bw.Write(++counter); bw.Write(Settings.GetDCV_OffsetMLP2);
                bw.Write(++counter); bw.Write(Settings.GetDCV_OffsetMLP3);
                bw.Write(++counter); bw.Write(Settings.GetDCV_OffsetMLP4);
                bw.Write(++counter); bw.Write(Settings.GetDCV_offsetMLP5);
                bw.Write(++counter); bw.Write(Settings.GetDCV_OffsetMLP6);
                bw.Write(++counter); bw.Write(Settings.GetDCV_Domain);
                bw.Write(++counter); bw.Write(Settings.GetDCV_factor);

                bw.Write(++counter); bw.Write(Settings.SetDCI_Offset);
                bw.Write(++counter); bw.Write(Settings.SetDCI_Domain);
                bw.Write(++counter); bw.Write(Settings.SetDCI_Select0);
                bw.Write(++counter); bw.Write(Settings.SetDCI_Select1);
                bw.Write(++counter); bw.Write(Settings.SetDCI_Select2);
                bw.Write(++counter); bw.Write(Settings.SetDCI_factor);

                bw.Write(++counter); bw.Write(Settings.GetDCI_Offset0d);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Offset1d);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Offset2);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Offset3d);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Offset4d);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Offset5d);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Offset6d);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Offset7d);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Domain);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Select0);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Select1);
                bw.Write(++counter); bw.Write(Settings.GetDCI_select2);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Select3);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Select4);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Select5);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Select6);
                bw.Write(++counter); bw.Write(Settings.GetDCI_Select7);
                bw.Write(++counter); bw.Write(Settings.GetDCI_factor);

                bw.Write(++counter); bw.Write(Settings.SetDigitalACV_Offset);
                bw.Write(++counter); bw.Write(Settings.SetDigitalACV_Domain);
                bw.Write(++counter); bw.Write(Settings.SetDigitalACV_factor);

                bw.Write(++counter); bw.Write(Settings.GetDigitalACV_Offset);
                bw.Write(++counter); bw.Write(Settings.GetDigitalACV_Domain);
                bw.Write(++counter); bw.Write(Settings.GetDigitalACV_factor);

                bw.Write(++counter); bw.Write(Settings.SetDigitalf_Min);
                bw.Write(++counter); bw.Write(Settings.SetDigitalf_Max);
                bw.Write(++counter); bw.Write(Settings.SetDigitalf_clock);
                bw.Write(++counter); bw.Write(Settings.SetDigitalf_factor);

                bw.Write(++counter); bw.Write(Settings.GetDigitalf_Min);
                bw.Write(++counter); bw.Write(Settings.GetDigitalf_Max);
                bw.Write(++counter); bw.Write(Settings.GetDigitalf_clock);
                bw.Write(++counter); bw.Write(Settings.GetDigitalf_factor);

                bw.Write(++counter); bw.Write(Settings.AnalogCommon_intOffset);
                bw.Write(++counter); bw.Write(Settings.AnalogCommon_Domain);
                bw.Write(++counter); bw.Write(Settings.AnalogCommon_factor);

                bw.Write(++counter); bw.Write(Settings.Zeroset0);
                bw.Write(++counter); bw.Write(Settings.Zeroset1);
                bw.Write(++counter); bw.Write(Settings.isEIS);
                bw.Write(++counter); bw.Write(Settings.isMSH);
                bw.Write(++counter); bw.Write(Settings.isCV);
                bw.Write(++counter); bw.Write(Settings.isIV0);
                bw.Write(++counter); bw.Write(Settings.isChrono);
                bw.Write(++counter); bw.Write(Settings.isPulse);

                bw.Write(++counter); bw.Write(Settings.GalvanostatI_Select4);
                bw.Write(++counter); bw.Write(Settings.GalvanostatI_Select5);
                bw.Write(++counter); bw.Write(Settings.GalvanostatI_Select6);
                bw.Write(++counter); bw.Write(Settings.GalvanostatI_Select7);

                bw.Write(++counter); bw.Write(Settings.GalvanostatI_Select0);
                bw.Write(++counter); bw.Write(Settings.GalvanostatI_Select1);
                bw.Write(++counter); bw.Write(Settings.GalvanostatI_Select2);
                bw.Write(++counter); bw.Write(Settings.GalvanostatI_Select3);

                bw.Write(++counter); bw.Write(Settings.SetDCV_Select0);
                bw.Write(++counter); bw.Write(Settings.SetDCV_Select1);

                bw.Write(++counter); bw.Write(Settings.GetDCV_Select0);
                bw.Write(++counter); bw.Write(Settings.GetDCV_Select1);

                bw.Write(++counter); bw.Write(Settings.ACMultFactor_Select0);
                bw.Write(++counter); bw.Write(Settings.ACMultFactor_Select1);

                bw.Write(++counter); bw.Write(Settings.FilterC_V1);
                bw.Write(++counter); bw.Write(Settings.FilterC_V2);
                bw.Write(++counter); bw.Write(Settings.FilterC_I1);
                bw.Write(++counter); bw.Write(Settings.FilterC_I2);

                bw.Write(++counter); bw.Write(Settings.SetACVMaxS0);
                bw.Write(++counter); bw.Write(Settings.SetACVResoloution);

                bw.Write(++counter); bw.Write(Settings.GetACVMaxS0);
                bw.Write(++counter); bw.Write(Settings.GetACVResoloution0);

                bw.Write(++counter); bw.Write(Settings.VTau_L);
                bw.Write(++counter); bw.Write(Settings.VTau_H);
                bw.Write(++counter); bw.Write(Settings.ITau_L0);
                bw.Write(++counter); bw.Write(Settings.ITau_H0);
                bw.Write(++counter); bw.Write(Settings.ITau_L1);
                bw.Write(++counter); bw.Write(Settings.ITau_H1);
                bw.Write(++counter); bw.Write(Settings.ITau_L2);
                bw.Write(++counter); bw.Write(Settings.ITau_H2);

                bw.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Save the settings from a binary file to the Micro Controller.
        /// </summary>
        public bool MicroSaveSettings(string FileName)
        {
            bool done = false;
            FileStream myStream = new FileStream(FileName, FileMode.Open);
            //BinaryReader br = new BinaryReader(myStream);

            //XmlSerializer^ serializer=gcnew XmlSerializer(Settings::typeid);
            //IO::Stream^ myStream;
            try
            {
                //myStream = gcnew FileStream(Directory::GetCurrentDirectory()+"\\"+ "settings",FileMode::Create );
                // Serialize the object, and close the TextWriter
                //serializer->Serialize( myStream, sett );

                String ans = null;
                Port.DiscardInBuffer();
                Port.DiscardOutBuffer();
                Port.WriteLine("save " + myStream.Length.ToString());
                Thread.Sleep(500);
                ans = Port.ReadLine();
                if (ans != "ready") throw (new Exception("Undefined command received from device ..."));
                byte[] b = new byte[myStream.Length];
                myStream.Position = 0;
                myStream.Read(b, 0, (int)myStream.Length);
                for (int i = 0; i < myStream.Length; i++)
                {
                    Port.Write(b, i, 1);
                    Thread.Sleep(50);
                }
                Thread.Sleep(500);
                ans = Port.ReadLine(); myStream.Close();
                if (ans != "finish") throw (new Exception("Undefined command received from device ..."));
                done = true;
            }
            catch (Exception ex)
            {
                myStream.Close();
                SetStatus("failed to save settings!", ex.Message, "private bool microsaveSetting(string FileName)");
            }


            FileStream s = new FileStream(FileName, FileMode.Open);
            FileStream ms = new FileStream("../microsettings.bin", FileMode.Create);
            for (int i = 0; i < s.Length; i++) ms.WriteByte((byte)s.ReadByte());
            ms.Close();
            s.Close();

            if (done) SetStatus("Device Settings are updated ... ver=" + Settings.Version.ToString());
            return done;
        }

        /// <summary>
        /// Load the settings from a binary file.<para />
        /// Returns the version of the settings written in the file.<para />
        /// Returns 0 if the file was not found.<para />
        /// Returns -1 if the file is an old verson or maybe it was corrupted during save process.
        /// </summary>
        public int LoadSettings(string FileName)
        {
            //FillSettingDefault(ref Settings);
            //string FileName = "./settings.bin";

            int FileVer = 0;

            if (File.Exists(FileName))
            {
                FileStream FileProtocol = new FileStream(FileName, FileMode.Open);
                BinaryReader br = new BinaryReader(FileProtocol);

                try
                {
                    br.ReadByte(); FileVer = br.ReadInt32();

                    br.ReadByte(); Settings.IsIVReceiverUnsigned = br.ReadBoolean();
                    br.ReadByte(); Settings.isDigitalEISReceiverUnsigned = br.ReadBoolean();

                    br.ReadByte(); Settings.SetDCV_Offset = br.ReadInt32();
                    br.ReadByte(); Settings.SetDCV_Domain = br.ReadSingle();
                    br.ReadByte(); Settings.SetDCV_factor = br.ReadSingle();

                    br.ReadByte(); Settings.GetDCV_OffsetMLP0 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCV_OffsetMLP1 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCV_OffsetMLP2 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCV_OffsetMLP3 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCV_OffsetMLP4 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCV_offsetMLP5 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCV_OffsetMLP6 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCV_Domain = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCV_factor = br.ReadSingle();

                    br.ReadByte(); Settings.SetDCI_Offset = br.ReadInt32();
                    br.ReadByte(); Settings.SetDCI_Domain = br.ReadSingle();
                    br.ReadByte(); Settings.SetDCI_Select0 = br.ReadSingle();
                    br.ReadByte(); Settings.SetDCI_Select1 = br.ReadSingle();
                    br.ReadByte(); Settings.SetDCI_Select2 = br.ReadSingle();
                    br.ReadByte(); Settings.SetDCI_factor = br.ReadSingle();

                    br.ReadByte(); Settings.GetDCI_Offset0d = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Offset1d = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Offset2 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Offset3d = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Offset4d = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Offset5d = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Offset6d = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Offset7d = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Domain = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Select0 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Select1 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_select2 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Select3 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Select4 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Select5 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Select6 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_Select7 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCI_factor = br.ReadSingle();

                    br.ReadByte(); Settings.SetDigitalACV_Offset = br.ReadInt32();
                    br.ReadByte(); Settings.SetDigitalACV_Domain = br.ReadSingle();
                    br.ReadByte(); Settings.SetDigitalACV_factor = br.ReadSingle();

                    br.ReadByte(); Settings.GetDigitalACV_Offset = br.ReadInt32();
                    br.ReadByte(); Settings.GetDigitalACV_Domain = br.ReadSingle();
                    br.ReadByte(); Settings.GetDigitalACV_factor = br.ReadSingle();

                    br.ReadByte(); Settings.SetDigitalf_Min = br.ReadSingle();
                    br.ReadByte(); Settings.SetDigitalf_Max = br.ReadSingle();
                    br.ReadByte(); Settings.SetDigitalf_clock = br.ReadInt32();
                    br.ReadByte(); Settings.SetDigitalf_factor = br.ReadSingle();

                    br.ReadByte(); Settings.GetDigitalf_Min = br.ReadSingle();
                    br.ReadByte(); Settings.GetDigitalf_Max = br.ReadSingle();
                    br.ReadByte(); Settings.GetDigitalf_clock = br.ReadInt32();
                    br.ReadByte(); Settings.GetDigitalf_factor = br.ReadSingle();

                    br.ReadByte(); Settings.AnalogCommon_intOffset = br.ReadInt32();
                    br.ReadByte(); Settings.AnalogCommon_Domain = br.ReadSingle();
                    br.ReadByte(); Settings.AnalogCommon_factor = br.ReadSingle();

                    br.ReadByte(); Settings.Zeroset0 = br.ReadInt32();
                    br.ReadByte(); Settings.Zeroset1 = br.ReadInt32();
                    br.ReadByte(); Settings.isEIS = br.ReadBoolean();
                    br.ReadByte(); Settings.isMSH = br.ReadBoolean();
                    br.ReadByte(); Settings.isCV = br.ReadBoolean();
                    br.ReadByte(); Settings.isIV0 = br.ReadBoolean();
                    br.ReadByte(); Settings.isChrono = br.ReadBoolean();
                    br.ReadByte(); Settings.isPulse = br.ReadBoolean();

                    br.ReadByte(); Settings.GalvanostatI_Select4 = br.ReadSingle();
                    br.ReadByte(); Settings.GalvanostatI_Select5 = br.ReadSingle();
                    br.ReadByte(); Settings.GalvanostatI_Select6 = br.ReadSingle();
                    br.ReadByte(); Settings.GalvanostatI_Select7 = br.ReadSingle();

                    br.ReadByte(); Settings.GalvanostatI_Select0 = br.ReadSingle();
                    br.ReadByte(); Settings.GalvanostatI_Select1 = br.ReadSingle();
                    br.ReadByte(); Settings.GalvanostatI_Select2 = br.ReadSingle();
                    br.ReadByte(); Settings.GalvanostatI_Select3 = br.ReadSingle();

                    br.ReadByte(); Settings.SetDCV_Select0 = br.ReadSingle();
                    br.ReadByte(); Settings.SetDCV_Select1 = br.ReadSingle();

                    br.ReadByte(); Settings.GetDCV_Select0 = br.ReadSingle();
                    br.ReadByte(); Settings.GetDCV_Select1 = br.ReadSingle();

                    if (FileVer >= 6) br.ReadByte(); Settings.ACMultFactor_Select0 = br.ReadSingle();
                    if (FileVer >= 6) br.ReadByte(); Settings.ACMultFactor_Select1 = br.ReadSingle();

                    if (FileVer >= 7) br.ReadByte(); Settings.FilterC_V1 = br.ReadSingle();
                    if (FileVer >= 7) br.ReadByte(); Settings.FilterC_V2 = br.ReadSingle();
                    if (FileVer >= 7) br.ReadByte(); Settings.FilterC_I1 = br.ReadSingle();
                    if (FileVer >= 7) br.ReadByte(); Settings.FilterC_I2 = br.ReadSingle();

                    br.ReadByte(); Settings.SetACVMaxS0 = br.ReadSingle();
                    br.ReadByte(); Settings.SetACVResoloution = br.ReadInt32();

                    br.ReadByte(); Settings.GetACVMaxS0 = br.ReadSingle();
                    br.ReadByte(); Settings.GetACVResoloution0 = br.ReadInt32();

                    br.ReadByte(); Settings.VTau_L = br.ReadSingle();
                    br.ReadByte(); Settings.VTau_H = br.ReadSingle();
                    br.ReadByte(); Settings.ITau_L0 = br.ReadSingle();
                    br.ReadByte(); Settings.ITau_H0 = br.ReadSingle();
                    br.ReadByte(); Settings.ITau_L1 = br.ReadSingle();
                    br.ReadByte(); Settings.ITau_H1 = br.ReadSingle();
                    br.ReadByte(); Settings.ITau_L2 = br.ReadSingle();
                    br.ReadByte(); Settings.ITau_H2 = br.ReadSingle();
                }
                catch
                {
                    return -1;
                }

                br.Close();
                return FileVer;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Load the settings from the device and save to a binary file.<para />
        /// Returns the version of the settings written in the file.<para />
        /// Returns 0 if the file was not found.<para />
        /// Returns -1 if the file is an old verson or maybe it was corrupted during save process.
        /// </summary>
        public bool LoadSettingsFromDevice(string FileName)
        {
            bool isdone = false;
            //string FileName = "../microsettings.bin";
            FileStream myStream = new FileStream(FileName, FileMode.Create);
            //BinaryReader bw = new BinaryReader(myStream);

            try
            {
                string ans = Command("load");
                Thread.Sleep(100);
                //if (ans != "ready") throw(L"Undefined command received from device ...");
                UInt16 n = Convert.ToUInt16(ans);
                if (n < 1)
                {
                    myStream.Close();
                    return isdone;
                }

                if (n <= 1586)
                {
                    myStream.Position = 0;
                    // Serialize the object, and close the TextWriter
                    for (int i = 0; i < n; i++)
                    {
                        byte b = (byte)Port.ReadByte();
                        myStream.WriteByte(b);
                        // Thread::Sleep(10);
                    }
                    Thread.Sleep(100);
                    ans = Port.ReadLine();
                    if (ans != "finish")
                    {
                        myStream.Close();
                        throw (new Exception("MicroLoadSetting error. Undefined command received from device ..."));
                    }
                    else
                    {
                        //XmlSerializer^ serializer=gcnew XmlSerializer(Settings::typeid);
                        myStream.Position = 0;
                        //sett=(Settings)(serializer->Deserialize(myStream));
                        myStream.Close();
                        isdone = true;
                    }
                }
                else
                {
                    myStream.Close();
                    //MessageBox.Show("micro save number 1");
                    //microSaveSetting("./settings.bin");
                }
            }
            catch (Exception e) { myStream.Close(); MessageBox.Show("Failed!\r" + e.Message); }

            return isdone;
        }

        /// <summary>
        /// Offset removal in Potentiostat mode.<para />
        /// Returns true if done.
        /// </summary>
        public bool OffsetRemoval()
        {
            if (!IsConnected())
            {
                SetStatus("Unable to send command to device. PGStat has not connected to device yet.");
                return false;
            }

            if (IsBusy())
            {
                SetStatus("Unable to send command to device.", "Port is busy by another async mode process.", "bool ACommand(string command, int Delay = 100)");
                return false;
            }

            try
            {
                Thread t = new Thread(new ThreadStart(OffsetRemoval_process));
                t.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        
        private void OffsetRemoval_process()
        {
            bool isOK = false;
            bool pingstatusHistory = IsPingEnabled();
            DisablePing();

            OffsetRemovalStartedEventArgs e1 = new OffsetRemovalStartedEventArgs();
            PG_EVT_OffsetRemovalStarted?.Invoke(this, e1);

            int iVoltChecker = 2047;
            int VmlpChecker = 2;
            double VoltRead = 2047;
            double Ioffset = 0;
            double Voffset = 0;

            try
            {
                acset(0);
                zeroset(Settings.Zeroset0);
                Thread.Sleep(1000);
                setselect(0);
                idcmlp(0);
                idcselect(0);
                for (int ivmlp = 2; ivmlp >= 0; ivmlp--)
                {
                    CalculateSpecificIVOffsets(0, 0, VmlpChecker, ref Ioffset, ref Voffset);
                    sample(1);
                    sample(0);
                    dummy(1);
                    VmlpChecker = ivmlp;
                    vdcmlp(VmlpChecker);
                    double[] output = ivset(iVoltChecker);
                    int FWaitTime = 3;
                    Thread.Sleep(FWaitTime * 1000);

                    output = ivset(iVoltChecker);
                    try
                    {
                        double Vmean = output[0];
                        if (Vmean < 1000 && Vmean > -1000 || VmlpChecker == 0)
                        {
                            VoltRead = GetDCVConvertWithNewOffset(Vmean, VmlpChecker, Voffset, 0);
                            break;
                        }
                    }
                    catch { }
                }
                
                int ideltaV = SetDCVConvert(VoltRead, 0, 0, 0) - Settings.SetDCV_Offset;
                double OldVoltRead = 0;
                int approximatelyZero = Settings.Zeroset0;
                if (Math.Abs(ideltaV) < 100)
                    approximatelyZero = approximatelyZero - 5 * ideltaV;
                int checkmin = 0;// approximatelyZero - 500;
                int checkmax = 4000;// approximatelyZero + 500;
                if (checkmin < 0) checkmin = 0;
                if (checkmax > 4095) checkmax = 4095;
                for (int zset = checkmin; zset <= checkmax; zset += 1)
                {
                    zeroset(zset);
                    double[] myoutput = ivset(iVoltChecker);
                    
                    try
                    {
                        double Vmean = myoutput[0];
                        VoltRead = GetDCVConvertWithNewOffset(Vmean, VmlpChecker, Voffset, 0);
                        if (VoltRead * OldVoltRead < 0 && zset > checkmin)
                        {
                            Settings.Zeroset0 = zset;
                            break;
                        }
                        OldVoltRead = VoltRead;
                    }
                    catch { }

                }

                //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " Zeroset0=" + Settings.Zeroset0.ToString());

                iVoltChecker = 2047;
                VmlpChecker = 2;
                VoltRead = 2047;

                zeroset(Settings.Zeroset1);
                Thread.Sleep(1000);
                setselect(1);
                //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " OK");

                for (int ivmlp = 2; ivmlp >= 0; ivmlp--)
                {
                    CalculateSpecificIVOffsets(0, 0, VmlpChecker, ref Ioffset, ref Voffset);
                    sample(1);
                    sample(0);
                    //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " sampleon 0");
                    dummy(1);
                    //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " OK");
                    VmlpChecker = ivmlp;
                    vdcmlp(VmlpChecker);
                    //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " OK");
                    double[] myoutput = ivset(iVoltChecker);
                    int FWaitTime = 3;
                    Thread.Sleep(FWaitTime * 1000);
                    myoutput = ivset(iVoltChecker);

                    try
                    {
                        double Vmean = myoutput[0];
                        if (Vmean < 1000 && Vmean > -1000 || VmlpChecker == 0)
                        {
                            VoltRead = GetDCVConvertWithNewOffset(Vmean, VmlpChecker, Voffset, 1);
                            break;
                        }
                    }
                    catch { }
                }
                
                int ideltaV1 = SetDCVConvert(VoltRead, 1, 0, 0) - Settings.SetDCV_Offset;
                //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " ideltaV1=" + ideltaV1.ToString());
                OldVoltRead = 0;
                approximatelyZero = Settings.Zeroset1;
                if (Math.Abs(ideltaV1) < 100)
                    approximatelyZero = approximatelyZero - 5 * ideltaV1;
                checkmin = 0;// approximatelyZero - 500;
                checkmax = 4000;//approximatelyZero + 500;
                if (checkmin < 0) checkmin = 0;
                if (checkmax > 4095) checkmax = 4095;
                for (int zset = checkmin; zset <= checkmax; zset++)
                {
                    zeroset(zset);
                    double[] myoutput = ivset(iVoltChecker);
                    
                    try
                    {
                        double Vmean = myoutput[0];
                        VoltRead = GetDCVConvertWithNewOffset(Vmean, VmlpChecker, Voffset, 1);
                        if (VoltRead * OldVoltRead < 0 && zset > checkmin)
                        {
                            Settings.Zeroset1 = zset;
                            break;
                        }
                        OldVoltRead = VoltRead;
                    }
                    catch { }

                }

                //DebugListBox.Items.Add("stp:" + DebugListBox.Items.Count.ToString() + " Zeroset1=" + Settings.Zeroset1.ToString());

                //saveSetting(ref Settings, "./settings.bin");

                vdcmlp(0);

                if (Lastdummy == 1)
                    dummy(1);
                else
                    dummy(0);

                if (Lastsample == 1)
                    sample(1);
                else
                    sample(0);

                SetStatus("Offset removal for Potentiostat has been done.");
                isOK = true;
            }
            catch (Exception ex)
            {
                SetStatus("Offset removal for Potentiostat failed.");
            }

            OffsetRemovalFinishedEventArgs e2 = new OffsetRemovalFinishedEventArgs();
            PG_EVT_OffsetRemovalFinished?.Invoke(this, e2);
            if (pingstatusHistory) EnablePing();
        }

        /// <summary>
        /// Offset removal in Galvanostat mode.<para />
        /// Returns true if done.
        /// </summary>
        public bool G_OffsetRemoval()
        {
            if (!IsConnected())
            {
                SetStatus("Unable to send command to device. PGStat has not connected to device yet.");
                return false;
            }

            if (IsBusy())
            {
                SetStatus("Unable to send command to device.", "Port is busy by another async mode process.", "bool ACommand(string command, int Delay = 100)");
                return false;
            }

            try
            {
                Thread t = new Thread(new ThreadStart(G_OffsetRemoval_process));
                t.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private void G_OffsetRemoval_process()
        {
            //LoadSettings(ref Settings);
            bool isOK = false;
            bool pingstatusHistory = IsPingEnabled();
            DisablePing();

            OffsetRemovalStartedEventArgs e1 = new OffsetRemovalStartedEventArgs();
            PG_EVT_OffsetRemovalStarted?.Invoke(this, e1);

            try
            {
                PGmode(3);
                int MyVFilter = 4; //For IV and chrono and pulse
                vfilter(MyVFilter);
                sample(1);
                sample(0);
                dummy(1);
                
                for (int iselect = 0; iselect < 8; iselect++)
                {
                    idcselect(iselect);
                    setselect(1);
                    acset(0);
                    vdcmlp(0);
                    idcmlp(0);
                    
                    double Z0 = 2047;
                    double OldI = 0;
                    for (int zerosett = 0; zerosett < 4095; zerosett += 10)
                    {
                        zeroset(zerosett);
                        double[] output = ivset(2047);
                        double Imean = output[1];
                        if ((OldI * Imean <= 0) && (zerosett > 0))
                        {
                            Z0 = zerosett;
                            break;
                        }
                        OldI = Imean;
                    }
                    
                    if (iselect == 0)
                        Settings.GalvanostatI_Select0 = (int)Z0;
                    else if (iselect == 1)
                        Settings.GalvanostatI_Select1 = (int)Z0;
                    else if (iselect == 2)
                        Settings.GalvanostatI_Select2 = (int)Z0;
                    else if (iselect == 3)
                        Settings.GalvanostatI_Select3 = (int)Z0;
                    else if (iselect == 4)
                        Settings.GalvanostatI_Select4 = (int)Z0;
                    else if (iselect == 5)
                        Settings.GalvanostatI_Select5 = (int)Z0;
                    else if (iselect == 6)
                        Settings.GalvanostatI_Select6 = (int)Z0;
                    else if (iselect == 7)
                        Settings.GalvanostatI_Select7 = (int)Z0;

                }

                if (Lastdummy == 1)
                    dummy(1);
                else
                    dummy(0);

                if (Lastsample == 1)
                    sample(1);
                else
                    sample(0);

                SetStatus("Offset removal for Galvanostat has been done.");
                isOK = true;
            }
            catch (Exception ex)
            {
                SetStatus("Offset removal for Galvanostat failed.");
            }

            OffsetRemovalFinishedEventArgs e2 = new OffsetRemovalFinishedEventArgs();
            PG_EVT_OffsetRemovalFinished?.Invoke(this, e2);
            if (pingstatusHistory) EnablePing();
        }

        /***************************************************************************************************************/
    }

    /// <summary>
    /// The information coming with the event PG_EVT_OffsetRemovalFinished.
    /// </summary>
    public class OffsetRemovalFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor of PG_EVT_AProcessStarted class.
        /// </summary>
        public OffsetRemovalFinishedEventArgs() { }
    }
    
    /// <summary>
    /// The information coming with the event PG_EVT_Ping.
    /// </summary>
    public class PingEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor of AdcgetTotalEventArgs class.
        /// </summary>
        public PingEventArgs(double voltage, double current) {Voltage = voltage; Current = current; }

        /// <summary>
        /// The raw value of Voltage passing through PG_EVT_Ping.
        /// </summary>
        public double Voltage { get; set; }
        /// <summary>
        /// The raw value of Current passing through PG_EVT_Ping.
        /// </summary>
        public double Current { get; set; }
    }

    /// <summary>
    /// The information coming with the event PG_EVT_OffsetRemovalFinished.
    /// </summary>
    public class OffsetRemovalStartedEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor of PG_EVT_AProcessStarted class.
        /// </summary>
        public OffsetRemovalStartedEventArgs() {}
    }

    /// <summary>
    /// The information coming with the event PG_EVT_AProcessStarted.
    /// </summary>
    public class AProcessStartedEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor of PG_EVT_AProcessStarted class.
        /// </summary>
        public AProcessStartedEventArgs(string ProcessName) { Process = ProcessName; }

        /// <summary>
        /// The name of process passing through PG_EVT_AProcessStarted.
        /// </summary>
        public string Process { get; set; }
    }

    /// <summary>
    /// The information coming with the event PG_EVT_AProcessFinished.
    /// </summary>
    public class AProcessFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor of PG_EVT_AProcessFinished class.
        /// </summary>
        public AProcessFinishedEventArgs(string ProcessName) { Process = ProcessName; }

        /// <summary>
        /// The name of process passing through PG_EVT_AProcessFinished.
        /// </summary>
        public string Process { get; set; }
    }

    /// <summary>
    /// The information coming with the event PG_EVT_LogReceived.
    /// </summary>
    public class LogReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor of LogReceivedEventArgs class.
        /// </summary>
        public LogReceivedEventArgs(string info) { Info = info; }

        /// <summary>
        /// The information to log all data.
        /// </summary>
        public string Info { get; set; }
    }

    /// <summary>
    /// The information coming with the event PG_EVT_AdcgetDataReceived.
    /// </summary>
    public class AdcgetEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor of AdcgetEventArgs class.
        /// </summary>
        public AdcgetEventArgs(int count, int index, int voltage, int current) { Count = count; Index = index; Voltage = voltage; Current = current; }

        /// <summary>
        /// The count of forthcoming data including Voltage and Current.
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// The index of the received data including Voltage and Current.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// The raw value of Voltage passing through PG_EVT_AdcgetDataReceived.
        /// </summary>
        public int Voltage { get; set; }
        /// <summary>
        /// The raw value of Current passing through PG_EVT_AdcgetDataReceived.
        /// </summary>
        public int Current { get; set; }
    }

    /// <summary>
    /// The information coming with the event PG_EVT_AdcgetTotalDataReceived.
    /// </summary>
    public class AdcgetTotalEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor of AdcgetTotalEventArgs class.
        /// </summary>
        public AdcgetTotalEventArgs(int count, int[] voltage, int[] current) { Count = count; Voltage = voltage; Current = current; }

        /// <summary>
        /// The count of the received data including Voltage and Current.
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// The raw value of Voltage passing through PG_EVT_AdcgetTotalDataReceived.
        /// </summary>
        public int[] Voltage { get; set; }
        /// <summary>
        /// The raw value of Current passing through PG_EVT_AdcgetTotalDataReceived.
        /// </summary>
        public int[] Current { get; set; }
    }

    /// <summary>
    /// The information coming with the event PG_EVT_ACommandDataReceived.
    /// </summary>
    public class ACommandEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor of AdcgetEventArgs class.
        /// </summary>
        public ACommandEventArgs(string ans) { Ans = ans; }

        /// <summary>
        /// The answere from device as a string passing through PG_EVT_ACommandDataReceived.
        /// </summary>
        public string Ans { get; set; }
    }

    /// <summary>
    /// The information coming with the event PG_EVT_AivsetDataReceived.
    /// </summary>
    public class AivsetEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor of AivsetEventArgs class.
        /// </summary>
        public AivsetEventArgs(double voltage, double current) { Voltage = voltage; Current = current; }

        /// <summary>
        /// The raw value of Voltage passing through PG_EVT_AivsetDataReceived.
        /// </summary>
        public double Voltage { get; set; }
        /// <summary>
        /// The raw value of Current passing through PG_EVT_AivsetDataReceived.
        /// </summary>
        public double Current { get; set; }
    }

    /// <summary>
    /// The information coming with the event PG_EVT_ACVDataReceived.
    /// </summary>
    public class ACVEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor of ACVEventArgs class.
        /// </summary>
        public ACVEventArgs(int count, int count0, int count1, int index, double maxtime, double time, double voltage, double current, double measuredvoltage)
        {
            Count = count;
            Count0 = count0;
            Count1 = count1;
            Index = index;
            MaxTime = maxtime;
            Time = time;
            Voltage = voltage;
            Current = current;
            MeasuredVoltage = measuredvoltage;
        }

        /// <summary>
        /// The count of forthcoming data.
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// The count of the first sweep.
        /// </summary>
        public int Count0 { get; set; }
        /// <summary>
        /// The count of other sweeps.
        /// </summary>
        public int Count1 { get; set; }
        /// <summary>
        /// The index of this data.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// The value of time in seconds passing through PG_EVT_ACVDataReceived.
        /// </summary>
        public double Time { get; set; }
        /// <summary>
        /// The maximum value of time in seconds passing through PG_EVT_ACVDataReceived.
        /// </summary>
        public double MaxTime { get; set; }
        /// <summary>
        /// The value of Voltage in Volt passing through PG_EVT_ACVDataReceived.
        /// </summary>
        public double Voltage { get; set; }
        /// <summary>
        /// The value of Current in Ampere passing through PG_EVT_ACVDataReceived.
        /// </summary>
        public double Current { get; set; }
        /// <summary>
        /// The value of measured Voltage in Volt passing through PG_EVT_ACVDataReceived.
        /// </summary>
        public double MeasuredVoltage { get; set; }
    }

    /// <summary>
    /// The information coming with the event PG_EVT_AIVDataReceived.
    /// </summary>
    public class AIVEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor of AIVEventArgs class.
        /// </summary>
        public AIVEventArgs(int count, int index, double maxtime, double time, double voltage, double current, double measuredvoltage)
        {
            Count = count;
            Index = index;
            MaxTime = maxtime;
            Time = time;
            Voltage = voltage;
            Current = current;
            MeasuredVoltage = measuredvoltage;
        }

        /// <summary>
        /// The count of forthcoming data.
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// The index of this data.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// The value of time in seconds passing through PG_EVT_AIVDataReceived.
        /// </summary>
        public double Time { get; set; }
        /// <summary>
        /// The maximum value of time in seconds passing through PG_EVT_AIVDataReceived.
        /// </summary>
        public double MaxTime { get; set; }
        /// <summary>
        /// The value of Voltage in Volt passing through PG_EVT_AIVDataReceived.
        /// </summary>
        public double Voltage { get; set; }
        /// <summary>
        /// The value of Current in Ampere passing through PG_EVT_AIVDataReceived.
        /// </summary>
        public double Current { get; set; }
        /// <summary>
        /// The value of measured Voltage in Volt passing through PG_EVT_AIVDataReceived.
        /// </summary>
        public double MeasuredVoltage { get; set; }
    }

    /// <summary>
    /// The information coming with the event PG_EVT_AChronoDataReceived.
    /// </summary>
    public class AChronoEventArgs : EventArgs
    {
        /// <summary>
        /// The constructor of AChronoEventArgs class.
        /// </summary>
        public AChronoEventArgs(int count, int index, double maxtime, double time, double current, double measuredvoltage)
        {
            Count = count;
            Index = index;
            MaxTime = maxtime;
            Time = time;
            Current = current;
            MeasuredVoltage = measuredvoltage;
        }

        /// <summary>
        /// The count of forthcoming data.
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// The index of this data.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// The value of time in seconds passing through PG_EVT_AChronoDataReceived.
        /// </summary>
        public double Time { get; set; }
        /// <summary>
        /// The maximum value of time in seconds passing through PG_EVT_AChronoDataReceived.
        /// </summary>
        public double MaxTime { get; set; }
        /// <summary>
        /// The value of Current in Ampere passing through PG_EVT_AChronoDataReceived.
        /// </summary>
        public double Current { get; set; }
        /// <summary>
        /// The value of measured Voltage in Volt passing through PG_EVT_AChronoDataReceived.
        /// </summary>
        public double MeasuredVoltage { get; set; }
    }

    /// <summary>
    /// The settings of the device.
    /// </summary>
    public static class Settings
    {
        public static int SetDCV_Offset;
        public static float SetDCV_Domain;
        public static float SetDCV_factor;

        public static float GetDCV_OffsetMLP0;
        public static float GetDCV_OffsetMLP1;
        public static float GetDCV_OffsetMLP2;
        public static float GetDCV_OffsetMLP3;
        public static float GetDCV_OffsetMLP4;
        public static float GetDCV_offsetMLP5;
        public static float GetDCV_OffsetMLP6;
        public static float GetDCV_Domain;
        public static float GetDCV_factor;

        public static int SetDCI_Offset;
        public static float SetDCI_Domain;
        public static float SetDCI_Select0;
        public static float SetDCI_Select1;
        public static float SetDCI_Select2;
        public static float SetDCI_factor;

        public static float GetDCI_Offset0d;
        public static float GetDCI_Offset1d;
        public static float GetDCI_Offset2;
        public static float GetDCI_Offset3d;
        public static float GetDCI_Offset4d;
        public static float GetDCI_Offset5d;
        public static float GetDCI_Offset6d;
        public static float GetDCI_Offset7d;
        public static float GetDCI_Domain;
        public static float GetDCI_Select0;
        public static float GetDCI_Select1;
        public static float GetDCI_select2;
        public static float GetDCI_Select3;
        public static float GetDCI_Select4;
        public static float GetDCI_Select5;
        public static float GetDCI_Select6;
        public static float GetDCI_Select7;
        public static float GetDCI_factor;

        public static int SetDigitalACV_Offset;
        public static float SetDigitalACV_Domain;
        public static float SetDigitalACV_factor;

        public static int GetDigitalACV_Offset;
        public static float GetDigitalACV_Domain;
        public static float GetDigitalACV_factor;

        public static float SetDigitalf_Min;
        public static float SetDigitalf_Max;
        public static int SetDigitalf_clock;
        public static float SetDigitalf_factor;

        public static float GetDigitalf_Min;
        public static float GetDigitalf_Max;
        public static int GetDigitalf_clock;
        public static float GetDigitalf_factor;

        public static int AnalogCommon_intOffset;
        public static float AnalogCommon_Domain;
        public static float AnalogCommon_factor;

        public static int Zeroset0;
        public static int Zeroset1;

        public static bool isEIS = false;
        public static bool isMSH = false;
        public static bool isCV = false;
        public static bool isIV0 = false;
        public static bool isChrono = false;
        public static bool isPulse = false;

        public static int Version;
        public static bool IsIVReceiverUnsigned;
        public static bool isDigitalEISReceiverUnsigned;

        public static float GalvanostatI_Select4;
        public static float GalvanostatI_Select5;
        public static float GalvanostatI_Select6;
        public static float GalvanostatI_Select7;

        public static float GalvanostatI_Select0;
        public static float GalvanostatI_Select1;
        public static float GalvanostatI_Select2;
        public static float GalvanostatI_Select3;

        public static float SetDCV_Select0;
        public static float SetDCV_Select1;

        public static float GetDCV_Select0;
        public static float GetDCV_Select1;

        public static float ACMultFactor_Select0;
        public static float ACMultFactor_Select1;

        public static float FilterC_V1;
        public static float FilterC_V2;

        public static float FilterC_I1;
        public static float FilterC_I2;

        public static float SetACVMaxS0;
        public static int SetACVResoloution;

        public static float GetACVMaxS0;
        public static int GetACVResoloution0;

        public static float DEISMeanPercent;
        public static int DEISNOverFlow0;

        public static float VTau_L;
        public static float VTau_H;
        public static float ITau_L0;
        public static float ITau_H0;
        public static float ITau_L1;
        public static float ITau_H1;
        public static float ITau_L2;
        public static float ITau_H2;
    }

    public static class FactoryDefault
    {
        public const bool active = true;
        public const bool isCVEnable = false;
        public const double V_OCT = 0.0;
        public const double IdealVoltage = 0.0;
        public const bool isOCP = false;
        public const bool isOCPAutoStart = true;
        public const int PGmode = 1;
        public const bool RelRef = true;
        public const int EqTime = 3;

        public const bool isDCVoltageConstant = true;
        public const double DCVoltageConstant = 0.0;
        public const double DCVoltageFrom = -0.5;
        public const double DCVoltageTo = 0.5;
        public const int DCVoltageStep = 100;

        public const bool isACAmpConstant = true;
        public const double ACAmpConstant = 0.02;
        public const double ACAmpFrom = 0.5;
        public const double ACAmpTo = 30.0;
        public const double ACAmpStep = 0.1;

        public const bool isACFrqConstant = false;
        public const double ACFrqConstant = 1000;
        public const double ACFrqFrom = 1.0;
        public const double ACFrqTo = 100000.0;
        public const int ACFrqNStep = 100;

        public const int IVCurrentRangeMode = 0;
        public const int IVVmlp = 0;
        public const int IVImlp = 0;
        public const double IVVoltageFrom = -0.5;
        public const double IVvoltageTo = 0.5;
        public const double ChronoEndTime = 1.0;
        public const int IVVoltageNStepp = 101;
        public const double CVStartpoint = 0;
        public const double PretreatmentVoltage = 0;
        public const bool isPreProcProbOn = true;
        public const bool isChBPostProcProbOff = true;
        public const int CVItteration = 2;
        public const int IVTimestep = 100;
        public const int Chrono_n = 38;
        public const double Chrono_Total_Period = 500;
        public const double Chrono_Pulse_Period = 250;
        public const double Chrono_Pulse_Level = -1.0;
        public const double Chrono_Pulse_Amplitude = 0.1;
        public const double Chrono_Level_Step = 0;
        public const double Chrono_Amplitude_step = 0.05;
        public const bool Chrono_isfast = false;

        public const int Chrono_nsteps = 1;

        public const double Chrono_t1 = 1;
        public const double Chrono_t2 = 1;
        public const double Chrono_t3 = 1;
        public const double Chrono_t4 = 1;
        public const double Chrono_t5 = 1;
        public const double Chrono_t6 = 1;
        public const double Chrono_t7 = 1;
        public const double Chrono_t8 = 1;
        public const double Chrono_t9 = 1;
        public const double Chrono_t10 = 1;

        public const double Chrono_v1 = 1;
        public const double Chrono_v2 = 1;
        public const double Chrono_v3 = 1;
        public const double Chrono_v4 = 1;
        public const double Chrono_v5 = 1;
        public const double Chrono_v6 = 1;
        public const double Chrono_v7 = 1;
        public const double Chrono_v8 = 1;
        public const double Chrono_v9 = 1;
        public const double Chrono_v10 = 1;

        public const double Chrono_dt = 10.0;

        public const bool Chrono_ocp1 = false;
        public const bool Chrono_ocp2 = false;
        public const bool Chrono_ocp3 = false;
        public const bool Chrono_ocp4 = false;
        public const bool Chrono_ocp5 = false;
        public const bool Chrono_ocp6 = false;
        public const bool Chrono_ocp7 = false;
        public const bool Chrono_ocp8 = false;
        public const bool Chrono_ocp9 = false;
        public const bool Chrono_ocp10 = false;

        public const int EISMode = 1;
        public const int EISfilterMode = 0;
        public const int EISAverageNumberL = 1;
        public const int EISAverageNumberH = 3;
        public const int EISOCMode = 1;
        public const int EISVoltageRangeMode = 1;
        public const int EISDCCurrentRangeModea = 0;
        public const int EISACRegulatorMode = 1;
        public const int EISACCurrentRangeMode = 0;
        public const int EISVmlpMax = 6;
        public const int EISImlpMax = 3;
        public const int IVVoltageRangeMode = 1;
        public const int IVChrono_VFilter = 3;
        public const int Pulse_VFilter = 3;

        public const int PulseVoltageRangeMode = 1;
        public const int PulseCurrentRangeMode = 0;
        public const int PulseVmlp = 0;
        public const int PulseImlpp = 0;
        public const int PulseReadingEdgemode = 1;
        public const int PulseVoltammetryMode = 0;

        public const int CheckPortTimeoutSec = 100;
        public const int PortTimeout = 5000;
        public const int ReScaleFactor = 100;

        //settings
        public const int SetDCV_Offset = 2047;
        public const float SetDCV_Domain = -5.0f;
        public const float SetDCV_factor = 1.0f;

        public const float GetDCV_OffsetMLP0 = 0;
        public const float GetDCV_OffsetMLP1 = 0;
        public const float GetDCV_OffsetMLP2 = 0;
        public const float GetDCV_OffsetMLP3 = 0;
        public const float GetDCV_OffsetMLP4 = 0;
        public const float GetDCV_offsetMLP5 = 0;
        public const float GetDCV_OffsetMLP6 = 0;
        public const float GetDCV_Domain = 11.0f;
        public const float GetDCV_factor = 1.0f;

        public const int SetDCI_Offset = 2047;
        public const float SetDCI_Domain = 1.0f;
        public const float SetDCI_Select0 = 1.0f;
        public const float SetDCI_Select1 = 100.0f;
        public const float SetDCI_Select2 = 10000.0f;
        public const float SetDCI_factor = 1.0f;

        public const float GetDCI_Offset0d = 0;
        public const float GetDCI_Offset1d = 0;
        public const float GetDCI_Offset2 = 0;
        public const float GetDCI_Offset3d = 0;
        public const float GetDCI_Offset4d = 0;
        public const float GetDCI_Offset5d = 0;
        public const float GetDCI_Offset6d = 0;
        public const float GetDCI_Offset7d = 0;
        public const float GetDCI_Domain = 1.0f;
        public const float GetDCI_Select0 = 1.0f;
        public const float GetDCI_Select1 = 10.0f;
        public const float GetDCI_select2 = 100.0f;
        public const float GetDCI_Select3 = 1000.0f;
        public const float GetDCI_Select4 = 10000.0f;
        public const float GetDCI_Select5 = 100000.0f;
        public const float GetDCI_Select6 = 1000000.0f;
        public const float GetDCI_Select7 = 10000000.0f;
        public const float GetDCI_factor = 1.0f;

        public const int SetDigitalACV_Offset = 0;
        public const float SetDigitalACV_Domain = 100.0f;  //in mV
        public const float SetDigitalACV_factor = 1.0f;

        public const int GetDigitalACV_Offset = 0;
        public const float GetDigitalACV_Domain = 100.0f;
        public const float GetDigitalACV_factor = 1.0f;

        public const float SetDigitalf_Min = 0.1192093f;
        public const float SetDigitalf_Max = 200000.0f;
        public const int SetDigitalf_clock = 32000000;
        public const float SetDigitalf_factor = 1.0f;

        public const float GetDigitalf_Min = 0.1192093f;
        public const float GetDigitalf_Max = 200000.0f;
        public const int GetDigitalf_clock = 32000000;
        public const float GetDigitalf_factor = 1.0f;

        public const int AnalogCommon_intOffset = 8388607;
        public const float AnalogCommon_Domain = 2.5f;
        public const float AnalogCommon_factor = 1.0f;
        //Domain set -5
        //zeroset 1500
        //get v domain 11
        //get I domain 1
        public const int Zeroset0 = 1500;
        public const int Zeroset1 = 1500;

        public const bool isEIS = false;
        public const bool isMSH = false;
        public const bool isCV = false;
        public const bool isIV0 = true;
        public const bool isChrono = false;
        public const bool isPulse = false;

        public const bool isIVReceiverUnsigned = false;
        public const bool isDigitalEISReceiverUnsigned = false;

        public const float GalvanostatI_Select4 = 2048.0f; //changed
        public const float GalvanostatI_Select5 = 2048.0f; //changed
        public const float GalvanostatI_Select6 = 2048.0f; //changed
        public const float GalvanostatI_Select7 = 2048.0f; //changed

        public const float GalvanostatI_Select0 = 2048.0f; //changed
        public const float GalvanostatI_Select1 = 2048.0f; //changed
        public const float GalvanostatI_Select2 = 2048.0f; //changed
        public const float GalvanostatI_Select3 = 2048.0f; //changed

        public const float SetDCV_Select0 = 1.0f;
        public const float SetDCV_Select1 = 0.2f;

        public const float GetDCV_Select0 = 1.0f;
        public const float GetDCV_Select1 = 0.2f;

        public const float ACMultFactor_Select0 = 5.0f;
        public const float ACMultFactor_Select1 = 5.0f;

        public const float FilterC_V1 = 1.0f;
        public const float FilterC_V2 = 100.0f;

        public const float FilterC_I1 = 1.0f;
        public const float FilterC_I2 = 100.0f;

        public const float SetACVMaxS0 = 1.0f;
        public const int SetACVResoloution = 255;

        public const float GetACVMaxS0 = 1;
        public const int GetACVResoloution0 = 255;

        public const float DEISMeanPercent = 35.0f;
        public const int DEISNOverFlow0 = 50;

        public const float VTau_L = 10.0f;
        public const float VTau_H = 0.0f;
        public const float ITau_L0 = 10.0f;
        public const float ITau_H0 = 0.0f;
        public const float ITau_L1 = 10.0f;
        public const float ITau_H1 = 0.0f;
        public const float ITau_L2 = 10.0f;
        public const float ITau_H2 = 0.0f;
    }
    
}
