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
    public partial class EditProduct : Form
    {
        public EditProduct(OleDbConnection conn, string name)
        {
            InitializeComponent();
            num = name;
            myConnection = conn;
        }
        OleDbConnection myConnection;
        bool suc = false;
        string num;


        public bool Successful
        {
            get { return suc; }
        }
        private void EditProduct_Load(object sender, EventArgs e)
        {
            OleDbCommand command = new OleDbCommand("SELECT Type, Price, Sale, Count FROM Products WHERE NameProd like '" + num + "'", myConnection);
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                textBox1.Text = num.ToString();
                textBox2.Text = reader[0].ToString();
                textBox3.Text = reader[1].ToString();
                textBox4.Text = reader[2].ToString();
                textBox5.Text = reader[3].ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty && textBox2.Text != string.Empty && textBox3.Text != string.Empty && textBox4.Text != string.Empty && textBox5.Text != string.Empty)
            {
                try
                {
                    string query = "UPDATE [Products] SET [NameProd] = '" + textBox1.Text + "', [Type] = '" + textBox2.Text + "', [Price] = '" + textBox3.Text + "', [Sale] = '" + textBox4.Text + "', [Count] = '" + textBox5.Text + "' WHERE [NameProd] = '"+num + "'";
                    OleDbCommand command = new OleDbCommand(query, myConnection);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Продукт успешно изменене");
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
    }
}
