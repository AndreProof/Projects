using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SunnyProfect
{
    public partial class AuthForm : Form
    {
        bool suc = false;
        OleDbConnection myConnection;

        public AuthForm(OleDbConnection myConn)
        {
            InitializeComponent();
            try
            {
                myConnection = myConn;
            }
            catch
            {
                MessageBox.Show("База данных не подключена");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty && textBox2.Text != string.Empty)
            {
                OleDbCommand command = new OleDbCommand("SELECT COUNT(*) from Workers where Login like '" + textBox1.Text + "' AND Password like '" + textBox2.Text + "'", myConnection);
                if ((int)command.ExecuteScalar() > 0)
                {
                    suc = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Пользователя с такой комбинацией логин/пароль не существует");
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля");
            }
        }

        public bool Successful
        {
            get { return suc; }
        }

        public string WorkerName
        {
            get { return textBox1.Text; }
        }
    }
}
