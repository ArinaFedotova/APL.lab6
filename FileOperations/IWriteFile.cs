
namespace FileOperations
{
    public interface IWriteFile
    {
        void WriteData(string filename, string headers, Dictionary<string, List<int>> content);
    }
}