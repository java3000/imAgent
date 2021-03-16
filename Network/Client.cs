using ImAgent.Entities;
using ImAgent.Helpers;
using ImAgent.Module;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
                PrintConsoleMessage(MessageType.ERROR, "ОШИБКА сервер недоступен", e.Message, e.StackTrace);
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
                        string result = "null";
                        string data = null;

                        try
                        {
                            using (StreamReader sr = new StreamReader(new NetworkStream(client.Client)))
                            {
                                data = sr.ReadLine();
                            }
                        }
                        catch (Exception e)
                        {
                            PrintConsoleMessage(MessageType.ERROR, "ОШИБКА чтения данных от сервера", e.Message, e.StackTrace);
                        }

                        if (!string.IsNullOrEmpty(data))
                        {
                            InquiryEntity ie = new InquiryEntity();

                            switch (data)
                            {
                                case "sendingjob":

                                    using (NetworkStream ns = new NetworkStream(client.Client))
                                    {
                                        ie = ParseCommand(ns);
                                    }

                                    if (ie != null)
                                    {
                                        result = ExecuteCommand(ie);
                                    }

                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        //TODO GOOD~~~!!!!!
                                        byte[] msg = Encoding.UTF8.GetBytes("readytosendresult\r\n");

                                        try
                                        {
                                            using (NetworkStream ns = new NetworkStream(client.Client))
                                            {
                                                ns.Write(msg, 0, msg.Length);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            PrintConsoleMessage(MessageType.ERROR, "ОШИБКА отправки команды серверу", e.Message, e.StackTrace);
                                        }

                                        //TODO GOOD~~~!!!!!
                                        msg = Encoding.UTF8.GetBytes(result);

                                        try
                                        {
                                            using (NetworkStream ns = new NetworkStream(client.Client))
                                            {
                                                ns.Write(msg, 0, msg.Length);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            PrintConsoleMessage(MessageType.ERROR, "ОШИБКА отправки результатов на сервер", e.Message, e.StackTrace);

                                            string FileName = string.Format(
                                            "{0}{1}-{2}-{3}.csv",
                                            (!string.IsNullOrEmpty(ie.OutputFolder)) ? ie.OutputFolder : "",
                                            ie.Tasks[0].Name,
                                            DateTime.Now.ToString().Replace(":", "-"),
                                            ie.Method);

                                            try
                                            {
                                                CSVFile.WriteCsvFile(FileName, result);
                                            }
                                            catch (Exception ex)
                                            {
                                                PrintConsoleMessage(MessageType.ERROR, $"ОШИБКА сохранения результата в файл {FileName}", ex.Message, ex.StackTrace);
                                            }

                                            PrintConsoleMessage(MessageType.WARNING, "данные сохранены в файл " + FileName);
                                        }
                                    }
                                    break;
                                default:

                                    PrintConsoleMessage(MessageType.WARNING, $"приняты непонятные данные от сервера: {data}");
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

        private string ExecuteCommand(InquiryEntity ie)
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

            return result;
        }

        private InquiryEntity ParseCommand(Stream data)
        {
            InquiryEntity result = new InquiryEntity();

            try
            {
                //todo сделать нормально
                using (StreamReader sr = new StreamReader(new NetworkStream(client.Client)))
                {
                    string a = sr.ReadLine();
                    string[] b = a.Split(',');

                    TaskEntity t = new TaskEntity();
                    t.Name = b[0];
                    t.Path = b[1];
                    t.Type = b[2];

                    List<TaskEntity> l = new List<TaskEntity>();
                    l.Add(t);

                    result.Method = "wmi";
                    result.Tasks = l;
                }
            }
            catch (Exception e)
            {
                PrintConsoleMessage(MessageType.ERROR, "ОШИБКА обработки задания", e.Message, e.StackTrace);
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

                PrintConsoleMessage(MessageType.SUCCESS, "клиент успешно зарегистрировался на сервере");
            }
            catch (Exception e)
            {
                PrintConsoleMessage(MessageType.ERROR, "ОШИБКА регистрации на сервере", e.Message, e.StackTrace);
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

                PrintConsoleMessage(MessageType.SUCCESS, "клиент отменил регистрацию на сервере");
            }
            catch (Exception e)
            {
                PrintConsoleMessage(MessageType.ERROR, "ОШИБКА отмены регистрации на сервере", e.Message, e.StackTrace);
            }
        }

        private void Connect()
        {
            try
            {
                if (!client.Connected)
                {
                    client.Connect(Address, Port);

                    PrintConsoleMessage(MessageType.SUCCESS, "клиент подключился к серверу");
                }
            }
            catch (Exception e)
            {
                PrintConsoleMessage(MessageType.ERROR, "ОШИБКА подключения к серверу", e.Message, e.StackTrace);
            }
        }

        private void Disconnect()
        {
            try
            {
                if (client.Connected)
                {
                    client.Close();

                    PrintConsoleMessage(MessageType.SUCCESS, "клиент отключился от сервера");
                }
            }
            catch (ObjectDisposedException e)
            {
                PrintConsoleMessage(MessageType.ERROR, "ОШИБКА отключения от сервера", e.Message, e.StackTrace);
            }
        }

    }
}
