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
    public partial class AddCard : Form
    {
        public AddCard(OleDbConnection conn)
        {
            InitializeComponent();
            myConnection = conn;
        }

        OleDbConnection myConnection;
        bool suc = false;

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty && textBox2.Text != string.Empty && textBox3.Text != string.Empty && textBox4.Text != string.Empty && textBox5.Text != string.Empty && textBox6.Text != string.Empty)
            {
                try
                {
                    string query = "INSERT INTO [Card] ([Number], [Name], [Location], [Phone], [Email], [Birthday]) VALUES ('" + textBox1.Text + "', '" + textBox2.Text + "', '" + textBox3.Text + "', '" + textBox4.Text + "','" + textBox5.Text + "','" + textBox6.Text + "')";
                    OleDbCommand command = new OleDbCommand(query, myConnection);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Карта успешно добавлена");
                    suc = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Не все поля заполнены");
            }
        }

        public bool Successful
        {
            get { return suc; }
        }
    }
}
