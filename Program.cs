using ImAgent.Entities;
using ImAgent.Helpers;
using ImAgent.Module;
using ImAgent.Network;
using System;
using System.Net;

namespace ImAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            InquiryEntity ie = new InquiryEntity();
            IFinder finder = null; ;
            string FileName;
            bool isNetworkMode = false;

            foreach (string arg in args)
            {
                if (arg.StartsWith("/"))
                {
                    switch (arg.Substring(1, 1))
                    {
                        case "m":
                            ie.Method = arg.Split(':')[1];
                            break;

                        case "p":
                            ie.Password = arg.Split(':')[1];
                            break;

                        case "u":
                            ie.UserName = arg.Split(':')[1];
                            break;

                        case "i":
                            ie.TaskFile = arg.Substring(arg.IndexOf(':') + 1);
                            break;

                        case "o":
                            ie.OutputFolder = arg.Substring(arg.IndexOf(':') + 1);
                            break;

                        case "1":
                            ie.File1 = arg.Substring(arg.IndexOf(':') + 1);
                            break;

                        case "2":
                            ie.File2 = arg.Substring(arg.IndexOf(':') + 1);
                            break;

                        case "c":
                            isNetworkMode = true;

                            int port = (arg.Contains(':')) ? int.Parse(arg.Split(':')[2]) : 9876;
                            IPAddress address = (arg.Contains(':')) ? IPAddress.Parse(arg.Split(':')[1]) : IPAddress.Loopback;
                            Client client = new Client(port, address);

                            client.Start();

                            break;

                        case "s":
                            isNetworkMode = true;

                            //todo а если задан только один параметр?
                            int sport = (arg.Contains(':')) ? int.Parse(arg.Split(':')[2]) : 9876;
                            IPAddress saddress = (arg.Contains(':')) ? IPAddress.Parse(arg.Split(':')[1]) : IPAddress.Any;
                            Server server = new Server(sport, saddress);

                            server.Start();

                            break;

                        default:

                            Console.WriteLine("непонятная команда");
                            ShowHelp();

                            break;
                    }
                }
            }

            if (!isNetworkMode)
            {
                if (!string.IsNullOrEmpty(ie.TaskFile))
                {
                    ie.Tasks = CSVFile.ReadCsvFile(ie.TaskFile);
                }

                if (ie.Method.Equals("wmi"))
                {
                    finder = new WMI(true, ie);
                }
                else if (ie.Method.Equals("ws"))
                {
                    //finder = new WindowsSearch(true);
                    finder = new WMI(true, ie);
                }
                else if (ie.Method.Equals("cmp"))
                {
                    CompareFiles(ie);
                    return;
                }

                if (ie.Tasks != null)
                {
                    foreach (TaskEntity task in ie.Tasks)
                    {
                        FileName = string.Format(
                            "{0}{1}-{2}-{3}.csv",
                            (!string.IsNullOrEmpty(ie.OutputFolder)) ? ie.OutputFolder : "",
                            task.Name,
                            DateTime.Now.ToString().Replace(":", "-"),
                            ie.Method);

                        CSVFile.WriteCsvFile(FileName, finder.Search(task));
                    }
                }
                else
                {
                    FileName = string.Format(
                            "{0}{1}-{2}-{3}.csv",
                            (!string.IsNullOrEmpty(ie.OutputFolder)) ? ie.OutputFolder : "",
                            Environment.MachineName,
                            DateTime.Now.ToString().Replace(":", "-"),
                            ie.Method);

                    CSVFile.WriteCsvFile(FileName, finder.Search("C:\\", "exe", "."));
                    //WriteCsvFile(FileName, finder.Search("D:\\projects\\ImAgent\\bin\\Debug\\netcoreapp3.1\\", "exe", "."));
                }
            }
        }

        private static void CompareFiles(InquiryEntity ie)
        {
            if (string.IsNullOrEmpty(ie.File1)) { Console.WriteLine(@"Не указан файл №1. Укажите его через опцию ""/1"""); return; }
            if (string.IsNullOrEmpty(ie.File2)) { Console.WriteLine(@"Не указан файл №2. Укажите его через опцию ""/2"""); return; }

            Comparsion.Compare(ie.File1, ie.File2);
        }

        private static void ShowHelp()
        {
            string Usage = @"Для работы программы укажите необходимые аргументы.
Примеры:

ImAgent.exe /m:method [/u:username /p:password] [/in:""c:\pcs.csv""] [/out:""c:\result\""]
ImAgent.exe /m:cmp /1:""c:\file1.csv"" /2:""c:\file1.csv""
ImAgent.exe /c[:server ip-address:port]
ImAgent.exe /s[:server ip-address:port]

/m           метод опроса, поддерживаются wmi, ws, cmp
             (Windows Search), либо ""cmp"" для сравнения
/in          входящий csv файл с заданиями.Колонка ""Name""
             отвечает за имя / IP ПК.Колонка ""Path"" за путь
             и колонка ""Type"" за тип файлов для поиска.
             Если не указано - опрос текущего ПК на ""exe"".
/out         Папка для результатов сканирования. Если не указано
             будет использоваться папка запуска приложения.
/u           указание имени пользователя для опроса.
/p           указание пароля для опроса.
/c           работа программы в качестве сетевого клиента. Без указания адреса
             старается подключиться на адрес 127.0.0.1 и порт 9876.
/s           забота программы в качестве сервера для сетевых клиентов. Без указания
             адреса и порта - прослушивает порт 9876 на всех интерфейсах.
/1,/2        файлы для сравнения.Файлы сравниваются построчно.
             Результптом сравнения также являются два файла.
             Файлы имеющиеся в первом списке и отсутствующие во втором.
             И наоборот.";
            Console.WriteLine(Usage);
        }

        
    }
}
