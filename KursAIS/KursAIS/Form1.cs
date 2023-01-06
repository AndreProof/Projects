using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace KursAIS
{
    public partial class Form1 : Form
    {
        SqlConnection conn;
        SqlCommand comm;
        SqlDataReader reader;
        SqlDataAdapter adapter;
        SqlCommandBuilder commbuil;
        DataSet dat;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dat = new DataSet();
        }

        public void connect(string IP, string Port, string DB, string Login, string Pass)
        {
            conn = new SqlConnection("Data Source="+IP+","+Port+";Network Library=DBMSSOCN;Initial Catalog="+DB+";User ID="+Login+";Password="+Pass+";");
            try
            {
                conn.Open();
                comm = new SqlCommand("SELECT name FROM sys.tables WHERE type_desc=N'USER_TABLE';", conn);
                reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader[0].ToString());
                }
                reader.Close();
                comboBox1.SelectedItem = comboBox1.Items[0];
                comboBox1.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void downloadTable(string tableName)
        {
            try
            {
                dat.Clear();
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                adapter = new SqlDataAdapter("select * from " + tableName, conn);
                commbuil = new SqlCommandBuilder(adapter);
                adapter.Fill(dat, tableName);
                dataGridView1.DataSource = dat;
                dataGridView1.DataMember = tableName;
            }
            catch (SqlException ex)
            {
                    MessageBox.Show(ex.Message);
            }
        }

        public void backup()
        {
            try
            {
                if(backupTextbox.Text != string.Empty)
                    comm = new SqlCommand("BACKUP DATABASE ["+ DBtextbox.Text +"] TO DISK = '" + backupTextbox.Text + ".bak'", conn);
                reader = comm.ExecuteReader();
                reader.Close();
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void restore()
        {
            try
            {
                if (backupTextbox.Text != string.Empty)
                    comm = new SqlCommand("USE MASTER; RESTORE DATABASE [" + DBtextbox.Text + "] FROM DISK = '" + backupTextbox.Text + ".bak' WITH REPLACE; USE [" + DBtextbox.Text + "];", conn);
                reader = comm.ExecuteReader();
                reader.Close();
                downloadTable(comboBox1.SelectedItem.ToString());
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void update()
        {
            Validate();
            try
            {
                adapter.Update(dat, comboBox1.SelectedItem.ToString());
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                downloadTable(comboBox1.SelectedItem.ToString());
            }
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            connect(IPtextbox.Text, Porttextbox.Text,DBtextbox.Text, Logintextbox.Text, Passtextbox.Text);
        }

        private void backupBtn_Click(object sender, EventArgs e)
        {
            backup();
        }

        private void restoreBtn_Click(object sender, EventArgs e)
        {
            restore();
        }

        private void dataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            update();
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Данные введены неверно (неправильный тип или формат)");
        }
    }
}
