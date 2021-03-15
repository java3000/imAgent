using CsvHelper;
using ImAgent.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ImAgent.Module
{
    public static class Comparsion
    {
        public static void Compare(string FileA, string FileB)
        {
            List<FileEntity> ListA = ReadCsv(FileA);
            List<FileEntity> ListB = ReadCsv(FileB);

            if (!ListA.SequenceEqual(ListB))
            {
                var firstNotSecond = ListA.Except(ListB).ToList();
                var secondNotFirst = ListB.Except(ListA).ToList();

                string NewFileAName = Path.GetDirectoryName(FileA) + "\\" + "InFile1-NotInFile2.csv";
                string NewFileBName = Path.GetDirectoryName(FileB) + "\\" + "InFile2-NotInFile1.csv";

                WriteCsv(NewFileAName, firstNotSecond);
                WriteCsv(NewFileBName, secondNotFirst); 
            }
        }

        public static void WriteCsv(string FileName, IList<FileEntity> entities)
        {
            try
            {
                using (var writer = new StreamWriter(FileName))
                {
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(entities);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("comparsion WriteCsvFile error", e);
            }
        }

        public static List<FileEntity> ReadCsv(string FileName)
        {
            List<FileEntity> result = new List<FileEntity>();
            try
            {
                using (var reader = new StreamReader(FileName))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        foreach (FileEntity r in csv.EnumerateRecords(new FileEntity()))
                        {
                            result.Add(r);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("comparsion ReadCsvFile error", e);
            }

            return result;
        }
    }
}
