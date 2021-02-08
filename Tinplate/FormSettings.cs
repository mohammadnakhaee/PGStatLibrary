using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;
using PGStatLibrary;

namespace Tinplate
{
    public partial class FormSettings : Form
    {
        //public static SettingsData Settings = new SettingsData();
        public static bool isAdvanced = false;
        public FormSettings()
        {
            InitializeComponent();

            if (isAdvanced)
            {
                groupBox30.Enabled = true;
            }
            else
            {
                groupBox30.Enabled = false;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            //Settings
            Form1.pg.SaveSettings("./settings.bin");
            //MessageBox.Show("micro save");
            Form1.pg.MicroSaveSettings("./settings.bin");
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            Form1.pg.LoadSettings("./settings.bin");
            FillForm();
        }

        private void DigitTextChange(ref double digit, TextBox tb)
        {
            try
            {
                digit = Convert.ToDouble(tb.Text);
            }
            catch
            {
                tb.Text = digit.ToString();
                MessageBox.Show("Please type digit only ...");
            }
        }

        private void DigitTextChange(ref float digit, TextBox tb)
        {
            try
            {
                digit = (float)Convert.ToDouble(tb.Text);
            }
            catch
            {
                tb.Text = digit.ToString();
                MessageBox.Show("Please type digit only ...");
            }
        }

        private void IntDigitTextChange(ref int digit, TextBox tb)
        {
            try
            {
                digit = Convert.ToInt32(tb.Text);
            }
            catch
            {
                tb.Text = digit.ToString();
                MessageBox.Show("Please type integer only ...");
            }
        }

        private void SetDCV_Offset_TextChanged(object sender, EventArgs e)
        {
            IntDigitTextChange(ref Settings.SetDCV_Offset, SetDCV_Offset);
        }

        private void SetDCV_Domain_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetDCV_Domain, SetDCV_Domain);
        }

