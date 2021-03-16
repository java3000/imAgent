using ImAgent.Entities;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;

namespace ImAgent.Module
{
    public class WindowsSearch : IFinder
    {
        public bool Recursive { get; set; }

        private string BaseQuery = @"SELECT 
               System.ItemName, 
               System.ItemFolderPathDisplay,
               System.ContentStatus,
               System.ContentType,
               System.Size,
               System.FileDescription,
               System.FileExtension,
               System.DateModified,
               System.DateAccessed,
               System.DateCreated,
               System.ApplicationName,
               System.Author,
               System.Company,
               System.Copyright,
               System.FileDescription,
               System.FileExtension,
               System.ItemTypeText,
               System.Language,
               System.SoftwareUsed

               FROM SystemIndex WHERE {0} = 'file:{1}' AND System.ItemName LIKE '%{2}'"; //,System.FileVersion

        public WindowsSearch(bool recursive)
        {
            Recursive = recursive;
        }

        public List<FileEntity> Search(string where, string what, string machine)
        {
            List<FileEntity> result = new List<FileEntity>();

            var connection = new OleDbConnection(@"Provider=Search.CollatorDSO;Extended Properties=""Application=Windows""");

            var query = string.Format(BaseQuery,
                (Recursive) ? "scope" : "directory",
                where,
                what);

            try
            {
                connection.Open();

                using (var command = new OleDbCommand(query, connection))
                {
                    using (var r = command.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            try
                            {
                                FileEntity fn = new FileEntity();

                                if (File.Exists(r[1].ToString() + "\\" + r[0].ToString()))
                                {
                                    fn.Name = r[0].ToString();
                                    fn.Path = r[1].ToString();
                                    fn.Size = r[4].ToString();
                                    fn.DateModified = (DateTime)r[7];
                                    fn.DateAccessed = (DateTime)r[8];
                                    fn.DateCreated = (DateTime)r[9];
                                    fn.FileDescription = r[14].ToString();
                                    fn.FileExtension = r[15].ToString();
                                    fn.ItemType = r[16].ToString();
                                    fn.SoftwareUsed = r[18].ToString();
                                    fn.Version = FileVersionInfo.GetVersionInfo(r[1].ToString() + "\\" + r[0].ToString()).ProductVersion;
                                    fn.ApplicationName = r[10].ToString() ?? FileVersionInfo.GetVersionInfo(r[1].ToString() + "\\" + r[0].ToString()).ProductName;
                                    fn.Author = r[11].ToString() ?? FileVersionInfo.GetVersionInfo(r[1].ToString() + "\\" + r[0].ToString()).LegalTrademarks;
                                    fn.Company = r[12].ToString() ?? FileVersionInfo.GetVersionInfo(r[1].ToString() + "\\" + r[0].ToString()).CompanyName;
                                    fn.Copyright = r[13].ToString() ?? FileVersionInfo.GetVersionInfo(r[1].ToString() + "\\" + r[0].ToString()).LegalCopyright;
                                    fn.Language = r[17].ToString() ?? FileVersionInfo.GetVersionInfo(r[1].ToString() + "\\" + r[0].ToString()).Language;


                                    result.Add(fn);
                                }
                            }
                            catch (FileNotFoundException e)
                            {
                                Console.WriteLine(e.Message);
                                Console.WriteLine(e.StackTrace);
                            }
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
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }

                    x.Crc32 = hash;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        public List<FileEntity> Search(TaskEntity task)
        {
            //string normalisedPath = (task.Path[task.Path.Length].Equals("\\")) ? task.Path : task.Path + "\\";
            return Search(task.Path, task.Type, task.Name);
        }
    }
}
