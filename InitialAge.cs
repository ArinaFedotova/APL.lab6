using System;
using System.Collections.Generic;
using System.IO;

namespace Demographic.FileOperations
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
            Dictionary<int, int> DataInitAge = new Dictionary<int, int>();

            try
            {
                using (StreamReader reader = new StreamReader(_fileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');
                        if (values.Length == 2 && int.TryParse(values[0], out int age) && double.TryParse(values[1].Replace(".",","), out double count))
                        {
                            DataInitAge.Add(age, (int)(Math.Round(count / 2.0) * 2));
                        }
                    }
                }
                Console.WriteLine("InitialAge");
                Console.WriteLine(string.Join(",", DataInitAge));
                return DataInitAge;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with reading a file InitialAge.csv: {e.Message}");
                throw;
            }
        }
    }
}