        private void SetDCV_factor_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetDCV_factor, SetDCV_factor);
        }

        private void GetDCV_Offset_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCV_OffsetMLP0, GetDCV_Offset);
        }

        private void GetDCV_Domain_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCV_Domain, GetDCV_Domain);
        }

        private void GetDCV_factor_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCV_factor, GetDCV_factor);
        }

        private void SetDCI_Offset_TextChanged(object sender, EventArgs e)
        {
            IntDigitTextChange(ref Settings.SetDCI_Offset, SetDCI_Offset);
        }

        private void SetDCI_Domain_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetDCI_Domain, SetDCI_Domain);
        }

        private void SetDCI_factor_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetDCI_factor, SetDCI_factor);
        }

        private void GetDCI_Offset_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Offset0d, GetDCI_Offset0);
        }

        private void GetDCI_Domain_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Domain, GetDCI_Domain);
        }

        private void GetDCI_factor_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_factor, GetDCI_factor);
        }

        private void SetDigitalACV_Offset_TextChanged(object sender, EventArgs e)
        {
            IntDigitTextChange(ref Settings.SetDigitalACV_Offset, SetDigitalACV_Offset);
        }

        private void SetDigitalACV_Domain_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetDigitalACV_Domain, SetDigitalACV_Domain);
        }

        private void SetDigitalACV_factor_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetDigitalACV_factor, SetDigitalACV_factor);
        }

        private void GetDigitalACV_Offset_TextChanged(object sender, EventArgs e)
        {
            IntDigitTextChange(ref Settings.GetDigitalACV_Offset, GetDigitalACV_Offset);
        }

        private void GetDigitalACV_Domain_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDigitalACV_Domain, GetDigitalACV_Domain);
        }

        private void GetDigitalACV_factor_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDigitalACV_factor, GetDigitalACV_factor);
        }

        private void SetDigitalf_Min_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetDigitalf_Min, SetDigitalf_Min);
        }

        private void SetDigitalf_Max_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetDigitalf_Max, SetDigitalf_Max);
        }

        private void SetDigitalf_clock_TextChanged(object sender, EventArgs e)
        {
            IntDigitTextChange(ref Settings.SetDigitalf_clock, SetDigitalf_clock);
        }

        private void SetDigitalf_factor_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetDigitalf_factor, SetDigitalf_factor);
        }

        private void GetDigitalf_Min_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDigitalf_Min, GetDigitalf_Min);
        }

        private void GetDigitalf_Max_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDigitalf_Max, GetDigitalf_Max);
        }

        private void GetDigitalf_clock_TextChanged(object sender, EventArgs e)
        {
            IntDigitTextChange(ref Settings.GetDigitalf_clock, GetDigitalf_clock);
        }

        private void GetDigitalf_factor_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDigitalf_factor, GetDigitalf_factor);
        }

        private void AnalogCommon_Offset_TextChanged(object sender, EventArgs e)
        {
            IntDigitTextChange(ref Settings.AnalogCommon_intOffset, AnalogCommon_Offset);
        }

        private void AnalogCommon_Domain_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.AnalogCommon_Domain, AnalogCommon_Domain);
        }

        private void AnalogCommon_factor_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.AnalogCommon_factor, AnalogCommon_factor);
        }

        private void FormSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1.isFormSetting = false;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Form1.isFormSetting = false;
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1.pg.ResetFactory();
            FillForm();
        }

        private void FillForm()
        {
            isIVUnsigned.Checked = Settings.IsIVReceiverUnsigned;
            isEISUnsigned.Checked = Settings.isDigitalEISReceiverUnsigned;

            SetDCV_Offset.Text = Settings.SetDCV_Offset.ToString();
            SetDCV_Domain.Text = Settings.SetDCV_Domain.ToString();
            SetDCV_factor.Text = Settings.SetDCV_factor.ToString();

            GetDCV_Offset.Text = Settings.GetDCV_OffsetMLP0.ToString();
            GetDCV_OffsetMLP1.Text = Settings.GetDCV_OffsetMLP1.ToString();
            GetDCV_OffsetMLP2.Text = Settings.GetDCV_OffsetMLP2.ToString();
            GetDCV_OffsetMLP3.Text = Settings.GetDCV_OffsetMLP3.ToString();
            GetDCV_OffsetMLP4.Text = Settings.GetDCV_OffsetMLP4.ToString();
            GetDCV_OffsetMLP5.Text = Settings.GetDCV_offsetMLP5.ToString();
            GetDCV_OffsetMLP6.Text = Settings.GetDCV_OffsetMLP6.ToString();

            GetDCV_Domain.Text = Settings.GetDCV_Domain.ToString();
            GetDCV_factor.Text = Settings.GetDCV_factor.ToString();

            SetDCI_Offset.Text = Settings.SetDCI_Offset.ToString();
            SetDCI_Domain.Text = Settings.SetDCI_Domain.ToString();
            SetDCI_Select0.Text = Settings.SetDCI_Select0.ToString();
            SetDCI_Select1.Text = Settings.SetDCI_Select1.ToString();
            SetDCI_Select2.Text = Settings.SetDCI_Select2.ToString();
            SetDCI_factor.Text = Settings.SetDCI_factor.ToString();

            GetDCI_Offset0.Text = Settings.GetDCI_Offset0d.ToString();
            GetDCI_Offset1.Text = Settings.GetDCI_Offset1d.ToString();
            GetDCI_Offset2.Text = Settings.GetDCI_Offset2.ToString();
            GetDCI_Offset3.Text = Settings.GetDCI_Offset3d.ToString();
            GetDCI_Offset4.Text = Settings.GetDCI_Offset4d.ToString();
            GetDCI_Offset5.Text = Settings.GetDCI_Offset5d.ToString();
            GetDCI_Offset6.Text = Settings.GetDCI_Offset6d.ToString();
            GetDCI_Offset7.Text = Settings.GetDCI_Offset7d.ToString();
            GetDCI_Domain.Text = Settings.GetDCI_Domain.ToString();
            GetDCI_Select0.Text = Settings.GetDCI_Select0.ToString();
            GetDCI_Select1.Text = Settings.GetDCI_Select1.ToString();
            GetDCI_Select2.Text = Settings.GetDCI_select2.ToString();
            GetDCI_Select3.Text = Settings.GetDCI_Select3.ToString();
            GetDCI_Select4.Text = Settings.GetDCI_Select4.ToString();
            GetDCI_Select5.Text = Settings.GetDCI_Select5.ToString();
            GetDCI_Select6.Text = Settings.GetDCI_Select6.ToString();
            GetDCI_Select7.Text = Settings.GetDCI_Select7.ToString();
            GetDCI_factor.Text = Settings.GetDCI_factor.ToString();

            SetDigitalACV_Offset.Text = Settings.SetDigitalACV_Offset.ToString();
            SetDigitalACV_Domain.Text = Settings.SetDigitalACV_Domain.ToString();
            SetDigitalACV_factor.Text = Settings.SetDigitalACV_factor.ToString();

            GetDigitalACV_Offset.Text = Settings.GetDigitalACV_Offset.ToString();
            GetDigitalACV_Domain.Text = Settings.GetDigitalACV_Domain.ToString();
            GetDigitalACV_factor.Text = Settings.GetDigitalACV_factor.ToString();

            SetDigitalf_Min.Text = Settings.SetDigitalf_Min.ToString();
            SetDigitalf_Max.Text = Settings.SetDigitalf_Max.ToString();
            SetDigitalf_clock.Text = Settings.SetDigitalf_clock.ToString();
            SetDigitalf_factor.Text = Settings.SetDigitalf_factor.ToString();

            GetDigitalf_Min.Text = Settings.GetDigitalf_Min.ToString();
            GetDigitalf_Max.Text = Settings.GetDigitalf_Max.ToString();
            GetDigitalf_clock.Text = Settings.GetDigitalf_clock.ToString();
            GetDigitalf_factor.Text = Settings.GetDigitalf_factor.ToString();

            AnalogCommon_Offset.Text = Settings.AnalogCommon_intOffset.ToString();
            AnalogCommon_Domain.Text = Settings.AnalogCommon_Domain.ToString();
            AnalogCommon_factor.Text = Settings.AnalogCommon_factor.ToString();

            ZeroSetTB.Text = Settings.Zeroset0.ToString();
            ZeroSetTB1.Text = Settings.Zeroset1.ToString();

            isEIS.Checked = Settings.isEIS;
            isMSH.Checked = Settings.isMSH;
            isChrono.Checked = Settings.isChrono;
            isIV.Checked = Settings.isIV0;
            isCV.Checked = Settings.isCV;
            isPulse.Checked = Settings.isPulse;

            SetACI_Select0.Text = Settings.GalvanostatI_Select4.ToString();
            SetACI_Select1.Text = Settings.GalvanostatI_Select5.ToString();
            SetACI_Select2.Text = Settings.GalvanostatI_Select6.ToString();
            SetACI_Select3.Text = Settings.GalvanostatI_Select7.ToString();

            GetACI_Select0.Text = Settings.GalvanostatI_Select0.ToString();
            GetACI_Select1.Text = Settings.GalvanostatI_Select1.ToString();
            GetACI_Select2.Text = Settings.GalvanostatI_Select2.ToString();
            GetACI_Select3.Text = Settings.GalvanostatI_Select3.ToString();

            SetDCV_Select0.Text = Settings.SetDCV_Select0.ToString();
            SetDCV_Select1.Text = Settings.SetDCV_Select1.ToString();

            GetDCV_Select0.Text = Settings.GetDCV_Select0.ToString();
            GetDCV_Select1.Text = Settings.GetDCV_Select1.ToString();

            ACMultFactor_Select0.Text = Settings.ACMultFactor_Select0.ToString();
            ACMultFactor_Select1.Text = Settings.ACMultFactor_Select1.ToString();

            FilterC_V1.Text = Settings.FilterC_V1.ToString();
            FilterC_V2.Text = Settings.FilterC_V2.ToString();
            FilterC_I1.Text = Settings.FilterC_I1.ToString();
            FilterC_I2.Text = Settings.FilterC_I2.ToString();

            SetACVMaxS0.Text = Settings.SetACVMaxS0.ToString();
            SetACVResoloution.Text = Settings.SetACVResoloution.ToString();

            GetACVMaxS0.Text = Settings.GetACVMaxS0.ToString();
            GetACVResoloution.Text = Settings.GetACVResoloution0.ToString();

            TBVTau_L.Text = Settings.VTau_L.ToString();
            TBVTau_H.Text = Settings.VTau_H.ToString();
            TBITau_L.Text = Settings.ITau_L0.ToString();
            TBITau_H.Text = Settings.ITau_H0.ToString();
            TBITau_L1.Text = Settings.ITau_L1.ToString();
            TBITau_H1.Text = Settings.ITau_H1.ToString();
            TBITau_L2.Text = Settings.ITau_L2.ToString();
            TBITau_H2.Text = Settings.ITau_H2.ToString();
        }

        private void SetDCI_Select0_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetDCI_Select0, SetDCI_Select0);
        }

        private void SetDCI_Select1_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetDCI_Select1, SetDCI_Select1);
        }

        private void SetDCI_Select2_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetDCI_Select2, SetDCI_Select2);
        }


        private void GetDCI_Select0_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Select0, GetDCI_Select0);
        }

        private void GetDCI_Select1_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Select1, GetDCI_Select1);
        }

        private void GetDCI_Select2_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_select2, GetDCI_Select2);
        }

        private void SetACI_Select0_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GalvanostatI_Select4, SetACI_Select0);
        }

        private void SetACI_Select1_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GalvanostatI_Select5, SetACI_Select1);
        }

        private void SetACI_Select2_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GalvanostatI_Select6, SetACI_Select2);
        }

        private void SetACI_Select3_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GalvanostatI_Select7, SetACI_Select3);
        }

        private void GetACI_Select0_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GalvanostatI_Select0, GetACI_Select0);
        }

        private void GetACI_Select1_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GalvanostatI_Select1, GetACI_Select1);
        }

        private void GetACI_Select2_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GalvanostatI_Select2, GetACI_Select2);
        }

        private void GetACI_Select3_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GalvanostatI_Select3, GetACI_Select3);
        }

        private void SetDCV_Select0_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetDCV_Select0, SetDCV_Select0);
        }

        private void SetDCV_Select1_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetDCV_Select1, SetDCV_Select1);
        }

        private void GetDCV_Select0_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCV_Select0, GetDCV_Select0);
        }

        private void GetDCV_Select1_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCV_Select1, GetDCV_Select1);
        }

        private void SetACVMaxS0_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.SetACVMaxS0, SetACVMaxS0);
        }

        private void SetACVResoloution_TextChanged(object sender, EventArgs e)
        {
            IntDigitTextChange(ref Settings.SetACVResoloution, SetACVResoloution);
        }

        private void GetACVMaxS0_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetACVMaxS0, GetACVMaxS0);
        }

        private void GetACVResoloution_TextChanged(object sender, EventArgs e)
        {
            IntDigitTextChange(ref Settings.GetACVResoloution0, GetACVResoloution);
        }

        private void isIVUnsigned_CheckedChanged(object sender, EventArgs e)
        {
            Settings.IsIVReceiverUnsigned = isIVUnsigned.Checked;
        }

        private void isEISUnsigned_CheckedChanged(object sender, EventArgs e)
        {
            Settings.isDigitalEISReceiverUnsigned = isEISUnsigned.Checked;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Offset1d, GetDCI_Offset1);
        }

        private void GetDCI_Offset2_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Offset2, GetDCI_Offset2);
        }

        private void ZeroSetTB_TextChanged(object sender, EventArgs e)
        {
            IntDigitTextChange(ref Settings.Zeroset0, ZeroSetTB);
        }

        private void isEIS_CheckedChanged(object sender, EventArgs e)
        {
            Settings.isEIS = isEIS.Checked;
        }

        private void isMSH_CheckedChanged(object sender, EventArgs e)
        {
            Settings.isMSH = isMSH.Checked;
        }

        private void isCV_CheckedChanged(object sender, EventArgs e)
        {
            Settings.isChrono = isChrono.Checked;
        }

        private void isIV_CheckedChanged(object sender, EventArgs e)
        {
            Settings.isIV0 = isIV.Checked;
        }

        private void ZeroSetTB1_TextChanged(object sender, EventArgs e)
        {
            IntDigitTextChange(ref Settings.Zeroset1, ZeroSetTB1);
        }

        private void TBVTau_L_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.VTau_L, TBVTau_L);
        }

        private void TBVTau_H_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.VTau_H, TBVTau_H);
        }

        private void TBITau_L_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.ITau_L0, TBITau_L);
        }

        private void TBITau_H_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.ITau_H0, TBITau_H);
        }

        private void TBITau_L1_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.ITau_L1, TBITau_L1);
        }

        private void TBITau_H1_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.ITau_H1, TBITau_H1);
        }

        private void TBITau_L2_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.ITau_L2, TBITau_L2);
        }

        private void TBITau_H2_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.ITau_H2, TBITau_H2);
        }

        private void GetDCI_Select3_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Select3, GetDCI_Select3);
        }

        private void GetDCI_Select4_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Select4, GetDCI_Select4);
        }

        private void GetDCI_Select5_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Select5, GetDCI_Select5);
        }

        private void GetDCI_Select6_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Select6, GetDCI_Select6);
        }

        private void GetDCI_Select7_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Select7, GetDCI_Select7);
        }

        private void GetDCI_Offset3_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Offset3d, GetDCI_Offset3);
        }

        private void GetDCI_Offset4_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Offset4d, GetDCI_Offset4);
        }

        private void GetDCI_Offset5_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Offset5d, GetDCI_Offset5);
        }

        private void GetDCI_Offset6_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Offset6d, GetDCI_Offset6);
        }

        private void GetDCI_Offset7_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCI_Offset7d, GetDCI_Offset7);
        }

        private void GetDCV_OffsetMLP1_TextChange(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCV_OffsetMLP1, GetDCV_OffsetMLP1);
        }

        private void GetDCV_OffsetMLP2_TextChange(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCV_OffsetMLP2, GetDCV_OffsetMLP2);
        }

        private void GetDCV_OffsetMLP3_TextChange(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCV_OffsetMLP3, GetDCV_OffsetMLP3);
        }

        private void GetDCV_OffsetMLP4_TextChange(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCV_OffsetMLP4, GetDCV_OffsetMLP4);
        }

        private void GetDCV_OffsetMLP5_TextChange(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCV_offsetMLP5, GetDCV_OffsetMLP5);
        }

        private void GetDCV_OffsetMLP6_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.GetDCV_OffsetMLP6, GetDCV_OffsetMLP6);
        }

        private void ACMultFactor_Select0_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.ACMultFactor_Select0, ACMultFactor_Select0);
        }

        private void ACMultFactor_Select1_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.ACMultFactor_Select1, ACMultFactor_Select1);
        }

        private void FilterC_V1_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.FilterC_V1, FilterC_V1);
        }

        private void FilterC_V2_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.FilterC_V2, FilterC_V2);
        }

        private void FilterC_I1_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.FilterC_I1, FilterC_I1);
        }

        private void FilterC_I2_TextChanged(object sender, EventArgs e)
        {
            DigitTextChange(ref Settings.FilterC_I2, FilterC_I2);
        }

        private void FormSettings_Shown(object sender, EventArgs e)
        {

            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.isCV = isCV.Checked;
        }

        private void ChBIsPulse_CheckedChanged(object sender, EventArgs e)
        {
            Settings.isPulse = isPulse.Checked;
        }
    }
}
