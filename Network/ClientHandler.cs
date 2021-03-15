using ImAgent.Entities;
using ImAgent.Helpers;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Serialization;

namespace ImAgent.Network
{
    class ClientHandler
    {
        private Server Server;
        public TcpClient Client;
        private NetworkStream Stream;

        public ClientHandler(TcpClient client, Server server)
        {
            Client = client;
            Server = server;
            Stream = client.GetStream();

            Listen();
        }

        private void Listen()
        {
            new Thread(() =>
            {
                try
                {
                    byte[] bytes = new byte[4096];
                    string data = null;

                    while (true)
                    {
                        try
                        {
                            StreamReader sr = new StreamReader(Stream);
                            data = sr.ReadLine();
                        }
                        catch (Exception e)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("ОШИБКА чтения данных из потока клиента " + Client.Client.RemoteEndPoint.ToString());
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                            Console.ResetColor();
                        }

                        if (!string.IsNullOrEmpty(data))
                        {
                            switch (data)
                            {
                                case "register":
                                    {
                                        Server.Register(this);

                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine("успешно зарегистрирован клиент: " + Client.Client.RemoteEndPoint.ToString());
                                        Console.ResetColor();

                                        break;
                                    }
                                case "unregister":
                                    {
                                        Server.Unregister(this);

                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine("успешно отменена регистрация клиента: " + Client.Client.RemoteEndPoint.ToString());
                                        Console.ResetColor();
                                        break;
                                    }
                                    
                                case "readytosendresult":
                                    
                                    GetJobResults();

                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("принят результат работы от клиента от клиента: " + Client.Client.RemoteEndPoint.ToString());
                                    Console.ResetColor();
                                    break;

                                default:
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine(string.Format("непонятные данные от клиента {0} : {1} ", Client.Client.RemoteEndPoint.ToString(), data.ToString()));
                                    Console.ResetColor();
                                    break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ОШИБКА!!!");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ResetColor();
                }

            }).Start();
        }

        public void SendClientJob(InquiryEntity ie)
        {
            var xmlSerializer = new XmlSerializer(typeof(InquiryEntity));
            if (Stream.CanWrite)
            {
                byte[] msg = System.Text.Encoding.UTF8.GetBytes("sendingjob\r\n");

                try
                {
                    Stream.Write(msg, 0, msg.Length);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ОШИБКА отправки подготовки к отправке задания клиенту");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ResetColor();
                }

                try
                {
                    StringWriter tw = new StringWriter();
                    xmlSerializer.Serialize(tw, ie);

                    byte[] msg1 = System.Text.Encoding.UTF8.GetBytes(tw.ToString() + "\r\nfff\r\n");
                    Stream.Write(msg1, 0, msg1.Length);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ОШИБКА отправки клиенту задания клиенту " + Client.Client.RemoteEndPoint.ToString());
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ResetColor();
                }
            }
        }

        public void SendClientJobString(string str)
        {
            if (Stream.CanWrite)
            {
                try
                {
                    byte[] msg = System.Text.Encoding.UTF8.GetBytes("sendingjob\r\n");
                    Stream.Write(msg, 0, msg.Length);
                    byte[] msg1 = System.Text.Encoding.UTF8.GetBytes(str + "\r\n");
                    Stream.Write(msg1, 0, msg1.Length);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ОШИБКА отправки клиенту задания клиенту " + Client.Client.RemoteEndPoint.ToString());
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ResetColor();
                }
            }
        }

        public void GetJobResults()
        {
            try
            {
                StreamReader sr = new StreamReader(Stream);
                string data = sr.ReadToEnd();

                string FileName = string.Format(
                                "{0}{1}-{2}-{3}.csv",
                                "",
                                Client.Client.RemoteEndPoint.ToString().Replace(":", "-"),
                                DateTime.Now.ToString().Replace(":", "-"),
                                "net");

                CSVFile.WriteCsvFile(FileName, data);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ОШИБКА получения результатов от клиента " + Client.Client.RemoteEndPoint.ToString());
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ResetColor();
            }
        }
    }
}
