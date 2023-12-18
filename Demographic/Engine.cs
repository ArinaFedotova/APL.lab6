using FileOperations;

namespace Demographic
{
    public class Engine : IEngine
    {
        public int BeginYear = 0;
        public int EndYear = 0;
        public int PopulationStart = 0;
        public string ToInitialAgePath;
        public string ToDeathRulesPath;
        private int Accuracy = 1000;
        public List<Person> People { get; set; }

        public List<RowDR> DataDeath;

        public Dictionary<string, List<int>> YearPopulation;
        public Dictionary<string, List<int>> GroupedByAgePeople;
        public List<Person> newChildren = new List<Person>();
        
        public YearTick eventYear;
        public Engine(string pathInitial, string pathDeath, List<string> other)
        {
            ToInitialAgePath = pathInitial;
            ToDeathRulesPath = pathDeath;

            List<int> intData = new List<int>(){BeginYear, EndYear, PopulationStart};
            for (int i = 0; i < other.Count; i++)
            {
                if (!int.TryParse(other[i], out int newArg) || newArg < 0)
                    throw new FormatException($"Error! Invalid type of input args!");
                intData[i] = newArg;
            }
            BeginYear = intData[0];
            EndYear = intData[1];
            PopulationStart = intData[2]/Accuracy;

            eventYear = new YearTick();
            YearPopulation = new Dictionary<string, List<int>>(){{BeginYear.ToString(), 
                new List<int>{PopulationStart, PopulationStart/2, PopulationStart/2}}};
            
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
            IInitialAge initage = new InitialAge(ToInitialAgePath);
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
                            Person newFemale = new Person('f', age, eventYear);
                            Person newMale = new Person('m', age, eventYear);
                            people.Add(newMale);
                            people.Add(newFemale);
                            newFemale.EventBorn += AddChild;
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

        public void AddChild(Person child)
        {
            if (child.Sex == 'f')
            {
                child.EventBorn += AddChild;
            }
            newChildren.Add(child);
        }

        public void Model()
        {
            Console.WriteLine("Model Start");
            List<int> yearPopulation = new List<int>(){People.Count, People.Count/2, People.Count/2};
            for (int i = BeginYear+1; i < EndYear+1; i++)
            {
                YearPopulation.Add(i.ToString(), yearPopulation);
                eventYear.OnEvent(i, DataDeath); 
                foreach (var baby in newChildren)
                {
                    yearPopulation[0]++;
                    if (baby.Sex == 'f') yearPopulation[2]++;
                    else yearPopulation[1]++;
                }

                foreach (var person in People)
                {
                    if (!person.IsAlive && person.Sex == 'f')
                        person.EventBorn -= AddChild;
                }
                //Если ссылка присутствует, объект подписчика не будет удален при сборке мусора.
                int diedWomen = People.RemoveAll(person => (!person.IsAlive && person.Sex == 'f'));
                int diedMen = People.RemoveAll(person => (!person.IsAlive && person.Sex == 'm'));
                yearPopulation[1] -= diedMen;
                yearPopulation[2] -= diedWomen;
                People.AddRange(newChildren);
                Console.WriteLine($"Year: {i}, Died: {(diedWomen + diedMen).ToString()}," +
                                  $" Born: {newChildren.Count.ToString()}");
                yearPopulation = new List<int>(){People.Count, YearPopulation[i.ToString()][1],
                    YearPopulation[i.ToString()][2]};
                newChildren.Clear();
            }
            
            Console.WriteLine($"YearPopulation in the begining is {YearPopulation[BeginYear.ToString()][0]}, " +
                              $"YearPopulation in the end is {YearPopulation[EndYear.ToString()][0]}");
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