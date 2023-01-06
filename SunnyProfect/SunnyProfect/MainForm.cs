using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;
using Word = Microsoft.Office.Interop.Word;

namespace SunnyProfect
{
    public partial class MainForm : Form
    {
        Socket server, client;
        Process proc;
        Thread telegramThread;
        string connectString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=DB1.accdb;";
        OleDbConnection myConnection;
        OleDbDataAdapter adapter, adapter2;
        DataSet dat, dat2;
        OleDbCommandBuilder builder, builder2;

        public MainForm()
        {
            InitializeComponent();
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 1313);
            server = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipEndPoint);
        }

        public void RecieveTelegramMessage()
        {
            server.Listen(1);
            client = server.Accept();
            byte[] bytes;
            int bytesRec;
            string data;
            while (true)
            {
                bytes = new byte[1024];
                try
                {
                    bytesRec = client.Receive(bytes);
                }
                catch
                {
                    return;
                }
                data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                listBox1.Invoke(new Action(() =>
                {
                    listBox1.Items.Add(data);
                }));
            }
        }

        public void SendTelegramMessage(long ID, string data)
        {
            if (!proc.HasExited)
            {
                byte[] msg = Encoding.UTF8.GetBytes(ID + "|" + data);
                int bytesSent = client.Send(msg);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            telegramThread = new Thread(RecieveTelegramMessage);
            telegramThread.IsBackground = true;
            telegramThread.Start();
            FileInfo fileInf = new FileInfo(@"../../TelegramClient/TelegramClient.exe");
            proc = new Process();
            proc.StartInfo.FileName = fileInf.FullName;
            proc.Exited += Proc_Exited;
            proc.Start();
        }

        private void Proc_Exited(object sender, EventArgs e)
        {
            if (telegramThread.IsAlive)
                telegramThread.Abort();
            client.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(telegramThread.IsAlive)
                telegramThread.Abort();
            if(!proc.HasExited)
                proc.Kill();
            client.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            myConnection = new OleDbConnection(connectString);
            myConnection.Open();
            AuthForm auth = new AuthForm(myConnection);
            auth.ShowDialog();
            if (!auth.Successful)
                Close();
            Text = "Продавец: " + auth.WorkerName;
            dat = new DataSet();
            LoadCardInfo();
            dat2 = new DataSet();
            LoadProductInfo();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                if (DialogResult.Yes != MessageBox.Show("В телеграм боте есть неотвеченные сообщения\nПри закрытии они будут потеряны",
                                                        "Уверены, что хотите закрыть программу?", 
                                                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                    e.Cancel = true;
            }
            if (proc != null)
                if(!proc.HasExited)
                    proc.Kill();
            myConnection.Close();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(listBox1.SelectedIndex >= 0)
            {
                long str = Convert.ToInt64(listBox1.SelectedItem.ToString().Split('|')[0].Split(' ')[1]);
                AnsForm ans = new AnsForm();
                ans.ShowDialog();
                if (ans.Successful)
                {
                    SendTelegramMessage(str, ans.MessageText);
                    listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                }
            }
        }


        
        //управление таблицей карт
        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddCard add = new AddCard(myConnection);
            add.ShowDialog();
            if (add.Successful)
                LoadCardInfo();
        }

        private void рассылкаСообщенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Email mail = new Email();
            mail.ShowDialog();
            if (mail.Successful)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    try
                    {
                        MailAddress from = new MailAddress("projectSunny@yandex.ru", "SunnyProject");
                        MailAddress to = new MailAddress(dataGridView1.Rows[i].Cells[4].Value.ToString());
                        MailMessage m = new MailMessage(from, to);
                        m.Subject = mail.ThemeText;
                        m.Body = mail.MessageText;
                        m.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 587);
                        smtp.Credentials = new NetworkCredential("projectSunny@yandex.ru", "qazxswedc18");
                        smtp.EnableSsl = true;
                        smtp.Send(m);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                MessageBox.Show("Сообщение разослано");
            }
        }

        private void удалитьtoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                string queryString = "DELETE FROM Card WHERE Number='" + dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value+"'";
                OleDbCommand command = new OleDbCommand(queryString, myConnection);
                command.ExecuteNonQuery();
                LoadCardInfo();
                MessageBox.Show("Карта успешно удалена");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditCard edit = new EditCard(myConnection, Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value));
            edit.ShowDialog();
            if (edit.Successful)
                LoadCardInfo();
        }

        private void сброситьПоискToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadCardInfo();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex >= 0)
            {
                
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1.Rows[i].Selected = false;
                }
                if (textBox1.Text != string.Empty)
                {
                    int ind = 0;
                    for(int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        if(dataGridView1.Columns[i].HeaderText == comboBox1.SelectedItem.ToString())
                        {
                            ind = i;
                            break;
                        }
                    }
                    dat.Clear();
                    dataGridView1.DataSource = null;
                    dataGridView1.Rows.Clear();
                    string queryString = "SELECT Number, Name, Location, Phone, Email, Birthday FROM Card WHERE "+comboBox1.SelectedItem.ToString()+" like '%"+ textBox1.Text +"%' ";
                    adapter = new OleDbDataAdapter(queryString, myConnection);
                    builder = new OleDbCommandBuilder(adapter);
                    adapter.Fill(dat, "Card");
                    dataGridView1.DataSource = dat;
                    dataGridView1.DataMember = "Card";
                }
            }
        }

        public void LoadCardInfo()
        {
            dat.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            string queryString = "SELECT Number, Name, Location, Phone, Email, Birthday FROM Card";
            adapter = new OleDbDataAdapter(queryString, myConnection);
            builder = new OleDbCommandBuilder(adapter);
            adapter.Fill(dat, "Card");
            dataGridView1.DataSource = dat;
            dataGridView1.DataMember = "Card";
            comboBox1.Items.Clear();
            for(int i = 0; i< dataGridView1.Columns.Count; i++)
            {
                comboBox1.Items.Add(dataGridView1.Columns[i].HeaderText);
            }
        }




        //управление таблицей товаров
        public void LoadProductInfo()
        {
            dat2.Clear();
            dataGridView2.DataSource = null;
            dataGridView2.Rows.Clear();
            string queryString = "SELECT NameProd, Type, Price, Sale, Count FROM Products";
            adapter2 = new OleDbDataAdapter(queryString, myConnection);
            builder2 = new OleDbCommandBuilder(adapter2);
            adapter2.Fill(dat2, "Products");
            dataGridView2.DataSource = dat2;
            dataGridView2.DataMember = "Products";
            comboBox2.Items.Clear();
            for (int i = 0; i < dataGridView2.Columns.Count; i++)
            {
                comboBox2.Items.Add(dataGridView2.Columns[i].HeaderText);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddProduct add = new AddProduct(myConnection);
            add.ShowDialog();
            if (add.Successful)
                LoadProductInfo();
        }


        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            EditProduct edit = new EditProduct(myConnection, dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells[0].Value.ToString());
            edit.ShowDialog();
            if (edit.Successful)
                LoadProductInfo();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            try
            {
                string queryString = "DELETE FROM Products WHERE NameProd='" + dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells[0].Value + "'";
                OleDbCommand command = new OleDbCommand(queryString, myConnection);
                command.ExecuteNonQuery();
                LoadProductInfo();
                MessageBox.Show("Карта успешно удалена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            LoadProductInfo();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex >= 0)
            {

                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    dataGridView2.Rows[i].Selected = false;
                }
                if (textBox2.Text != string.Empty)
                {
                    int ind = 0;
                    for (int i = 0; i < dataGridView2.ColumnCount; i++)
                    {
                        if (dataGridView2.Columns[i].HeaderText == comboBox2.SelectedItem.ToString())
                        {
                            ind = i;
                            break;
                        }
                    }
                    dat2.Clear();
                    dataGridView2.DataSource = null;
                    dataGridView2.Rows.Clear();
                    string queryString = "SELECT NameProd, Type, Price, Sale, Count FROM Products WHERE " + comboBox2.SelectedItem.ToString() + " like '%" + textBox2.Text + "%' ";
                    adapter2 = new OleDbDataAdapter(queryString, myConnection);
                    builder2 = new OleDbCommandBuilder(adapter2);
                    adapter2.Fill(dat2, "Products");
                    dataGridView2.DataSource = dat2;
                    dataGridView2.DataMember = "Products";
                }
            }
        }



        //работа с заказами

        private void добавитьВЗаказToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt16(dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells[4].Value) > 0)
            {
                int index = dataGridView3.RowCount;
                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    if (dataGridView3.Rows[i].Cells[0].Value.ToString() == dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells[0].Value.ToString())
                        index = i;
                }
                if (index == dataGridView3.RowCount)
                {
                    dataGridView3.Rows.Add();
                    dataGridView3.Rows[dataGridView3.Rows.Count - 1].Cells[0].Value = dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells[0].Value.ToString();
                    dataGridView3.Rows[dataGridView3.Rows.Count - 1].Cells[1].Value = dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells[1].Value.ToString();
                    dataGridView3.Rows[dataGridView3.Rows.Count - 1].Cells[2].Value = dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells[2].Value.ToString();
                    dataGridView3.Rows[dataGridView3.Rows.Count - 1].Cells[3].Value = dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells[3].Value.ToString();
                    dataGridView3.Rows[dataGridView3.Rows.Count - 1].Cells[4].Value = 1;
                }
                else
                {
                    dataGridView3.Rows[index].Cells[4].Value = Convert.ToInt16(dataGridView3.Rows[dataGridView3.Rows.Count - 1].Cells[4].Value) + 1;
                }
                string query = "UPDATE [Products] SET [Count] = '" + (Convert.ToInt16(dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells[4].Value) - 1).ToString() + "' WHERE NameProd = '" + dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells[0].Value.ToString() + "'";
                OleDbCommand command = new OleDbCommand(query, myConnection);
                command.ExecuteNonQuery();
                LoadProductInfo();
                int price, sale, count, allprice = 0;
                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    price = Convert.ToInt16(dataGridView3.Rows[i].Cells[2].Value);
                    sale = Convert.ToInt16(dataGridView3.Rows[i].Cells[3].Value);
                    count = Convert.ToInt16(dataGridView3.Rows[i].Cells[4].Value);
                    allprice += (price - price * sale / 100) * count;
                }
                label3.Text = "Цена: " + allprice;
            }
            else
            {
                MessageBox.Show("Товара больше нет");
            }
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            int index = 0;
            string name = dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells[0].Value.ToString();
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                if (dataGridView2.Rows[i].Cells[0].Value.ToString() == name)
                {
                    index = i;
                    break;
                }
            }

            if (Convert.ToInt16(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells[4].Value) == 1)
            {
                dataGridView3.Rows.RemoveAt(dataGridView3.CurrentCell.RowIndex);
            }
            else
            {
                dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells[4].Value = Convert.ToInt16(dataGridView3.Rows[dataGridView3.CurrentCell.RowIndex].Cells[4].Value) - 1;
            }
            string query = "UPDATE [Products] SET [Count] = '" + (Convert.ToInt16(dataGridView2.Rows[index].Cells[4].Value) + 1).ToString() + "' WHERE NameProd = '" + name + "'";
            OleDbCommand command = new OleDbCommand(query, myConnection);
            command.ExecuteNonQuery();
            LoadProductInfo();
            int price, sale, count, allprice = 0;
            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                price = Convert.ToInt16(dataGridView3.Rows[i].Cells[2].Value);
                sale = Convert.ToInt16(dataGridView3.Rows[i].Cells[3].Value);
                count = Convert.ToInt16(dataGridView3.Rows[i].Cells[4].Value);
                allprice += (price - price * sale / 100) * count;
            }
            label3.Text = "Цена: " + allprice;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Word.Document doc;
            Word.Application app = new Word.Application();
            FileInfo fileInf = new FileInfo(@"../../Zakaz.docx");
            string source = fileInf.FullName;
            doc = app.Documents.Add(source);
            doc.Activate();
            Word.Bookmarks wBookmarks = doc.Bookmarks;
            Word.Range wRange;
            int i = 0;
            string sellprod = "";
            for(int j = 0; j < dataGridView3.RowCount; j++)
            {
                sellprod = sellprod + "Название: " + dataGridView3.Rows[j].Cells[0].Value.ToString() + " | Цена: " + dataGridView3.Rows[j].Cells[2].Value.ToString() + " | Количество: " + dataGridView3.Rows[j].Cells[4].Value.ToString() + "\n";
            }
            string[] data = new string[3] { Text.Split(':')[1], label3.Text.Split(':')[1], sellprod};
            foreach (Word.Bookmark mark in wBookmarks)
            {
                wRange = mark.Range;
                wRange.Text = data[i];
                i++;
            }
            doc.Close();
            doc = null;
        }
    }
}
