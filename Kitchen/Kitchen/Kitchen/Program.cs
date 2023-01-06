using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.OleDb;
using System.Net.NetworkInformation;

namespace Kitchen
{
    public class KitchenAgent
    {
        public Socket socket;
        public string type;
        public int ID;
        public bool work = false;

        public KitchenAgent(Socket soc, string tp, int id)
        {
            socket = soc;
            type = tp;
            ID = id;
        }
    }

    class Program
    {
        static int count = 0;

        static int port = 1313;
        static IPAddress ipAddress;
        static OleDbConnection myConnection;

        static List<KitchenAgent> kitchenagent = new List<KitchenAgent>();

        static void Main(string[] args)
        {
            ipAddress = IPAddress.Parse(GetLocalIPv4(NetworkInterfaceType.Wireless80211));
            string connectString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=DB.mdb;";
            myConnection = new OleDbConnection(connectString);
            myConnection.Open();

            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Попытка подключить сервер ...");
            try
            {
                socket.Bind(ipEndPoint);
                socket.Listen(1);
                Console.WriteLine("Сервер подключен\nЖдем подключения агентов\n...");
                while (true)
                {
                    Socket handler = socket.Accept();
                    Thread clientThread = new Thread(() => Talking(handler));
                    clientThread.Start();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void Talking(Socket handler)
        {
            byte[] bytes = new byte[1024];
            int bytesRec = handler.Receive(bytes);
            string type = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            int id = count++;

            Console.WriteLine("Подключен новый агент: "+ type);
            if (kitchenagent.Exists(x => x.type == type) && type != "Ввод")
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                Console.WriteLine("Запрещено подключение: " + type);
                return;
            }
            else
                kitchenagent.Add(new KitchenAgent(handler, type, id));

            if (type == "Ввод")
            {
                string data = " ";
                if (kitchenagent.Count != 0)
                {
                    foreach (KitchenAgent ka in kitchenagent)
                    {
                        if (ka.type != "Ввод")
                            data = data + " " + ka.type;
                    }
                }
                SendMessage(data, id);
            }
            else
            {
                if(kitchenagent.Exists(x=> x.type == "Ввод"))
                    foreach(KitchenAgent ka in kitchenagent)
                    {
                        if (ka.type == "Ввод")
                            SendMessage(type, ka.ID);
                    }
            }

            Thread recieveThread = new Thread(() => RecieveMessages(id));
            recieveThread.Start();
        }

        static void RecieveMessages(int id)
        {
            KitchenAgent ka = kitchenagent.Find(x => x.ID == id);
            byte[] bytes;
            int bytesRec = 0;
            string data, reciever, recieve;
            while (true)
            {
                bytes = new byte[1024];
                try
                {
                    bytesRec = ka.socket.Receive(bytes);
                }
                catch
                {
                    Console.WriteLine("Агент : "+ ka.type + " | ID : " + ka.ID + " отключился");
                    if(ka.type != "Ввод")
                    {
                        if (kitchenagent.Exists(x => x.type == "Ввод"))
                            foreach (KitchenAgent kak in kitchenagent)
                            {
                                if (kak.type == "Ввод")
                                    SendMessage(ka.type, kak.ID);
                            }
                    }
                    kitchenagent.RemoveAll(x => x.ID == ka.ID);
                    return;
                }
                data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                if (data.Contains("Указание"))
                {
                    string[] slt = data.Split(';');
                    kitchenagent.Find(x => x.type == slt[1]).work = false;
                }
                else
                {
                    Console.WriteLine(ka.type + " : " + data);
                    try
                    {
                        OleDbCommand command = new OleDbCommand("SELECT Send_message FROM Answers WHERE Recieve_message='" + data + "'", myConnection);
                        recieve = command.ExecuteScalar().ToString();

                        command.CommandText = "SELECT Reciever FROM Answers WHERE Recieve_message = '" + data + "'";
                        reciever = command.ExecuteScalar().ToString();

                        SendMessage(recieve, kitchenagent.Find(x => x.type == reciever && !x.work).ID);
                        kitchenagent.Find(x => x.type == reciever).work = true;

                        command.CommandText = "SELECT Time FROM Answers WHERE Recieve_message = '" + data + "'";
                        recieve = command.ExecuteScalar().ToString();
                        SendMessage("Указание;"+reciever+";"+recieve, kitchenagent.Find(x => x.type == reciever).ID);

                        foreach (KitchenAgent kak in kitchenagent)
                        {
                            if (kak.type == "Ввод")
                                SendMessage("Указание;" + reciever + ";" + recieve, kak.ID);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Невозможно выполнить действие : " + data);
                        SendMessage("Невозможно выполнить действие: " + data, kitchenagent.Find(x => x.type == "Ввод").ID);
                    }
                }
            }
        }

        static void SendMessage(string message, int id)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = kitchenagent.Find(x => x.ID == id).socket.Send(msg);
        }
    
        static string GetLocalIPv4(NetworkInterfaceType _type)
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
