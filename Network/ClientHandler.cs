﻿using ImAgent.Entities;
using ImAgent.Helpers;
using System;
using System.IO;
using System.Net.Sockets;
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
                            StreamReader sr = new StreamReader(Stream);
                            data = sr.ReadLine();
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
                byte[] msg = System.Text.Encoding.UTF8.GetBytes("sendingjob\r\n");

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

                    byte[] msg1 = System.Text.Encoding.UTF8.GetBytes(tw.ToString() + "\r\nfff\r\n");
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
                    byte[] msg = System.Text.Encoding.UTF8.GetBytes("sendingjob\r\n");
                    Stream.Write(msg, 0, msg.Length);
                    byte[] msg1 = System.Text.Encoding.UTF8.GetBytes(str + "\r\n");
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
                PrintConsoleMessage(MessageType.ERROR, $"ОШИБКА получения результатов от клиента {Client.Client.RemoteEndPoint}", e.Message, e.StackTrace);
            }
        }
    }
}
