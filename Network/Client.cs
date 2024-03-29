﻿using ImAgent.Entities;
using ImAgent.Helpers;
using ImAgent.Module;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

using static ImAgent.Helpers.Helper;

namespace ImAgent.Network
{
    internal class Client
    {
        private TcpClient client;

        public int Port { get; set; }
        public IPAddress Address { get; set; }

        public Client(int? port, IPAddress address)
        {
            Port = port ?? throw new ArgumentNullException(nameof(port));
            Address = address ?? throw new ArgumentNullException(nameof(address));

            try
            {
                client = new TcpClient();
            }
            catch (Exception e)
            {
                PrintConsoleMessage(MessageType.Error, "ОШИБКА сервер недоступен", e.Message, e.StackTrace);
            }
        }

        public void Start()
        {
            Connect();
            Register();
            Work();
        }

        public void Stop()
        {
            UnRegister();
            Disconnect();
        }

        private void Work()
        {
            new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        //string result = "null";
                        List<FileEntity> res = new List<FileEntity>();
                        string data = null;
                        byte[] msg;

                        try
                        {
                            using (StreamReader sr = new StreamReader(new NetworkStream(client.Client)))
                            {
                                data = sr.ReadLine();
                            }
                        }
                        catch (Exception e)
                        {
                            PrintConsoleMessage(MessageType.Error, "ОШИБКА чтения данных от сервера", e.Message, e.StackTrace);
                        }

                        if (!string.IsNullOrEmpty(data))
                        {
                            InquiryEntity ie = new InquiryEntity();

                            switch (data)
                            {
                                case "sendingjob":

                                    ie = ParseCommand();

                                    if (ie != null)
                                    {
                                        res = ExecuteCommand(ie);
                                    }

                                    if (res.Count > 0)
                                    {
                                        //TODO GOOD~~~!!!!!
                                        try
                                        {
                                            using (NetworkStream ns = new NetworkStream(client.Client))
                                            {
                                                msg = Encoding.UTF8.GetBytes("readytosendresult\r\n");
                                                ns.Write(msg, 0, msg.Length);

                                                var xs = new XmlSerializer(typeof(List<FileEntity>));
                                                xs.Serialize(ns, res);

                                                msg = Encoding.UTF8.GetBytes("\r\nfff\r\n"); //чтобы хоть как-то остановить на другой стороне прием.
                                                ns.Write(msg, 0, msg.Length);


                                                PrintConsoleMessage(MessageType.Success, "Результат задания передан на сервер");
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            PrintConsoleMessage(MessageType.Error, "ОШИБКА отправки результатов на сервер", e.Message, e.StackTrace);

                                            string FileName = string.Format(
                                            "{0}{1}-{2}-{3}.csv",
                                            (!string.IsNullOrEmpty(ie.OutputFolder)) ? ie.OutputFolder : "",
                                            ie.Tasks[0].Name,
                                            DateTime.Now.ToString().Replace(":", "-"),
                                            ie.Method);

                                            try
                                            {
                                                CSVFile.WriteCsvFile(FileName, res);
                                            }
                                            catch (Exception ex)
                                            {
                                                PrintConsoleMessage(MessageType.Error, $"ОШИБКА сохранения результата в файл {FileName}", ex.Message, ex.StackTrace);
                                            }

                                            PrintConsoleMessage(MessageType.Warning, "данные сохранены в файл " + FileName);
                                        }
                                    }
                                    break;
                                default:

                                    PrintConsoleMessage(MessageType.Warning, $"приняты непонятные данные от сервера: {data}");
                                    break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    PrintConsoleMessage(MessageType.Error, "ОШИБКА!!!", e.Message, e.StackTrace);
                }

            }).Start();
        }

        /*private string ExecuteCommand(InquiryEntity ie)
        {
            IList<FileEntity> res = new List<FileEntity>();
            string result = string.Empty;

            if (ie.Tasks != null)
            {
                IFinder finder = new WMI(true, ie);
                //todo for many tasks
                res = finder.Search(ie.Tasks[0]);
            }

            if (res.Count > 0) result = CSVFile.WriteCsvString(res);

            return res; // result;
        }*/

        private List<FileEntity> ExecuteCommand(InquiryEntity ie)
        {
            List<FileEntity> res = new List<FileEntity>();
            //string result = string.Empty;

            if (ie.Tasks != null)
            {
                IFinder finder = new WMI(true, ie);
                //todo for many tasks
                res = finder.Search(ie.Tasks[0]);
            }

            //if (res.Count > 0) result = CSVFile.WriteCsvString(res);

            return res; // result;
        }

        private InquiryEntity ParseCommand()
        {
            InquiryEntity result = new InquiryEntity();

            try
            {
                //todo сделать нормально
                using (StreamReader sr = new StreamReader(new NetworkStream(client.Client)))
                {
                    string a = sr.ReadLine();
                    string[] b = a.Split(',');

                    TaskEntity t = new TaskEntity
                    {
                        Name = b[0],
                        Path = b[1],
                        //todo fail with "C:\Program Files (x86)\" path
                        Type = b[2]
                    };

                    List<TaskEntity> l = new List<TaskEntity>();
                    l.Add(t);

                    result.Method = "wmi";
                    result.Tasks = l;
                }
            }
            catch (Exception e)
            {
                PrintConsoleMessage(MessageType.Error, "ОШИБКА обработки задания", e.Message, e.StackTrace);
            }

            return result;
        }

        private void Register()
        {
            try
            {
                using (NetworkStream ns = new NetworkStream(client.Client))
                {
                    byte[] msg = Encoding.UTF8.GetBytes("register\r\n");
                    ns.Write(msg, 0, msg.Length);
                }

                PrintConsoleMessage(MessageType.Success, "клиент успешно зарегистрировался на сервере");
            }
            catch (Exception e)
            {
                PrintConsoleMessage(MessageType.Error, "ОШИБКА регистрации на сервере", e.Message, e.StackTrace);
            }
        }

        private void UnRegister()
        {
            try
            {
                using (NetworkStream ns = new NetworkStream(client.Client))
                {
                    byte[] msg = Encoding.UTF8.GetBytes("unregister\r\n");
                    ns.Write(msg, 0, msg.Length);
                }

                PrintConsoleMessage(MessageType.Success, "клиент отменил регистрацию на сервере");
            }
            catch (Exception e)
            {
                PrintConsoleMessage(MessageType.Error, "ОШИБКА отмены регистрации на сервере", e.Message, e.StackTrace);
            }
        }

        private void Connect()
        {
            try
            {
                if (!client.Connected)
                {
                    client.Connect(Address, Port);

                    PrintConsoleMessage(MessageType.Success, "клиент подключился к серверу");
                }
            }
            catch (Exception e)
            {
                PrintConsoleMessage(MessageType.Error, "ОШИБКА подключения к серверу", e.Message, e.StackTrace);
            }
        }

        private void Disconnect()
        {
            try
            {
                if (client.Connected)
                {
                    client.Close();

                    PrintConsoleMessage(MessageType.Success, "клиент отключился от сервера");
                }
            }
            catch (ObjectDisposedException e)
            {
                PrintConsoleMessage(MessageType.Error, "ОШИБКА отключения от сервера", e.Message, e.StackTrace);
            }
        }

    }
}
