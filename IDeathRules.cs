using System.Collections.Generic;

namespace Demographic.FileOperations
{
    public interface IDeathRules
    {
        List<RowDR> GetDataDeath();
    }
    public class RowDR
    {
        public int BeginAge { get; set; }
        public int EndAge { get; set; }
        public double ManDeathProb { get; set; }
        public double WemanDeathProb { get; set; }
    }
}