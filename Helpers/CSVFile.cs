using CsvHelper;
using ImAgent.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ImAgent.Helpers
{
    class CSVFile
    {
        public static void WriteCsvFile(string FileName, IList<FileEntity> entities)
        {
            using (var writer = new StreamWriter(FileName))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    try
                    {
                        csv.WriteRecords(entities);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        public static List<TaskEntity> ReadCsvFile(string FileName)
        {
            List<TaskEntity> result = new List<TaskEntity>();
            try
            {
                using (var reader = new StreamReader(FileName))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        try
                        {
                            var records = csv.EnumerateRecords(new TaskEntity());
                            foreach (var r in records)
                            {
                                result.Add(r);
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return result;
        }

        public static Stream WriteCsvStream(IList<FileEntity> entities)
        {
            MemoryStream ms = new MemoryStream();

            using (var writer = new StreamWriter(ms))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    try
                    {
                        csv.WriteRecords(entities);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            return ms;
        }

        public static string WriteCsvString(IList<FileEntity> entities)
        {
            string result;

            using (var writer = new StringWriter())
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    try
                    {
                        csv.WriteRecords(entities);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                result = writer.ToString();
            }
            return result;
        }

        internal static void WriteCsvFile(string FileName, string data)
        {
            using (var writer = new StreamWriter(FileName))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    try
                    {
                        csv.WriteRecords(data);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
