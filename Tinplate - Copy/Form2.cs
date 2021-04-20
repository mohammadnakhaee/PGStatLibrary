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
    public partial class Form2 : Form
    {
        Form1 form1;
        public Form2(Form1 _parent)
        {
            form1 = _parent;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = form1.userData1.Tables["Data"];
        }
    }
}
