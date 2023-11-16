
namespace FileOperations
{
    public class WriteFile : IWriteFile
    {
        public void WriteData(string filename, string headers, Dictionary<string, List<int>> content)
        {
            // Создаем экземпляр StreamWriter для записи в файл CSV
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine(headers);
                // Записываем каждую строку данных из списка в файл CSV
                foreach (KeyValuePair<string, List<int>> data in content)
                {
                    string csvLine = string.Join(",", data.Value);
                    sw.WriteLine($"{Convert.ToString(data.Key)},{csvLine}");
                    
                }
            }
        }
    }
}