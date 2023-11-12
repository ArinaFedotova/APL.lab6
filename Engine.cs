using System;
using System.Collections.Generic;
using System.Linq;
using Demographic.FileOperations;

namespace Demographic
{
    public class Engine : IEngine
    {
        public int BeginYear { get; }
        public int EndYear { get; }

        public int PopulationStart;

        public string ToInitialAgePath;
        public string ToDeathRulesPath { get; }
        public List<Person> People { get; set; }

        public List<RowDR> DataDeath;

        public Dictionary<string, List<int>> YearPopulation;
        public Dictionary<string, List<int>> GroupedByAgePeople;

    public Engine(string fileinitial, string filedeath, int begin, int end, int population)
        {
            BeginYear = begin;
            EndYear = end;
            PopulationStart = population;
            ToInitialAgePath = fileinitial;
            ToDeathRulesPath = filedeath;
            People = InitialPopulation();
        }
        
        public List<Person> InitialPopulation() 
        {
            List<Person> people = new List<Person>();
            InitialAge initage = new InitialAge(ToInitialAgePath);
            var data = initage.GetDataInitAge();
            List<int> populationCount = new List<int>{0, 0, 0};
            Console.WriteLine("Init");
            for (int n = 0; n < PopulationStart / 2000000; n++)
            {
                foreach (KeyValuePair<int, int> line in data)
                {
                    for (int i = 0; i < line.Value; i++) // Create 1000 people with the same age
                    {
                        int age = BeginYear - line.Key;
                        people.Add(new Person('m', age));
                        people.Add(new Person('f', age));
                    }
                }
            } //130 000 000
            
            Console.WriteLine(people.Count);
            People = people;
            populationCount[0] = People.Count;
            populationCount[1] = People.Count / 2;
            populationCount[2] = People.Count / 2;

            Dictionary<string, List<int>> beginYearPop = new Dictionary<string, List<int>>();
            beginYearPop.Add(BeginYear.ToString(), populationCount);
            YearPopulation = beginYearPop;
            
            DeathRules deathRules = new DeathRules(ToDeathRulesPath);
            DataDeath = deathRules.GetDataDeath();
            Console.WriteLine(people.Count);
            return people;
        }

        public void Model()
        {
            List<int> yearPopulation = new List<int>();
            yearPopulation = YearPopulation[BeginYear.ToString()].ToList();
            Console.WriteLine("!");
            Console.WriteLine(yearPopulation[2]);
            for (int i = BeginYear+1; i < EndYear+1; i++)
            {
                List<Person> children = new List<Person>();
                List<Person> diedPeople = new List<Person>();
                foreach (var person in People)
                {
                    Person child = person.BornChild(i);
                    if (child != null)
                    {
                        children.Add(child);
                        yearPopulation[0]++;
                        if (child.Sex == 'f') yearPopulation[2]++;
                        else yearPopulation[1]++;
                    }
                    person.DethDecision(i, DataDeath);

                    if (person.IsAlive == false)
                    {
                        yearPopulation[0]--;
                        if (person.Sex == 'f') yearPopulation[2]--;
                        else yearPopulation[1]--;
                        diedPeople.Add(person);
                    }

                    var status = person.IsAlive ? "alive" : "died";
                }
                YearPopulation.Add(i.ToString(), yearPopulation);
                // Console.WriteLine($"Year: {i}, Died: {diedPeople.Count.ToString()}, Born: {children.Count.ToString()}");
                People.RemoveAll(person => diedPeople.Contains(person));
                People.AddRange(children);
            }
            Console.WriteLine("!");
            Console.WriteLine(YearPopulation[BeginYear.ToString()][2]);
            Console.WriteLine(YearPopulation.Count);
            GroupingPeople();
        }

        public void GroupingPeople()
        {
            Dictionary<string, List<int>> ageGroups = new Dictionary<string, List<int>>
            {
                {"0-18", new List<int>{0, 0, 0}}, 
                {"19-44", new List<int>{0, 0, 0}},
                {"45-64", new List<int>{0, 0, 0}},
                {"65-100", new List<int>{0, 0, 0}}
            };
            GroupedByAgePeople = ageGroups;
            foreach (var person in People)
            {
                var age = EndYear - person.BirthYear;
                if (age < 65)
                {
                    if (age < 45)
                    {
                        if (age < 19)
                            PopulationCount("0-18", person.Sex);
                        else
                            PopulationCount("19-44", person.Sex);
                        
                    }
                    else
                        PopulationCount("45-64", person.Sex);
                }
                else
                {
                    Console.WriteLine($"1 M: {GroupedByAgePeople["65-100"][1].ToString()}, F: {GroupedByAgePeople["65-100"][2].ToString()}");
                    PopulationCount("65-100", person.Sex);
                    Console.WriteLine($"2 M: {GroupedByAgePeople["65-100"][1].ToString()}, F: {GroupedByAgePeople["65-100"][2].ToString()}");
                }
            }
            Console.WriteLine($"{GroupedByAgePeople["45-64"][0].ToString()}, {GroupedByAgePeople["0-18"][1].ToString()}");
        }

        public void PopulationCount(string keey, char gender)
        {
            GroupedByAgePeople[keey][0]++;
            if (gender == 'f') GroupedByAgePeople[keey][2]++;
            else GroupedByAgePeople[keey][1]++;
        }
    }
}