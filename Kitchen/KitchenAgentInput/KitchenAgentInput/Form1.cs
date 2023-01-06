using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Speech.Recognition;
using System.Threading;

namespace KitchenAgentInput
{
    public partial class Form1 : Form
    {
        static Socket socket;
        int port = 1313;
        static Label l;
        static SpeechRecognitionEngine sre;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Connect();
        }

        void Connect()
        {
            IPAddress[] ipAddresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            if (ipAddresses.Length != 0)
            {
                bool connect = false;
                foreach (IPAddress ip in ipAddresses)
                {
                    try
                    {
                        IPEndPoint ipPoint = new IPEndPoint(ip, port);
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(ipPoint);
                        connect = true;
                        SendToServer("Ввод");
                        string data = RecieveMessage();
                        if (data.Contains("Кофеварка"))
                        {
                            groupBox1.Visible = true;
                            label5.Visible = false;
                        }
                        if (data.Contains("Чайник"))
                        {
                            groupBox2.Visible = true;
                            label6.Visible = false;
                        }
                        if (data.Contains("Тостер"))
                        {
                            groupBox3.Visible = true;
                            label7.Visible = false;
                        }
                        Thread recieveThread = new Thread(() => RecieveFromServer());
                        recieveThread.IsBackground = true;
                        recieveThread.Start();
                        return;
                    }
                    catch { }
                }
                if (!connect)
                {
                    MessageBox.Show("Нет подключения к серверу");
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Нет подключения к серверу");
                Close();
            }
        }

        static void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > 0.8)
            {
                l.Text = "Голосовой ввод: " + e.Result.Text;
                SendToServer(e.Result.Text);
            }
            sre.RecognizeAsyncCancel();
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("ru-ru");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
            GrammarBuilder gb = new GrammarBuilder();
            gb.Culture = ci;
            gb.Append(new Choices(new string[] { "Приготовить", "Заварить", "Готовить", "Включить" }));
            gb.Append(new Choices(new string[] { "кофе", "чай", "тостер", "тосты" }));
            Grammar g = new Grammar(gb);
            sre.LoadGrammar(g);
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        static void Restart()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 10000;
            timer.Start();
            timer.Tick += Timer_Tick;
            
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            l = label1;
            Restart();
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("ru-ru");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
            GrammarBuilder gb = new GrammarBuilder();
            gb.Culture = ci;
            gb.Append(new Choices(new string[] { "Приготовить", "Заварить", "Готовить", "Включить" }));
            gb.Append(new Choices(new string[] { "кофе", "чай", "тостер", "тосты" }));
            Grammar g = new Grammar(gb);
            sre.LoadGrammar(g);
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        static void SendToServer(string message)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = socket.Send(msg);
        }

        void RecieveFromServer()
        {
            string data;
            while (true)
            {
                data = RecieveMessage();
                if (data == "End")
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    return;
                }
                else
                if (data.Contains("Невозможно выполнить действие"))
                {
                    MessageBox.Show(data);
                }
                if (data == "Кофеварка")
                    groupBox1.Invoke(new Action(() => {
                        groupBox1.Visible = !groupBox1.Visible;
                        label5.Visible = !label5.Visible;
                    }));
                else if (data == "Чайник")
                    groupBox2.Invoke(new Action(() => {
                        groupBox2.Visible = !groupBox2.Visible;
                        label6.Visible = !label6.Visible;
                    }));
                else if (data == "Тостер")
                {
                    groupBox3.Invoke(new Action(() => {
                        groupBox3.Visible = !groupBox3.Visible;
                        label7.Visible = !label7.Visible;
                    }));
                }
                if (data.Contains("Указание"))
                {
                    string[] slt = data.Split(';');
                    if (slt[1] == "Кофеварка")
                    {
                        progressBar1.Invoke(new Action(() => {
                            progressBar1.Maximum = Convert.ToInt16(slt[2]);
                            timer1.Start();
                            label2.Text = "Статус: готовит";
                        }));
                    }
                    else if (slt[1] == "Чайник")
                    {
                        progressBar2.Invoke(new Action(() => {
                            progressBar2.Maximum = Convert.ToInt16(slt[2]);
                            timer2.Start();
                            label3.Text = "Статус: готовит";
                        }));   
                    }
                    else if (slt[1] == "Тостер")
                    {
                        progressBar3.Invoke(new Action(() => {
                            progressBar3.Maximum = Convert.ToInt16(slt[2]);
                            timer3.Start();
                            label4.Text = "Статус: готовит";
                        }));
                    }
                }
            }
        }

        static string RecieveMessage()
        {
            byte[] bytes = new byte[1024];
            int bytesRec;
            try
            {
                bytesRec = socket.Receive(bytes);
            }
            catch
            {
                MessageBox.Show("Сервер отключился");
                return "End";
            }
            string data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            return data;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
                SendToServer(comboBox1.SelectedItem.ToString());
            else
                MessageBox.Show("Сначала выберите действие");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)
                SendToServer(comboBox2.SelectedItem.ToString());
            else
                MessageBox.Show("Сначала выберите действие");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem != null)
                SendToServer(comboBox3.SelectedItem.ToString());
            else
                MessageBox.Show("Сначала выберите действие");
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value++;
            if (progressBar1.Value == progressBar1.Maximum)
            {
                progressBar1.Value = 0;
                timer1.Stop();
                label2.Text = "Статус: готов к работе";
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            progressBar2.Value++;
            if (progressBar2.Value == progressBar2.Maximum)
            {
                progressBar2.Value = 0;
                timer2.Stop();
                label3.Text = "Статус: готов к работе";
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            progressBar3.Value++;
            if (progressBar3.Value == progressBar3.Maximum)
            {
                progressBar3.Value = 0;
                timer3.Stop();
                label4.Text = "Статус: готов к работе";
            }
        }
    }
}
