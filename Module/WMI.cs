using ImAgent.Entities;
using ImAgent.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using static ImAgent.Helpers.Helper;
using EnumerationOptions = System.Management.EnumerationOptions;

namespace ImAgent.Module
{
    internal class WMI : IFinder
    {
        public bool Recursive { get; set; }
        private ManagementScope ms;
        private ConnectionOptions co;
        private EnumerationOptions eo;

        //SELECT * FROM CIM_DataFile WHERE Drive = 'D:' AND Extension = 'exe'

        private string BaseQuery = @"SELECT * FROM CIM_DataFile WHERE {0} {1} {2}";
        private string DriveQuery = "Drive = '{0}'";
        private string PathQuery = "And Path='{0}'";
        private string ExtensionQuery = "AND Extension = '{0}'";
        private string ExcludeQuery = @"AND NOT Path LIKE '%\WINDOWS\%'";

        public WMI(bool recursive, InquiryEntity ie)
        {
            Recursive = recursive;
            co = new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate,
                Authentication = AuthenticationLevel.PacketIntegrity,
                EnablePrivileges = true,
                Timeout = TimeSpan.MaxValue
            };

            if (!string.IsNullOrEmpty(ie.UserName)) co.Username = ie.UserName;
            if (!string.IsNullOrEmpty(ie.Password)) co.Password = ie.Password;

            eo = new EnumerationOptions()
            {
                BlockSize = 1,
                DirectRead = true,
                ReturnImmediately = true,
                Timeout = TimeSpan.MaxValue
            };
        }

        public IList<FileEntity> Search(string where, string what, string machine)
        {
            List<FileEntity> result = new List<FileEntity>();

            string drive = Path.GetPathRoot(where).Trim('\\');
            string path = where.Split(':')[1];
            string queryString = string.Format(BaseQuery,
                string.Format(DriveQuery, drive),
                (!string.IsNullOrEmpty(path) && !path.Equals("\\")) ? string.Format(PathQuery, path.Replace(@"\", @"\\")) : "",
                string.Format(ExtensionQuery, what));


            ms = new ManagementScope(@"\\", co)
            {
                Path = new ManagementPath(string.Concat(@"\\", machine, @"\root\cimv2"))
            };

            try
            {
                if (!ms.IsConnected)
                {
                    ms.Connect();
                }

                ObjectQuery query = new ObjectQuery(queryString);

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(ms, query, eo))
                {
                    using (ManagementObjectCollection moc = searcher.Get())
                    {
                        foreach (ManagementObject obj in moc)
                        {
                            FileEntity fe = new FileEntity
                            {
                                Name = obj["FileName"].ToString().Trim(),
                                Path = drive + obj["Path"].ToString().Trim(),
                                DateAccessed = ManagementDateTimeConverter.ToDateTime(obj["LastAccessed"].ToString().Trim()),
                                DateCreated = ManagementDateTimeConverter.ToDateTime(obj["CreationDate"].ToString().Trim()),
                                DateModified = ManagementDateTimeConverter.ToDateTime(obj["LastModified"].ToString().Trim()),
                                Size = obj["FileSize"].ToString().Trim(),

                                ApplicationName = FileVersionInfo.GetVersionInfo(obj["Description"].ToString().Trim()).ProductName,
                                Author = FileVersionInfo.GetVersionInfo(obj["Description"].ToString().Trim()).LegalTrademarks,
                                Copyright = FileVersionInfo.GetVersionInfo(obj["Description"].ToString().Trim()).LegalCopyright,
                                Company = FileVersionInfo.GetVersionInfo(obj["Description"].ToString().Trim()).CompanyName,
                                Version = FileVersionInfo.GetVersionInfo(obj["Description"].ToString().Trim()).ProductVersion,
                                FileDescription = obj["Description"].ToString().Trim(),
                                FileExtension = obj["Extension"].ToString().Trim(),
                                ItemType = obj["FileType"].ToString().Trim(),
                                Language = FileVersionInfo.GetVersionInfo(obj["Description"].ToString().Trim()).Language
                                //SoftwareUsed = obj["FileName"].ToString().Trim()
                            };

                            result.Add(fe);
                            obj.Dispose();
                        }
                    }
                }

                foreach (var x in result)
                {
                    var crc32 = new Crc32();
                    var hash = string.Empty;

                    try
                    {
                        using (var fs = File.Open(x.Path + "\\" + x.Name, FileMode.Open, FileAccess.Read))
                            foreach (byte b in crc32.ComputeHash(fs)) hash += b.ToString("x2").ToLower();
                    }
                    catch (Exception e)
                    {
                        PrintConsoleMessage(MessageType.ERROR, "ОШИБКА вычисления crc32", e.Message, e.StackTrace);
                    }

                    x.Crc32 = hash;
                }
            }
            catch (Exception e)
            {
                PrintConsoleMessage(MessageType.ERROR, "ОШИБКА получения данных от WMI", e.Message, e.StackTrace);
            }
            return result;
        }

        public IList<FileEntity> Search(TaskEntity task)
        {
            //string normalisedPath = (task.Path[task.Path.Length -1].Equals("\\")) ? task.Path : task.Path + "\\";
            //return Search(normalisedPath, task.Type, task.Name);
            return Search(task.Path, task.Type, task.Name);
        }
    }
}