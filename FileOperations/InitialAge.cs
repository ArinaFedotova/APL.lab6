using System;
using System.Collections.Generic;
using System.IO;

namespace FileOperations
{
    public class InitialAge : IInitialAge
    {
        private readonly string _fileName;

        public InitialAge(string filename)
        {
            _fileName = filename;
        }

        public Dictionary<int, int> GetDataInitAge()
        {
            if (!File.Exists(_fileName))
                throw new FileNotFoundException("Error with reading a file InitialAge.csv");
            FileInfo fileInfo = new FileInfo(_fileName);
            if (fileInfo.Length == 0)
                throw new Exception("File InitialAge.csv is empty");
            if (fileInfo.Length >= Int32.MaxValue)
                throw new Exception("File is too big");
            
            Dictionary<int, int> DataInitAge = new Dictionary<int, int>();
            using (StreamReader reader = new StreamReader(_fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');
                    if (values.Length == 2 && int.TryParse(values[0], out int age) &&
                        double.TryParse(values[1].Replace(".", ","), out double count))
                    {
                        DataInitAge.Add(age, (int)(Math.Round(count / 2.0) * 2));
                    }
                }
            }
            Console.WriteLine("InitialAge");
            Console.WriteLine(string.Join(",", DataInitAge));
            return DataInitAge;
        }
    }
}