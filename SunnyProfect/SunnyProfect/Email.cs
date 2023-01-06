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
    public partial class Email : Form
    {
        public Email()
        {
            InitializeComponent();
        }
        bool suc = false;

        private void button1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text != string.Empty)
            {
                MessageBox.Show("Письмо готово");
                suc = true;
                Close();
            }
            else
                MessageBox.Show("Письмо не готово");
        }

        public string MessageText
        {
            get {
                return richTextBox1.Text;
            }
        }

        public bool Successful
        {
            get { return suc; }
        }

        public string ThemeText
        {
            get { return textBox1.Text; }
        }
    }
}
