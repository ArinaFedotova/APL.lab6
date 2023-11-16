
namespace FileOperations
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
            if (!File.Exists(_filename))
                throw new FileNotFoundException("Error with reading a file DeathRules.csv");
            FileInfo fileInfo = new FileInfo(_filename);
            if (fileInfo.Length == 0)
                throw new Exception("File DeathRules.csv is empty");

            List<RowDR> dataDeath = new List<RowDR>();
            using (StreamReader reader = new StreamReader(_filename))
            {
                string line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    RowDR row = new RowDR();
                    string[] values = line.Split(',');
                    if (values.Length == 4)
                    {
                        row.BeginAge = int.Parse(values[0]);
                        row.EndAge = int.Parse(values[1]);
                        row.ManDeathProb = double.Parse(values[2], System.Globalization.CultureInfo.InvariantCulture);
                        row.WomanDeathProb = double.Parse(values[3], System.Globalization.CultureInfo.InvariantCulture);
                        dataDeath.Add(row);
                    }
                    else
                        throw new InvalidDataException("Format is incorrect in file DeathRules.csv");
                }
            }
            return dataDeath;
        }
    }
}