using System;
using FileOperations;
using Demographic;

namespace Exec
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                string InitialAgeFileName = args[0];
                string DeathRulesFileName = args[1];

                List<string> otherData = new List<string>();
                for (int i = 2; i < args.Length; i++)
                    otherData.Add(args[i]);
                // string InitialAgeFileName = "/Users/arinafedotova/Downloads/InitialAge.csv";
                // string DeathRulesFileName = "/Users/arinafedotova/Downloads/DeathRules.csv";
                // int startYear = 1970;
                // int endYear = 2000;
                // int populationBegin = 130000000;
                
                try
                {
                    var engine = new Engine(InitialAgeFileName, DeathRulesFileName, otherData);
                    Console.WriteLine("Init End");
                    engine.Model();
                    Console.WriteLine("Model End");
                    
                    IWriteFile writing = new WriteFile();      
                    writing.WriteData("YearPopulationFile.csv", "Year,Population,Men,Women", engine.YearPopulation);
                    writing.WriteData("AgePopulation.csv", "Age,Population,Men,Women", engine.GroupedByAgePeople);
                    Console.WriteLine("Success!");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
                
            }
            else
            {
                Console.WriteLine("Недостаточно аргументов командной строки.");
            }
        }
    }
}