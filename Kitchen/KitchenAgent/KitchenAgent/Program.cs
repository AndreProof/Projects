using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace KitchenAgent
{
    class Program
    {
        static Socket socket;
        static int port = 1313;

        static void Main(string[] args)
        {
            Connect();
            Console.ReadKey();
        }

        static void Connect()
        {
            Console.WriteLine("Введите тип агента . . .");
            string str = Console.ReadLine();
            Console.WriteLine("Поиск сервера . . .");
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
                        Console.WriteLine("Найден сервер: "+ ipPoint.Address);
                        SendToServer(str);
                        Talking();
                        return;
                    }
                    catch { }
                }
                if (!connect)
                {
                    Console.WriteLine("Нет подключения к серверу");
                }
            }
            else
            {
                Console.WriteLine("Нет подключения к серверу");
            }
        }

        static void SendToServer(string message)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = socket.Send(msg);
        }

        static string RecieveMessage()
        {
            int bytesRec=0;
            byte[] bytes = new byte[1024];
            try
            {
                bytesRec = socket.Receive(bytes);
            }
            catch
            {
                return "Нет подключения к серверу";
            }
            string data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
            return data; 
        }

        static void Talking()
        {
            string data;
            while (true)
            {
                data = RecieveMessage();
                Console.WriteLine(data);
                if (data == "Нет подключения к серверу")
                    return;
                if (data.Contains("Указание"))
                {
                    string[] slt = data.Split(';');
                    Thread recieveThread = new Thread(() => WorkDone(slt[1],Convert.ToInt16(slt[2])));
                    recieveThread.Start();
                }
            }
        }

        static public void WorkDone(string name, int sec)
        {
            Thread.Sleep(1000 * sec);
            SendToServer("Указание;"+ name +";Выполнено");
        }
    }
}
