using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tinplate
{
    public partial class AdminPassForm : Form
    {
        Form1 myparent;
        public AdminPassForm(Form1 parent)
        {
            myparent = parent;
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Return) || e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (textBox1.Text == "sharifSolar0001")
                {
                    myparent.IsAdmin = true;
                    this.Dispose();
                }
                else
                {
                    label3.Visible = true;
                }
            }
        }
    }
}
