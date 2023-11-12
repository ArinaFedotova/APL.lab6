using System;
using System.Collections.Generic;
using System.IO;

namespace Demographic.FileOperations
{
    public class DeathRules : IDeathRules
    {
        private readonly string _filename;

        public DeathRules(string filename)
        {
            _filename = filename;
        }

        public List<RowDR> GetDataDeath()
        {
            List<RowDR> dataDeath = new List<RowDR>();
            try
            {
                using (StreamReader reader = new StreamReader(_filename))
                {
                    string line = reader.ReadLine();
                    RowDR row = new RowDR();
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');
                        if (values.Length == 4)
                        {
                            row.BeginAge = int.Parse(values[0]);
                            row.EndAge = int.Parse(values[1]);
                            row.ManDeathProb = double.Parse(values[2], System.Globalization.CultureInfo.InvariantCulture);
                            row.WemanDeathProb = double.Parse(values[3], System.Globalization.CultureInfo.InvariantCulture);
                            dataDeath.Add(row);
                        }
                    }
                }
                return dataDeath;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with reading a file DeathRules.csv: {e.Message}");
                throw;
            }
            
        }
    }
}