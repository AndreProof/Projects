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
    public partial class EditCard : Form
    {
        public EditCard(OleDbConnection conn, int number)
        {
            InitializeComponent();
            myConnection = conn;
            num = number;
        }

        OleDbConnection myConnection;
        bool suc = false;
        int num;


        public bool Successful
        {
            get { return suc; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty && textBox2.Text != string.Empty && textBox3.Text != string.Empty && textBox4.Text != string.Empty && textBox5.Text != string.Empty && textBox6.Text != string.Empty)
            {
                try
                {
                    string query = "UPDATE [Card] SET [Number] = '" + textBox1.Text + "', [Name] = '" + textBox2.Text + "', [Location] = '" + textBox3.Text + "', [Phone] = '" + textBox4.Text + "', [Email] = '" + textBox5.Text + "', [Birthday] = '" + textBox6.Text + "' WHERE [Number] = '" + num + "'";
                    OleDbCommand command = new OleDbCommand(query, myConnection);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Карта успешно изменена");
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

        private void EditCard_Load(object sender, EventArgs e)
        {
            OleDbCommand command = new OleDbCommand("SELECT Name, Location, Phone, Email, Birthday FROM Card WHERE Number like '" + num + "'", myConnection);
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                textBox1.Text = num.ToString();
                textBox2.Text = reader[0].ToString();
                textBox3.Text = reader[1].ToString();
                textBox4.Text = reader[2].ToString();
                textBox5.Text = reader[3].ToString();
                textBox6.Text = reader[4].ToString();
            }
        }
    }
}
