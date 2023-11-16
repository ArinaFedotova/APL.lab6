using FileOperations;

namespace Demographic
{
    public class Person
    {
        public event EventHandler ChildBirth;
        protected virtual void OnChildBirth(EventArgs e)  
        {  
            ChildBirth?.Invoke(this, e);  
        }
        public char Sex { get; } // f-female m-male
        public int BirthYear { get; }
        public int? DeathYear { get; private set; }
        public bool IsAlive => DeathYear == null;

        public Person(char sex, int birthYear)
        {
            Sex = sex;
            BirthYear = birthYear;
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
                    }
                    break;
                }
            }
        }

        public Person BornChild(int year)
        {
            int age = year - BirthYear;
            if (IsAlive && Sex == 'f' && age >= 18 && age <= 45)
            {
                if (ProbabilityCalculator.IsEventHappened(0.151))
                {
                    OnChildBirth(EventArgs.Empty);
                    char genderChild = ProbabilityCalculator.IsEventHappened(0.55) ? 'f' : 'm';
                    Person child = new Person(genderChild, year);
                    return child;
                }
            }
            return null;
        }
    }
}