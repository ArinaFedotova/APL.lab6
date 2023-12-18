using FileOperations;
namespace Demographic;

public class YearTick
{
    public delegate void curData(int curYear, List<RowDR> DeathRules);

    public event curData? EventTick;

    public void OnEvent(int year, List<RowDR> rules)
    {
        EventTick?.Invoke(year, rules);
    }
}