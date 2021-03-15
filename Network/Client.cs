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
using System.Xml.Serialization;

namespace ImAgent.Network
{
    class Client
    {
        private TcpClient client;       
        private NetworkStream stream;

        public int Port { get; set; }
        public IPAddress Address { get; set; }

        public Client(int? port, IPAddress address)
        {
            Port = port ?? throw new ArgumentNullException(nameof(port));
            Address = address ?? throw new ArgumentNullException(nameof(address));

            try
            {
                client = new TcpClient(Address.ToString(), Port);
                stream = client.GetStream();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ОШИБКА сервер недоступен");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ResetColor();
            }
        }

        public void Start()
        {
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
                    //while (isConnected)
                    //while (client.Connected)
                    while (true)
                    {
                        string result = "null";
                        string data = null;
                        stream = client.GetStream();

                        try
                        {
                            //using (StreamReader sr = new StreamReader(client.GetStream()))
                            //{
                                StreamReader sr = new StreamReader(client.GetStream());
                                data = sr.ReadLine(); 
                            //}
                        }
                        catch (Exception e)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("ОШИБКА чтения данных от сервера");
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                            Console.ResetColor();
                        }

                        if (!string.IsNullOrEmpty(data))
                        {
                            InquiryEntity ie = new InquiryEntity();

                            switch (data)
                            {
                                case "sendingjob":

                                        ie = ParseCommand(client.GetStream());

                                        if (ie != null)
                                        {
                                            result = ExecuteCommand(ie);
                                        }

                                    if (!string.IsNullOrEmpty(result))
                                    {

                                        //TODO GOOD~~~!!!!!
                                        if (client.GetStream().CanWrite)
                                        {
                                            byte[] msg = System.Text.Encoding.UTF8.GetBytes("readytosendresult\r\n");

                                            try
                                            {
                                                stream.Write(msg, 0, msg.Length);

                                                Console.WriteLine("7");
                                            }
                                            catch (Exception e)
                                            {
                                                Console.ForegroundColor = ConsoleColor.Red;
                                                Console.WriteLine("ОШИБКА отправки команды серверу");
                                                Console.WriteLine(e.Message);
                                                Console.WriteLine(e.StackTrace);
                                                Console.ResetColor();
                                            }

                                            Console.WriteLine("8");

                                            //TODO GOOD~~~!!!!!
                                            msg = System.Text.Encoding.UTF8.GetBytes(result);

                                            try
                                            {
                                                stream.Write(msg, 0, msg.Length);

                                                Console.WriteLine("9");
                                            }
                                            catch (Exception e)
                                            {
                                                Console.ForegroundColor = ConsoleColor.Red;
                                                Console.WriteLine("ОШИБКА отправки результатов на сервер");
                                                Console.WriteLine(e.Message);
                                                Console.WriteLine(e.StackTrace);
                                                Console.ResetColor();

                                                string FileName = string.Format(
                                                "{0}{1}-{2}-{3}.csv",
                                                (!string.IsNullOrEmpty(ie.OutputFolder)) ? ie.OutputFolder : "",
                                                ie.Tasks[0].Name,
                                                DateTime.Now.ToString().Replace(":", "-"),
                                                ie.Method);

                                                CSVFile.WriteCsvFile(FileName, result);

                                                Console.ForegroundColor = ConsoleColor.Yellow;
                                                Console.WriteLine("данные сохранены в файл " + FileName);
                                                Console.ResetColor();
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("приняты непонятные данные от сервера: " + data.ToString());
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
                /*Stream s = client.GetStream();
                using (StreamReader sr = new StreamReader(s))
                {
                    string x = "";
                    string y = "";
                    while (!(x = sr.ReadLine()).Equals("fff"))
                    {
                        y += x;
                        Console.WriteLine(x);
                    }

                    Console.WriteLine("=================");
                    Console.WriteLine(y);

                    Stream ss = new MemoryStream(Encoding.UTF8.GetBytes(y));
                    Console.WriteLine("00");
                    XmlSerializer xs = new XmlSerializer(typeof(InquiryEntity));
                    Console.WriteLine("000");
                    result = (InquiryEntity)xs.Deserialize(ss);
                    Console.WriteLine("0000");
                }*/

                //todo сделать нормально
                using (StreamReader sr = new StreamReader(data))
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ОШИБКА обработки задания");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ResetColor();
            }

            return result;
        }

        private void Register()
        {
            if (stream.CanWrite)
            {
                try
                {
                    
                    byte[] msg = System.Text.Encoding.UTF8.GetBytes("register\r\n");
                    //stream.WriteAsync(msg, 0, msg.Length);
                    stream.Write(msg, 0, msg.Length);
                    //stream.Flush();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("клиент успешно зарегистрировался на сервере");
                    Console.ResetColor();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ОШИБКА регистрации на сервере");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ResetColor();
                }
            }
        }

        private void UnRegister()
        {
            if (stream.CanWrite)
            {
                try
                {
                    byte[] msg = System.Text.Encoding.UTF8.GetBytes("unregister\r\n");
                    stream.Write(msg, 0, msg.Length);
                    //stream.Flush();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("клиент отменил регистрацию на сервере");
                    Console.ResetColor();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ОШИБКА отмены регистрации на сервере");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ResetColor();
                }
            }
        }

        private void Connect()
        {
            try
            {
                if (!client.Connected)
                {
                    client.Connect(Address, Port);
                    stream = client.GetStream();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("клиент подключился к серверу");
                    Console.ResetColor();
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ОШИБКА подключения к серверу");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ResetColor();
            }
        }

        private void Disconnect()
        {
            try
            {
                if (client.Connected)
                {
                    client.Close();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("клиент отключился от сервера");
                    Console.ResetColor();
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ОШИБКА отключения от сервера");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ResetColor();
            }
        }

    }
}
