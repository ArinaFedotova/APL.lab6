using FileOperations;

namespace Demographic
{
    public class Engine : IEngine
    {
        public int BeginYear;
        public int EndYear;
        public int PopulationStart;
        public string ToInitialAgePath;
        public string ToDeathRulesPath;
        public List<Person> People { get; set; }

        public List<RowDR> DataDeath;

        public Dictionary<string, List<int>> YearPopulation;
        public Dictionary<string, List<int>> GroupedByAgePeople;

        
        public event EventHandler<EventArgs> YearTick; 
        protected virtual void OnYearTick(EventArgs e)
        {
            EventHandler<EventArgs> handler = YearTick;
            handler?.Invoke(this, e);
        }

        private void Person_ChildBirth(object sender, EventArgs e)
        {
            Person person = (Person)sender;
        }
        public Engine(string pathInitial, string pathDeath, List<string> other)
        {
            ToInitialAgePath = pathInitial;
            ToDeathRulesPath = pathDeath;
            
            BeginYear = 1970;
            EndYear = 2021;
            PopulationStart = 130000000/1000;

            List<int> intData = new List<int>(){BeginYear, EndYear, PopulationStart};
            for (int i = 0; i < other.Count; i++)
            {
                if (!int.TryParse(other[i], out int newArg) || newArg < 0)
                    throw new FormatException($"Error! Invalid type of input args!");
                intData[i] = newArg;
            }
            BeginYear = intData[0];
            EndYear = intData[1];
            PopulationStart = intData[2]/1000;
            
            YearPopulation = new Dictionary<string, List<int>>(){{BeginYear.ToString(), new List<int>(){PopulationStart, PopulationStart/2, PopulationStart/2}}};
            
            try
            {
                People = InitialPopulation();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public List<Person> InitialPopulation()
        {
            List<Person> people = new List<Person>();
            InitialAge initage = new InitialAge(ToInitialAgePath);
            var data = initage.GetDataInitAge();
            List<int> populationCount = new List<int>{0, 0, 0};
            Console.WriteLine("Init Start");
            try
            {
                for (int n = 0; n < PopulationStart / 2000; n++)
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
                People = people;
                populationCount[0] = People.Count;
                populationCount[1] = People.Count / 2;
                populationCount[2] = People.Count / 2;
            
                DeathRules deathRules = new DeathRules(ToDeathRulesPath);
                DataDeath = deathRules.GetDataDeath();
                Console.WriteLine($"Number of People in the begining: {People.Count}");
                return people;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Model()
        {
            Console.WriteLine("Model Start");
            List<int> yearPopulation = new List<int>(){People.Count, People.Count/2, People.Count/2};
            for (int i = BeginYear+1; i < EndYear+1; i++)
            {
                OnYearTick(EventArgs.Empty);
                YearPopulation.Add(i.ToString(), yearPopulation);
                List<Person> children = new List<Person>();
                List<Person> diedPeople = new List<Person>();
                foreach (var person in People)
                {
                    Person child = person.BornChild(i);
                    if (child != null)
                    {
                        person.ChildBirth += Person_ChildBirth;
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
                }
                Console.WriteLine($"Year: {i}, Died: {diedPeople.Count.ToString()}, Born: {children.Count.ToString()}");
                People.RemoveAll(person => diedPeople.Contains(person));
                People.AddRange(children);
                yearPopulation = new List<int>(){People.Count, YearPopulation[i.ToString()][1], YearPopulation[i.ToString()][2]};
            }
            
            Console.WriteLine($"YearPopulation in the begining is {YearPopulation[BeginYear.ToString()][0]}, YearPopulation in the end is {YearPopulation[EndYear.ToString()][0]}");
            Console.WriteLine($"Difference = {YearPopulation[EndYear.ToString()][0] - YearPopulation[BeginYear.ToString()][0]}");
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
                    PopulationCount("65-100", person.Sex);
                }
            }
        }

        public void PopulationCount(string keey, char gender)
        {
            GroupedByAgePeople[keey][0]++;
            if (gender == 'f') GroupedByAgePeople[keey][2]++;
            else GroupedByAgePeople[keey][1]++;
        }
    }
}