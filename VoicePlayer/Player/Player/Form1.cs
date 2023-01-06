using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Speech.Recognition;
using WMPLib;
using System.Globalization;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;

namespace Player
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string prevcommand;

        WindowsMediaPlayer mus = new WindowsMediaPlayer();
        bool play; 
        GrammarBuilder gb; 
        CultureInfo ci; 
        SpeechRecognitionEngine sre; 
        Grammar g; 
        Socket socket, client; 
        int port = 1313; 
        IPAddress ipAddress;

        public void Command(string comm)
        {
                if (comm != "Ещё") 
                    prevcommand = comm;
                switch(prevcommand)
                {
                    case "Громче": 
                        trackBar2.Value += 20;
                        if (checkBox1.Checked == true) 
                        {
                            mus.settings.volume = 0;
                            label4.Text = "Volume: mute"; 
                        }
                        else 
                        {
                            mus.settings.volume = trackBar2.Value;
                            label4.Text = "Volume: " + trackBar2.Value; 
                        }
                        break;
                    case "Тише":
                        trackBar2.Value -= 20;
                        if (checkBox1.Checked == true)
                        {
                            mus.settings.volume = 0;
                            label4.Text = "Volume: mute";
                        }
                        else
                        {
                            mus.settings.volume = trackBar2.Value;
                            label4.Text = "Volume: " + trackBar2.Value;
                        }
                        break;
                    case "Звук": 
                        checkBox1.Checked = !checkBox1.Checked; 
                        break;
                    case "Стоп": 
                        if (!play) 
                            PausePlayBtn_Click(this, new EventArgs()); 
                        break;
                    case "Играть": 
                        if (play)
                            PausePlayBtn_Click(this, new EventArgs());
                        break;
                    case "Следующий": 
                        listBox1.SelectedIndex += 1; 
                                                     
                        break;
                    case "Предыдущий": 
                        listBox1.SelectedIndex -= 1; 
                        break;
                    case "Открыть": 
                        openBtn_Click(this, new EventArgs());
                        break;
                    default:
                        {
                            string[] str = prevcommand.Split('|');
                            for (int i = 0; i < listBox1.Items.Count; i++) 
                                if (prevcommand.Contains(listBox1.Items[i].ToString().ToLower().Replace("  ", " ").Split(' ')[1]))
                                    listBox1.SelectedIndex = i;
                        }
                        break;
                }
        }

        private void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > 0.8)
            {
                if (e.Result.Text != "Ещё")
                    prevcommand = e.Result.Text;
                switch (prevcommand)
                {
                    case "Громче":
                        trackBar2.Value += 20;
                        if (checkBox1.Checked == true)
                        {
                            mus.settings.volume = 0;
                            label4.Text = "Volume: mute";
                        }
                        else
                        {
                            mus.settings.volume = trackBar2.Value;
                            label4.Text = "Volume: " + trackBar2.Value;
                        }
                        break;
                    case "Тише":
                        trackBar2.Value -= 20;
                        if (checkBox1.Checked == true)
                        {
                            mus.settings.volume = 0;
                            label4.Text = "Volume: mute";
                        }
                        else
                        {
                            mus.settings.volume = trackBar2.Value;
                            label4.Text = "Volume: " + trackBar2.Value;
                        }
                        break;
                    case "Звук":
                        checkBox1.Checked = !checkBox1.Checked;
                        break;
                    case "Стоп":
                        if (!play)
                            PausePlayBtn_Click(this, e);
                        break;
                    case "Играть":
                        if(play)
                            PausePlayBtn_Click(this, e);
                        break;
                    case "Следующий":
                        listBox1.SelectedIndex +=1;
                        break;
                    case "Предыдущий":
                        listBox1.SelectedIndex -= 1;
                        break;
                    case "Открыть":
                        openBtn_Click(this, e);
                        break;
                }
            }
            
            ci = new CultureInfo("ru-ru");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
            gb = new GrammarBuilder();
            gb.Culture = ci;
            gb.Append(new Choices(new string[] { "Громче", "Тише", "Следующий", "Предыдущий", "Стоп", "Играть", "Убрать звук", "Ещё", "Открыть" }));
            g = new Grammar(gb);
            sre.LoadGrammar(g);
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void openBtn_Click(object sender, EventArgs e)
        {
            IWMPPlaylist playlist = mus.playlistCollection.newPlaylist("myplaylist");
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = " MP3 Files|*.mp3|WAV Files|*.wav|M4A Files|*.m4a"; 
            ofd.Multiselect = true;
            if ((ofd.ShowDialog() == DialogResult.OK) && (ofd.FileName != string.Empty)) 
            { 
                listBox1.Items.Clear(); 
                foreach(string file in ofd.FileNames)
                {
                    playlist.appendItem(mus.newMedia(file));
                    listBox1.Items.Add((listBox1.Items.Count + 1) + ".  " + file.Split('\\').Last().Split('.')[0]); 
                }
                mus.settings.volume = trackBar2.Value;
                mus.currentPlaylist = playlist; 
                mus.controls.play(); 
                listBox1.SelectedIndex = 0; 
                mus.MediaChange += Mus_MediaChange;
                timer1.Enabled = true;
                timer1.Interval = 1000; 
                label5.Text = mus.currentMedia.sourceURL;
                button2.Text = "Paused"; 
            }
        }

        private void Mus_MediaChange(object Item)
        {
            label5.Text = mus.currentMedia.sourceURL;
        }

        private void PausePlayBtn_Click(object sender, EventArgs e)
        {
            play = !play;
            if (play) 
            {
                mus.controls.pause();
                button2.Text = "Play";
            }
            if (!play)
            {
                mus.controls.play();
                button2.Text = "Paused";
            }
        }

        private void PlaySong_Scroll(object sender, EventArgs e)
        {
            mus.controls.currentPosition = trackBar1.Value;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            trackBar1.Maximum = Convert.ToInt32(mus.currentMedia.duration);
            trackBar1.Value = Convert.ToInt32(mus.controls.currentPosition);
            int s = (int)mus.currentMedia.duration;
            int h = s / 3600; 
            int m = (s - (h * 3600)) / 60;
            s = s - (h * 3600 + m * 60);
            label3.Text = String.Format("{0:D}:{1:D2}:{2:D2}", h, m, s);
            s = (int)mus.controls.currentPosition;
            h = s / 3600;
            m = (s - (h * 3600)) / 60;
            s = s - (h * 3600 + m * 60);
            label2.Text = String.Format("{0:D}:{1:D2}:{2:D2}", h, m, s);
        }

        private void Volume_Scroll(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                mus.settings.volume = 0;
                label4.Text = "Volume: mute";
            }
            else
            {
                mus.settings.volume = trackBar2.Value;
                label4.Text = "Volume: " + trackBar2.Value;
            }
        }

        private void Form2_FormClosing(object sender, EventArgs e)
        {
            mus.controls.stop();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                mus.settings.volume = 0;
                label4.Text = "Volume: mute";
            }
            else
            {
                mus.settings.volume = trackBar2.Value;
                label4.Text = "Volume: " + trackBar2.Value;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            mus.controls.playItem(mus.currentPlaylist.Item[listBox1.SelectedIndex]);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            ci = new CultureInfo("ru-ru");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized); 
            gb = new GrammarBuilder();
            gb.Culture = ci; 
            gb.Append(new Choices(new string[] { "Громче", "Тише", "Следующий", "Предыдущий", "Стоп", "Играть", "Звук", "Ещё", "Открыть"}));
            g = new Grammar(gb);
            sre.LoadGrammar(g);
            sre.RecognizeAsync(RecognizeMode.Multiple);
            Thread work = new Thread(RecieveMessage);
            work.IsBackground = true;
            work.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.ShowDialog();
        }

        public void RecieveMessage()
        {
            ipAddress = IPAddress.Parse(GetLocalIPv4(NetworkInterfaceType.Wireless80211));
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipEndPoint);
            while (true)
            {
                socket.Listen(1);
                client = socket.Accept();
                byte[] bytes;
                int bytesRec = 0;
                string data;
                while (true) 
                {
                    bytes = new byte[1024];
                    try
                    {
                        bytesRec = client.Receive(bytes);
                        data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        if (data == "")
                            break;
                        Invoke(new Action(() =>
                        {
                            Command(data);
                        }));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                client.Close();
            }
        }

        public string GetLocalIPv4(NetworkInterfaceType _type)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }
    }
}
