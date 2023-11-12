using System;
using System.Collections.Generic;
using Demographic.FileOperations;

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
        public bool IsAlive => !DeathYear.HasValue;

        public Person(char sex, int birthYear)
        {
            Sex = sex;
            BirthYear = birthYear;
        }

        public void DethDecision(int year, List<RowDR> DeathRules)
        {
            int age = year - BirthYear;
            bool die = false;
            
            foreach (var rowData in DeathRules)
            {
                if (rowData.BeginAge < age && age < rowData.EndAge)
                {
                    die = ProbabilityCalculator.IsEventHappened((Sex == 'f') ? rowData.WemanDeathProb : rowData.ManDeathProb);
                    break;
                }
            }

            if (die)
            {
                DeathYear = year;
            }
        }

        public Person BornChild(int year)
        {
            int age = year - BirthYear;
            if (IsAlive && Sex == 'f' && age >= 18 && age <= 45)
            {
                if (ProbabilityCalculator.IsEventHappened(0.151))
                {
                    char genderChild = ProbabilityCalculator.IsEventHappened(0.55) ? 'f' : 'm';
                    Person child = new Person(genderChild, year);
                    return child;
                }
            }
            return null;
        }
    }
}