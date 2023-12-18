using FileOperations;

namespace Demographic
{
    public class Person
    {
        public char Sex { get; } // f-female m-male
        public int BirthYear { get; }
        public int? DeathYear { get; private set; }

        public YearTick NewYear;
        public bool IsAlive => DeathYear == null;
        
        public delegate void newChild(Person human);
        public event newChild? EventBorn;

        public Person(char sex, int birthYear, YearTick yearTickEvent)
        {
            Sex = sex;
            BirthYear = birthYear;
            NewYear = yearTickEvent;
            NewYear.EventTick += LiveYear;
        }

        public void LiveYear(int year, List<RowDR> DeathRules)
        {
            BornChild(year);
            DethDecision(year, DeathRules);
        }

        public void DethDecision(int year, List<RowDR> DeathRules)
        {
            int age = year - BirthYear;
            for(int i = 0; i < DeathRules.Count; i++)
            {
                if (DeathRules[i].BeginAge < age && age < DeathRules[i].EndAge)
                {
                    if (ProbabilityCalculator.IsEventHappened((Sex == 'f')
                            ? DeathRules[i].WomanDeathProb
                            : DeathRules[i].ManDeathProb))
                    {
                        DeathYear = year;
                        NewYear.EventTick -= LiveYear;
                    }
                    break;
                }
            }
        }

        public void BornChild(int year)
        {
            int age = year - BirthYear;
            if (IsAlive && Sex == 'f' && age >= 18 && age <= 45)
            {
                if (ProbabilityCalculator.IsEventHappened(0.151))
                {
                    char genderChild = ProbabilityCalculator.IsEventHappened(0.55) ? 'f' : 'm';
                    Person child = new Person(genderChild, year, NewYear);
                    EventBorn?.Invoke(child);
                    
                }
            }
        }
    }
}