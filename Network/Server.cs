using ImAgent.Entities;
using ImAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static ImAgent.Helpers.Helper;

namespace ImAgent.Network
{
    internal class Server
    {
        public int Port { get; set; }
        public IPAddress Address { get; set; }
        public InquiryEntity CurrentJob { get; set; }
        public Dictionary<int, ClientHandler> Clients { get; private set; }

        private TcpListener server;

        public Server(int port, IPAddress address)
        {
            Port = port;
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Clients = new Dictionary<int, ClientHandler>();
        }

        public void Start()
        {
            try
            {
                server = new TcpListener(Address, Port);
                server.Start();

                PrintConsoleMessage(MessageType.Success, "сервер успешно запущен");

                Listen();
                ProcessServerCommands();
            }
            catch (SocketException e)
            {
                PrintConsoleMessage(MessageType.Error, "ОШИБКА запуска сервера", e.Message, e.StackTrace);
            }
        }

        private void Listen()
        {
            new Thread(() =>
                {
                    while (true)
                    {
                        try
                        {
                            TcpClient client = server.AcceptTcpClient();
                            ClientHandler handler = new ClientHandler(client, this);

                            Console.WriteLine($"входящий {client.Client.RemoteEndPoint}");
                        }
                        catch (SocketException e)
                        {
                            PrintConsoleMessage(MessageType.Error, "ОШИБКА сети", e.Message, e.StackTrace);
                        }
                    }
                }
                ).Start();
        }

        public void Stop()
        {
            try
            {
                server.Stop();

                PrintConsoleMessage(MessageType.Success, "сервер успешно остановлен");
            }
            catch (Exception e)
            {
                PrintConsoleMessage(MessageType.Error, "ОШИБКА остановки сервера", e.Message, e.StackTrace);
            }
        }

        public bool CheckPort(int port)
        {
            try
            {
                foreach (TcpConnectionInformation tcpi in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections())
                {
                    if (tcpi.LocalEndPoint.Port == port)
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                PrintConsoleMessage(MessageType.Error, "ОШИБКА проверки доступности порта", e.Message, e.StackTrace);

                return false;
            }

            return true;
        }

        public void Register(ClientHandler client)
        {
            Clients.Add(Clients.Count + 1, client);
        }

        public void Unregister(ClientHandler client)
        {
            foreach (var c in Clients)
            {
                if (c.Value.Equals(client))
                    Clients.Remove(c.Key);
            }
        }

        public void Unregister(int Number)
        {
            Clients.Remove(Number);
        }

        public void UnregisterAll()
        {
            Clients.Clear();
        }

        public void SendBroadcastJob(InquiryEntity ie)
        {
            foreach (var client in Clients)
            {
                client.Value.SendClientJob(ie);
            }
        }

        public void SendBroadcastJobString(string s)
        {
            foreach (var client in Clients)
            {
                client.Value.SendClientJobString(s);
            }
        }

        private void ProcessServerCommands()
        {
            Console.WriteLine("Сервер готов к приему команд(help для полного списка):\r\n");

            while (true)
            {
                string input = Console.ReadLine();

                switch (input.Split(' ')[0])
                {
                    case "job":
                        {
                            /*InquiryEntity ie = new InquiryEntity();
                            ie.Method = "wmi";
                            ie.TaskFile = input.Split(' ')[2];
                            ie.Tasks = CSVFile.ReadCsvFile(ie.TaskFile);

                            foreach (var c in Clients)
                            {
                                if (c.Key.Equals(int.Parse(input.Split(' ')[1])))
                                    c.Value.SendClientJob(ie);
                            }*/

                            foreach (var c in Clients)
                            {
                                if (c.Key.Equals(int.Parse(input.Split(' ')[1])))
                                    c.Value.SendClientJobString(input.Split(' ')[2]);
                            }

                            break;
                        }
                    case "batchjob":
                        {
                            /*InquiryEntity ie = new InquiryEntity();
                            ie.Method = "wmi";
                            ie.TaskFile = input.Split(' ')[1];
                            ie.Tasks = CSVFile.ReadCsvFile(ie.TaskFile);

                            SendBroadcastJob(ie);*/
                            SendBroadcastJobString(input.Split(' ')[1]);
                            break;
                        }
                    case "stop":
                        {
                            Stop();
                            break;
                        }
                    case "list":
                        {
                            if (Clients.Count == 0)
                            {
                                Console.WriteLine("подключенные клиенты отсутствуют");
                            }
                            else
                            {
                                foreach (var item in Clients)
                                {
                                    Console.WriteLine($"{item.Key}. {item.Value.Client.Client.RemoteEndPoint}");
                                }

                                Console.WriteLine("");
                            }


                            break;
                        }
                    case "unregister":
                        {
                            Unregister(int.Parse(input.Split(' ')[1]));
                            break;
                        }
                    case "unregisterall":
                        {
                            UnregisterAll();
                            break;
                        }
                    case "help":
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("\r\nДоступны следующие команды\r\n");
                            sb.Append("job - передать задание кокретноmy клиенту(Номер [пробел] путь к файлу с заданием)\r\n");
                            sb.Append("batchjob - передать задание всем подключенным клиентам\r\n");
                            sb.Append("stop - остановить сервер\r\n");
                            sb.Append("list - список подключенных клиентов\r\n");
                            sb.Append("unregister - отменить регистрацию конкретного клиента (номер)\r\n");
                            sb.Append("unregisterall - отменить регистрацию всех клиентов\r\n");
                            sb.Append("help - справка по командам\r\n");
                            sb.Append("exit - завершение приложения\r\n");

                            Console.WriteLine(sb.ToString());

                            break;
                        }
                    case "exit":
                        {
                            Environment.Exit(0);
                            break;
                        }

                    default: Console.WriteLine("команда не распознана"); break;
                }
            }
        }
    }
}
