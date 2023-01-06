using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SunnyProfect
{
    public partial class AnsForm : Form
    {
        public AnsForm()
        {
            InitializeComponent();
        }
        bool suc = false;

        private void button1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text != string.Empty)
            {
                MessageBox.Show("Ответ отправлен");
                suc = true;
                Close();
            }
            else
                MessageBox.Show("Ответ не отправлен");
        }

        public string MessageText{
            get{ return richTextBox1.Text; }
        }

        public bool Successful
        {
            get { return suc; }
        }
    }
}
