using ImAgent.Entities;
using ImAgent.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

using static ImAgent.Helpers.Helper;

namespace ImAgent.Network
{
    internal class ClientHandler
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
                            using (NetworkStream ns = new NetworkStream(Client.Client))
                            {
                                using (StreamReader sr = new StreamReader(ns))
                                {
                                    data = sr.ReadLine();
                                } 
                            }
                        }
                        catch (Exception e)
                        {
                            PrintConsoleMessage(MessageType.ERROR, $"ОШИБКА чтения данных из потока клиента {Client.Client.RemoteEndPoint}", e.Message, e.StackTrace);
                        }

                        if (!string.IsNullOrEmpty(data))
                        {
                            switch (data)
                            {
                                case "register":
                                    {
                                        Server.Register(this);

                                        PrintConsoleMessage(MessageType.SUCCESS, $"успешно зарегистрирован клиент: {Client.Client.RemoteEndPoint}");

                                        break;
                                    }
                                case "unregister":
                                    {
                                        Server.Unregister(this);

                                        PrintConsoleMessage(MessageType.SUCCESS, $"успешно отменена регистрация клиента: {Client.Client.RemoteEndPoint}");
                                        break;
                                    }

                                case "readytosendresult":

                                    GetJobResults();

                                    PrintConsoleMessage(MessageType.SUCCESS, $"принят результат работы от клиента от клиента: {Client.Client.RemoteEndPoint}");
                                    break;

                                default:

                                    PrintConsoleMessage(MessageType.WARNING, $"непонятные данные от клиента {Client.Client.RemoteEndPoint} : {data} ");
                                    break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    PrintConsoleMessage(MessageType.ERROR, "ОШИБКА!!!", e.Message, e.StackTrace);
                }

            }).Start();
        }

        public void SendClientJob(InquiryEntity ie)
        {
            var xmlSerializer = new XmlSerializer(typeof(InquiryEntity));
            if (Stream.CanWrite)
            {
                byte[] msg = Encoding.UTF8.GetBytes("sendingjob\r\n");

                try
                {
                    Stream.Write(msg, 0, msg.Length);
                }
                catch (Exception e)
                {
                    PrintConsoleMessage(MessageType.ERROR, "ОШИБКА отправки подготовки к отправке задания клиенту", e.Message, e.StackTrace);
                }

                try
                {
                    StringWriter tw = new StringWriter();
                    xmlSerializer.Serialize(tw, ie);

                    byte[] msg1 = Encoding.UTF8.GetBytes(tw.ToString() + "\r\nfff\r\n");
                    Stream.Write(msg1, 0, msg1.Length);
                }
                catch (Exception e)
                {
                    PrintConsoleMessage(MessageType.ERROR, $"ОШИБКА отправки клиенту задания клиенту {Client.Client.RemoteEndPoint}", e.Message, e.StackTrace);
                }
            }
        }

        public void SendClientJobString(string str)
        {
            if (Stream.CanWrite)
            {
                try
                {
                    byte[] msg = Encoding.UTF8.GetBytes("sendingjob\r\n");
                    Stream.Write(msg, 0, msg.Length);
                    byte[] msg1 = Encoding.UTF8.GetBytes(str + "\r\n");
                    Stream.Write(msg1, 0, msg1.Length);
                }
                catch (Exception e)
                {
                    PrintConsoleMessage(MessageType.ERROR, $"ОШИБКА отправки клиенту задания клиенту {Client.Client.RemoteEndPoint}", e.Message, e.StackTrace);
                }
            }
        }

        public void GetJobResults()
        {
            try
            {
                string data = "";
                //todo сделать нормально
                NetworkStream ns = new NetworkStream(Client.Client);
                using (StreamReader sr = new StreamReader(ns))
                {
                    StringBuilder sb = new StringBuilder();

                    string s = string.Empty;
                    while (!(s = sr.ReadLine()).Equals("fff"))
                    {
                        sb.Append(s);
                    }

                    data = sb.ToString();
                }

                if (!string.IsNullOrEmpty(data))
                {
                    List<FileEntity> lfe = new List<FileEntity>();
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data)))
                    {
                        var xs = new XmlSerializer(typeof(List<FileEntity>));
                        lfe = (List<FileEntity>)xs.Deserialize(ms);
                    }

                    string FileName = string.Format(
                                    "{0}{1}-{2}-{3}.csv",
                                    $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\",
                                    Client.Client.RemoteEndPoint.ToString().Replace(":", "-"),
                                    DateTime.Now.ToString().Replace(":", "-"),
                                    "net");

                    CSVFile.WriteCsvFile(FileName, lfe);

                    PrintConsoleMessage(MessageType.SUCCESS, $"результат работы клиента {Client.Client.RemoteEndPoint} сохранен в файл {FileName}"); 
                }
                else
                {
                    PrintConsoleMessage(MessageType.SUCCESS, $"от клиента {Client.Client.RemoteEndPoint} пришли пустые данные");
                }
            }
            catch (Exception e)
            {
                PrintConsoleMessage(MessageType.ERROR, $"ОШИБКА получения результатов от клиента {Client.Client.RemoteEndPoint}", e.Message, e.StackTrace);
            }
        }
    }
}
